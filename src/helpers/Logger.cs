using System.Collections.Concurrent;

namespace TrgHelpers.Logging
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

    public static class Logging
    {
        private static readonly BlockingCollection<string> _logQueue = new BlockingCollection<string>();
        private static readonly Task _processingTask;
        private static CancellationTokenSource _cancellationTokenSource;
        
        private static string _logFilePath;

        static Logging()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            string logDirectory = "Logs";
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            string timestamp = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
            _logFilePath = Path.Combine(logDirectory, $"log_{timestamp}.log");

            _processingTask = Task.Factory.StartNew(
                ProcessLogQueue,
                _cancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private static void ProcessLogQueue()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_logFilePath, true))
                {
                    writer.AutoFlush = true;

                    foreach (string message in _logQueue.GetConsumingEnumerable(_cancellationTokenSource.Token))
                    {
                        Console.WriteLine(message);
                        writer.WriteLine(message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // FIXME
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur fatale dans le logger: {ex.Message}");
            }
        }

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (!_logQueue.IsAddingCompleted)
            {
                string logLine = $"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] [{level}] {message}";
                _logQueue.Add(logLine);
            }
        }

        public static void Stop()
        {
            _logQueue.CompleteAdding();

            try
            {
                _processingTask.Wait(1500);
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine($"Exception dans le logger lors de l'arrêt: {e.Message}");
                }
            }
            finally
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public static void LogInfo(string message) => Log(message, LogLevel.Info);
        public static void LogWarning(string message) => Log(message, LogLevel.Warning);
        public static void LogError(string message) => Log(message, LogLevel.Error);
        public static void LogDebug(string message) => Log(message, LogLevel.Debug);
    }
}