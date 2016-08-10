using System;
using System.IO;
using System.Text;


namespace PerformanceEvaluator.WEB.Domain
{
    public class ErrorLoggingService
    {
        public void LogError(Exception exception, string url)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "/Logger.txt";
            var currentDate = DateTime.Now.ToShortDateString();
            var currentTime = DateTime.Now.ToLongTimeString();
            var message = $"{exception.Message} | {url}";
            var text = $"{currentDate} {currentTime} | {message}";

            using (var writer = new StreamWriter(filePath,
                                            true,
                                            Encoding.GetEncoding("Windows-1251")))
            {
                writer.WriteLine(text, Encoding.Unicode);
            }
        }
    }
}