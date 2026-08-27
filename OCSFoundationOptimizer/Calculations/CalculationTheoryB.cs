using OCSFoundationOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCSFoundationOptimizer.Calculations
{
    public class CalculationTheoryB : ICalculationModule
    {
        public CalculationTheoryType Theory =>
            CalculationTheoryType.B;

        public string Name =>
            "计算理论 B";


        public CalculationResult Calculate(
            IReadOnlyList<ParameterItem> parameters)
        {
            var result = new CalculationResult();

            try
            {
                // ==========================================
                // 保存输入参数
                // ==========================================

                result.InputParameters =
                    parameters
                        .Select(x => new ParameterItem
                        {
                            Key = x.Key,
                            Name = x.Name,
                            Value = x.Value,
                            Unit = x.Unit,
                            Type = x.Type
                        })
                        .ToList();


                // ==========================================
                // 获取输入参数
                // ==========================================

                double width =
                    GetNumber(
                        parameters,
                        "a0");

                double length =
                    GetNumber(
                        parameters,
                        "b0");

                double height =
                    GetNumber(
                        parameters,
                        "a1");


                // ==========================================
                // 计算过程参数
                // ==========================================

                double area =
                    width * length;

                result.ProcessParameters.Add(
                    new ParameterItem
                    {
                        Key = "FoundationArea",
                        Name = "基础面积",
                        Value = area.ToString("F3"),
                        Unit = "m²",
                        IsReadOnly = true
                    });


                double volume =
                    area * height;

                result.ProcessParameters.Add(
                    new ParameterItem
                    {
                        Key = "FoundationVolume",
                        Name = "基础体积",
                        Value = volume.ToString("F3"),
                        Unit = "m³",
                        IsReadOnly = true
                    });


                // ==========================================
                // 最终结果
                // ==========================================

                double resultValue =
                    volume * 20;

                result.ResultParameters.Add(
                    new ParameterItem
                    {
                        Key = "ResultValueB",
                        Name = "计算结果",
                        Value = resultValue.ToString("F3"),
                        Unit = "kN",
                        IsReadOnly = true
                    });


                result.IsSuccess = true;

                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;

                return result;
            }
        }


        private double GetNumber(
            IReadOnlyList<ParameterItem> parameters,
            string key)
        {
            var parameter =
                parameters.FirstOrDefault(
                    x => x.Key == key);

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
    }
}