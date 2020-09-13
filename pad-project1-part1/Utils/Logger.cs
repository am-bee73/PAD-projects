using System;
using System.IO;

namespace Utils
{
    public static class Logger
    {
        public static void Log(string logMessage)
        {
            using (StreamWriter streamWriter = File.AppendText("log.txt"))
            {
       
                if (streamWriter != null)
                {
                    streamWriter.Write("\r\nLog Entry: ");
                    streamWriter.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    streamWriter.Write(" :");
                    streamWriter.Write($" :\n{logMessage}");
                    streamWriter.Write("\n-------------------------------");
                }
            }
        }
    }
}
