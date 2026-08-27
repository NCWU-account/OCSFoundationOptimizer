using OCSFoundationOptimizer.Models;
using System.Collections.Generic;

namespace OCSFoundationOptimizer.Calculations
{
    /// <summary>
    /// 所有计算理论的统一接口
    /// </summary>
    public interface ICalculationModule
    {
        /// <summary>
        /// 计算理论类型
        /// </summary>
        CalculationTheoryType Theory { get; }

        /// <summary>
        /// 理论名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 执行计算
        /// </summary>
        CalculationResult Calculate(
            IReadOnlyList<ParameterItem> parameters);
    }
}