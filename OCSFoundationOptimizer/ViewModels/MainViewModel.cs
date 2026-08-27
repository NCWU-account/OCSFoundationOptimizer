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


        // =====================================================
        // 输出结果
        // =====================================================

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
        /// 生成计算书
        /// </summary>
        public ICommand GenerateCalculationBookCommand { get; }


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


            // =================================================
            // 生成计算书
            // =================================================

            GenerateCalculationBookCommand =
                new RelayCommand(_ => { GenerateCalculationBook(); });
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
            // =================================================
            // 切换理论
            // =================================================

            CurrentTheory = theory;


            // =================================================
            // 清除原来的计算结果
            // =================================================

            ClearCalculationResult();


            // =================================================
            // 如果参数已经完整且合法
            // 自动重新计算新的理论
            // =================================================

            if (AreAllParametersValid())
            {
                CalculationStatus =
                    $"正在执行 {CurrentTheoryDisplayName} 计算...";


                _autoCalculateTimer.Stop();

                _autoCalculateTimer.Start();
            }
            else
            {
                CalculationStatus =
                    $"当前选择：{CurrentTheoryDisplayName}";
            }
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
            // =================================================
            // 参数值发生变化
            // =================================================

            if (e.PropertyName ==
                nameof(ParameterItem.Value))
            {
                // =================================================
                // 先停止之前的自动计算计时器
                //
                // 用户继续输入时重新计时
                // =================================================

                _autoCalculateTimer.Stop();


                // =================================================
                // 参数发生变化以后，
                // 原来的计算结果立即失效
                // =================================================

                ClearCalculationResult();


                // =================================================
                // 检查参数状态
                // =================================================

                if (InputParameters.Any(x => x.HasError))
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
                    // =================================================
                    // 参数全部填写并且目前合法
                    //
                    // 不立即计算
                    // 等待用户停止输入 300ms
                    // =================================================

                    CalculationStatus =
                        "参数已就绪，正在准备计算...";


                    _autoCalculateTimer.Start();
                }


                // =================================================
                // 更新计算按钮
                // =================================================

                OnPropertyChanged(
                    nameof(CanCalculate));


                CommandManager
                    .InvalidateRequerySuggested();
            }


            // =====================================================
            // 参数错误状态发生变化
            // =====================================================

            if (e.PropertyName ==
                nameof(ParameterItem.HasError))
            {
                // 错误状态发生变化
                _autoCalculateTimer.Stop();


                OnPropertyChanged(
                    nameof(CanCalculate));


                CommandManager
                    .InvalidateRequerySuggested();


                // =================================================
                // 如果所有参数已经填写并且全部合法
                // 则启动自动计算
                // =================================================

                if (AreAllParametersValid())
                {
                    CalculationStatus =
                        "参数已就绪，正在准备计算...";


                    _autoCalculateTimer.Start();
                }
            }
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
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "b0",
                    Name = "b0",
                    Value = "1300",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "a1",
                    Name = "a1",
                    Value = "3500",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "b1",
                    Name = "b1",
                    Value = "3100",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "h",
                    Name = "h",
                    Value = "3500",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "na",
                    Name = "na",
                    Value = "2",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "nb",
                    Name = "nb",
                    Value = "2",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "nh",
                    Name = "nh",
                    Value = "2",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_a",
                    Name = "Δa",
                    Value = "450",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_b",
                    Name = "Δb",
                    Value = "450",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_h",
                    Name = "Δh",
                    Value = "450",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "M1",
                    Name = "垂直线路向荷载M",
                    Value = "450",
                    Unit = "KN∙m",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "PH1",
                    Name = "垂直线路向荷载P_H",
                    Value = "38",
                    Unit = "KN",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "M2",
                    Name = "线路向荷载M",
                    Value = "50",
                    Unit = "KN∙m",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "PH2",
                    Name = "线路向荷载P_H",
                    Value = "4",
                    Unit = "KN",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Q",
                    Name = "垂直力Q",
                    Value = "70",
                    Unit = "KN",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

                        InputParameters.Add(
                new ParameterItem
                {
                    Key = "fak",
                    Name = "地基承载力特征值f_ak",
                    Value = "130",
                    Unit = "kPa",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "r",
                    Name = "水上填土湿容重γ",
                    Value = "16",
                    Unit = "kN∕m3",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "fai",
                    Name = "土体内摩擦角φ",
                    Value = "30",
                    Unit = "°",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "[k_0]",
                    Name = "抗倾稳定系数允许值[K_0]",
                    Value = "",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Key = "[k_c]",
                    Name = "抗滑稳定系数允许值[K_c]",
                    Value = "",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true
                });





        }


        // =====================================================
        // 监听输入参数
        // =====================================================

        private void SubscribeParameterEvents()
        {
            foreach (var parameter
                     in InputParameters)
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