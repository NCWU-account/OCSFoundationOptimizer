
using OCSFoundationOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCSFoundationOptimizer.Calculations
{
    public class CalculationAOptimization : ICalculationModule
    {
        // ====================================================
        // 计算理论类型
        // ====================================================
        public CalculationTheoryType Theory =>
            CalculationTheoryType.AOptimization;

        public string Name =>
            "理论 A 优化";

        public CalculationResult Calculate(
            IReadOnlyList<ParameterItem> parameters)
        {
            var result = new CalculationResult();

            try
            {
                // =====================================================
                // 保存输入参数
                // =====================================================

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


                // =====================================================
                // 公共计算参数
                // =====================================================

                double a0 =
                    GetNumber(parameters, "a0") / 1000.0;

                double b0 =
                    GetNumber(parameters, "b0") / 1000.0;

                double h0 =
                    GetNumber(parameters, "h_0") / 1000.0;

                double na =
                    GetNumber(parameters, "na");

                double nb =
                    GetNumber(parameters, "nb");

                double nh =
                    GetNumber(parameters, "nh");

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

                double fak =
                    GetNumber(parameters, "fak");

                double r =
                    GetNumber(parameters, "r");

                double fai =
                    GetNumber(parameters, "fai");

                double f =
                    GetNumber(parameters, "f");
                double nd =
                    GetNumber(parameters, "nd");
                // =====================================================
                // B 专用优化目标
                // =====================================================

                double targetK0 =
                    GetNumber(parameters, "B_K0_MIN");

                double targetKc =
                    GetNumber(parameters, "B_KC_MIN");


                // =====================================================
                // B 专用尺寸范围
                // =====================================================

                double a1Min =
                    GetNumber(parameters, "B_A1_MIN");

                double a1Max =
                    GetNumber(parameters, "B_A1_MAX");

                double deltaA =
                    GetNumber(parameters, "B_DELTA_A");

                double b1Min =
                    GetNumber(parameters, "B_B1_MIN");

                double b1Max =
                    GetNumber(parameters, "B_B1_MAX");

                double deltaB =
                    GetNumber(parameters, "B_DELTA_B");

                double hMin =
                    GetNumber(parameters, "B_H_MIN");

                double hMax =
                    GetNumber(parameters, "B_H_MAX");

                double deltaH =
                    GetNumber(parameters, "B_DELTA_H");


                // =====================================================
                // 参数合法性检查
                // =====================================================

                if (a1Min <= 0 ||
                    b1Min <= 0 ||
                    hMin <= 0)
                {
                    throw new Exception(
                        "优化尺寸最小值必须大于 0。");
                }

                if (a1Max < a1Min)
                {
                    throw new Exception(
                        "a1 最大值不能小于最小值。");
                }

                if (b1Max < b1Min)
                {
                    throw new Exception(
                        "b1 最大值不能小于最小值。");
                }

                if (hMax < hMin)
                {
                    throw new Exception(
                        "h 最大值不能小于最小值。");
                }

                if (deltaA <= 0 ||
                    deltaB <= 0 ||
                    deltaH <= 0)
                {
                    throw new Exception(
                        "优化步长必须大于 0。");
                }


                // =====================================================
                // 被动土压力系数
                // =====================================================

                double Kp =
                    CountKp(fai);


                // =====================================================
                // 开始搜索最优方案
                //
                // 目标：
                //
                // K0 >= targetK0
                // Kc >= targetKc
                //
                // 并且：
                //
                // a1 × b1 × h 最小
                // =====================================================

                bool found = false;

                double bestA1 = 0;
                double bestB1 = 0;
                double bestH = 0;

                double bestK0 = 0;
                double bestKc = 0;
                double bestSMin = 0;
                double bestSMax = 0;

                double bestVolume =
                    double.MaxValue;


                for (
                    double a1mm = a1Min;
                    a1mm <= a1Max + 0.000001;
                    a1mm += deltaA)
                {
                    double a1 = a1mm / 1000.0;

                    for (
                        double b1mm = b1Min;
                        b1mm <= b1Max + 0.000001;
                        b1mm += deltaB)
                    {
                        double b1 = b1mm / 1000.0;

                        for (
                            double hmm = hMin;
                            hmm <= hMax + 0.000001;
                            hmm += deltaH)
                        {
                            double h =
                                hmm / 1000.0;


                            // =================================================
                            // 计算候选方案
                            // =================================================

                            double Ep =
                                (r *
                                 (h - h0) *
                                 (h - h0) *
                                 Kp) / 2.0;

                            double rz = 20.0;

                            double hc =
                                (h - h0) / 3.0;

                            double d =
                                h - h0;

                            double fa =
                                fak +
                                nd *
                                r *
                                (d - 0.5);

                            double denominator =
                                Q +
                                a1 *
                                b1 *
                                (h - h0) *
                                rz;

                            double K0 =
                                (
                                    (
                                        denominator *
                                        a1 / 2.0
                                    )
                                    +
                                    Ep * hc
                                )
                                /
                                (M1 + PH1 * h);


                            double Kc =
                                (
                                    denominator * f
                                    +
                                    Ep
                                )
                                /
                                PH1;


                            // =================================================
                            // 基地压力
                            // =================================================

                            double Pk =
                                denominator /
                                (a1 * b1);

                            double SMin =
                                Pk -
                                (
                                    (
                                        M1 +
                                        PH1 * h -
                                        Ep * hc
                                    )
                                    /
                                    (
                                        a1 *
                                        a1 *
                                        b1 / 6.0
                                    )
                                );

                            double SMax =
                                Pk +
                                (
                                    (
                                        M1 +
                                        PH1 * h -
                                        Ep * hc
                                    )
                                    /
                                    (
                                        a1 *
                                        a1 *
                                        b1 / 6.0
                                    )
                                );


                            // =================================================
                            // 判断安全条件
                            // =================================================

                            if (K0 < targetK0)
                                continue;

                            if (Kc < targetKc)
                                continue;


                            // =================================================
                            // 当前方案满足要求
                            //
                            // 判断体积是否更小
                            // =================================================

                            double volume =
                                a1 * b1 * h;

                            if (volume < bestVolume)
                            {
                                found = true;

                                bestVolume = volume;

                                bestA1 = a1mm;
                                bestB1 = b1mm;
                                bestH = hmm;

                                bestK0 = K0;
                                bestKc = Kc;

                                bestSMin = SMin;
                                bestSMax = SMax;
                            }
                        }
                    }
                }


                // =====================================================
                // 没有找到方案
                // =====================================================

                if (!found)
                {
                    result.IsSuccess = false;

                    result.ErrorMessage =
                        "在当前设定的优化范围内，没有找到同时满足 K₀ 和 Kc 要求的基础尺寸方案。";

                    return result;
                }


                // =====================================================
                // 保存最终优化结果
                // =====================================================

                AddResult(
                    result,
                    "B_A1",
                    "优化后基础长度a1",
                    bestA1.ToString("F0"),
                    "mm");

                AddResult(
                    result,
                    "B_B1",
                    "优化后基础宽度b1",
                    bestB1.ToString("F0"),
                    "mm");

                AddResult(
                    result,
                    "B_H",
                    "优化后基础高度h",
                    bestH.ToString("F0"),
                    "mm");

                AddResult(
                    result,
                    "B_VOLUME",
                    "基础体积",
                    (
                        bestA1 *
                        bestB1 *
                        bestH /
                        1000000000.0
                    ).ToString("F3"),
                    "m³");


                // =====================================================
                // 安全校核结果
                // =====================================================

                AddResult(
                    result,
                    "K_0",
                    "抗倾覆稳定安全系数K₀",
                    bestK0.ToString("F3"),
                    "");

                AddResult(
                    result,
                    "K_c",
                    "抗滑稳定安全系数Kc",
                    bestKc.ToString("F3"),
                    "");

                AddResult(
                    result,
                    "K_0_TARGET",
                    "K₀允许最小值",
                    targetK0.ToString("F3"),
                    "");

                AddResult(
                    result,
                    "K_C_TARGET",
                    "Kc允许最小值",
                    targetKc.ToString("F3"),
                    "");


                // =====================================================
                // 其他最终结果
                // =====================================================

                AddResult(
                    result,
                    "P_k",
                    "轴心荷载作用基地压力Pk",
                    (
                        CalculatePk(
                            Q,
                            bestA1 / 1000.0,
                            bestB1 / 1000.0,
                            bestH / 1000.0,
                            h0,
                            r)
                    ).ToString("F3"),
                    "kPa");

                AddResult(
                    result,
                    "S_min",
                    "基地最小压力Smin",
                    bestSMin.ToString("F3"),
                    "kPa");

                AddResult(
                    result,
                    "S_max",
                    "基地最大压力Smax",
                    bestSMax.ToString("F3"),
                    "kPa");


                // =====================================================
                // 计算过程参数
                // =====================================================

                double finalA1 =
                    bestA1 / 1000.0;

                double finalB1 =
                    bestB1 / 1000.0;

                double finalH =
                    bestH / 1000.0;

                double finalEp =
                    (
                        r *
                        (finalH - h0) *
                        (finalH - h0) *
                        Kp
                    ) / 2.0;

                double finalHc =
                    (finalH - h0) / 3.0;

                double finalD =
                    finalH - h0;

                double finalRz = 20.0;

                double finalFa =
                    fak +
                    nd *
                    r *
                    (finalD - 0.5);

                double finalE =
                    (
                        (M1 + PH1 * finalH)
                        -
                        finalEp * finalHc
                    )
                    /
                    (
                        Q +
                        finalA1 *
                        finalB1 *
                        (finalH - h0) *
                        finalRz
                    );

                double finalE0 =
                    finalA1 / 6.0;


                AddProcess(
                    result,
                    "k_p",
                    "被动土压力系数Kp",
                    Kp.ToString("F3"),
                    "");

                AddProcess(
                    result,
                    "E_p",
                    "被动土压力Ep",
                    finalEp.ToString("F3"),
                    "kN");

                AddProcess(
                    result,
                    "rz",
                    "建筑物+回填土的中和重度rz",
                    finalRz.ToString("F3"),
                    "kN/m³");

                AddProcess(
                    result,
                    "hc",
                    "被动土压力合力作用点距基底高度hc",
                    finalHc.ToString("F3"),
                    "m");

                AddProcess(
                    result,
                    "d",
                    "埋置深度d",
                    finalD.ToString("F3"),
                    "m");

                AddProcess(
                    result,
                    "f_a",
                    "修正后地基承载力特征值fa",
                    finalFa.ToString("F3"),
                    "kPa");

                AddProcess(
                    result,
                    "e",
                    "基础底面偏心距e",
                    finalE.ToString("F3"),
                    "");

                AddProcess(
                    result,
                    "e0",
                    "偏心距限值e0",
                    finalE0.ToString("F3"),
                    "");


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


        // =====================================================
        // 计算Pk
        // =====================================================

        private double CalculatePk(
            double Q,
            double a1,
            double b1,
            double h,
            double h0,
            double r)
        {
            double rz = 20.0;

            return
                (
                    Q +
                    a1 *
                    b1 *
                    (h - h0) *
                    rz
                )
                /
                (a1 * b1);
        }


        // =====================================================
        // 添加结果参数
        // =====================================================

        private void AddResult(
            CalculationResult result,
            string key,
            string name,
            string value,
            string unit)
        {
            result.ResultParameters.Add(
                new ParameterItem
                {
                    Key = key,
                    Name = name,
                    Value = value,
                    Unit = unit,
                    IsReadOnly = true
                });
        }


        // =====================================================
        // 添加计算过程参数
        // =====================================================

        private void AddProcess(
            CalculationResult result,
            string key,
            string name,
            string value,
            string unit)
        {
            result.ProcessParameters.Add(
                new ParameterItem
                {
                    Key = key,
                    Name = name,
                    Value = value,
                    Unit = unit,
                    IsReadOnly = true
                });
        }


        // =====================================================
        // 获取数字
        // =====================================================

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


        // =====================================================
        // 计算被动土压力系数Kp
        // =====================================================

        public static double CountKp(
            double phiDegrees)
        {
            double angleDegrees =
                45.0 +
                phiDegrees / 2.0;

            double angleRadians =
                angleDegrees *
                Math.PI /
                180.0;

            double t =
                Math.Tan(angleRadians);

            return t * t;
        }
    }
}
