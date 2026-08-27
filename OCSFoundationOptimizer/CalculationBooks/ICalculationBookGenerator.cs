using OCSFoundationOptimizer.Models;

namespace OCSFoundationOptimizer.CalculationBooks
{
    public interface ICalculationBookGenerator
    {
        string Name { get; }

        void Generate(
            CalculationResult result,
            string filePath);
    }
}