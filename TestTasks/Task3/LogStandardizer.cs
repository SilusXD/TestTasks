using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestTasks.Task3
{
    internal static class LogStandardizer
    {
        public static void GetStandardizedLogFiles(string inputFilePath, string outputFilePath, string problemsFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException("File not found!");
            }

            var lines = File.ReadAllLines(inputFilePath);
            var standardizedLogs = new List<string>();
            var problematicLogs = new List<string>();


            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                { 
                    continue; 
                }

                string standardizedLine = Standardize(line);

                if (standardizedLine != null)
                {
                    standardizedLogs.Add(standardizedLine);
                }
                else
                {
                    problematicLogs.Add(line);
                }

                File.WriteAllLines(outputFilePath, standardizedLogs);
                File.WriteAllLines(problemsFilePath, problematicLogs);
            }
        }

        private static string Standardize(string logLine)
        {
            var parsed = ParseFormat1(logLine);

            if (parsed == null)
            {
                parsed = ParseFormat2(logLine);

                if (parsed == null)
                {
                    return null;
                }
            }

            string formattedDate = parsed.Date.ToString("dd-MM-yyyy");

            // Преобразование уровня логирования
            string logLevel = StandardizeLogLevel(parsed.LogLevel);

            // Определение вызвавшего метода
            string callingMethod = string.IsNullOrEmpty(parsed.CallingMethod) ? "DEFAULT" : parsed.CallingMethod;

            // Формирование выходной строки с разделителем табуляцией
            return $"{formattedDate}\t{parsed.Time}\t{logLevel}\t{callingMethod}\t{parsed.Message}";
        }

        private static Log ParseFormat1(string log)
        {
            //Формат 1: 10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'
            string pattern = @"^(\d{2})\.(\d{2})\.(\d{4})\s+(\d{2}:\d{2}:\d{2}\.\d+)\s+(\w+)\s+(.+)$";
            Match match = Regex.Match(log, pattern);


            if (!match.Success)
            { 
                return null; 
            }

            try
            {
                int day = int.Parse(match.Groups[1].Value);
                int month = int.Parse(match.Groups[2].Value);
                int year = int.Parse(match.Groups[3].Value);

                DateTime date = new DateTime(year, month, day);
                string time = match.Groups[4].Value;
                string logLevel = match.Groups[5].Value;
                string message = match.Groups[6].Value.Trim();

                return new Log
                {
                    Date = date,
                    Time = time,
                    LogLevel = logLevel,
                    CallingMethod = null,
                    Message = message
                };
            }
            catch
            {
                return null;
            }
        }
        private static Log ParseFormat2(string Log)
        {
            // Формат 2: 2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'
            string pattern = @"^(\d{4}-\d{2}-\d{2})\s+([\d:\.]+)\|\s*(\w+)\|\d+\|([^|]+)\|\s*(.+)$";
            Match match = Regex.Match(Log, pattern);

            if (!match.Success)
            {
                return null;
            }

            try
            {
                DateTime date = DateTime.ParseExact(match.Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string time = match.Groups[2].Value;
                string logLevel = match.Groups[3].Value;
                string callingMethod = match.Groups[4].Value.Trim();
                string message = match.Groups[5].Value.Trim();

                return new Log
                {
                    Date = date,
                    Time = time,
                    LogLevel = logLevel,
                    CallingMethod = callingMethod,
                    Message = message
                };
            }
            catch
            {
                return null;
            }
        }

        static string StandardizeLogLevel(string inputLevel)
        {
            string upperLevel = inputLevel.ToUpper();

            switch (upperLevel)
            {
                case "INFORMATION":
                case "INFO":
                    return "INFO";
                case "WARNING":
                case "WARN":
                    return "WARN";
                case "ERROR":
                    return "ERROR";
                case "DEBUG":
                    return "DEBUG";
                default:
                    return "INFO";
            }
        }
    }
}
