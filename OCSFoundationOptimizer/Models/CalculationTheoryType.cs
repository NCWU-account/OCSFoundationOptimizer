namespace OCSFoundationOptimizer.Models
{
    /// <summary>
    /// 计算理论类型
    /// </summary>
    public enum CalculationTheoryType
    {
        /// <summary>
        /// 理论 A
        /// </summary>
        A,

        /// <summary>
        /// 基于理论 A 的反向优化
        /// </summary>
        AOptimization,

        /// <summary>
        /// 真正的理论 B
        /// </summary>
        B,
        /// <summary>
        /// 基于理论 B 的反向优化
        /// </summary>
        BOptimization
    }
}