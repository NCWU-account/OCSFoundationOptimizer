using OCSFoundationOptimizer.Models;
using System;
using System.Linq;
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
            // 再次检查参数
            // =================================================

            if (!AreAllParametersValid())
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


            // =====================================================
            // 理论 A
            // =====================================================

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


            // =====================================================
            // 理论 A 优化
            // =====================================================

            else if (CurrentTheory ==
                     CalculationTheoryType.AOptimization)
            {
                CalculationStatus =
                    "当前选择：理论 A 优化，请设置优化目标并开始优化。";
            }


            // =====================================================
            // 理论 B
            // =====================================================

            else if (CurrentTheory ==
                     CalculationTheoryType.B)
            {
                CalculationStatus =
                    "当前选择：理论 B（暂未实现）。";
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
            // 第一步：检查参数
            // =================================================

            if (!AreAllParametersValid())
            {
                CalculationStatus =
                    "参数存在错误，请检查输入";

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
                        InputParameters);


                // =================================================
                // 显示结果
                // =================================================

                ShowResult(result);
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