using OCSFoundationOptimizer.Calculations;
using OCSFoundationOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCSFoundationOptimizer.Services
{
    public class CalculationService
    {
        private readonly Dictionary<
            CalculationTheoryType,
            ICalculationModule> _modules;


        public CalculationService()
        {
            var modules =
                new List<ICalculationModule>
                {
                    new CalculationTheoryA(),
                    new CalculationTheoryB()
                };

            _modules =
                modules.ToDictionary(
                    x => x.Theory,
                    x => x);
        }


        /// <summary>
        /// 执行计算
        /// </summary>
        public CalculationResult Calculate(
            CalculationTheoryType theory,
            IReadOnlyList<ParameterItem> parameters)
        {
            if (!_modules.TryGetValue(
                    theory,
                    out var module))
            {
                throw new Exception(
                    $"没有找到计算理论：{theory}");
            }

            return module.Calculate(parameters);
        }
    }
}