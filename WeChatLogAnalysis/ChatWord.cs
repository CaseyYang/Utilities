using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WeChatLogAnalysis
{
    public class ChatWord
    {
        static string expressionMark = "[表情-";
        static string[] emotionMarks = { "！", "!", "?", "？" };
        [XmlAttribute("时间")]
        public DateTime timeStamp;
        [XmlElementAttribute()]
        public string chatContent;
        [XmlIgnoreAttribute]
        public int contentLength;
        [XmlAttribute("发送人")]
        public string user;
        [XmlIgnoreAttribute]
        public bool expressionSignal;
        [XmlIgnoreAttribute]
        public int expressionCount;
        [XmlIgnoreAttribute]
        public bool emotionSignal;
        [XmlIgnoreAttribute]
        public int emotionCount;
        public ChatWord(DateTime time, string user, string content)
        {
            this.timeStamp = time;
            this.user = user;
            this.chatContent = content;
            this.contentLength = SetContentLength();
            this.expressionSignal = SetExpression();
            this.emotionSignal = SetEmotion();
        }

        public ChatWord()
        {
            // TODO: Complete member initialization
        }
        private int SetContentLength()
        {
            this.contentLength = this.chatContent.Length;
            int noWordStart = -1;
            for (int index = 0; index < chatContent.Length; index++)
            {
                if (chatContent[index].Equals('['))
                {
                    noWordStart = index;
                    continue;
                }
                if (chatContent[index].Equals(']'))
                {
                    this.contentLength -= (index - noWordStart + 1);
                    this.contentLength++;
                    noWordStart = -1;
                    continue;
                }
            }
            return this.contentLength;
        }
        private bool SetExpression()
        {
            expressionCount = 0;
            if (chatContent.Contains(expressionMark))
            {
                int strIndex = 0;
                while (strIndex != -1)
                {
                    expressionCount++;
                    strIndex++;
                    strIndex = chatContent.IndexOf(expressionMark, strIndex);
                };
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool SetEmotion()
        {
            emotionCount = 0;
            foreach (string mark in emotionMarks)
            {
                if (chatContent.Contains(mark))
                {
                    int strIndex = 0;
                    while (strIndex != -1)
                    {
                        emotionCount++;
                        strIndex++;
                        strIndex = chatContent.IndexOf(mark, strIndex);
                    };
                }
            }
            return emotionCount != 0;
        }
        public override string ToString()
        {
            return this.timeStamp.ToShortTimeString() + "：" + this.user + "：" + this.chatContent;
        }
    }
}
