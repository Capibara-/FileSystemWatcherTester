using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemWatcherTester
{
    class Program
    {
        private static bool m_firstGame = false;
        private static long m_lastOffset = 0;
        private static string m_logFileFullPath = @"d:\Program Files (x86)\Hearthstone\Hearthstone_Data\output_log.txt";
        private static int m_delay = 100;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting.");
            while (true)
            {
                ReadFileAsync();
            }

            //int delay = 100;
            //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            //CancellationToken token = cancellationTokenSource.Token;
            //Task listener = Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {

            //        using (FileStream fs = new FileStream(m_logFileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //        {
            //            if (FirstGame)
            //            {
            //                FirstGame = false;
            //                m_lastOffset = fs.Length;
            //                await Task.Delay(m_delay);
            //                continue;
            //            }
            //            else
            //            {
            //                if (fs.Length != m_lastOffset)
            //                {
            //                    Console.WriteLine("File Changed!");
            //                    m_lastOffset = fs.Length;
            //                }
            //            }
            //        }

            //        Thread.Sleep(delay);
            //        if (token.IsCancellationRequested)
            //            break;
            //    }

            //    //cleanup
            //}, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        }

		private static async void ReadFileAsync()
        {
            FirstGame = true;
            while (true)
            {
                using (FileStream fs = new FileStream(m_logFileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (FirstGame)
                    {
                        FirstGame = false;
                        m_lastOffset = fs.Length;
                        await Task.Delay(m_delay);
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
                                string newLines = sr.ReadToEnd();
                                // This is the if that parses the actual line, any regex logic would go here (keep the !newLines.EndsWith("\n") in the if).
                                if (newLines.EndsWith("\n") && newLines.Contains("Zone") && newLines.ToLower().Contains("missiles"))
                                {
                                    Console.WriteLine(string.Format("[+] Line written: {0}\n\n", newLines));
                                    m_lastOffset = fs.Length;   
                                }
                                else
                                {
                                    await Task.Delay(m_delay);
                                    continue;
                                }
                            }
                          
                        }
                    }
                }
            }
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
