using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Util;

namespace LordOfRingCommentarySubtitles
{
    class Program
    {
        static void GetSubtitleHtmlFromFile()
        {
            List<List<NodeFilter>> filters = new List<List<NodeFilter>>();
            filters.Add(new List<NodeFilter>());
            filters.Add(new List<NodeFilter>());
            filters.Add(new List<NodeFilter>());
            filters.Add(new List<NodeFilter>());
            filters.Add(new List<NodeFilter>());
            filters.Add(new List<NodeFilter>());
            filters.Add(new List<NodeFilter>());
            filters.Add(new List<NodeFilter>());
            filters[0].Add(new HasAttributeFilter("class", "xl29"));
            filters[0].Add(new HasAttributeFilter("class", "xl31"));
            filters[0].Add(new HasAttributeFilter("class", "xl32"));
            filters[0].Add(new HasAttributeFilter("class", "xl33"));
            filters[1].Add(new HasAttributeFilter("class", "xl25"));
            filters[1].Add(new HasAttributeFilter("class", "xl26"));
            filters[1].Add(new HasAttributeFilter("class", "xl27"));
            filters[1].Add(new HasAttributeFilter("class", "xl28"));
            filters[2].Add(new HasAttributeFilter("class", "xl27"));
            filters[2].Add(new HasAttributeFilter("class", "xl28"));
            filters[2].Add(new HasAttributeFilter("class", "xl29"));
            filters[2].Add(new HasAttributeFilter("class", "xl30"));
            filters[3].Add(new HasAttributeFilter("class", "xl27"));
            filters[3].Add(new HasAttributeFilter("class", "xl28"));
            filters[3].Add(new HasAttributeFilter("class", "xl29"));
            filters[3].Add(new OrFilter(new HasAttributeFilter("class", "xl31"), new HasAttributeFilter("class", "xl30")));
            filters[4].Add(new HasAttributeFilter("class", "xl27"));
            filters[4].Add(new HasAttributeFilter("class", "xl28"));
            filters[4].Add(new HasAttributeFilter("class", "xl29"));
            filters[4].Add(new HasAttributeFilter("class", "xl30"));
            filters[5].Add(new HasAttributeFilter("class", "xl33"));
            filters[5].Add(new HasAttributeFilter("class", "xl32"));
            filters[5].Add(new HasAttributeFilter("class", "xl30"));
            filters[5].Add(new HasAttributeFilter("class", "xl28"));
            filters[6].Add(new HasAttributeFilter("class", "xl29"));
            filters[6].Add(new HasAttributeFilter("class", "xl30"));
            filters[6].Add(new HasAttributeFilter("class", "xl31"));
            filters[6].Add(new HasAttributeFilter("class", "xl32"));
            filters[7].Add(new HasAttributeFilter("class", "xl28"));
            filters[7].Add(new HasAttributeFilter("class", "xl24"));
            filters[7].Add(new HasAttributeFilter("class", "xl30"));
            filters[7].Add(new HasAttributeFilter("class", "xl31"));
            DirectoryInfo directory = new DirectoryInfo(@"D:\Download\魔戒三部曲电影导演评论字幕\mht");
            int count = 0;
            foreach (FileInfo file in directory.GetFiles("*.htm"))
            {
                StreamReader reader = new StreamReader(file.FullName);
                string htmlContent = reader.ReadToEnd();
                reader.Close();
                string fileName = file.Name.Substring(0, file.Name.IndexOf('.'));
                Lexer lexer = new Lexer(htmlContent);
                Parser parser = new Parser(lexer);
                //红色的是演员解说
                NodeList redNodeList = parser.ExtractAllNodesThatMatch(filters[count][0]);
                GetSubtitleFromHtml(redNodeList, fileName + "_演员解说");
                //黄色的是导演编剧解说
                parser.Reset();
                NodeList yelloNodeList = parser.ExtractAllNodesThatMatch(filters[count][1]);
                GetSubtitleFromHtml(yelloNodeList, fileName + "_导演编剧解说");
                //蓝色的是特技制作组
                parser.Reset();
                NodeList blueNodeList = parser.ExtractAllNodesThatMatch(filters[count][2]);
                GetSubtitleFromHtml(blueNodeList, fileName + "_特技制作组解说");
                //绿色的是幕后制作团队
                parser.Reset();
                NodeList greenNodeList = parser.ExtractAllNodesThatMatch(filters[count][3]);
                GetSubtitleFromHtml(greenNodeList, fileName + "_幕后制作团队解说");
                count++;
            }
        }
        static void GetSubtitleFromHtml(NodeList nodeList, string subtitleType)
        {
            Console.WriteLine(subtitleType);
            StreamWriter writer = new StreamWriter(subtitleType + ".txt");
            for (int i = 0; i < nodeList.Count; i++)
            {
                INode currentNode = (INode)nodeList[i];
                while (currentNode.NextSibling != null && !currentNode.NextSibling.GetType().Equals(typeof(TableColumn)))
                {
                    currentNode = currentNode.NextSibling;
                }
                if (currentNode.NextSibling != null)
                {
                    TableColumn contentNode = (TableColumn)currentNode.NextSibling;
                    string content = "";
                    NodeList childrenNode = contentNode.Children;
                    if (childrenNode != null)
                    {
                        for (int j = 0; j < childrenNode.Count; j++)
                        {
                            if (childrenNode[j].GetText().Equals("br"))
                            {
                                writer.WriteLine(content);
                                content = "";
                                continue;
                            }
                            string tmpStr = HttpUtility.HtmlDecode(childrenNode[j].ToPlainTextString());
                            tmpStr = tmpStr.Trim();
                            content += tmpStr;
                        }
                        //TableColumn speakerNode=(TableColumn)colorCell.NextSibling.NextSibling;
                        writer.WriteLine(content);
                    }
                }
            }
            writer.Close();
        }
        
        static void Main(string[] args)
        {
        }
    }
}
