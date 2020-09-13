using System;
using System.IO;

namespace Utils
{
    public static class Logger
    {
        private static TextWriter _textWritter;

        public static void Log(string logMessage)
        {
            using (StreamWriter streamWriter = File.AppendText("log.txt"))
            {
                if(_textWritter != null)
                {
                    _textWritter.Write("\r\nLog Entry: ");
                    _textWritter.Write($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    _textWritter.Write(" :");
                    _textWritter.Write($" :{logMessage}");
                    _textWritter.Write("-------------------------------");
                }
            }
        }
    }
}
