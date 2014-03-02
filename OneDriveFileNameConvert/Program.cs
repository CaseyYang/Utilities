using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OneDriveFileNameConvert
{
    class Program
    {
        static DirectoryInfo rootDirectory;
        static DirectoryInfo newDirectory;
        static List<string> originFileNameList;
        static List<string> changedFileNameList;
        static int needToRecoverCount = 0;
        static int recoverCount = 0;
        static int remainCount = 0;
        static int unchangedCount = 0;

        /// <summary>
        /// 读取Encoding Errors.txt
        /// </summary>
        /// <returns>读取成功，返回true；否则返回false</returns>
        static bool ReadInErrorTxt()
        {
            StreamReader fileReader = new StreamReader("Encoding Errors.txt");
            if (!fileReader.EndOfStream)
            {
                //跳过Encoding Errors.txt开头的三行
                fileReader.ReadLine();
                fileReader.ReadLine();
                fileReader.ReadLine();
                while (!fileReader.EndOfStream)
                {
                    //取得Encoding Errors.txt中某条记录的原文件名和自动重命名后的文件名
                    string rawStr = fileReader.ReadLine();
                    if (!rawStr.Equals(""))
                    {
                        int indexOfSplit = rawStr.IndexOf(" -> ");
                        string originFilePath = rawStr.Substring(0, indexOfSplit);
                        string changedFilePath = rawStr.Substring(indexOfSplit + 4, rawStr.Length - indexOfSplit - 4);
                        originFileNameList.Add(originFilePath);
                        changedFileNameList.Add(changedFilePath);
                    }
                }
                needToRecoverCount = originFileNameList.Count;
                return true;
            }
            else
            {
                Console.WriteLine("读入Encoding Errors.txt出错！");
                fileReader.Close();
                return false;
            }
        }

        /// <summary>
        /// 根据Encoding Errors.txt中的条目把文件按原来的文件夹名和文件名转移到“文件名复原”文件夹下
        /// </summary>
        /// <param name="originFileName">原来的文件名</param>
        /// <param name="changedFileName">被OneDrive服务器重新设置的文件名</param>
        static void RecoverChangedFileName(string originFileName, string changedFileName)
        {
            string[] originDirectories = originFileName.Split('/');
            DirectoryInfo tmpDirectory = newDirectory;
            for (int i = 0; i < originDirectories.Length - 1; i++)
            {
                tmpDirectory.CreateSubdirectory(originDirectories[i]);
                i++;
            }
            File.Move(rootDirectory.FullName + "/" + changedFileName, newDirectory.FullName + "/" + originFileName);
            recoverCount++;
        }

        /// <summary>
        /// 移动留在原文件夹中，文件夹名/文件名未被修改过的文件，到“文件名复原”文件夹下
        /// </summary>
        static void MoveRemainFiles()
        {
            FileInfo[] remainFileList = rootDirectory.GetFiles("*", SearchOption.AllDirectories);
            remainCount = remainFileList.Length;
            foreach (FileInfo remainFile in remainFileList)
            {
                //当前文件不是程序本身和Encoding Error.txt
                if (!remainFile.Name.Equals("Encoding Errors.txt") && !remainFile.FullName.Equals(Process.GetCurrentProcess().MainModule.FileName))
                {
                    string fullDirectoryPath = remainFile.FullName;
                    string partDirectoryPath = fullDirectoryPath.Remove(0, rootDirectory.FullName.Length + 1);
                    string[] directories = partDirectoryPath.Split('/');
                    DirectoryInfo tmpDirectory = newDirectory;
                    for (int i = 0; i < directories.Length - 1; i++)
                    {
                        tmpDirectory.CreateSubdirectory(directories[i]);
                        i++;
                    }
                    File.Move(remainFile.FullName, newDirectory.FullName + "/" + partDirectoryPath);
                    unchangedCount++;
                }
            }
        }

        static void Main(string[] args)
        {
            rootDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            string currentDirectoryPath = rootDirectory.FullName;
            newDirectory = rootDirectory.Parent.CreateSubdirectory("文件名复原");//新的文件夹名字为“文件名复原”
            originFileNameList = new List<string>();
            changedFileNameList = new List<string>();
            needToRecoverCount = 0;
            recoverCount = 0;
            remainCount = 0;
            unchangedCount = 0;
            if (ReadInErrorTxt())
            {
                int originFileNameIndex = 0;
                foreach (string originFileName in originFileNameList)
                {
                    RecoverChangedFileName(originFileName, changedFileNameList[originFileNameIndex]);
                    originFileNameIndex++;
                }
                MoveRemainFiles();
            }
            Console.WriteLine("读取" + needToRecoverCount + "个条目；共修复" + recoverCount + "个文件名；");
            Console.WriteLine("无需修改" + remainCount + "个文件名，移动" + unchangedCount + "个文件！");
            Console.Write("操作完毕！按任意键继续……");
            Console.Read();
        }
    }
}
