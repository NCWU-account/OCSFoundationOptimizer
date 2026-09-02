using OCSFoundationOptimizer.Models;
using System.Windows;

namespace OCSFoundationOptimizer.ViewModels
{
    public partial class MainViewModel
    {
        // =====================================================
        // 显示计算结果
        // =====================================================

        private void ShowResult(
            CalculationResult result)
        {
            // =================================================
            // 计算失败
            // =================================================

            if (!result.IsSuccess)
            {
                CurrentResult = null;

                ResultParameters.Clear();


                CalculationStatus =
                    "计算失败";


                MessageBox.Show(
                    result.ErrorMessage,
                    "计算错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // =================================================
            // 保存完整计算结果
            // =================================================

            CurrentResult = result;


            // =================================================
            // 更新前台结果
            // =================================================

            ResultParameters.Clear();


            foreach (var item
                     in result.ResultParameters)
            {
                ResultParameters.Add(item);
            }


            // =================================================
            // 计算成功后自动展开结果
            // =================================================

            IsResultExpanded = true;


            // =================================================
            // 更新计算状态
            // =================================================

            CalculationStatus =
                $"{CurrentTheoryDisplayName} 计算完成";
        }


        // =====================================================
        // 清除当前计算结果
        // =====================================================

        private void ClearCalculationResult()
        {
            CurrentResult = null;

            ResultParameters.Clear();
        }


        // =====================================================
        // 生成计算书
        // =====================================================

        private void GenerateCalculationBook()
        {
            // =================================================
            // 没有计算结果
            // =================================================

            if (CurrentResult == null ||
                !CurrentResult.IsSuccess)
            {
                MessageBox.Show(
                    "请先完成计算，再生成计算书。",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            // =================================================
            // 保存文件
            // =================================================

            var dialog =
                new Microsoft.Win32.SaveFileDialog
                {
                    Title =
                        "生成计算书",

                    Filter =
                        "Word文档 (*.docx)|*.docx",

                    FileName =
                        $"基础计算书_{CurrentTheoryDisplayName}.docx"
                };


            if (dialog.ShowDialog() != true)
            {
                return;
            }


            // =================================================
            // 生成计算书
            // =================================================

            try
            {
                _calculationBookService.Generate(
                    CurrentResult,
                    CurrentTheory,
                    dialog.FileName);


                MessageBox.Show(
                    "计算书生成成功。",
                    "完成",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    $"计算书生成失败：{ex.Message}",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}