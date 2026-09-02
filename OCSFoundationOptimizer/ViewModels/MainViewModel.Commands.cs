using OCSFoundationOptimizer.Models;

namespace OCSFoundationOptimizer.ViewModels
{
    public partial class MainViewModel
    {
        // =====================================================
        // 初始化命令
        // =====================================================

        private void InitializeCommands()
        {
            // =====================================================
            // 理论 A
            // =====================================================

            SelectTheoryACommand =
                new RelayCommand(_ =>
                {
                    SelectTheory(
                        CalculationTheoryType.A);
                });


            // =====================================================
            // 理论 A 优化
            // =====================================================

            SelectTheoryAOptimizationCommand =
                new RelayCommand(_ =>
                {
                    SelectTheory(
                        CalculationTheoryType.AOptimization);
                });


            // =====================================================
            // 理论 B
            // =====================================================

            SelectTheoryBCommand =
                new RelayCommand(_ =>
                {
                    SelectTheory(
                        CalculationTheoryType.B);
                });


            // =====================================================
            // 执行计算
            // =====================================================

            CalculateCommand =
                new RelayCommand(
                    _ => { Calculate(); },
                    _ => CanCalculate);


            // =====================================================
            // A 优化
            // =====================================================

            OptimizeCommand =
                new RelayCommand(
                    _ => { Optimize(); },
                    _ =>
                    {
                        return
                            CurrentTheory ==
                            CalculationTheoryType.AOptimization &&
                            CanOptimize;
                    });


            // =====================================================
            // 生成计算书
            // =====================================================

            GenerateCalculationBookCommand =
                new RelayCommand(_ => { GenerateCalculationBook(); });


            // =====================================================
            // 展开 / 折叠计算结果
            // =====================================================

            ToggleResultCommand =
                new RelayCommand(_ =>
                {
                    IsResultExpanded =
                        !IsResultExpanded;
                });
        }
    }
}