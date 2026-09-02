using OCSFoundationOptimizer.Models;
using OCSFoundationOptimizer.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace OCSFoundationOptimizer.ViewModels
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        // =====================================================
        // 服务
        // =====================================================

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


        // =====================================================
        // 计算结果展开状态
        // =====================================================

        private bool _isResultExpanded = false;


        // =====================================================
        // 当前计算结果
        // =====================================================

        private CalculationResult? _currentResult;


        // =====================================================
        // 计算状态
        // =====================================================

        private string _calculationStatus =
            "等待输入参数";


        // =====================================================
        // 当前选择的计算理论
        // =====================================================


        public CalculationTheoryType CurrentTheory
        {
            get => _currentTheory;

            private set
            {
                if (_currentTheory == value)
                    return;

                _currentTheory = value;

                OnPropertyChanged();

                OnPropertyChanged(
                    nameof(IsTheoryASelected));

                OnPropertyChanged(
                    nameof(IsTheoryAOptimizationSelected));

                OnPropertyChanged(
                    nameof(IsTheoryBSelected));

                OnPropertyChanged(
                    nameof(CurrentTheoryDisplayName));
            }
        }

        // =====================================================
        // 计算结果展开状态
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

                OnPropertyChanged(
                    nameof(ResultToggleText));
            }
        }


        public string ResultToggleText =>
            IsResultExpanded
                ? "折叠结果"
                : "展开结果";


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

        public bool IsTheoryAOptimizationSelected
        {
            get
            {
                return CurrentTheory ==
                       CalculationTheoryType.AOptimization;
            }
        }
        // =====================================================
        // 当前理论显示名称
        // =====================================================

        public string CurrentTheoryDisplayName
        {
            get
            {
                switch (CurrentTheory)
                {
                    case CalculationTheoryType.A:
                        return "理论 A";

                    case CalculationTheoryType.AOptimization:
                        return "理论 A 优化";

                    case CalculationTheoryType.B:
                        return "理论 B";

                    default:
                        return "";
                }
            }
        }


        // =====================================================
        // 当前计算结果
        // =====================================================

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
        // 输入参数分组
        // =====================================================

        public ObservableCollection<ParameterGroup>
            ParameterGroups { get; }
            = new();


        // =====================================================
        // 优化参数
        // =====================================================

        public ObservableCollection<ParameterItem>
            OptimizationParameters { get; }
            = new();


        // =====================================================
        // 优化参数分组
        // =====================================================

        public ObservableCollection<ParameterGroup>
            OptimizationGroups { get; }
            = new();


        // =====================================================
        // 输出结果参数
        // =====================================================

        public ObservableCollection<ParameterItem>
            ResultParameters { get; }
            = new();


        public ObservableCollection<ParameterItem> TheoryBParameters { get; } = new();
        public ObservableCollection<ParameterGroup> TheoryBParameterGroups { get; } = new();
        
        
        
        // =====================================================
        // 当前计算状态
        // =====================================================

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
            get
            {
                if (CurrentTheory ==
                    CalculationTheoryType.A)
                {
                    return AreAllParametersValid();
                }

                if (CurrentTheory ==
                    CalculationTheoryType.B)
                {
                    return AreAllTheoryBParametersValid();
                }

                return false;
            }
        }


        // =====================================================
        // 是否可以执行优化
        // =====================================================

        public bool CanOptimize
        {
            get
            {
                return
                    CurrentTheory ==
                    CalculationTheoryType.AOptimization
                    &&
                    AreAllParametersValid()
                    &&
                    AreOptimizationParametersValid();
            }
        }


        // =====================================================
        // 命令
        // =====================================================

        public ICommand SelectTheoryACommand { get; set; }
        public ICommand SelectTheoryAOptimizationCommand { get;set; }
        public ICommand SelectTheoryBCommand { get;set; }

        public ICommand CalculateCommand { get;set; }

        public ICommand OptimizeCommand { get;set; }

        public ICommand GenerateCalculationBookCommand { get;set; }

        public ICommand ToggleResultCommand { get;set; }


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


            // =================================================
            // 初始化参数
            // =================================================
            // 初始化理论 A 参数
            InitializeParameters();
            // 初始化优化参数
            InitializeOptimizationParameters();

            // 初始化理论 B 参数
            InitializeTheoryBParameters();
            // =================================================
            // 根据 Group 创建参数分组
            // =================================================

            BuildParameterGroups();

            BuildOptimizationGroups();
            BuildTheoryBParameterGroups();

            // =================================================
            // 监听参数变化
            // =================================================

            SubscribeParameterEvents();


            // =================================================
            // 初始化命令
            // =================================================

            InitializeCommands();
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