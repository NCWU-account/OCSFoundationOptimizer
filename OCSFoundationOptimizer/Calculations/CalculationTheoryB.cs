using System;
using System.Collections.Generic;
using System.Linq;
using OCSFoundationOptimizer.Calculations;
using OCSFoundationOptimizer.Models;

public class CalculationTheoryB : ICalculationModule
{
    public CalculationTheoryType Theory =>
        CalculationTheoryType.B;

    public string Name =>
        "理论 B";

    public CalculationResult Calculate(
        IReadOnlyList<ParameterItem> parameters)
    {
        double a0 = GetNumber(parameters, "B_A0");
        double b0 = GetNumber(parameters, "B_B0");

        // B 的计算公式
        double result = a0 * b0;

        return new CalculationResult
        {
            IsSuccess = true
        };
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
                $"找不到理论 B 参数：{key}");
        }

        if (!double.TryParse(
                parameter.Value,
                out double value))
        {
            throw new Exception(
                $"理论 B 参数 {key} 不是有效数字。");
        }

        return value;
    }
}