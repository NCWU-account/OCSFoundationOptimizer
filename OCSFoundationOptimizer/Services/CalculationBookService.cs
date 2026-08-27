using System;
using OCSFoundationOptimizer.CalculationBooks;
using OCSFoundationOptimizer.Models;

namespace OCSFoundationOptimizer.Services
{
    public class CalculationBookService
    {
        private readonly ICalculationBookGenerator _bookA;
        private readonly ICalculationBookGenerator _bookB;

        public CalculationBookService()
        {
            _bookA = new CalculationBookA();
            _bookB = new CalculationBookB();
        }

        public void Generate(
            CalculationResult result,
            CalculationTheoryType theory,
            string filePath)
        {
            if (theory == CalculationTheoryType.A)
            {
                _bookA.Generate(
                    result,
                    filePath);

                return;
            }

            if (theory == CalculationTheoryType.B)
            {
                _bookB.Generate(
                    result,
                    filePath);

                return;
            }

            throw new Exception(
                "未知的计算理论");
        }
    }
}