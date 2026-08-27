using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OCSFoundationOptimizer.Models
{
    public class ParameterItem : INotifyPropertyChanged
    {
        private string _key = "";
        private string _name = "";
        private string _value = "";
        private string _unit = "";

        private ParameterType _type = ParameterType.Number;

        private bool _isRequired = true;
        private bool _isReadOnly = false;

        /// <summary>
        /// 程序内部唯一标识
        /// </summary>
        public string Key
        {
            get => _key;
            set
            {
                if (_key != value)
                {
                    _key = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 参数值
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            get => _unit;
            set
            {
                if (_unit != value)
                {
                    _unit = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 参数类型
        /// </summary>
        public ParameterType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否为必填参数
        /// </summary>
        public bool IsRequired
        {
            get => _isRequired;
            set
            {
                if (_isRequired != value)
                {
                    _isRequired = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    OnPropertyChanged();
                }
            }
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