using System;
using System.ComponentModel;
using System.Globalization;
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

        private bool _hasError = false;
        private string _errorMessage = "";


        // =====================================================
        // 程序内部唯一标识
        // =====================================================

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


        // =====================================================
        // 参数名称
        // =====================================================

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


        // =====================================================
        // 参数值
        // =====================================================

        public string Value
        {
            get => _value;

            set
            {
                if (_value != value)
                {
                    _value = value;

                    OnPropertyChanged();

                    // 输入以后立即验证
                    ValidateValue();
                }
            }
        }


        // =====================================================
        // 单位
        // =====================================================

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


        // =====================================================
        // 参数类型
        // =====================================================

        public ParameterType Type
        {
            get => _type;

            set
            {
                if (_type != value)
                {
                    _type = value;

                    OnPropertyChanged();

                    ValidateValue();
                }
            }
        }


        // =====================================================
        // 是否必填
        // =====================================================

        public bool IsRequired
        {
            get => _isRequired;

            set
            {
                if (_isRequired != value)
                {
                    _isRequired = value;

                    OnPropertyChanged();

                    ValidateValue();
                }
            }
        }


        // =====================================================
        // 是否只读
        // =====================================================

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


        // =====================================================
        // 是否存在错误
        // =====================================================

        public bool HasError
        {
            get => _hasError;

            private set
            {
                if (_hasError != value)
                {
                    _hasError = value;

                    OnPropertyChanged();
                }
            }
        }


        // =====================================================
        // 错误信息
        // =====================================================

        public string ErrorMessage
        {
            get => _errorMessage;

            private set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;

                    OnPropertyChanged();
                }
            }
        }


        // =====================================================
        // 参数验证
        // =====================================================

        private void ValidateValue()
        {
            string value =
                Value?.Trim() ?? "";


            // =================================================
            // 空值
            // =================================================

            if (string.IsNullOrWhiteSpace(value))
            {
                if (IsRequired)
                {
                    SetError("请输入参数值。");
                }
                else
                {
                    ClearError();
                }

                return;
            }


            // =================================================
            // 数字类型
            // =================================================

            if (Type == ParameterType.Number)
            {
                bool success =
                    double.TryParse(
                        value,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double number);


                if (!success)
                {
                    SetError("请输入有效的数字。");
                    return;
                }


                // NaN / Infinity
                if (double.IsNaN(number) ||
                    double.IsInfinity(number))
                {
                    SetError("请输入有效的数字。");
                    return;
                }


                // 工程计算一般不允许负数
                if (number < 0)
                {
                    SetError("参数不能小于 0。");
                    return;
                }
            }


            // =================================================
            // 验证通过
            // =================================================

            ClearError();
        }


        // =====================================================
        // 设置错误
        // =====================================================

        private void SetError(string message)
        {
            HasError = true;
            ErrorMessage = message;
        }


        // =====================================================
        // 清除错误
        // =====================================================

        private void ClearError()
        {
            HasError = false;
            ErrorMessage = "";
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
}