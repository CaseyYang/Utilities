using System.Xml.Serialization;

namespace XmlSerializationSample
{
    public class Area
    {
        [XmlAttribute("AreaName")]//属性：效果如<Area AreaName="Xuhui">
        public string Name
        {
            get;
            set;
        }

        [XmlElementAttribute("AreaId", IsNullable = false)]//子节点：效果如<AreaId>XH002</AreaId>
        public string Id
        {
            get;
            set;
        }

        [XmlElementAttribute("Street", IsNullable = false)]//也是集合，但是被序列化为子节点，和City类中Aera类集合做对比：效果如<Street>street 004</Street>
        public string[] Streets
        {
            get;
            set;
        }
    }
}
