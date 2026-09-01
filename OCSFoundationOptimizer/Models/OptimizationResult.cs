namespace OCSFoundationOptimizer.Models
{
    public class OptimizationResult
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; } = "";

        public double A1 { get; set; }

        public double B1 { get; set; }

        public double H { get; set; }

        public double K0 { get; set; }

        public double Kc { get; set; }

        public double SMin { get; set; }

        public double SMax { get; set; }

        public double Volume
        {
            get
            {
                return A1 * B1 * H;
            }
        }
    }
}