using OCSFoundationOptimizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using OCSFoundationOptimizer.CalculationBooks.CalculationBookUtil;

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
            // 获取计算结果
            List<ParameterItem> inputParameter = result.InputParameters;
            List<ParameterItem> processParameter = result.ProcessParameters;
            List<ParameterItem> resultParameter = result.ResultParameters;


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
            // 获取参数值
            double r =
                GetNumber(
                    inputParameter,
                    "r");
            
            // 执行计算书生成逻辑
            var data = new Dictionary<string, string>
            {
                { "test", "Hello, World!wwww" },
                { "ok", "bad你好还" },
                { "name", "张三" },
                { "hello", "hhhhh" }
            };
            
            WordHelper.ReplaceContentControls(templatePath, filePath, data);
            Console.WriteLine("生成成功！");


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

        private double GetNumber(
            IReadOnlyList<ParameterItem> parameters,
            string key)
        {
            var parameter =
                parameters.FirstOrDefault(x => x.Key == key);

            if (parameter == null)
            {
                throw new Exception(
                    $"找不到参数：{key}");
            }

            if (!double.TryParse(
                    parameter.Value,
                    out double value))
            {
                throw new Exception(
                    $"参数【{parameter.Name}】不是有效数字");
            }

            return value;
        }
        // private double GetNumber(
        //     CalculationResult result,
        //     string key)
        // {
        //     var parameter =
        //         result.InputParameters.FirstOrDefault(
        //             x => x.Key == key)
        //         ?? result.ProcessParameters.FirstOrDefault(
        //             x => x.Key == key)
        //         ?? result.ResultParameters.FirstOrDefault(
        //             x => x.Key == key);
        //
        //     if (parameter == null)
        //     {
        //         throw new Exception(
        //             $"找不到参数：{key}");
        //     }
        //
        //     if (!double.TryParse(
        //             parameter.Value,
        //             NumberStyles.Float,
        //             CultureInfo.InvariantCulture,
        //             out double value))
        //     {
        //         throw new Exception(
        //             $"参数【{parameter.Name}】不是有效数字");
        //     }
        //
        //     return value;
        // }
    }
}