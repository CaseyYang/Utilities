using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using WeChatLogAnalysis;

namespace WeChatLogAnalysis
{
    public class ChatDialogue
    {
        [XmlAttribute("对话开始时间")]
        public DateTime startTimeStamp;
        [XmlArrayAttribute()]
        public List<ChatWord> chatWords;
        public ChatDialogue() { }
        public ChatDialogue(DateTime time)
        {
            this.startTimeStamp = time;
            this.chatWords = new List<ChatWord>();
        }
        static public void CovertChatLogsToDialogue(ref SortedList<DateTime, List<ChatWord>> chatLogs, ref SortedList<DateTime, List<ChatDialogue>> chatDialogues, int timeSpan = 60)
        {
            DateTime currentDate = DateTime.Today;
            DateTime currentDateTime = DateTime.Today;
            foreach (var dialogue in chatLogs)
            {
                if (currentDateTime.Equals(DateTime.Today) || (dialogue.Value.First().timeStamp - currentDateTime).TotalMinutes > timeSpan)
                {
                    chatDialogues.Add(dialogue.Key, new List<ChatDialogue>());
                    currentDate = dialogue.Key;
                }
                foreach (var log in dialogue.Value)
                {
                    if (currentDateTime.Equals(DateTime.Today) || (log.timeStamp - currentDateTime).TotalMinutes > timeSpan)
                    {
                        if (!currentDate.Date.Equals(log.timeStamp.Date))
                        {
                            chatDialogues.Add(dialogue.Key, new List<ChatDialogue>());
                            currentDate = dialogue.Key;
                        }
                        chatDialogues[currentDate].Add(new ChatDialogue(log.timeStamp));
                    }
                    chatDialogues[currentDate].Last().chatWords.Add(log);
                    currentDateTime = log.timeStamp;
                }
            }
        }
        public override string ToString()
        {
            string result = startTimeStamp.ToShortDateString() + "\r\n";
            foreach (var word in chatWords)
            {
                result += word.timeStamp.ToShortTimeString() + "：" + word.user + "：" + word.chatContent + "\r\n";
            }
            return result;
        }
    }
}
