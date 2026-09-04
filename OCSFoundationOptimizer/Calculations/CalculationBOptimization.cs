using OCSFoundationOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCSFoundationOptimizer.Calculations
{
    public class CalculationBOptimization : ICalculationModule
    {
        public CalculationTheoryType Theory =>
            CalculationTheoryType.BOptimization;

        public string Name =>
            "蓝皮书优化";


        public CalculationResult Calculate(
            IReadOnlyList<ParameterItem> parameters)
        {
            var result = new CalculationResult();

            try
            {
                // =================================================
                // 保存输入参数
                // =================================================

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


                // =================================================
                // 理论 B 基础参数
                // =================================================

                double H =
                    GetNumber(parameters, "H");

                double a0 =
                    GetNumber(parameters, "a0") / 1000.0;

                double b0 =
                    GetNumber(parameters, "b0") / 1000.0;

                double h0 =
                    GetNumber(parameters, "h_0") / 1000.0;

                double Delta_a =
                    GetNumber(parameters, "Delta_a") / 1000.0;

                double Delta_b =
                    GetNumber(parameters, "Delta_b") / 1000.0;

                double Delta_h =
                    GetNumber(parameters, "Delta_h") / 1000.0;

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

                double nd =
                    GetNumber(parameters, "nd");

                double kr =
                    GetNumber(parameters, "kr");


                // =================================================
                // B 优化目标
                // =================================================

                double targetK =
                    GetNumber(parameters, "B_K_MIN");


                // =================================================
                // B 优化范围
                // =================================================

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


                // =================================================
                // 参数合法性检查
                // =================================================

                if (H <= 0)
                {
                    throw new Exception(
                        "钢柱高度H必须大于0。");
                }

                if (a1Min <= 0 ||
                    b1Min <= 0 ||
                    hMin <= 0)
                {
                    throw new Exception(
                        "优化尺寸最小值必须大于0。");
                }

                if (a1Max < a1Min)
                {
                    throw new Exception(
                        "a1最大值不能小于最小值。");
                }

                if (b1Max < b1Min)
                {
                    throw new Exception(
                        "b1最大值不能小于最小值。");
                }

                if (hMax < hMin)
                {
                    throw new Exception(
                        "h最大值不能小于最小值。");
                }

                if (deltaA <= 0 ||
                    deltaB <= 0 ||
                    deltaH <= 0)
                {
                    throw new Exception(
                        "优化步长必须大于0。");
                }

                if (targetK <= 0)
                {
                    throw new Exception(
                        "K最小允许值必须大于0。");
                }


                // =================================================
                // 计算土体内部摩擦系数
                // 与理论 B 保持一致
                // =================================================

                double f =
                    Math.Tan(
                        fai *
                        Math.PI /
                        180.0);


                // =================================================
                // 开始搜索
                //
                // 目标：
                //
                // K >= targetK
                //
                // 并且：
                //
                // a1 × b1 × h 最小
                // =================================================

                bool found = false;


                double bestA1 = 0;
                double bestB1 = 0;
                double bestH = 0;

                double bestK = 0;

                double bestMr = 0;
                double bestM0 = 0;

                double bestM1B = 0;
                double bestM2B = 0;
                double bestM3B = 0;

                double bestSigmaH = 0;
                double bestA = 0;
                double bestM = 0;

                double bestVPhi = 0;
                double bestGPhi = 0;
                double bestGr = 0;
                double bestG = 0;
                double bestSigma0 = 0;


                double bestVolume =
                    double.MaxValue;


                // =================================================
                // 穷举 a1
                // =================================================

                for (
                    double a1mm = a1Min;
                    a1mm <= a1Max + 0.000001;
                    a1mm += deltaA)
                {
                    double a1 =
                        a1mm / 1000.0;


                    // =================================================
                    // 穷举 b1
                    // =================================================

                    for (
                        double b1mm = b1Min;
                        b1mm <= b1Max + 0.000001;
                        b1mm += deltaB)
                    {
                        double b1 =
                            b1mm / 1000.0;


                        // =================================================
                        // 穷举 h
                        // =================================================

                        for (
                            double hmm = hMin;
                            hmm <= hMax + 0.000001;
                            hmm += deltaH)
                        {
                            double h =
                                hmm / 1000.0;


                            // =================================================
                            // 基本几何合法性
                            // =================================================

                            if (h <= h0)
                            {
                                continue;
                            }


                            // =================================================
                            // 理论 B 计算过程
                            // =================================================

                            double sigmaH =
                                200.0 +
                                kr *
                                r *
                                (h - h0 - 2.0);


                            if (sigmaH <= 0)
                            {
                                continue;
                            }


                            double a =
                                (h - h0) / h;


                            double mDenominator =
                                6 * a * a -
                                8 * a +
                                3;


                            if (Math.Abs(mDenominator) < 1e-12)
                            {
                                continue;
                            }


                            double m =
                                12 /
                                mDenominator;


                            // =================================================
                            // 基础体积
                            // =================================================

                            double Vphi =
                                a0 *
                                b0 *
                                (h - 2 * Delta_h)
                                +
                                a1 *
                                b1 *
                                Delta_h
                                +
                                (
                                    (a0 + Delta_a) *
                                    (b0 + Delta_b) *
                                    Delta_h
                                );


                            if (Vphi <= 0)
                            {
                                continue;
                            }


                            // =================================================
                            // 基础自重
                            // =================================================

                            double Gphi =
                                25 *
                                Vphi;


                            // =================================================
                            // 基础台阶上覆土重量
                            // =================================================

                            double Gr =
                                (
                                    a1 *
                                    b1 *
                                    (h - h0)
                                    -
                                    (
                                        Vphi -
                                        a0 *
                                        b0 *
                                        h0
                                    )
                                )
                                * r;


                            // =================================================
                            // 基底总垂直力
                            // =================================================

                            double G =
                                Gphi +
                                Gr +
                                G0;


                            if (G <= 0)
                            {
                                continue;
                            }


                            // =================================================
                            // M1B
                            // =================================================

                            double M1B =
                                (
                                    sigmaH *
                                    sigmaH *
                                    b0 *
                                    b1 *
                                    Math.Pow(
                                        h - h0,
                                        3)
                                )
                                /
                                (
                                    2.4 *
                                    m *
                                    G
                                );


                            // =================================================
                            // σ0
                            // =================================================

                            double sigma0 =
                                G /
                                (a1 * b1);


                            // =================================================
                            // M2B
                            // =================================================

                            double M2B =
                                G *
                                a1 *
                                (
                                    0.5 -
                                    (
                                        (2 * sigma0) /
                                        (3 * sigmaH)
                                    )
                                );


                            // =================================================
                            // M3B
                            // =================================================

                            double M3B =
                                (r * f / 24.0)
                                *
                                Math.Pow(
                                    h - h0,
                                    2)
                                *
                                a1
                                *
                                (
                                    10 * b1 +
                                    7 * a1
                                );


                            // =================================================
                            // 抗倾覆力矩
                            // =================================================

                            double Mr =
                                M1B +
                                M2B +
                                M3B;


                            // =================================================
                            // 倾覆力矩
                            // 与理论 B 保持完全一致
                            // =================================================

                            double M0 =
                                M1 / H *
                                (
                                    H +
                                    h0 +
                                    a *
                                    (h - h0)
                                );


                            if (M0 <= 0)
                            {
                                continue;
                            }


                            // =================================================
                            // 最终安全系数K
                            // =================================================

                            double K =
                                Mr / M0;


                            // =================================================
                            // 判断安全条件
                            // =================================================

                            if (K < targetK)
                            {
                                continue;
                            }
                            // =================================================
                            // 判断 a1 与 b1 的尺寸差
                            //
                            // 要求：|a1 - b1| <= 400 mm
                            // =================================================

                            if (Math.Abs(a1mm - b1mm) > 400.0)
                                continue;

                            // =================================================
                            // 优化目标：基础体积最小
                            // =================================================

                            double volume =
                                a1 *
                                b1 *
                                h;


                            if (volume < bestVolume)
                            {
                                found = true;

                                bestVolume =
                                    volume;

                                bestA1 =
                                    a1mm;

                                bestB1 =
                                    b1mm;

                                bestH =
                                    hmm;

                                bestK =
                                    K;

                                bestMr =
                                    Mr;

                                bestM0 =
                                    M0;

                                bestM1B =
                                    M1B;

                                bestM2B =
                                    M2B;

                                bestM3B =
                                    M3B;

                                bestSigmaH =
                                    sigmaH;

                                bestA =
                                    a;

                                bestM =
                                    m;

                                bestVPhi =
                                    Vphi;

                                bestGPhi =
                                    Gphi;

                                bestGr =
                                    Gr;

                                bestG =
                                    G;

                                bestSigma0 =
                                    sigma0;
                            }
                        }
                    }
                }


                // =================================================
                // 没找到方案
                // =================================================

                if (!found)
                {
                    result.IsSuccess = false;

                    result.ErrorMessage =
                        "在当前设定的优化范围内，没有找到满足理论 B K 要求的基础尺寸方案。";

                    return result;
                }


                // =================================================
                // 最终结果
                // =================================================

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


                AddResult(
                    result,
                    "K",
                    "抗倾覆稳定安全系数K",
                    bestK.ToString("F3"),
                    "");


                AddResult(
                    result,
                    "K_TARGET",
                    "K允许最小值",
                    targetK.ToString("F3"),
                    "");




                // =================================================
                // 最终过程参数
                // =================================================

                AddProcess(
                    result,
                    "σ_h",
                    "基础底部土壤的允许承载力σh",
                    bestSigmaH.ToString("F3"),
                    "kN/m²");


                AddProcess(
                    result,
                    "a",
                    "基础转动中心位置的系数a",
                    bestA.ToString("F3"),
                    "");


                AddProcess(
                    result,
                    "m",
                    "基础转动中心的位置决定的系数m",
                    bestM.ToString("F3"),
                    "");


                AddProcess(
                    result,
                    "Vɸ",
                    "基础体积Vɸ",
                    bestVPhi.ToString("F3"),
                    "m³");


                AddProcess(
                    result,
                    "Gɸ",
                    "基础自重Gɸ",
                    bestGPhi.ToString("F3"),
                    "kN");


                AddProcess(
                    result,
                    "Gr",
                    "基础台阶上覆土的重量Gr",
                    bestGr.ToString("F3"),
                    "kN");


                AddProcess(
                    result,
                    "G",
                    "作用于基底的总垂直力G",
                    bestG.ToString("F3"),
                    "kN");


                AddProcess(
                    result,
                    "σ0",
                    "σ0",
                    bestSigma0.ToString("F3"),
                    "kN/m²");


                AddProcess(
                    result,
                    "f",
                    "土壤的内部摩擦系数f-理论B",
                    f.ToString("F3"),
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
        // 添加结果
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
        // 添加计算过程
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
                    $"找不到理论 B 优化参数：{key}");
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