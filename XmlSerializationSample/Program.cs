using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XmlSerializationSample
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">序列化文件输出路径</param>
        /// <param name="sourceObj">序列化对象</param>
        /// <param name="type">序列化对象的类型</param>
        /// <param name="xmlRootName">手动指定根结点名称。注意：此处会覆盖xmlRootName参数</param>
        static void SaveToXml(string filePath, object sourceObj, Type type, string xmlRootName)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && sourceObj != null)
            {
                type = type != null ? type : sourceObj.GetType();

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = string.IsNullOrWhiteSpace(xmlRootName) ?
                        new System.Xml.Serialization.XmlSerializer(type) :
                        new System.Xml.Serialization.XmlSerializer(type, new XmlRootAttribute(xmlRootName));
                    xmlSerializer.Serialize(writer, sourceObj);
                }
            }
        }

        static object LoadFromXml(string filePath, Type type)
        {
            object result = null;

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(type);
                    result = xmlSerializer.Deserialize(reader);
                }
            }

            return result;
        }

        static void Main(string[] args)
        {
            Area area1 = new Area();
            area1.Name = "Pudong";
            area1.Id = "PD001";
            area1.Streets = new string[] { "street 001", "street 002" };
            Area area2 = new Area();
            area2.Name = "Xuhui";
            area2.Id = "XH002";
            area2.Streets = new string[] { "street 003", "street 004" };

            City city1 = new City();
            city1.Name = "Shanghai";
            city1.Id = "SH001";
            city1.Areas = new Area[] { area1, area2 };

            SaveToXml(@"output003.xml", city1,city1.GetType(),"MyCity2");
        }
    }
}
