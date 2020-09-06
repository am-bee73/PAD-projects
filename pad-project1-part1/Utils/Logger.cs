using System;
using System.IO;

namespace Utils
{
    public static class Logger
    {
        private static TextWriter textWriter;

        public static void Log(string logMessage)
        {
            using(StreamWriter streamWriter = File.AppendText("log.txt"))
            {
                textWriter.Write("\r\nLog Entry: ");
                textWriter.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                textWriter.Write(" :");
                textWriter.Write($" :{logMessage}");
                textWriter.Write("-------------------------------");
            }
        }
    }
}
