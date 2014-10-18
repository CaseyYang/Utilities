using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WeChatLogAnalysis
{
    class Program
    {
        /// <summary>
        /// User1：男方；User2：女方
        /// </summary>
        static string User1 = "杨恺希", User2 = "解戎";
        static SortedList<DateTime, List<ChatWord>> chatLogs = new SortedList<DateTime, List<ChatWord>>();
        static SortedList<DateTime, List<ChatDialogue>> chatDialogues = new SortedList<DateTime, List<ChatDialogue>>();
        /// <summary>
        /// 处理单条聊天记录：找出其时间戳；发送方和聊天内容
        /// </summary>
        /// <param name="rawStr">单条聊天记录原始字符串</param>
        /// <param name="timeStamp">时间戳</param>
        /// <param name="user">发送方</param>
        /// <param name="chatContent">聊天内容</param>
        static void ProcessRawLog(string rawStr, ref DateTime timeStamp, ref string user, ref string chatContent)
        {
            int index = rawStr.IndexOf('：');
            if (index != -1)
            {
                user = rawStr.Substring(0, index);
                chatContent = rawStr.Substring(index + 1);
            }
            else
            {
                string[] dateStr = rawStr.Split('.');
                if (dateStr.Length != 6)
                {
                    Console.WriteLine(rawStr);
                    Console.Read();
                }
                timeStamp = new DateTime(Int32.Parse(dateStr[0]), Int32.Parse(dateStr[1]), Int32.Parse(dateStr[2]), Int32.Parse(dateStr[3]), Int32.Parse(dateStr[4]), Int32.Parse(dateStr[5]));
            }
        }
        /// <summary>
        /// 读入微信聊天记录，并保存至集合chatLogs和集合chatDialogues中
        /// </summary>
        /// <param name="filePath">聊天记录文件路径</param>
        /// <returns>读取成功返回true；聊天记录中有错即返回false</returns>
        static bool ReadInWeChatLog(string filePath)
        {
            StreamReader fReader = new StreamReader(filePath, System.Text.Encoding.Default);
            DateTime currentDay = DateTime.Today;//记录当前记录所在日期
            DateTime currentDateTime = DateTime.Today;//记录当前记录所在具体时间
            DateTime lastDateTime = DateTime.Today;
            int lineIndex = 1;
            while (!fReader.EndOfStream)
            {
                string rawStr = fReader.ReadLine();
                lineIndex++;
                string user = "", chatContent = "";
                ProcessRawLog(rawStr, ref currentDateTime, ref user, ref chatContent);
                if (currentDateTime < lastDateTime && !lastDateTime.Equals(DateTime.Today))
                {
                    Console.WriteLine("聊天记录时间戳出错！");
                    Console.WriteLine(rawStr);
                    Console.Read();
                    return false;
                }
                if (!currentDateTime.Date.Equals(currentDay.Date))
                {
                    currentDay = currentDateTime;
                    chatLogs.Add(currentDay, new List<ChatWord>());
                }
                if (!user.Equals(""))
                {
                    chatLogs[currentDay].Add(new ChatWord(currentDateTime, user, chatContent));
                }
                lastDateTime = currentDateTime;
            }
            fReader.Close();
            ChatDialogue.CovertChatLogsToDialogue(ref chatLogs, ref chatDialogues, 60);
            return true;
        }
        /// <summary>
        /// 按天统计女方发送消息数量并输出至给定路径的文件
        /// 文件格式：日期\t女方发送消息数量
        /// </summary>
        /// <param name="filePath">输出文件路径</param>
        static void User2ChatCountPerDay(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);
            foreach (var dialogue in chatLogs)
            {
                fWriter.Write(dialogue.Key.ToShortDateString() + "\t");
                int count = 0;
                foreach (var log in dialogue.Value)
                {
                    if (log.user.Equals(User2))
                    {
                        count++;
                    }
                }
                fWriter.Write(count + "\r\n");
            }
            fWriter.Close();
        }
        /// <summary>
        /// 按天统计女方发送消息中包含的情绪符号数量并输出至给定路径的文件
        /// 文件格式：日期\t女方发送消息中包含的情绪符号数量
        /// </summary>
        /// <param name="filePath">输出文件路径</param>
        static void User2EmotionCountPerDay(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);
            foreach (var dayLogs in chatLogs)
            {
                fWriter.Write(dayLogs.Key.ToShortDateString() + "\t");
                int count = 0;
                foreach (var log in dayLogs.Value)
                {
                    if (log.user.Equals(User2) && log.emotionSignal)
                    {
                        count += log.emotionCount;
                    }
                }
                fWriter.Write(count + "\r\n");
            }
            fWriter.Close();
        }
        /// <summary>
        /// 按天统计女方发送消息中包含的表情数量并输出至给定路径的文件
        /// 文件格式：日期\t女方发送消息中包含的表情数量
        /// </summary>
        /// <param name="filePath">输出文件路径</param>
        static void ExpressionCountBasedOnChatLogs(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);
            foreach (var dialogue in chatLogs)
            {
                fWriter.Write(dialogue.Key.ToShortDateString() + "\t");
                int count = 0;
                foreach (var log in dialogue.Value)
                {
                    if (log.user.Equals(User2) && log.expressionSignal)
                    {
                        count += log.expressionCount;
                    }
                }
                fWriter.Write(count + "\r\n");
            }
            fWriter.Close();
        }
        /// <summary>
        /// 按天统计对话长度及女方发送消息数量并输出至给定路径的文件
        /// 文件格式：日期\t当天对话数量\t当天平均对话长度\t女方平均发送消息数量
        /// </summary>
        /// <param name="filePath">输出文件路径</param>
        static void AverageDialogueLengthPerDay(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);
            foreach (var dialoguesOneDay in chatDialogues)
            {
                fWriter.Write(dialoguesOneDay.Key.ToShortDateString() + "\t");
                int wordsCount = 0;
                int user2WordsCount = 0;
                foreach (var dialogue in dialoguesOneDay.Value)
                {
                    wordsCount += dialogue.chatWords.Count;
                    foreach (var word in dialogue.chatWords)
                    {
                        if (word.user.Equals(User2))
                        {
                            user2WordsCount++;
                        }
                    }
                }
                double averageDialogueLength = (double)wordsCount / dialoguesOneDay.Value.Count;
                double averageUser2DialogueLength = (double)user2WordsCount / dialoguesOneDay.Value.Count;
                fWriter.Write(dialoguesOneDay.Value.Count + "\t" + averageDialogueLength + "\t" + averageUser2DialogueLength + "\r\n");
            }
            fWriter.Close();
        }
        /// <summary>
        /// 按对话统计并输出至给定路径的文件
        /// 统计信息：对话长度；女方发送消息数量；女方发送消息中情绪符号数量；女方发送消息中表情数量；女方发送消息平均长度
        /// 文件格式：对话开始时间\t对话长度\t女方发送消息数量\t女方发送消息中情绪符号数量\t女方发送消息中表情数量\t女方发送消息平均长度
        /// </summary>
        /// <param name="filePath">输出文件路径</param>
        static void StatisticsPerDialogue(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);
            fWriter.WriteLine("对话开始时间\t对话长度\t女方发送消息数量\t女方发送消息中情绪符号数量\t女方发送消息中表情数量\t女方发送消息平均长度");
            foreach (var dialoguesOneDay in chatDialogues)
            {
                foreach (var dialogue in dialoguesOneDay.Value)
                {
                    fWriter.Write(dialogue.startTimeStamp + "\t" + dialogue.chatWords.Count + "\t");//对话开始时间和对话长度
                    int user2WordsCount = 0;//女方发送消息数量
                    int user2EmotionCount = 0;//女方发送消息中情绪符号数量
                    int user2ExpressionCount = 0;//女方发送消息中表情数量
                    double user2AverageWordLength = 0;
                    foreach (var word in dialogue.chatWords)
                    {
                        if (word.user.Equals(User2))
                        {
                            user2WordsCount++;
                            user2EmotionCount += word.emotionCount;
                            user2ExpressionCount += word.expressionCount;
                            user2AverageWordLength += word.contentLength;
                        }
                    }
                    user2AverageWordLength /= user2WordsCount;
                    fWriter.Write(user2WordsCount + "\t" + user2EmotionCount + "\t" + user2ExpressionCount + "\t" + user2AverageWordLength + "\r\n");
                }
            }
            fWriter.Close();
        }
        /// <summary>
        /// 判断前后消息是否跨过睡眠时间：是返回true；否返回false
        /// </summary>
        /// <param name="user1Hour">前一消息发送小时</param>
        /// <param name="user2Hour">后一消息发送小时</param>
        /// <returns></returns>
        static bool SleepingTime(ref DateTime user1TimeStamp, ref DateTime user2TimeStamp)
        {
            if (user1TimeStamp.Hour == 23 || user1TimeStamp.Hour == 22)
            {
                if (user2TimeStamp.Date.Equals(user1TimeStamp) || user2TimeStamp.Hour < 2)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            if (user1TimeStamp.Hour < 2)
            {
                if (user2TimeStamp.Hour < user1TimeStamp.Hour + 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 计算聊天连续度并输出至给定路径的文件
        /// 聊天连续度定义为：在前一条聊天记录为用户1（男方）所发情况下，后面第一条用户2（女方）回复的聊天记录出现的平均间隔时间
        /// 
        /// 文件格式：聊天起始时间\t聊天条数时间比
        /// </summary>
        /// <param name="filePath">输出文件路径</param>
        static void ContinuityStatistic(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);

            DateTime lastTimeStamp = DateTime.Today;
            ChatWord lastChatWord = new ChatWord();
            foreach (var logsOneDay in chatLogs)
            {
                double timeSpan = 0;
                int crossCount = 0;
                foreach (var log in logsOneDay.Value)
                {
                    if (!lastTimeStamp.Equals(DateTime.Today) && lastChatWord.user.Equals(User1) && log.user.Equals(User2) && !SleepingTime(ref lastChatWord.timeStamp, ref log.timeStamp))
                    {
                        timeSpan += (log.timeStamp - lastChatWord.timeStamp).TotalMinutes;
                        crossCount++;
                    }
                    lastChatWord = log;
                    lastTimeStamp = log.timeStamp;
                }
                timeSpan /= crossCount;
                fWriter.WriteLine(logsOneDay.Key.ToShortDateString() + "\t" + timeSpan);
            }
            fWriter.Close();
        }
        /// <summary>
        /// 按天输出聊天记录至给定路径的文件
        /// </summary>
        /// <param name="filePath">输出文件路径</param>
        static void ChatLogToFile(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);
            foreach (var chatDialogue in chatLogs)
            {
                fWriter.WriteLine(chatDialogue.Key.Date.ToShortDateString());
                foreach (var log in chatDialogue.Value)
                {
                    fWriter.WriteLine(log.ToString());
                }
            }
            fWriter.Close();
        }
        /// <summary>
        /// 按对话输出聊天记录至给定路径的文件
        /// </summary>
        /// <param name="filePath">输出文件路径</param>
        static void ChatDialogueToFile(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);
            foreach (var dialoguesPreDay in chatDialogues)
            {
                //fWriter.WriteLine(chatDialogue.Key.ToShortDateString());
                foreach (var dialogue in dialoguesPreDay.Value)
                {
                    fWriter.WriteLine(dialogue.ToString());
                }
            }
            fWriter.Close();
        }
        static void ChatDialogueToXmlFile(string filePath)
        {
            List<ChatDialogue> allDialogues = new List<ChatDialogue>();
            foreach (var dialoguesPreDay in chatDialogues)
            {
                foreach(var dialogue in dialoguesPreDay.Value){
                    allDialogues.Add(dialogue);
                }
            }
            StreamWriter xmlWriter = new StreamWriter(filePath);
            XmlSerializer sr = new XmlSerializer(allDialogues.GetType());
            sr.Serialize(xmlWriter, allDialogues);
            xmlWriter.Close();
        }
        static void Main(string[] args)
        {
            User1 = "杨恺希";
            User2 = "解戎";
            if (ReadInWeChatLog(@"../../聊天记录-" + User2 + ".txt"))
            {
                //AverageDialogueLengthPerDay("AvergeDialogueLength.txt");
                //StatisticsPerDialogue("DialoguesStatistics-解戎.txt");
                //ContinuityStatistic("ContinuityStatistic-" + User2 + ".txt");
                ChatDialogueToFile("Dailogue-" + User2 + ".txt");
                ChatDialogueToXmlFile("Dailogue-" + User2 + ".xml");
                //ChatLogToFile(@"output.txt");
            }
        }
    }
}
