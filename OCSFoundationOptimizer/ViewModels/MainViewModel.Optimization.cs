using OCSFoundationOptimizer.Models;
using System;
using System.Linq;
using System.Windows;

namespace OCSFoundationOptimizer.ViewModels
{
    public partial class MainViewModel
    {
        // =====================================================
        // 执行优化
        // =====================================================

        private void Optimize()
        {
            if (!AreAllParametersValid())
            {
                CalculationStatus =
                    "基础计算参数存在错误，请检查输入。";

                return;
            }


            if (!AreOptimizationParametersValid())
            {
                CalculationStatus =
                    "优化参数存在错误，请检查输入。";

                return;
            }


            try
            {
                CalculationStatus =
                    "正在搜索最优基础尺寸...";


                var allParameters =
                    InputParameters
                        .Concat(OptimizationParameters)
                        .ToList();


                var result =
                    _calculationService.Calculate(
                        CalculationTheoryType.AOptimization,
                        allParameters);


                ShowResult(result);


                if (result.IsSuccess)
                {
                    CalculationStatus =
                        "理论 A 优化完成";
                }
            }
            catch (Exception ex)
            {
                CalculationStatus =
                    "优化过程中发生错误";


                MessageBox.Show(
                    ex.Message,
                    "优化错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}