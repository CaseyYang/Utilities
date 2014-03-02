using System;
using System.Web;
using System.Text;
using System.Xml.Serialization;

namespace ProxyLogAnalysis
{
    /// <summary>
    /// 表示一条代理连接记录的类
    /// </summary>
    [Serializable]
    public class LogRecord
    {
        /// <summary>
        /// 该条记录所在的原始文件
        /// </summary>
        [XmlAttributeAttribute()]
        public string FileName { get; set; }
        /// <summary>
        /// 该条记录在原始文件中的顺序号
        /// </summary>
        [XmlAttributeAttribute(AttributeName = "No.")]
        public int Num { get; set; }
        /// <summary>
        /// 用户ip地址
        /// </summary>
        [XmlElementAttribute()]
        public string IpAddress { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [XmlElementAttribute()]
        public string User { get; set; }
        /// <summary>
        /// 连接时间
        /// </summary>
        [XmlElementAttribute()]
        public string Date { get; set; }
        /// <summary>
        /// 代理方式：有GET、CONNECT等
        /// </summary>
        [XmlElementAttribute()]
        public string RequestType { get; set; }
        /// <summary>
        /// URL属性的私有字段
        /// </summary>
        private string url;
        /// <summary>
        /// 请求访问的URL
        /// </summary>
        [XmlElementAttribute()]
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                url = HttpUtility.UrlDecode(value,Encoding.UTF8);
            }
        }
        /// <summary>
        /// 被请求服务器返回的状态码
        /// </summary>
        [XmlElementAttribute()]
        public int ResponseStatusCode { get; set; }
        /// <summary>
        /// 被请求服务器返回的数据包大小
        /// </summary>
        [XmlElementAttribute()]
        public int DatagramLength { get; set; }
        /// <summary>
        /// 连接类型：有HTTP、HTTPS等
        /// </summary>
        [XmlElementAttribute()]
        public string ProtocolType { get; set; }
        /// <summary>
        /// ContentSummary属性的私有字段
        /// </summary>
        private string contentSummary;
        /// <summary>
        /// 网页内容摘要
        /// </summary>
        [XmlElementAttribute()]
        public string ContentSummary
        {
            get
            {
                return contentSummary;
            }
            set
            {
                contentSummary = HttpUtility.UrlDecode(value, Encoding.UTF8);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LogRecord()
        {
        }
    }
}
