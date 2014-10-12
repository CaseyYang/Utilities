using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatLogAnalysis
{
    class Program
    {
        static string User1 = "杨恺希", User2 = "解戎";
        static SortedList<DateTime, List<ChatWord>> chatLogs = new SortedList<DateTime, List<ChatWord>>();
        static SortedList<DateTime, List<ChatDialogue>> chatDialogues = new SortedList<DateTime, List<ChatDialogue>>();
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
        static void AverageChatCountBasedOnChatLogs(string filePath)
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
        static void AverageEmotionCountBasedOnChatLogs(string filePath)
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
        static void AverageExpressionCountBasedOnChatLogs(string filePath)
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
        static void AverageDialogueLengthPerDialogue(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);
            foreach (var dialoguesOneDay in chatDialogues)
            {
                foreach (var dialogue in dialoguesOneDay.Value)
                {
                    fWriter.Write(dialogue.startTimeStamp + "\t" + dialogue.chatWords.Count + "\t");
                    int user2WordsCount = 0;
                    foreach (var word in dialogue.chatWords)
                    {
                        if (word.user.Equals(User2))
                        {
                            user2WordsCount++;
                        }
                    }
                    double averageUser2Words = (double)user2WordsCount / dialogue.chatWords.Count;
                    fWriter.Write(averageUser2Words + "\r\n");
                }
            }
            fWriter.Close();
        }
        static void AverageEmotionAndExpressionBasedOnDialogues(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);
            foreach (var dialogues in chatDialogues)
            {
                foreach (var dialogue in dialogues.Value)
                {
                    int emotionCount = 0;
                    int expressionCount = 0;
                    foreach (var log in dialogue.chatWords)
                    {
                        emotionCount += log.emotionCount;
                        expressionCount += log.expressionCount;
                    }
                    double averageEmotionBasedOnDialogue = (double)emotionCount / dialogue.chatWords.Count;
                    double averageExpressionBasedOnDialogue = (double)expressionCount / dialogue.chatWords.Count;
                    fWriter.WriteLine(dialogue.startTimeStamp + "\t" + averageEmotionBasedOnDialogue + "\t" + averageExpressionBasedOnDialogue);
                }
            }
            fWriter.Close();
        }
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
        static void ChatDialogueToFile(string filePath)
        {
            StreamWriter fWriter = new StreamWriter(filePath);
            foreach (var chatDialogue in chatDialogues)
            {
                //fWriter.WriteLine(chatDialogue.Key.ToShortDateString());
                foreach (var dialogue in chatDialogue.Value)
                {
                    fWriter.WriteLine(dialogue.ToString());
                }
            }
            fWriter.Close();
        }
        static void Main(string[] args)
        {
            if (ReadInWeChatLog(@"C:\Users\Casey\Desktop\Project\聊天记录.txt"))
            {
                //AverageChatCountBasedOnChatLogs("AvergeLogCount.txt");
                //AverageEmotionCountBasedOnChatLogs("AvergeEmotionCount.txt");
                //AverageExpressionCountBasedOnChatLogs("AvergeExpressionCount.txt");
                //AverageDialogueLengthPerDay("AvergeDialogueLength.txt");
                AverageDialogueLengthPerDialogue("AvergeDialogueLengthPerDialogue.txt");
                //AverageEmotionAndExpressionBasedOnDialogues("AverageEmotion&ExpressionBasedOnDialogue.txt");
                //ChatDialogueToFile("Dailogue.txt");
                //ChatLogToFile(@"output.txt");
            }
        }
    }
}
