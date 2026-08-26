using OCSFoundationOptimizer.Models;
using System.Collections.Generic;

namespace OCSFoundationOptimizer.Services
{
    public class FoundationCalculationService
    {
        public List<ParameterItem> Calculate(
            IEnumerable<ParameterItem> parameters)
        {
            var result = new List<ParameterItem>();

            // =====================================================
            // 这里以后替换成你的实际计算公式
            // =====================================================

            result.Add(new ParameterItem
            {
                Name = "计算结果1",
                Value = "100"
            });

            result.Add(new ParameterItem
            {
                Name = "计算结果2",
                Value = "200"
            });

            result.Add(new ParameterItem
            {
                Name = "计算结果3",
                Value = "300"
            });

            return result;
        }
    }
}