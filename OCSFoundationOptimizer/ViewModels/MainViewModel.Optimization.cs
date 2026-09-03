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
            try
            {
                // =================================================
                // 理论 A 优化
                // =================================================

                if (CurrentTheory ==
                    CalculationTheoryType.AOptimization)
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
                            "理论 A 优化参数存在错误，请检查输入。";

                        return;
                    }


                    CalculationStatus =
                        "正在搜索理论 A 最优基础尺寸...";


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
                    else
                    {
                        CalculationStatus =
                            result.ErrorMessage ??
                            "理论 A 优化失败";
                    }

                    return;
                }


                // =================================================
                // 理论 B 优化
                // =================================================

                if (CurrentTheory ==
                    CalculationTheoryType.BOptimization)
                {
                    if (!AreAllTheoryBParametersValid())
                    {
                        CalculationStatus =
                            "理论 B 基础参数存在错误，请检查输入。";

                        return;
                    }

                    if (!AreTheoryBOptimizationParametersValid())
                    {
                        CalculationStatus =
                            "理论 B 优化参数存在错误，请检查输入。";

                        return;
                    }


                    CalculationStatus =
                        "正在搜索理论 B 最优基础尺寸...";


                    var allParameters =
                        TheoryBParameters
                            .Concat(
                                TheoryBOptimizationParameters)
                            .ToList();


                    var result =
                        _calculationService.Calculate(
                            CalculationTheoryType.BOptimization,
                            allParameters);


                    ShowResult(result);


                    if (result.IsSuccess)
                    {
                        CalculationStatus =
                            "理论 B 优化完成";
                    }
                    else
                    {
                        CalculationStatus =
                            result.ErrorMessage ??
                            "理论 B 优化失败";
                    }

                    return;
                }


                CalculationStatus =
                    "当前模式不支持优化。";
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