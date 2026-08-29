using OCSFoundationOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCSFoundationOptimizer.Calculations
{
    public class CalculationTheoryA : ICalculationModule
    {
        public CalculationTheoryType Theory =>
            CalculationTheoryType.A;

        public string Name =>
            "计算理论 A";


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
                    GetNumber(
                        parameters,
                        "a0") / 1000.0;

                double b0 =
                    GetNumber(
                        parameters,
                        "b0") / 1000.0;

                double a1 =
                    GetNumber(
                        parameters,
                        "a1") / 1000.0;

                double b1 =
                    GetNumber(
                        parameters,
                        "b1") / 1000.0;

                double h =
                    GetNumber(
                        parameters,
                        "h") / 1000.0;

                double h0 =
                    GetNumber(
                        parameters,
                        "h_0") / 1000.0;

                double Delta_a =
                    GetNumber(
                        parameters,
                        "Delta_a") / 1000.0;

                double Delta_b =
                    GetNumber(
                        parameters,
                        "Delta_b") / 1000.0;

                double Delta_h =
                    GetNumber(
                        parameters,
                        "Delta_h") / 1000.0;

                double na =
                    GetNumber(
                        parameters,
                        "na");

                double nb =
                    GetNumber(
                        parameters,
                        "nb");

                double nh =
                    GetNumber(
                        parameters,
                        "nh");
                
                double M1 =
                    GetNumber(
                        parameters,
                        "M1");

                double PH1 =
                    GetNumber(
                        parameters,
                        "PH1");

                double M2 =
                    GetNumber(
                        parameters,
                        "M2");

                double PH2 =
                    GetNumber(
                        parameters,
                        "PH2");

                double Q =
                    GetNumber(
                        parameters,
                        "Q");

                double fak =
                    GetNumber(
                        parameters,
                        "fak");

                double r =
                    GetNumber(
                        parameters,
                        "r");

                double fai =
                    GetNumber(
                        parameters,
                        "fai");
                
                double f =
                    GetNumber(
                        parameters,
                        "f");

                double k0 =
                    GetNumber(
                        parameters,
                        "[k_0]");

                double kc =
                    GetNumber(
                        parameters,
                        "[k_c]");
                
                double nd =
                    GetNumber(
                        parameters,
                        "nd");



                // ==========================================
                // 计算过程参数
                // ==========================================
                
                double Kp = CountKp(fai);
                result.ProcessParameters.Add(
                    new ParameterItem
                    {
                        Key = "k_p",
                        Name = "被动土压力系数Kp",
                        Value = Kp.ToString("F3"),
                        Unit = "",
                        IsReadOnly = true
                    });
                
                double Ep = (r*(h-h0)*(h-h0)*Kp)/2;
                result.ProcessParameters.Add(
                    new ParameterItem
                    {
                        Key = "E_p",
                        Name = "被动土压力Ep",
                        Value = Ep.ToString("F3"),
                        Unit = "KN",
                        IsReadOnly = true
                    });
                
                double rz = 20;
                result.ProcessParameters.Add(
                    new ParameterItem
                    {
                        Key = "rz",
                        Name = "建筑物+回填土的中和重度rz",
                        Value = rz.ToString("F3"),
                        Unit = "kN/m³",
                        IsReadOnly = true
                    });

                double hc = (h-h0)/3;
                result.ProcessParameters.Add(
                    new ParameterItem
                    {
                        Key = "hc",
                        Name = "被动土压力合力作用点距基底高度hc",
                        Value = hc.ToString("F3"),
                        Unit = "m",
                        IsReadOnly = true
                    });
                
                double d = h-h0;
                result.ProcessParameters.Add(
                    new ParameterItem
                    {
                        Key = "d",
                        Name = "埋置深度d",
                        Value = d.ToString("F3"),
                        Unit = "m",
                        IsReadOnly = true
                    });
                
                double fa = fak+nd*r*(d-0.5);
                result.ProcessParameters.Add( 
                    new ParameterItem
                    {
                        Key = "f_a",
                        Name = "修正后地基承载力特征值fa",
                        Value = fa.ToString("F3"),
                        Unit = "kPa",
                        IsReadOnly = true
                    });
                
                double e = ((M1+PH1*h)-Ep*hc)/(Q+a1*b1*(h-h0)*rz);
                result.ProcessParameters.Add( 
                    new ParameterItem
                    {
                        Key = "e",
                        Name = "基础底面偏心距e",
                        Value = e.ToString("F3"),
                        Unit = "",
                        IsReadOnly = true
                    });
                
                double e0 = a1/6;
                result.ProcessParameters.Add( 
                    new ParameterItem
                    {
                        Key = "e0",
                        Name = "偏心距限值e0",
                        Value = e0.ToString("F3"),
                        Unit = "",
                        IsReadOnly = true
                    });


                // ==========================================
                // 最终结果
                // ==========================================

                double resultK_0 =
                    (((Q+a1*b1*(h-h0)*rz)*a1/2)+Ep*hc)/(M1+PH1*h);

                result.ResultParameters.Add(
                    new ParameterItem
                    {
                        Key = "K_0",
                        Name = "抗倾覆稳定安全系数K_0",
                        Value = resultK_0.ToString("F3"),
                        Unit = "",
                        IsReadOnly = true
                    });

                double resultK_c =
                    ((Q+a1*b1*(h-h0)*rz)*f+Ep)/(PH1);

                result.ResultParameters.Add(
                    new ParameterItem
                    {
                        Key = "K_c",
                        Name = "抗滑稳定安全系数K_c",
                        Value = resultK_c.ToString("F3"),
                        Unit = "",
                        IsReadOnly = true
                    });


                double resultp_k =
                    (Q+a1*b1*(h-h0)*rz)/(a1*b1);

                result.ResultParameters.Add(
                    new ParameterItem
                    {
                        Key = "P_k",
                        Name = "轴心荷载作用基地压力P_k",
                        Value = resultp_k.ToString("F3"),
                        Unit = "kPa",
                        IsReadOnly = true
                    });

                double resultS_min =
                    ((Q+a1*b1*(h-h0)*rz)/(a1*b1))-((M1+PH1*h-(Ep*hc))/(a1*a1*b1/6));

                result.ResultParameters.Add(
                    new ParameterItem
                    {
                        Key = "S_min",
                        Name = "基地最小压力S_min",
                        Value = resultS_min.ToString("F3"),
                        Unit = "kPa",
                        IsReadOnly = true
                    });


                double resultS_max =
                    ((Q+a1*b1*(h-h0)*rz)/(a1*b1))+((M1+PH1*h-(Ep*hc))/(a1*a1*b1/6));

                result.ResultParameters.Add(
                    new ParameterItem
                    {
                        Key = "S_max",
                        Name = "基地最大压力S_max",
                        Value = resultS_max.ToString("F3"),
                        Unit = "kPa",
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
        
        // 计算被动土压力系数Kp
        public static double CountKp(double phiDegrees)
        {
            // 计算角度：45° + φ/2
            double angleDegrees = 45.0 + phiDegrees / 2.0;
            // 转换为弧度
            double angleRadians = angleDegrees * Math.PI / 180.0;
            // 计算 tan 并平方
            double t = Math.Tan(angleRadians);
            return t * t;
        }
        
        
    }
}