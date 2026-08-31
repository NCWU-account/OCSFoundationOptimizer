using System;
using System.Collections.Generic;
using System.Linq;
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
            List<ParameterItem> inputParameter =result.InputParameters;
            List<ParameterItem> processParameter =result.ProcessParameters;
            List<ParameterItem> resultParameter =result.ResultParameters;
            
            
            
            // 保存Word
        }
        
        private double GetNumber(
            IReadOnlyList<ParameterItem> parameters,
            string key)
        {
            var parameter =
                parameters.FirstOrDefault(
                    x => x.Key == key);

            if (parameter == null)
            {
                throw new Exception(
                    $"找不到参数：{key}");
            }

            if (!double.TryParse(
                    parameter.Value,
                    out double value))
            {
                throw new Exception(
                    $"参数【{parameter.Name}】不是有效数字");
            }

            return value;
        }
        
    }
    
}