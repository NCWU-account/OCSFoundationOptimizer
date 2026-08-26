using OCSFoundationOptimizer.Models;
using OCSFoundationOptimizer.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OCSFoundationOptimizer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 输入参数
        /// </summary>
        public ObservableCollection<ParameterItem> InputParameters
        {
            get;
        } = new();

        /// <summary>
        /// 计算结果
        /// </summary>
        public ObservableCollection<ParameterItem> ResultParameters
        {
            get;
        } = new();

        /// <summary>
        /// 图片1
        /// </summary>
        private string? _image1Path;

        public string? Image1Path
        {
            get => _image1Path;
            set
            {
                if (_image1Path != value)
                {
                    _image1Path = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 图片2
        /// </summary>
        private string? _image2Path;

        public string? Image2Path
        {
            get => _image2Path;
            set
            {
                if (_image2Path != value)
                {
                    _image2Path = value;
                    OnPropertyChanged();
                }
            }
        }

        private readonly FoundationCalculationService _calculationService;

        public MainViewModel()
        {
            _calculationService = new FoundationCalculationService();

            // 这里暂时放几个测试参数
            InputParameters.Add(
                new ParameterItem
                {
                    Name = "基础宽度",
                    Value = ""
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Name = "基础长度",
                    Value = ""
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Name = "基础高度",
                    Value = ""
                });

            InputParameters.Add(
                new ParameterItem
                {
                    Name = "混凝土强度",
                    Value = ""
                });

            // 监听参数变化
            foreach (var parameter in InputParameters)
            {
                parameter.PropertyChanged += Parameter_PropertyChanged;
            }

            // 图片以后换成你的实际图片
            // Image1Path = "Images/house1.png";
            // Image2Path = "Images/house2.png";
        }

        /// <summary>
        /// 参数发生变化
        /// </summary>
        private void Parameter_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ParameterItem.Value))
            {
                AutoCalculate();
            }
        }

        /// <summary>
        /// 自动计算
        /// </summary>
        public void AutoCalculate()
        {
            // 参数没有填写完整，不计算
            if (!AreAllParametersFilled())
            {
                return;
            }

            Calculate();
        }

        /// <summary>
        /// 判断所有输入参数是否填写完成
        /// </summary>
        private bool AreAllParametersFilled()
        {
            return InputParameters.Count > 0 &&
                   InputParameters.All(x =>
                       !string.IsNullOrWhiteSpace(x.Value));
        }

        /// <summary>
        /// 点击计算按钮
        /// </summary>
        public void Calculate()
        {
            var result = _calculationService.Calculate(
                InputParameters);

            ResultParameters.Clear();

            foreach (var item in result)
            {
                ResultParameters.Add(item);
            }
        }

        /// <summary>
        /// 添加输入参数
        /// </summary>
        public void AddInputParameter(
            string name,
            string value = "")
        {
            var parameter = new ParameterItem
            {
                Name = name,
                Value = value
            };

            parameter.PropertyChanged += Parameter_PropertyChanged;

            InputParameters.Add(parameter);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}