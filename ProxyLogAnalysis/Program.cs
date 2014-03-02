using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace ProxyLogAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            List<LogRecord> records = new List<LogRecord>();
            DirectoryInfo directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            List<FileInfo> files = directory.GetFiles().ToList<FileInfo>();
            foreach (FileInfo file in files)
            {

                if (file.Name.Substring(0, 4).Equals("log2"))
                {
                    StreamReader reader = new StreamReader(file.FullName);
                    reader.ReadLine();
                    int count = 0;
                    while (!reader.EndOfStream)
                    {
                        LogRecord record = new LogRecord();
                        count++;
                        //填入文件名和顺序号
                        record.FileName = file.Name;
                        record.Num = count;
                        //获取用户的ip地址
                        string rawStr = reader.ReadLine();
                        string ipAddress = rawStr.Substring(0, rawStr.IndexOf(' ') + 1);
                        record.IpAddress = ipAddress;
                        //获取用户名称
                        string user = rawStr.Substring(rawStr.IndexOf('-') + 1, rawStr.IndexOf('[') - rawStr.IndexOf('-') - 1);
                        user = user.Trim();
                        record.User = user;
                        //获取连接时间
                        string date = rawStr.Substring(rawStr.IndexOf('[') + 1, rawStr.IndexOf(']') - rawStr.IndexOf('[') - 1);
                        record.Date = date;
                        //获取代理方式、URL、请求状态码、数据报长度、HTTP请求类型、内容摘要等信息
                        rawStr = rawStr.Substring(rawStr.IndexOf('"'));
                        char[] split1 = { '"' };
                        char[] split2 = { ' ' };
                        string[] recordElements = rawStr.Split(split1, StringSplitOptions.RemoveEmptyEntries);
                        if (recordElements.Length >= 4)
                        {
                            //获取代理方式和URL
                            string[] urlElements = recordElements[0].Split(split2, StringSplitOptions.RemoveEmptyEntries);
                            //此处得到的格式应该类似“CONNECT hm.baidu.com:443 HTTP/1.1”
                            if (urlElements.Length.Equals(3))
                            {
                                record.RequestType = urlElements[0];
                                record.Url = urlElements[1];
                            }
                            else
                            {
                                Console.WriteLine("在" + file.Name + "中获取代理方式和URL出错！该条记录的时间戳为" + record.Date);
                            }
                            //获取请求状态码和数据报长度
                            string[] dataElements = recordElements[1].Split(split2, StringSplitOptions.RemoveEmptyEntries);
                            //此处得到的格式应该类似“ 200 0 ”
                            if (dataElements.Length.Equals(2))
                            {
                                record.ResponseStatusCode = Int32.Parse(dataElements[0]);
                                record.DatagramLength = Int32.Parse(dataElements[1]);
                            }
                            else
                            {
                                Console.WriteLine("在" + file.Name + "中获取请求状态码和数据报长度出错！该条记录的时间戳为" + record.Date);
                            }
                            //获取HTTP请求类型
                            record.ProtocolType = recordElements[2];
                            //存在内容摘要
                            if (recordElements.Length.Equals(5))
                            {
                                record.ContentSummary = recordElements[4];
                            }
                        }
                        else
                        {
                            Console.WriteLine("在" + file.Name + "中获取连接内容出错！该条记录的时间戳为" + record.Date);
                        }
                        records.Add(record);
                    }
                    reader.Close();
                }
            }
            //输出为XML文件
            StreamWriter writer = new StreamWriter("result.xml");
            XmlSerializer sr = new XmlSerializer(records.GetType());
            sr.Serialize(writer, records);
            writer.Close();
        }
    }
}
