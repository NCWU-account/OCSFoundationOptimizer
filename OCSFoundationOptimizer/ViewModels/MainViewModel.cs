using OCSFoundationOptimizer.Models;
using OCSFoundationOptimizer.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace OCSFoundationOptimizer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CalculationService
            _calculationService;

        private readonly CalculationBookService
            _calculationBookService;


        // =====================================================
        // 当前计算理论
        // =====================================================

        private CalculationTheoryType _currentTheory
            = CalculationTheoryType.A;


        public CalculationTheoryType CurrentTheory
        {
            get => _currentTheory;

            private set
            {
                if (_currentTheory != value)
                {
                    _currentTheory = value;

                    OnPropertyChanged();
                }
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
            InputParameters
        {
            get;
        } = new();


        // =====================================================
        // 输出结果
        // =====================================================

        public ObservableCollection<ParameterItem>
            ResultParameters
        {
            get;
        } = new();


        // =====================================================
        // 命令
        // =====================================================

        public ICommand SelectTheoryACommand
        {
            get;
        }

        public ICommand SelectTheoryBCommand
        {
            get;
        }

        public ICommand GenerateCalculationBookCommand
        {
            get;
        }


        // =====================================================
        // 构造函数
        // =====================================================

        public MainViewModel()
        {
            _calculationService =
                new CalculationService();

            _calculationBookService =
                new CalculationBookService();


            // 初始化参数
            InitializeParameters();

            // 监听参数变化
            SubscribeParameterEvents();


            // 理论 A
            SelectTheoryACommand =
                new RelayCommand(
                    _ => SelectTheory(
                        CalculationTheoryType.A));


            // 理论 B
            SelectTheoryBCommand =
                new RelayCommand(
                    _ => SelectTheory(
                        CalculationTheoryType.B));


            // 生成计算书
            GenerateCalculationBookCommand =
                new RelayCommand(
                    _ => GenerateCalculationBook());
        }


        // =====================================================
        // 选择理论
        // =====================================================

        private void SelectTheory(
            CalculationTheoryType theory)
        {
            // 这里只负责“选择”
            CurrentTheory = theory;

            // 如果参数已经填写完整
            // 选择理论后立即重新计算
            if (AreAllParametersFilled())
            {
                Calculate();
            }
        }


        // =====================================================
        // 统一计算入口
        // =====================================================

        private void Calculate()
        {
            try
            {
                var result =
                    _calculationService.Calculate(
                        CurrentTheory,
                        InputParameters);


                ShowResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "计算错误");
            }
        }


        // =====================================================
        // 显示计算结果
        // =====================================================

        private void ShowResult(
            CalculationResult result)
        {
            if (!result.IsSuccess)
            {
                MessageBox.Show(
                    result.ErrorMessage,
                    "计算错误");

                return;
            }


            // 保存完整计算结果
            //
            // 注意：
            //
            // ProcessParameters
            // 虽然不显示在界面，
            // 但是仍然保存在这里。
            //
            CurrentResult = result;


            // ================================================
            // 前台只显示 ResultParameters
            // ================================================

            ResultParameters.Clear();

            foreach (var item
                     in result.ResultParameters)
            {
                ResultParameters.Add(item);
            }
        }


        // =====================================================
        // 参数变化
        // =====================================================

        private void Parameter_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName ==
                nameof(ParameterItem.Value))
            {
                AutoCalculate();
            }
        }


        // =====================================================
        // 自动计算
        // =====================================================

        private void AutoCalculate()
        {
            // 参数没有全部填写
            // 不进行计算
            if (!AreAllParametersFilled())
                return;


            // ================================================
            // 注意这里！
            //
            // 不再判断 A / B
            //
            // 统一调用 Calculate()
            // ================================================

            Calculate();
        }


        // =====================================================
        // 判断输入参数是否填写完整
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
        // 初始化参数
        // =====================================================

        private void InitializeParameters()
        {
            InputParameters.Add(
                new ParameterItem
                {
                    Key = "FoundationWidth",
                    Name = "基础宽度",
                    Value = "",
                    Unit = "m",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "FoundationLength",
                    Name = "基础长度",
                    Value = "",
                    Unit = "m",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "FoundationHeight",
                    Name = "基础高度",
                    Value = "",
                    Unit = "m",
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
            if (CurrentResult == null ||
                !CurrentResult.IsSuccess)
            {
                MessageBox.Show(
                    "请先完成计算，再生成计算书。",
                    "提示");

                return;
            }


            var dialog =
                new Microsoft.Win32.SaveFileDialog
                {
                    Title = "生成计算书",

                    Filter =
                        "Word文档 (*.docx)|*.docx",

                    FileName =
                        "基础计算书.docx"
                };


            if (dialog.ShowDialog() != true)
                return;


            try
            {
                _calculationBookService.Generate(
                    CurrentResult,
                    CurrentTheory,
                    dialog.FileName);


                MessageBox.Show(
                    "计算书生成成功。",
                    "完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"计算书生成失败：{ex.Message}",
                    "错误");
            }
        }


        // =====================================================
        // INotifyPropertyChanged
        // =====================================================

        public event PropertyChangedEventHandler?
            PropertyChanged;


        protected void OnPropertyChanged(
            [CallerMemberName]
            string? propertyName = null)
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