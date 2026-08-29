using OCSFoundationOptimizer.Models;
using System;
using System.IO;
using System.Diagnostics;

namespace OCSFoundationOptimizer.CalculationBooks
{
    public class CalculationBookA
        : ICalculationBookGenerator
    {
        public string Name => "理论A计算书";

        public void Generate(
            CalculationResult result,
            string filePath)
        {
            // =================================================
            // 1. 获取模板路径
            // =================================================

            string templatePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "pic",
                "基础支柱计算书模板.docx");


            // =================================================
            // 2. 检查模板是否存在
            // =================================================

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException(
                    "找不到基础支柱计算书模板。",
                    templatePath);
            }


            // =================================================
            // 3. 复制模板到用户指定位置
            // =================================================

            File.Copy(
                templatePath,
                filePath,
                true);
            
            // =================================================
            // 4. 自动打开生成的Word文档
            // =================================================

            Process.Start(
                new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
        }
        
    }
}