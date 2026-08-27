using OCSFoundationOptimizer.Models;

namespace OCSFoundationOptimizer.CalculationBooks
{
    public class CalculationBookB
        : ICalculationBookGenerator
    {
        public string Name => "理论B计算书";

        public void Generate(
            CalculationResult result,
            string filePath)
        {
            // ============================
            // 这里以后写 B 理论计算书
            // ============================

            foreach (var parameter in result.InputParameters)
            {
                // 写入输入参数
            }

            foreach (var parameter in result.ProcessParameters)
            {
                // 写入B理论计算过程
            }

            foreach (var parameter in result.ResultParameters)
            {
                // 写入计算结果
            }

            // 保存Word
        }
    }
}