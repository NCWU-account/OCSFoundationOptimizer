using OCSFoundationOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace OCSFoundationOptimizer.ViewModels
{
    public partial class MainViewModel
    {
        // =====================================================
        // 自动计算 Timer
        // =====================================================

        private void AutoCalculateTimer_Tick(
            object? sender,
            EventArgs e)
        {
            // 停止计时器
            _autoCalculateTimer.Stop();


            // =================================================
            // 根据当前理论检查参数
            // =================================================

            if (CurrentTheory ==
                CalculationTheoryType.A)
            {
                if (!AreAllParametersValid())
                {
                    return;
                }
            }
            else if (CurrentTheory ==
                     CalculationTheoryType.B)
            {
                if (!AreAllTheoryBParametersValid())
                {
                    return;
                }
            }
            else
            {
                return;
            }


            // =================================================
            // 自动执行计算
            // =================================================

            Calculate();
        }


        // =====================================================
        // 选择理论
        // =====================================================

        private void SelectTheory(
            CalculationTheoryType theory)
        {
            CurrentTheory = theory;

            ClearCalculationResult();

            _autoCalculateTimer.Stop();


            // =================================================
            // 理论 A
            // =================================================

            if (CurrentTheory ==
                CalculationTheoryType.A)
            {
                if (AreAllParametersValid())
                {
                    CalculationStatus =
                        "参数已就绪，正在准备计算...";

                    _autoCalculateTimer.Start();
                }
                else
                {
                    CalculationStatus =
                        "当前选择：理论 A";
                }
            }


            // =================================================
            // 理论 A 优化
            // =================================================

            else if (CurrentTheory ==
                     CalculationTheoryType.AOptimization)
            {
                CalculationStatus =
                    "当前选择：理论 A 优化，请设置优化目标并开始优化。";
            }


            // =================================================
            // 理论 B
            // =================================================

            else if (CurrentTheory ==
                     CalculationTheoryType.B)
            {
                if (AreAllTheoryBParametersValid())
                {
                    CalculationStatus =
                        "参数已就绪，正在准备计算...";

                    _autoCalculateTimer.Start();
                }
                else
                {
                    CalculationStatus =
                        "当前选择：理论 B";
                }
            }


            OnPropertyChanged(
                nameof(CanCalculate));

            OnPropertyChanged(
                nameof(CanOptimize));

            CommandManager
                .InvalidateRequerySuggested();
        }


        // =====================================================
        // 执行计算
        // =====================================================

        private void Calculate()
        {
            // =================================================
            // 根据当前理论取得参数
            // =================================================

            IReadOnlyList<ParameterItem> parameters;


            if (CurrentTheory ==
                CalculationTheoryType.A)
            {
                if (!AreAllParametersValid())
                {
                    CalculationStatus =
                        "参数存在错误，请检查输入";

                    return;
                }

                parameters =
                    InputParameters;
            }
            else if (CurrentTheory ==
                     CalculationTheoryType.B)
            {
                if (!AreAllTheoryBParametersValid())
                {
                    CalculationStatus =
                        "理论 B 参数存在错误，请检查输入";

                    return;
                }

                parameters =
                    TheoryBParameters;
            }
            else
            {
                return;
            }


            try
            {
                CalculationStatus =
                    $"正在执行 {CurrentTheoryDisplayName} 计算...";


                // =================================================
                // 调用统一计算服务
                // =================================================

                var result =
                    _calculationService.Calculate(
                        CurrentTheory,
                        parameters);


                // =================================================
                // 显示结果
                // =================================================

                ShowResult(result);


                // =================================================
                // 更新计算状态
                // =================================================

                if (result.IsSuccess)
                {
                    CalculationStatus =
                        $"{CurrentTheoryDisplayName} 计算完成";
                }
                else
                {
                    CalculationStatus =
                        result.ErrorMessage ??
                        $"{CurrentTheoryDisplayName} 计算失败";
                }
            }
            catch (Exception ex)
            {
                CalculationStatus =
                    "计算过程中发生错误";


                MessageBox.Show(
                    ex.Message,
                    "计算错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}