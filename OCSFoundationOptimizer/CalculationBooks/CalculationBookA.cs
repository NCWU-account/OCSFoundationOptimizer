using OCSFoundationOptimizer.Models;

namespace OCSFoundationOptimizer.CalculationBooks
{
    public class CalculationBookA
        : ICalculationBookGenerator
    {
        public string Name => "理论A计算书";

        public void Generate(
            CalculationResult result,
            string filePath)
        {
            // ============================
            // 这里以后写 A 理论计算书
            // ============================

            // 1. 创建Word

            // 2. 写入工程信息

            // 3. 写入输入参数
            foreach (var parameter in result.InputParameters)
            {
                // 参数名称
                // 参数值
                // 单位
            }

            // 4. 写入A理论计算过程参数
            foreach (var parameter in result.ProcessParameters)
            {
                // 参数名称
                // 参数值
                // 单位
            }

            // 5. 写入计算结果
            foreach (var parameter in result.ResultParameters)
            {
                // 参数名称
                // 参数值
                // 单位
            }

            // 6. 保存Word
        }
    }
}