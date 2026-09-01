using System.ComponentModel;
using System.Runtime.CompilerServices;
//这个类专门给 B 用。
namespace OCSFoundationOptimizer.Models
{
    public class OptimizationParameter : INotifyPropertyChanged
    {
        private string _value = "";

        public string Key { get; set; } = "";

        public string Name { get; set; } = "";

        public string Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                _value = value;
                OnPropertyChanged();
            }
        }

        public string Unit { get; set; } = "";

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
