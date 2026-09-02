using OCSFoundationOptimizer.Models;
using System.Collections.Generic;

namespace OCSFoundationOptimizer.Calculations
{
    /// <summary>
    /// 理论 B
    ///
    /// 当前仅保留接口位置。
    /// 后续在这里实现真正的理论 B。
    /// </summary>
    public class CalculationTheoryB : ICalculationModule
    {
        public CalculationTheoryType Theory =>
            CalculationTheoryType.B;

        public string Name =>
            "理论 B";


        public CalculationResult Calculate(
            IReadOnlyList<ParameterItem> parameters)
        {
            return new CalculationResult
            {
                IsSuccess = false,
                ErrorMessage =
                    "理论 B 尚未实现。"
            };
        }
    }
}