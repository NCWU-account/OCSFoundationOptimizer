using System.Collections.ObjectModel;

namespace OCSFoundationOptimizer.Models
{
    public class ParameterGroup
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name { get; set; } = "";


        /// <summary>
        /// 当前分组中的参数
        /// </summary>
        public ObservableCollection<ParameterItem> Parameters { get; set; }
            = new();
    }
}