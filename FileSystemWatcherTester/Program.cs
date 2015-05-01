using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                        if (fs.Length != m_lastOffset)
                        {
                            Console.WriteLine("File Changed!");
                            m_lastOffset = fs.Length;
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
