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
        "蓝皮书";

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

            double a0 =
                GetNumber(parameters, "a0") / 1000.0;

            double b0 =
                GetNumber(parameters, "b0") / 1000.0;

            double a1 =
                GetNumber(parameters, "a1") / 1000.0;

            double b1 =
                GetNumber(parameters, "b1") / 1000.0;

            double h =
                GetNumber(parameters, "h") / 1000.0;

            double h0 =
                GetNumber(parameters, "h_0") / 1000.0;

            double Delta_a =
                GetNumber(parameters, "Delta_a") / 1000.0;

            double Delta_b =
                GetNumber(parameters, "Delta_b") / 1000.0;

            double Delta_h =
                GetNumber(parameters, "Delta_h") / 1000.0;
            
            double H =
                GetNumber(parameters, "H") ;


            double M1 =
                GetNumber(parameters, "M1");

            double PH1 =
                GetNumber(parameters, "PH1");

            double M2 =
                GetNumber(parameters, "M2");

            double PH2 =
                GetNumber(parameters, "PH2");

            double Q =
                GetNumber(parameters, "Q");

            double G0 =
                GetNumber(parameters, "G0");


            double fak =
                GetNumber(parameters, "fak");

            double r =
                GetNumber(parameters, "r");

            double fai =
                GetNumber(parameters, "fai");


            double k0 =
                GetNumber(parameters, "[k_0]");

            double kc =
                GetNumber(parameters, "[k_c]");

            double nd =
                GetNumber(parameters, "nd");

            double kr =
                GetNumber(parameters, "kr");


            // ==========================================
            // 计算过程参数
            // ==========================================

            double σh = 200 + (kr * r * (h - h0 - 2));
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "σ_h",
                    Name = "基础底部土壤的允许承载力[σh]",
                    Value = σh.ToString("F3"),
                    Unit = "kN/m2",
                    IsReadOnly = true
                });

            double a = (h - h0) / h;
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "a",
                    Name = "基础转动中心位置的系数a",
                    Value = a.ToString("F3"),
                    Unit = "",
                    IsReadOnly = true
                });

            double m = 12 / (6 * a * a - 8 * a + 3);
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "m",
                    Name = "基础转动中心的位置决定的系数m",
                    Value = m.ToString("F3"),
                    Unit = "",
                    IsReadOnly = true
                });

            double Vɸ = (a0 * b0 * (h - 2 * Delta_h) + (a1 * b1 * Delta_h) +
                         ((a0 + Delta_a) * (b0 + Delta_b) * Delta_h));
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "Vɸ",
                    Name = "基础体积Vɸ",
                    Value = Vɸ.ToString("F3"),
                    Unit = "m³",
                    IsReadOnly = true
                });

            double Gϕ = 25 * Vɸ;
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "Gɸ",
                    Name = "基础自重Gɸ",
                    Value = Gϕ.ToString("F3"),
                    Unit = "kN",
                    IsReadOnly = true
                });

            double Gr = ((a1 * b1 * (h - h0)) - (Vɸ - a0 * b0 * h0)) * r;
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "Gr",
                    Name = "基础台阶上覆土的重量Gr",
                    Value = Gr.ToString("F3"),
                    Unit = "kN",
                    IsReadOnly = true
                });

            double G = Gϕ + Gr + G0;
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "G",
                    Name = "作用于基底的总垂直力G",
                    Value = G.ToString("F3"),
                    Unit = "kN",
                    IsReadOnly = true
                });

            double M1B = (σh * σh * b0 * b1 * ((h - h0) * (h - h0) * (h - h0)) / (2.4 * m * G));
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "M1B",
                    Name = "基础前后面土壤抗压力的力矩M1",
                    Value = M1B.ToString("F3"),
                    Unit = "kN∙m",
                    IsReadOnly = true
                });

            double σ0 = G / (a1 * b1);
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "σ0",
                    Name = "σ0",
                    Value = σ0.ToString("F3"),
                    Unit = "kN/m2",
                    IsReadOnly = true
                });
            

            double M2B = G * a1 * (0.5 - ((2 * σ0) / (3 * σh)));
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "M2B",
                    Name = "基础底部的土壤抵抗力矩M2",
                    Value = M2B.ToString("F3"),
                    Unit = "kN∙m",
                    IsReadOnly = true
                });

            double f = Math.Tan(fai * Math.PI / 180.0);
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "f",
                    Name = "土壤的内部摩擦系数f-理论B",
                    Value = f.ToString("F3"),
                    Unit = "",
                    IsReadOnly = true
                });
            
            double M3B = (r*f/24 )*((h - h0) * (h - h0) * a1 * (10 * b1 + 7 * a1));
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = "M3B",
                    Name = "摩擦力矩M3",
                    Value = M3B.ToString("F3"),
                    Unit = "kN∙m",
                    IsReadOnly = true
                });



            // ==========================================
            // 最终结果
            // ==========================================

            double Mr =
                M1B+M2B+M3B;

            result.ResultParameters.Add(
                new ParameterItem
                {
                    Key = "M_r",
                    Name = "抗倾覆力矩Mr",
                    Value = Mr.ToString("F3"),
                    Unit = " kN∙m",
                    IsReadOnly = true
                });
            
            double M0 = M1/H*(H+h0+a*(h-h0));

            result.ResultParameters.Add(
                new ParameterItem
                {
                    Key = "M_0",
                    Name = "倾覆力矩M0",
                    Value = M0.ToString("F3"),
                    Unit = " kN∙m",
                    IsReadOnly = true
                });
            
            double resultK =
                Mr / M0;

            result.ResultParameters.Add(
                new ParameterItem
                {
                    Key = "K",
                    Name = "抗倾覆稳定安全系数K--(理论B)",
                    Value = resultK.ToString("F3"),
                    Unit = "",
                    IsReadOnly = true
                });


            // ==========================================
            // 计算完成
            // ==========================================

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
            parameters.FirstOrDefault(x => x.Key == key);

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