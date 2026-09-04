using System.Collections.ObjectModel;
using OCSFoundationOptimizer.Models;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace OCSFoundationOptimizer.ViewModels
{
    public partial class MainViewModel
    {
        // =====================================================
        // 判断参数是否全部填写
        // =====================================================

        private bool AreAllParametersFilled()
        {
            return InputParameters
                .Where(x => x.IsRequired)
                .All(x =>
                    !string.IsNullOrWhiteSpace(
                        x.Value));
        }


        // =====================================================
        // 判断理论B参数是否全部填写
        // =====================================================
        private bool AreAllTheoryBParametersFilled()
        {
            return TheoryBParameters
                .Where(x => x.IsRequired)
                .All(x =>
                    !string.IsNullOrWhiteSpace(
                        x.Value));
        }


        // =====================================================
        // 判断理论B优化参数是否合法
        // =====================================================

        private bool AreTheoryBOptimizationParametersValid()
        {
            if (TheoryBOptimizationParameters.Any(x =>
                    string.IsNullOrWhiteSpace(x.Value)))
            {
                return false;
            }

            if (TheoryBOptimizationParameters.Any(x =>
                    x.HasError))
            {
                return false;
            }

            return true;
        }

        // =====================================================
        // 判断参数是否全部合法
        // =====================================================

        private bool AreAllParametersValid()
        {
            // 必填参数没有全部填写
            if (!AreAllParametersFilled())
            {
                return false;
            }


            // 存在非法参数
            if (InputParameters.Any(x => x.HasError))
            {
                return false;
            }


            return true;
        }

        private bool AreAllTheoryBParametersValid()
        {
            if (!AreAllTheoryBParametersFilled())
            {
                return false;
            }

            if (TheoryBParameters.Any(x => x.HasError))
            {
                return false;
            }

            return true;
        }
        // =====================================================
        // 判断优化参数是否合法
        // =====================================================

        private bool AreOptimizationParametersValid()
        {
            if (OptimizationParameters.Any(x =>
                    string.IsNullOrWhiteSpace(
                        x.Value)))
            {
                return false;
            }


            if (OptimizationParameters.Any(x => x.HasError))
            {
                return false;
            }


            return true;
        }


        // =====================================================
        // 初始化A输入参数
        // =====================================================

        private void InitializeParameters()
        {
            InputParameters.Add(
                new ParameterItem
                {
                    Key = "a0",
                    Name = "a0",
                    Value = "1700",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "b0",
                    Name = "b0",
                    Value = "1300",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "a1",
                    Name = "a1",
                    Value = "3100",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "b1",
                    Name = "b1",
                    Value = "2600",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "h",
                    Name = "h",
                    Value = "3500",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "h_0",
                    Name = "h0",
                    Value = "200",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "na",
                    Name = "na",
                    Value = "1",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "nb",
                    Name = "nb",
                    Value = "1",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "nh",
                    Name = "nh",
                    Value = "1",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_a",
                    Name = "Δa",
                    Value = "650",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_b",
                    Name = "Δb",
                    Value = "650",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_h",
                    Name = "Δh",
                    Value = "650",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "M1",
                    Name = "垂直线路向荷载M",
                    Value = "450",
                    Unit = "KN∙m",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "荷载参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "PH1",
                    Name = "垂直线路向荷载P_H",
                    Value = "38",
                    Unit = "KN",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "荷载参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "M2",
                    Name = "线路向荷载M",
                    Value = "50",
                    Unit = "KN∙m",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "荷载参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "PH2",
                    Name = "线路向荷载P_H",
                    Value = "4",
                    Unit = "KN",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "荷载参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "Q",
                    Name = "垂直力Q",
                    Value = "70",
                    Unit = "KN",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "荷载参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "fak",
                    Name = "地基承载力特征值f_ak",
                    Value = "130",
                    Unit = "kPa",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "r",
                    Name = "水上填土湿容重γ",
                    Value = "16",
                    Unit = "kN∕m3",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "fai",
                    Name = "土体内摩擦角φ",
                    Value = "30",
                    Unit = "°",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "f",
                    Name = "基底与土的摩擦系数f",
                    Value = "0.25",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "[k_0]",
                    Name = "抗倾稳定系数允许值[K_0]",
                    Value = "1.6",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "[k_c]",
                    Name = "抗滑稳定系数允许值[K_c]",
                    Value = "1.3",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数"
                });


            InputParameters.Add(
                new ParameterItem
                {
                    Key = "nd",
                    Name = "深度修正系数ηd",
                    Value = "1.0",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "材料及设计参数"
                });
        }


        // =====================================================
        // 初始化A优化参数
        // =====================================================

        private void InitializeOptimizationParameters()
        {
            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_K0_MIN",
                    Name = "K₀最小允许值",
                    Value = "1.60",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "优化目标"
                });


            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_KC_MIN",
                    Name = "Kc最小允许值",
                    Value = "1.30",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "优化目标"
                });


            // =================================================
            // a1
            // =================================================

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_A1_MIN",
                    Name = "a1最小值",
                    Value = "1000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "a1优化范围"
                });


            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_A1_MAX",
                    Name = "a1最大值",
                    Value = "4000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "a1优化范围"
                });


            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_DELTA_A",
                    Name = "a1步长Δa",
                    Value = "50",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "a1优化范围"
                });


            // =================================================
            // b1
            // =================================================

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_B1_MIN",
                    Name = "b1最小值",
                    Value = "1000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "b1优化范围"
                });


            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_B1_MAX",
                    Name = "b1最大值",
                    Value = "4000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "b1优化范围"
                });


            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_DELTA_B",
                    Name = "b1步长Δb",
                    Value = "50",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "b1优化范围"
                });


            // =================================================
            // h
            // =================================================

            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_H_MIN",
                    Name = "h最小值",
                    Value = "3500",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "h优化范围"
                });


            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_H_MAX",
                    Name = "h最大值",
                    Value = "3500",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "h优化范围"
                });


            OptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "A_DELTA_H",
                    Name = "h步长Δh",
                    Value = "50",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "h优化范围"
                });
        }

        // =====================================================
        // 初始化B输入参数
        // =====================================================
        private void InitializeTheoryBParameters()
        {
            TheoryBParameters.Clear();

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "H",
                    Name = "钢柱高度H",
                    Value = "15",
                    Unit = "m",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "几何尺寸"
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "a0",
                    Name = "a0",
                    Value = "1700",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "b0",
                    Name = "b0",
                    Value = "1300",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "a1",
                    Name = "a1",
                    Value = "3100",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "b1",
                    Name = "b1",
                    Value = "2600",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "h",
                    Name = "h",
                    Value = "3500",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "h_0",
                    Name = "h0",
                    Value = "200",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "na",
                    Name = "na",
                    Value = "1",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "nb",
                    Name = "nb",
                    Value = "1",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "nh",
                    Name = "nh",
                    Value = "1",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_a",
                    Name = "Δa",
                    Value = "650",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_b",
                    Name = "Δb",
                    Value = "650",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "Delta_h",
                    Name = "Δh",
                    Value = "650",
                    Unit = "mm",
                    Group = "几何尺寸",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "M1",
                    Name = "垂直线路向荷载M",
                    Value = "450",
                    Unit = "KN∙m",
                    Group = "荷载参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "PH1",
                    Name = "垂直线路向荷载P_H",
                    Value = "38",
                    Unit = "KN",
                    Group = "荷载参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "M2",
                    Name = "线路向荷载M",
                    Value = "50",
                    Unit = "KN∙m",
                    Group = "荷载参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "PH2",
                    Name = "线路向荷载P_H",
                    Value = "4",
                    Unit = "KN",
                    Group = "荷载参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "Q",
                    Name = "垂直力Q",
                    Value = "70",
                    Unit = "KN",
                    Group = "荷载参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "G0",
                    Name = "由钢柱传给基础的垂直力G0",
                    Value = "7.5",
                    Unit = "KN",
                    Group = "荷载参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "fak",
                    Name = "地基承载力特征值f_ak",
                    Value = "130",
                    Unit = "kPa",
                    Group = "材料及设计参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "r",
                    Name = "水上填土湿容重γ",
                    Value = "16",
                    Unit = "kN∕m3",
                    Group = "材料及设计参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "fai",
                    Name = "土体内摩擦角φ",
                    Value = "30",
                    Unit = "°",
                    Group = "材料及设计参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "[k_0]",
                    Name = "抗倾稳定系数允许值[K_0]",
                    Value = "1.6",
                    Unit = "",
                    Group = "材料及设计参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "[k_c]",
                    Name = "抗滑稳定系数允许值[K_c]",
                    Value = "1.3",
                    Unit = "",
                    Group = "材料及设计参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "nd",
                    Name = "深度修正系数ηd",
                    Value = "1.0",
                    Unit = "",
                    Group = "材料及设计参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });

            TheoryBParameters.Add(
                new ParameterItem
                {
                    Key = "kr",
                    Name = "由土壤种类决定的系数kr",
                    Value = "2.0",
                    Unit = "",
                    Group = "材料及设计参数",
                    Type = ParameterType.Number,
                    IsRequired = true
                });


            // 后续继续添加 B 的参数
        }

        // =====================================================
        // 初始化B优化参数
        // =====================================================

        private void InitializeTheoryBOptimizationParameters()
        {
            TheoryBOptimizationParameters.Clear();


            // =================================================
            // 优化目标
            // =================================================

            TheoryBOptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_K_MIN",
                    Name = "K最小允许值",
                    Value = "1.60",
                    Unit = "",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "优化目标"
                });


            // =================================================
            // a1
            // =================================================

            TheoryBOptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_A1_MIN",
                    Name = "a1最小值",
                    Value = "1000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "a1优化范围"
                });


            TheoryBOptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_A1_MAX",
                    Name = "a1最大值",
                    Value = "4000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "a1优化范围"
                });


            TheoryBOptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_DELTA_A",
                    Name = "a1步长Δa",
                    Value = "50",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "a1优化范围"
                });


            // =================================================
            // b1
            // =================================================

            TheoryBOptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_B1_MIN",
                    Name = "b1最小值",
                    Value = "1000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "b1优化范围"
                });


            TheoryBOptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_B1_MAX",
                    Name = "b1最大值",
                    Value = "4000",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "b1优化范围"
                });


            TheoryBOptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_DELTA_B",
                    Name = "b1步长Δb",
                    Value = "50",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "b1优化范围"
                });


            // =================================================
            // h
            // =================================================

            TheoryBOptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_H_MIN",
                    Name = "h最小值",
                    Value = "3500",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "h优化范围"
                });


            TheoryBOptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_H_MAX",
                    Name = "h最大值",
                    Value = "3500",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "h优化范围"
                });


            TheoryBOptimizationParameters.Add(
                new ParameterItem
                {
                    Key = "B_DELTA_H",
                    Name = "h步长Δh",
                    Value = "50",
                    Unit = "mm",
                    Type = ParameterType.Number,
                    IsRequired = true,
                    Group = "h优化范围"
                });
        }


        // =====================================================
        // 根据 Group 创建参数分组
        // =====================================================

        private void BuildParameterGroups()
        {
            ParameterGroups.Clear();

            var groups =
                InputParameters
                    .GroupBy(x => x.Group)
                    .Select(group =>
                        new ParameterGroup
                        {
                            Name = group.Key,

                            Parameters =
                                new ObservableCollection<ParameterItem>(
                                    group)
                        });


            foreach (var group in groups)
            {
                ParameterGroups.Add(group);
            }
        }

        private void BuildTheoryBParameterGroups()
        {
            TheoryBParameterGroups.Clear();

            foreach (var parameter in TheoryBParameters)
            {
                var group =
                    TheoryBParameterGroups
                        .FirstOrDefault(x => x.Name == parameter.Group);

                if (group == null)
                {
                    group = new ParameterGroup
                    {
                        Name = parameter.Group
                    };

                    TheoryBParameterGroups.Add(group);
                }

                group.Parameters.Add(parameter);
            }
        }

        // =====================================================
        // 创建优化参数分组
        // =====================================================

        private void BuildOptimizationGroups()
        {
            OptimizationGroups.Clear();

            var groups =
                OptimizationParameters
                    .GroupBy(x => x.Group)
                    .Select(group =>
                        new ParameterGroup
                        {
                            Name = group.Key,

                            Parameters =
                                new ObservableCollection<ParameterItem>(
                                    group)
                        });


            foreach (var group in groups)
            {
                OptimizationGroups.Add(group);
            }
        }


        // =====================================================
        // 创建理论B优化参数分组
        // =====================================================

        private void BuildTheoryBOptimizationGroups()
        {
            TheoryBOptimizationGroups.Clear();

            var groups =
                TheoryBOptimizationParameters
                    .GroupBy(x => x.Group)
                    .Select(group =>
                        new ParameterGroup
                        {
                            Name = group.Key,

                            Parameters =
                                new ObservableCollection<ParameterItem>(
                                    group)
                        });

            foreach (var group in groups)
            {
                TheoryBOptimizationGroups.Add(group);
            }
        }


        // =====================================================
        // 监听输入参数
        // =====================================================

        private void SubscribeParameterEvents()
        {
            foreach (var parameter in InputParameters)
            {
                parameter.PropertyChanged +=
                    Parameter_PropertyChanged;
            }

            foreach (var parameter in OptimizationParameters)
            {
                parameter.PropertyChanged +=
                    Parameter_PropertyChanged;
            }

            foreach (var parameter in TheoryBParameters)
            {
                parameter.PropertyChanged +=
                    Parameter_PropertyChanged;
            }

            foreach (var parameter in TheoryBOptimizationParameters)
            {
                parameter.PropertyChanged +=
                    Parameter_PropertyChanged;
            }
        }


        // =====================================================
        // 参数发生变化
        // =====================================================

        private void Parameter_PropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName !=
                nameof(ParameterItem.Value))
            {
                return;
            }

            _autoCalculateTimer.Stop();

            ClearCalculationResult();


            // =====================================================
            // 理论 A 优化
            // =====================================================

            if (CurrentTheory ==
                CalculationTheoryType.AOptimization)
            {
                CalculationStatus =
                    "优化参数已修改，请点击“开始优化”。";

                OnPropertyChanged(
                    nameof(CanOptimize));

                OnPropertyChanged(
                    nameof(CanCalculate));

                CommandManager
                    .InvalidateRequerySuggested();

                return;
            }
            
            
            // =====================================================
            // 理论 B 优化
            // =====================================================

            if (CurrentTheory ==
                CalculationTheoryType.BOptimization)
            {
                CalculationStatus =
                    "优化参数已修改，请点击“开始优化”。";

                OnPropertyChanged(
                    nameof(CanOptimize));

                OnPropertyChanged(
                    nameof(CanCalculate));

                CommandManager
                    .InvalidateRequerySuggested();

                return;
            }


            // =====================================================
            // 理论 B
            // =====================================================

            if (CurrentTheory ==
                CalculationTheoryType.B)
            {
                if (TheoryBParameters.Any(x => x.HasError))
                {
                    CalculationStatus =
                        "参数输入存在错误";
                }
                else if (!AreAllTheoryBParametersFilled())
                {
                    CalculationStatus =
                        "等待输入完整参数";
                }
                else
                {
                    CalculationStatus =
                        "参数已就绪，正在准备计算...";

                    _autoCalculateTimer.Start();
                }

                OnPropertyChanged(
                    nameof(CanCalculate));

                CommandManager
                    .InvalidateRequerySuggested();

                return;
            }


            // =====================================================
            // 理论 A
            // =====================================================

            if (InputParameters.Any(x => x.HasError))
            {
                CalculationStatus =
                    "参数输入存在错误";
            }
            else if (!AreAllParametersFilled())
            {
                CalculationStatus =
                    "等待输入完整参数";
            }
            else
            {
                CalculationStatus =
                    "参数已就绪，正在准备计算...";

                _autoCalculateTimer.Start();
            }


            OnPropertyChanged(
                nameof(CanCalculate));

            OnPropertyChanged(
                nameof(CanOptimize));

            CommandManager
                .InvalidateRequerySuggested();
        }
    }
}