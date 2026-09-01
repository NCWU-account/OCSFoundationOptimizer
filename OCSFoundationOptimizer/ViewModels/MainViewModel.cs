using OCSFoundationOptimizer.Models;
using OCSFoundationOptimizer.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace OCSFoundationOptimizer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CalculationService _calculationService;

        private readonly CalculationBookService _calculationBookService;

        // =====================================================
        // 自动计算计时器
        // =====================================================

        private readonly DispatcherTimer _autoCalculateTimer;
        // =====================================================
        // 当前计算理论
        // =====================================================

        private CalculationTheoryType _currentTheory =
            CalculationTheoryType.A;

        // 展示计算结果的展开状态
        private bool _isResultExpanded = false;

        /// <summary>
        /// 当前选择的计算理论
        /// </summary>
        public CalculationTheoryType CurrentTheory
        {
            get => _currentTheory;

            private set
            {
                if (_currentTheory != value)
                {
                    _currentTheory = value;

                    OnPropertyChanged();

                    // 通知理论 A / B 的选中状态发生变化
                    OnPropertyChanged(
                        nameof(IsTheoryASelected));

                    OnPropertyChanged(
                        nameof(IsTheoryBSelected));

                    // 通知界面显示文字
                    OnPropertyChanged(
                        nameof(CurrentTheoryDisplayName));
                }
            }
        }

        // =====================================================
        // 展示计算结果的展开状态
        // =====================================================
        public bool IsResultExpanded
        {
            get => _isResultExpanded;
            set
            {
                if (_isResultExpanded == value)
                    return;

                _isResultExpanded = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ResultToggleText));
            }
        }

        public string ResultToggleText =>
            IsResultExpanded ? "折叠结果" : "展开结果";
        // =====================================================
        // 理论 A 是否选中
        // =====================================================

        public bool IsTheoryASelected
        {
            get
            {
                return CurrentTheory ==
                       CalculationTheoryType.A;
            }
        }


        // =====================================================
        // 理论 B 是否选中
        // =====================================================

        public bool IsTheoryBSelected
        {
            get
            {
                return CurrentTheory ==
                       CalculationTheoryType.B;
            }
        }


        // =====================================================
        // 当前理论显示名称
        // =====================================================

        public string CurrentTheoryDisplayName
        {
            get
            {
                return CurrentTheory ==
                       CalculationTheoryType.A
                    ? "理论 A"
                    : "理论 B";
            }
        }


        // =====================================================
        // 当前计算结果
        // =====================================================

        private CalculationResult? _currentResult;


        public CalculationResult? CurrentResult
        {
            get => _currentResult;

            private set
            {
                if (_currentResult != value)
                {
                    _currentResult = value;

                    OnPropertyChanged();
                }
            }
        }


        // =====================================================
        // 输入参数
        // =====================================================

        public ObservableCollection<ParameterItem>
            InputParameters { get; } = new();

        public ObservableCollection<ParameterGroup> ParameterGroups { get; }
            = new();

        public ObservableCollection<ParameterGroup> OptimizationGroups { get; }
            = new();
        
        private void BuildOptimizationGroups()
        {
            OptimizationGroups.Clear();

            var groups =
                OptimizationParameters
                    .GroupBy(x => x.Group)
                    .Select(group =>
                        new ParameterGroup
                        {
                            Name = group.Key,
                            Parameters =
                                new ObservableCollection<ParameterItem>(
                                    group)
                        });

            foreach (var group in groups)
            {
                OptimizationGroups.Add(group);
            }
        }
        // =====================================================
        // 输出结果
        // =====================================================
        public ObservableCollection<ParameterItem>
            OptimizationParameters { get; }
            = new();

        public ObservableCollection<ParameterItem>
            ResultParameters { get; } = new();


        // =====================================================
        // 计算状态
        // =====================================================

        private string _calculationStatus =
            "等待输入参数";


        /// <summary>
        /// 当前计算状态
        /// </summary>
        public string CalculationStatus
        {
            get => _calculationStatus;

            private set
            {
                if (_calculationStatus != value)
                {
                    _calculationStatus = value;

                    OnPropertyChanged();
                }
            }
        }


        // =====================================================
        // 是否可以执行计算
        // =====================================================

        public bool CanCalculate
        {
            get { return AreAllParametersValid(); }
        }

        public bool CanOptimize
        {
            get
            {
                return
                    AreAllParametersValid() &&
                    AreOptimizationParametersValid();
            }
        }

        private bool AreOptimizationParametersValid()
        {
            if (OptimizationParameters.Any(x =>
                    string.IsNullOrWhiteSpace(x.Value)))
            {
                return false;
            }

            if (OptimizationParameters.Any(x =>
                    x.HasError))
            {
                return false;
            }

            return true;
        }
        // =====================================================
        // 命令
        // =====================================================

        /// <summary>
        /// 选择理论 A
        /// </summary>
        public ICommand SelectTheoryACommand { get; }


        /// <summary>
        /// 选择理论 B
        /// </summary>
        public ICommand SelectTheoryBCommand { get; }


        /// <summary>
        /// 执行计算
        /// </summary>
        public ICommand CalculateCommand { get; }

        /// <summary>
        ///  
        /// </summary>
        public ICommand OptimizeCommand { get; }

        /// <summary>
        /// 生成计算书
        /// </summary>
        public ICommand GenerateCalculationBookCommand { get; }

        /// <summary>
        /// 展开 / 折叠计算结果
        /// </summary>
        public ICommand ToggleResultCommand { get; }

        // =====================================================
        // 构造函数
        // =====================================================

        public MainViewModel()
        {
            _calculationService =
                new CalculationService();

            _calculationBookService =
                new CalculationBookService();

            // =================================================
            // 自动计算计时器
            // =================================================

            _autoCalculateTimer =
                new DispatcherTimer
                {
                    Interval =
                        TimeSpan.FromMilliseconds(300)
                };

            _autoCalculateTimer.Tick +=
                AutoCalculateTimer_Tick;
            // 初始化参数
            InitializeParameters();
            InitializeOptimizationParameters();
            // =================================================
            // 根据 Group 创建参数分组
            // =================================================

            BuildParameterGroups();
            BuildOptimizationGroups();
            // 监听参数变化
            SubscribeParameterEvents();


            // =================================================
            // 理论 A
            // =================================================

            SelectTheoryACommand =
                new RelayCommand(_ =>
                {
                    SelectTheory(
                        CalculationTheoryType.A);
                });


            // =================================================
            // 理论 B
            // =================================================

            SelectTheoryBCommand =
                new RelayCommand(_ =>
                {
                    SelectTheory(
                        CalculationTheoryType.B);
                });


            // =================================================
            // 执行计算
            // =================================================

            CalculateCommand =
                new RelayCommand(
                    _ => { Calculate(); },
                    _ => { return CanCalculate; });
            OptimizeCommand =
                new RelayCommand(
                    _ => { Optimize(); },
                    _ => { return CanOptimize; });

            // =================================================
            // 生成计算书
            // =================================================

            GenerateCalculationBookCommand =
                new RelayCommand(_ => { GenerateCalculationBook(); });
            // =================================================
            // 展开 / 折叠计算结果
            // =================================================

            ToggleResultCommand =
                new RelayCommand(_ => { IsResultExpanded = !IsResultExpanded; });
        }
        // =====================================================
        // 根据参数 Group 创建参数分组
        // =====================================================

        private void BuildParameterGroups()
        {
            // 先清空，防止重复构建
            ParameterGroups.Clear();


            // =================================================
            // 按照参数出现的顺序进行分组
            //
            // GroupBy 会把相同 Group 的参数放在一起
            // =================================================

            var groups = InputParameters
                .GroupBy(x => x.Group)
                .Select(group => new ParameterGroup
                {
                    Name = group.Key,

                    Parameters = new ObservableCollection<ParameterItem>(
                        group)
                });


            // =================================================
            // 添加到前台绑定的 ParameterGroups
            // =================================================

            foreach (var group in groups)
            {
                ParameterGroups.Add(group);
            }
        }

        private void AutoCalculateTimer_Tick(
            object? sender,
            EventArgs e)
        {
            // 停止计时器
            _autoCalculateTimer.Stop();


            // =================================================
            // 再次检查参数
            // =================================================

            if (!AreAllParametersValid())
            {
                return;
            }


            // =================================================
            // 自动执行计算
            // =================================================

            Calculate();
        }
        // =====================================================
        // 选择理论
        // =====================================================

        private void SelectTheory(
            CalculationTheoryType theory)
        {
            CurrentTheory = theory;

            ClearCalculationResult();

            _autoCalculateTimer.Stop();

            if (CurrentTheory ==
                CalculationTheoryType.A)
            {
                if (AreAllParametersValid())
                {
                    CalculationStatus =
                        "参数已就绪，正在准备计算...";

                    _autoCalculateTimer.Start();
                }
                else
                {
                    CalculationStatus =
                        "当前选择：理论 A";
                }
            }
            else
            {
                CalculationStatus =
                    "当前选择：理论 B，请设置优化目标并开始优化。";
            }

            OnPropertyChanged(
                nameof(CanCalculate));

            OnPropertyChanged(
                nameof(CanOptimize));

            CommandManager
                .InvalidateRequerySuggested();
        }


        // =====================================================
        // 执行计算
        // =====================================================

        private void Calculate()
        {
            // =================================================
            // 第一步：检查参数
            // =================================================

            if (!AreAllParametersValid())
            {
                CalculationStatus =
                    "参数存在错误，请检查输入";

                return;
            }


            try
            {
                CalculationStatus =
                    $"正在执行 {CurrentTheoryDisplayName} 计算...";


                // =================================================
                // 调用统一计算服务
                //
                // 当前理论由 CurrentTheory 决定
                // =================================================

                var result =
                    _calculationService.Calculate(
                        CurrentTheory,
                        InputParameters);


                // =================================================
                // 显示结果
                // =================================================

                ShowResult(result);
            }
            catch (Exception ex)
            {
                CalculationStatus =
                    "计算过程中发生错误";


                MessageBox.Show(
                    ex.Message,
                    "计算错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Optimize()
        {
            if (!AreAllParametersValid())
            {
                CalculationStatus =
                    "基础计算参数存在错误，请检查输入。";

                return;
            }

            if (!AreOptimizationParametersValid())
            {
                CalculationStatus =
                    "优化参数存在错误，请检查输入。";

                return;
            }

            try
            {
                CalculationStatus =
                    "正在搜索最优基础尺寸...";

                var allParameters =
                    InputParameters
                        .Concat(OptimizationParameters)
                        .ToList();

                var result =
                    _calculationService.Calculate(
                        CalculationTheoryType.B,
                        allParameters);

                ShowResult(result);

                if (result.IsSuccess)
                {
                    CalculationStatus =
                        "理论 B 优化完成";
                }
            }
            catch (Exception ex)
            {
                CalculationStatus =
                    "优化过程中发生错误";

                MessageBox.Show(
                    ex.Message,
                    "优化错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // =====================================================
        // 显示计算结果
        // =====================================================

        private void ShowResult(
            CalculationResult result)
        {
            // =================================================
            // 计算失败
            // =================================================

            if (!result.IsSuccess)
            {
                CurrentResult = null;

                ResultParameters.Clear();


                CalculationStatus =
                    "计算失败";


                MessageBox.Show(
                    result.ErrorMessage,
                    "计算错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // =================================================
            // 保存完整计算结果
            //
            // 注意：
            //
            // ProcessParameters
            // 即使前台暂时不显示，
            // 仍然完整保存在 CurrentResult 中。
            // =================================================

            CurrentResult = result;


            // =================================================
            // 更新前台结果
            // =================================================

            ResultParameters.Clear();


            foreach (var item
                     in result.ResultParameters)
            {
                ResultParameters.Add(item);
            }


            // =================================================
            // 计算成功后自动展开结果
            // =================================================

            IsResultExpanded = true;


            // =================================================
            // 更新计算状态
            // =================================================

            CalculationStatus =
                $"{CurrentTheoryDisplayName} 计算完成";
        }


        // =====================================================
        // 参数发生变化
        // =====================================================

        private void Parameter_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName !=
                nameof(ParameterItem.Value))
            {
                return;
            }

            _autoCalculateTimer.Stop();

            ClearCalculationResult();


            // =====================================================
            // 理论 B
            // =====================================================

            if (CurrentTheory ==
                CalculationTheoryType.B)
            {
                CalculationStatus =
                    "优化参数已修改，请点击“开始优化”。";

                OnPropertyChanged(
                    nameof(CanOptimize));

                CommandManager
                    .InvalidateRequerySuggested();

                return;
            }


            // =====================================================
            // 理论 A
            // =====================================================

            if (InputParameters.Any(
                    x => x.HasError))
            {
                CalculationStatus =
                    "参数输入存在错误";
            }
            else if (!AreAllParametersFilled())
            {
                CalculationStatus =
                    "等待输入完整参数";
            }
            else
            {
                CalculationStatus =
                    "参数已就绪，正在准备计算...";

                _autoCalculateTimer.Start();
            }


            OnPropertyChanged(
                nameof(CanCalculate));

            CommandManager
                .InvalidateRequerySuggested();
        }


        // =====================================================
        // 清除当前计算结果
        // =====================================================

        private void ClearCalculationResult()
        {
            CurrentResult = null;

            ResultParameters.Clear();
        }


        // =====================================================
        // 判断参数是否全部填写
        // =====================================================

        private bool AreAllParametersFilled()
        {
            return InputParameters
                .Where(x => x.IsRequired)
                .All(x =>
                    !string.IsNullOrWhiteSpace(
                        x.Value));
        }


        // =====================================================
        // 判断参数是否全部合法
        // =====================================================

        private bool AreAllParametersValid()
        {
            // 必填参数没有全部填写
            if (!AreAllParametersFilled())
            {
                return false;
            }


            // 存在非法参数
            if (InputParameters.Any(x => x.HasError))
            {
                return false;
            }


            return true;
        }


        // =====================================================
        // 初始化参数
        // =====================================================

        private void InitializeParameters()
        {
            InputParameters.Add(
                new ParameterItem
                {
                    Key = "a0",
                    Name = "a0",
                    Value = "1700",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "b0",
                    Name = "b0",
                    Value = "1300",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "a1",
                    Name = "a1",
                    Value = "3100",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "b1",
                    Name = "b1",
                    Value = "2600",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "h",
                    Name = "h",
                    Value = "3500",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "h_0",
                    Name = "h0",
                    Value = "200",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "na",
                    Name = "na",
                    Value = "1",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "nb",
                    Name = "nb",
                    Value = "1",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "nh",
                    Name = "nh",
                    Value = "1",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_a",
                    Name = "Δa",
                    Value = "650",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_b",
                    Name = "Δb",
                    Value = "650",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_h",
                    Name = "Δh",
                    Value = "650",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "M1",
                    Name = "垂直线路向荷载M",
                    Value = "450",
                    Unit = "KN∙m",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "荷载参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "PH1",
                    Name = "垂直线路向荷载P_H",
                    Value = "38",
                    Unit = "KN",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "荷载参数"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "M2",
                    Name = "线路向荷载M",
                    Value = "50",
                    Unit = "KN∙m",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "荷载参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "PH2",
                    Name = "线路向荷载P_H",
                    Value = "4",
                    Unit = "KN",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "荷载参数"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Q",
                    Name = "垂直力Q",
                    Value = "70",
                    Unit = "KN",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "荷载参数"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "fak",
                    Name = "地基承载力特征值f_ak",
                    Value = "130",
                    Unit = "kPa",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "r",
                    Name = "水上填土湿容重γ",
                    Value = "16",
                    Unit = "kN∕m3",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "fai",
                    Name = "土体内摩擦角φ",
                    Value = "30",
                    Unit = "°",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数"
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "f",
                    Name = "基底与土的摩擦系数f",
                    Value = "0.25",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数",
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "[k_0]",
                    Name = "抗倾稳定系数允许值[K_0]",
                    Value = "1.6",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数",
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "[k_c]",
                    Name = "抗滑稳定系数允许值[K_c]",
                    Value = "1.3",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数",
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "nd",
                    Name = "深度修正系数ηd",
                    Value = "1.0",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数",
                });
        }

        private void InitializeOptimizationParameters()
        {
            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_K0_MIN",
                    Name = "K₀最小允许值",
                    Value = "1.50",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "优化目标"
                });

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_KC_MIN",
                    Name = "Kc最小允许值",
                    Value = "1.30",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "优化目标"
                });


            // =====================================================
            // a1
            // =====================================================

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_A1_MIN",
                    Name = "a1最小值",
                    Value = "2000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "a1优化范围"
                });

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_A1_MAX",
                    Name = "a1最大值",
                    Value = "4000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "a1优化范围"
                });

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_DELTA_A",
                    Name = "a1步长Δa",
                    Value = "50",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "a1优化范围"
                });


            // =====================================================
            // b1
            // =====================================================

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_B1_MIN",
                    Name = "b1最小值",
                    Value = "1800",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "b1优化范围"
                });

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_B1_MAX",
                    Name = "b1最大值",
                    Value = "3500",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "b1优化范围"
                });

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_DELTA_B",
                    Name = "b1步长Δb",
                    Value = "50",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "b1优化范围"
                });


            // =====================================================
            // h
            // =====================================================

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_H_MIN",
                    Name = "h最小值",
                    Value = "2500",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "h优化范围"
                });

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_H_MAX",
                    Name = "h最大值",
                    Value = "5000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "h优化范围"
                });

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_DELTA_H",
                    Name = "h步长Δh",
                    Value = "50",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "h优化范围"
                });
        }

        // =====================================================
        // 监听输入参数
        // =====================================================

        private void SubscribeParameterEvents()
        {
            foreach (var parameter in InputParameters)
            {
                parameter.PropertyChanged +=
                    Parameter_PropertyChanged;
            }

            foreach (var parameter in OptimizationParameters)
            {
                parameter.PropertyChanged +=
                    Parameter_PropertyChanged;
            }
        }


        // =====================================================
        // 生成计算书
        // =====================================================

        private void GenerateCalculationBook()
        {
            // =================================================
            // 没有计算结果
            // =================================================

            if (CurrentResult == null ||
                !CurrentResult.IsSuccess)
            {
                MessageBox.Show(
                    "请先完成计算，再生成计算书。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            // =================================================
            // 保存文件
            // =================================================

            var dialog =
                new Microsoft.Win32.SaveFileDialog
                {
                    Title =
                        "生成计算书",

                    Filter =
                        "Word文档 (*.docx)|*.docx",

                    FileName =
                        $"基础计算书_{CurrentTheoryDisplayName}.docx"
                };


            if (dialog.ShowDialog() != true)
            {
                return;
            }


            // =================================================
            // 生成计算书
            // =================================================

            try
            {
                _calculationBookService.Generate(
                    CurrentResult,
                    CurrentTheory,
                    dialog.FileName);


                MessageBox.Show(
                    "计算书生成成功。",
                    "完成",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"计算书生成失败：{ex.Message}",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // INotifyPropertyChanged
        // =====================================================

        public event PropertyChangedEventHandler?
            PropertyChanged;


        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(
                    propertyName));
        }
    }


    // =========================================================
    // RelayCommand
    // =========================================================

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;

        private readonly Predicate<object?>?
            _canExecute;


        public RelayCommand(
            Action<object?> execute,
            Predicate<object?>? canExecute = null)
        {
            _execute = execute;

            _canExecute = canExecute;
        }


        public bool CanExecute(
            object? parameter)
        {
            return _canExecute == null ||
                   _canExecute(parameter);
        }


        public void Execute(
            object? parameter)
        {
            _execute(parameter);
        }


        public event EventHandler?
            CanExecuteChanged
            {
                add
                {
                    CommandManager
                        .RequerySuggested += value;
                }

                remove
                {
                    CommandManager
                        .RequerySuggested -= value;
                }
            }
    }
}