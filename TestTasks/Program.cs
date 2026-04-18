using System.Text;
using TestTasks.Task1;
using TestTasks.Task2;
using TestTasks.Task3;

namespace TestTasks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("---Тесты сжатия и распаковки---");
            Task1_Test();
            Console.WriteLine();

            Console.WriteLine("---Тест парралельности работы сервера---");
            Task2_Test();
            Console.WriteLine();

            Console.WriteLine("---Тест стандартизации логов #1---");
            Task3_Test_1();
            Console.WriteLine();

            Console.WriteLine("---Тест стандартизации логов #2---");
            Task3_Test_2();
            Console.WriteLine();
        }
        public static void Task1_Test()
        {
            // Сжатие
            Console.WriteLine($"Compress(\"aaabbcccdde\") = {StringCompressor.Compress("aaabbcccdde")}"); // a3b2c3d2e
            Console.WriteLine($"Compress(\"abc\") = {StringCompressor.Compress("abc")}");                 // abc
            Console.WriteLine($"Compress(\"a\") = {StringCompressor.Compress("a")}");                     // a
            Console.WriteLine($"Compress(\"aaaa\") = {StringCompressor.Compress("aaaa")}");               // a4
            Console.WriteLine($"Compress(\"abbbccccc\") = {StringCompressor.Compress("abbbccccc")}");     // ab3c5

            // Распаковка
            Console.WriteLine($"Decompress(\"a3b2c3d2e\") = {StringCompressor.Decompress("a3b2c3d2e")}"); // aaabbcccdde
            Console.WriteLine($"Decompress(\"abc\") = {StringCompressor.Decompress("abc")}");             // abc
            Console.WriteLine($"Decompress(\"a4\") = {StringCompressor.Decompress("a4")}");               // aaaa
            Console.WriteLine($"Decompress(\"z10\") = {StringCompressor.Decompress("z10")}");             // zzzzzzzzzz

            // Интеграционные
            string original = "aaabbcccdde";
            string compressed = StringCompressor.Compress(original);
            string decompressed = StringCompressor.Decompress(compressed);
            Console.WriteLine($"Round-trip: {original} -> {compressed} -> {decompressed} = {original == decompressed}");

            // Ошибки
            try { StringCompressor.Compress(""); }
            catch (ArgumentException e) { Console.WriteLine($"Compress(\"\") -> Exception: {e.Message}"); }

            try { StringCompressor.Compress(null); }
            catch (ArgumentException e) { Console.WriteLine($"Compress(null) -> Exception: {e.Message}"); }
        }
        private static void Task2_Test()
        {
            int expectedSum = 0;
            var random = new Random();
            var tasks = new Task[100];

            // Запуск 100 параллельных операций
            Parallel.For(0, tasks.Length, i =>
            {
                // 30% задач — писатели, остальные — читатели
                if (i % 3 == 0)
                {
                    // Сценарий писателя
                    int value = random.Next(1, 5);
                    Server.AddToCount(value);

                    // Безопасное обновление ожидаемой суммы
                    Interlocked.Add(ref expectedSum, value);
                }
                else
                {
                    // Сценарий читателя
                    _ = Server.GetCount();
                }
            });

            // Проверка ожидания читателей
            int actualCount = Server.GetCount();

            Console.WriteLine($"Ожидаемая сумма: {expectedSum}");
            Console.WriteLine($"Фактический счетчик: {actualCount}");

            if (actualCount == expectedSum)
            {
                Console.WriteLine("Тест пройден: Синхронизация работает корректно.");
            }
            else
            {
                Console.WriteLine("Тесть провален: Обнаружена гонка данных.");
            }
        }

        private static void Task3_Test_1()
        {
            string inputPath = @"..\..\..\Task3\non-standardized.txt";
            string outputFilePath = @"..\..\..\Task3\standardized.txt";
            string problemsFilePath = @"..\..\..\Task3\problems.txt";

            try
            {
                LogStandardizer.GetStandardizedLogFiles(inputPath, outputFilePath, problemsFilePath);
                Console.WriteLine($"Обработка завершена!");
                Console.WriteLine($"Стандартизированные логи: {new DirectoryInfo(outputFilePath).FullName}");
                Console.WriteLine($"Проблемные записи: {new DirectoryInfo(problemsFilePath).FullName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        static void Task3_Test_2()
        {
            // Формат 1
            string input1 = "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'";
            string expected1 = "10-03-2025\t15:14:49.523\tINFO\tDEFAULT\tВерсия программы: '3.4.0.48729'";

            // Формат 2
            string input2 = "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";
            string expected2 = "10-03-2025\t15:14:51.5882\tINFO\tMobileComputer.GetDeviceId\tКод устройства: '@MINDEO-M40-D-410244015546'";

            // Тестирование
            string result1 = LogStandardizer.Standardize(input1);
            string result2 = LogStandardizer.Standardize(input2);

            Console.WriteLine($"Тест 1: {(result1 == expected1 ? "OK" : "ERROR")}");
            Console.WriteLine($"Тест 2: {(result2 == expected2 ? "OK" : "ERROR")}");

            // Проверка невалидной строки
            string invalid = "мусор";
            Console.WriteLine($"Тест 3 (невалидная строка -> null): {(LogStandardizer.Standardize(invalid) == null ? "OK" : "ERROR")}");
        }
    }
}
