using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Data.Xml.Xsl;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Split_It.Utils
{
    /// <summary>
    /// Usage: 
    /// 1) In a XAML file, declare the above namespace, e.g.:
    ///    xmlns:common="using:html2xaml"
    ///     
    /// 2) In RichTextBlock controls, set or databind the Html property, e.g.:
    /// 
    ///    <RichTextBlock common:Properties.Html="{Binding ...}"/>
    ///    
    ///    or
    ///    
    ///    <RichTextBlock>
    ///       <common:Properties.Html>
    ///         <![CDATA[
    ///             <p>This is a list:</p>
    ///             <ul>
    ///                 <li>Item 1</li>
    ///                 <li>Item 2</li>
    ///                 <li>Item 3</li>
    ///             </ul>
    ///         ]]>
    ///       </common:Properties.Html>
    ///    </RichTextBlock>
    /// </summary>
    public sealed class Properties
    {
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string),
            typeof(Properties), new PropertyMetadata(false, HtmlChanged));

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextBlock richText = d as RichTextBlock;
            if (richText == null) return;

            //Generate blocks
            string xhtml = e.NewValue as string;

            List<Block> blocks = GenerateBlocksForHtml(xhtml);

            //Add the blocks to the RichTextBlock
            richText.Blocks.Clear();
            foreach (Block b in blocks)
            {
                richText.Blocks.Add(b);
            }
        }

        private static List<Block> GenerateBlocksForHtml(string xhtml)
        {
            List<Block> bc = new List<Block>();

            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(xhtml);

                Block b = GenerateParagraph(doc.DocumentNode);
                bc.Add(b);
            }
            catch (Exception ex)
            {
            }

            return bc;
        }

        private static Block GenerateParagraph(HtmlNode node)
        {
            Paragraph p = new Paragraph();
            AddChildren(p, node);
            return p;
        }

        private static void AddChildren(Paragraph p, HtmlNode node)
        {
            bool added = false;
            foreach (HtmlNode child in node.ChildNodes)
            {
                Inline i = GenerateBlockForNode(child);
                if (i != null)
                {
                    p.Inlines.Add(i);
                    added = true;
                }
            }
            if (!added)
            {
                p.Inlines.Add(new Run() { Text = CleanText(node.InnerText) });
            }
        }

        private static void AddChildren(Span s, HtmlNode node) 
        { 
            bool added = false; 
             
            foreach (HtmlNode child in node.ChildNodes) 
            { 
                Inline i = GenerateBlockForNode(child); 
                if (i != null) 
                { 
                    s.Inlines.Add(i); 
                    added = true; 
                } 
            } 
            if (!added) 
            { 
                s.Inlines.Add(new Run() { Text = CleanText(node.InnerText) }); 
            } 
        } 

        private static Inline GenerateBlockForNode(HtmlNode node)
        {
            switch (node.Name)
            {
                case "b":
                case "B":
                case "strong":
                    return GenerateBold(node);
                case "i":
                case "I":
                    return GenerateItalic(node);
                case "u":
                case "U":
                    return GenerateUnderline(node);
                case "br":
                case "BR":
                    return new LineBreak();
                case "#text":
                    if (!string.IsNullOrWhiteSpace(node.InnerText))
                        return new Run() { Text = CleanText(node.InnerText) };
                    break;
                case "strike":
                    return GenerateStrike(node);
                case "font":
                    return GenerateFont(node);
                default:
                    return new LineBreak();
            }
            return null;
        }

        private static Inline GenerateFont(HtmlNode node)
        {
            Span s = new Span();
            foreach (var attribute in node.Attributes)
            {
                if(attribute.Name == "color")
                {
                    SolidColorBrush foregroundBrush = new SolidColorBrush(Colors.Black);
                    switch (attribute.Value)
                    {
                        case "#5bc5a7":
                            foregroundBrush = Application.Current.Resources["positive"] as SolidColorBrush;
                            break;
                        case "#ff652f":
                            foregroundBrush = Application.Current.Resources["negative"] as SolidColorBrush;
                            break;
                        /*case "#5bc5a7":
                            foregroundBrush = Application.Current.Resources["settled"] as SolidColorBrush;
                            break;*/
                        default:
                            break;
                    }
                    s.Foreground = foregroundBrush;
                }
            }
            AddChildren(s, node);
            return s;
        }

        private static Inline GenerateBold(HtmlNode node)
        {
            Bold b = new Bold();
            b.FontWeight = FontWeights.SemiBold;
            AddChildren(b, node);
            Run r = new Run() { Text = " " };
            b.Inlines.Add(r);
            return b;
        }

        private static Inline GenerateUnderline(HtmlNode node)
        {
            Underline u = new Underline();
            AddChildren(u, node);
            Run r = new Run() { Text = " " };
            u.Inlines.Add(r);
            return u;
        }

        private static Inline GenerateItalic(HtmlNode node)
        {
            Italic i = new Italic();
            AddChildren(i, node);
            Run r = new Run() { Text = " " };
            i.Inlines.Add(r);
            return i;
        }

        private static Inline GenerateStrike(HtmlNode node)
        {
            Italic s = new Italic();
            AddChildren(s, node);
            return s;
        }

        /// <summary> 
        /// Cleans HTML text for display in paragraphs 
        /// </summary> 
        /// <param name="input"></param> 
        /// <returns></returns> 
        private static string CleanText(string input)
        {
            string clean = Windows.Data.Html.HtmlUtilities.ConvertToText(input);
            //clean = System.Net.WebUtility.HtmlEncode(clean); 
            if (clean == "\0")
                clean = "\n";
            return clean;
        }
    }
}

