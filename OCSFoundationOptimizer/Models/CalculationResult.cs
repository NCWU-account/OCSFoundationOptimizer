using System.Collections.Generic;

namespace OCSFoundationOptimizer.Models
{
    /// <summary>
    /// 一次完整计算的结果
    /// </summary>
    public class CalculationResult
    {
        /// <summary>
        /// 输入参数
        /// </summary>
        public List<ParameterItem> InputParameters { get; set; }
            = new();

        /// <summary>
        /// 计算过程参数
        /// </summary>
        public List<ParameterItem> ProcessParameters { get; set; }
            = new();

        /// <summary>
        /// 最终结果参数
        /// </summary>
        public List<ParameterItem> ResultParameters { get; set; }
            = new();

        /// <summary>
        /// 是否计算成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; } = "";
    }
}