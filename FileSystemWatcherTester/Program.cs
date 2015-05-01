using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemWatcherTester
{
    class Program
    {
        private static bool m_firstGame = false;
        private static long m_lastOffset = 0;


        // CHANGE THIS PATH TO THE PATH OF THE LOGFILE
        private static string m_logFileFullPath = @"d:\Program Files (x86)\Hearthstone\Hearthstone_Data\output_log.txt";
        private static string m_outputFilePath = @"D:\Temp\output.txt";
        // CHANGE THIS PATH FOR OUTPUT IF NEEDED
        
        
        private static int m_delay = 100;

        static void Main(string[] args)
        {
            FirstGame = true;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            Task listener = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task started.");
                Regex cardMovementRegex = new Regex(@"\w*(cardId=(?<Id>(\w*))).*(zone\ from\ (?<from>((\w*)\s*)*))((\ )*->\ (?<to>(\w*\s*)*))*.*");
                try
                {
                    while (true)
                    {
                        using (FileStream fs = new FileStream(m_logFileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            if (FirstGame)
                            {
                                FirstGame = false;
                                m_lastOffset = fs.Length;
                                Thread.Sleep(m_delay);
                                continue;
                            }
                            else
                            {
                                fs.Seek(m_lastOffset, SeekOrigin.Begin);
                                if (fs.Length != m_lastOffset)
                                {
                                    long newLength = fs.Length;
                                    using (StreamReader sr = new StreamReader(fs))
                                    {
                                        string newLine = sr.ReadToEnd();
                                        if (cardMovementRegex.IsMatch(newLine))
                                        {

                                            Match match = cardMovementRegex.Match(newLine);
                                            string id = match.Groups["Id"].Value.Trim();
                                            string from = match.Groups["from"].Value.Trim();
                                            string to = match.Groups["to"].Value.Trim();
                                            if (id != "")
                                            {
                                                string output = string.Format("\n[+] Card Moved - ID: {0} FROM: {1} TO: {2}", id, from, to);
                                                Console.WriteLine(output);
                                            }
                                            m_lastOffset = fs.Length;
                                        }
                                        else
                                        {
                                            Thread.Sleep(m_delay);
                                            continue;
                                        }
                                    }
                                }
                            }
                            Thread.Sleep(m_delay);
                            if (token.IsCancellationRequested)
                            {
                                Console.WriteLine("Task ended.");
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            string input = Console.ReadLine();
            if (input == "exit")
            {
                cancellationTokenSource.Cancel();
            }
            Console.ReadLine();
        }

        public static bool FirstGame
        {
            get
            {
                return m_firstGame;
            }
            set
            {
                m_firstGame = value;
            }
        }
    }
}
