using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
namespace OCSFoundationOptimizer.CalculationBooks.CalculationBookUtil;

public static class WordHelper
{
    /// <summary>
    /// 替换文档中所有 Tag 匹配的内容控件的文本
    /// </summary>
    /// <param name="templatePath">模板文件路径</param>
    /// <param name="outputPath">输出文件路径</param>
    /// <param name="data">键为 Tag 名称，值为要填充的文本</param>
    public static void ReplaceContentControls(string templatePath, string outputPath,
        Dictionary<string, string> data)
    {
        // 复制模板，避免修改原文件（或直接打开）
        // File.Copy(templatePath, outputPath, overwrite: true);

        using (WordprocessingDocument doc = WordprocessingDocument.Open(outputPath, true))
        {
            // 获取文档主体
            Body body = doc.MainDocumentPart.Document.Body;

            // 查找所有内容控件 (SdtElement)
            var sdtElements = body.Descendants<SdtElement>().ToList();

            foreach (var sdt in sdtElements)
            {
                // 获取控件的 Tag 属性
                string tag = sdt.SdtProperties?.GetFirstChild<Tag>()?.Val;
                if (string.IsNullOrEmpty(tag)) continue;

                // 如果数据字典中有匹配的键，则替换文本
                if (data.TryGetValue(tag, out string newText))
                {
                    // 获取控件内的第一个 Text 元素
                    var textElement = sdt.Descendants<Text>().FirstOrDefault();
                    if (textElement != null)
                    {
                        textElement.Text = newText;
                    }
                }
            }

            // 如果需要移除内容控件框，只保留纯文本
            // 可以取消注释以下代码：
            foreach (var sdt in sdtElements)
            {
                // 获取控件内所有段落
                var paragraphs = sdt.Descendants<Paragraph>().ToList();
                var parent = sdt.Parent;
                foreach (var p in paragraphs)
                {
                    // 将段落移到控件外面
                    parent.InsertBefore(p.CloneNode(true), sdt);
                }
                // 移除控件本身
                sdt.Remove();
            }
            doc.Save();
        }
    }
}