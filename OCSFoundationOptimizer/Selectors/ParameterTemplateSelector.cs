using System.Windows;
using System.Windows.Controls;
using OCSFoundationOptimizer.Models;

namespace OCSFoundationOptimizer.Selectors
{
    /// <summary>
    /// 根据参数类型自动选择参数模板
    /// </summary>
    public class ParameterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? NumberTemplate
        {
            get;
            set;
        }


        public DataTemplate? SelectionTemplate
        {
            get;
            set;
        }


        public override DataTemplate? SelectTemplate(
            object item,
            DependencyObject container)
        {
            if (item is not ParameterItem parameter)
            {
                return base.SelectTemplate(
                    item,
                    container);
            }


            return parameter.Type switch
            {
                ParameterType.Number =>
                    NumberTemplate,

                ParameterType.Selection =>
                    SelectionTemplate,

                _ =>
                    NumberTemplate
            };
        }
    }
}