using System.Xml.Serialization;

namespace XmlSerializationSample
{
    [XmlRootAttribute("MyCity", Namespace = "abc.abc", IsNullable = false)]//根结点
    public class City
    {
        [XmlAttribute("CityName")]//属性
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("CityId")]//属性
        public string Id
        {
            get;
            set;
        }

        [XmlArrayAttribute("Areas")]//层次结构，和Area类中的Streets集合进行对比，最能体现两种不同的对集合的序列化方式
        public Area[] Areas
        {
            get;
            set;
        }
    }
}
