using System;
using System.IO;
using System.Threading;

namespace TaskPipeline
{
    enum TaskStatus
    {
        New,
        InProgress,
        Completed,
        Failed
    }
    struct TaskInfo
    {
        public int Id;
        public string Name;

        public TaskInfo (int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    class PipelineTask
    {
        public TaskInfo Info;
        public TaskStatus Status;

        public PipelineTask(int  id, string name)
        {
            Info = new TaskInfo (id, name);
            Status = TaskStatus.New;
        }

        public void Execute()
        {
            Status = TaskStatus.InProgress;
            Thread.Sleep(500);
            Status = TaskStatus.Completed;
        }
    }

    class Logger : IDisposable
    {
        private StreamWriter _writer;

        public Logger(string filePath)
        {
            _writer = new StreamWriter(filePath, true);
            _writer.AutoFlush = true;
        }

        public void Log(string message)
        {
            string logLine = $"[{DateTime.Now:HH:mm:ss}] {message}";
            _writer.WriteLine(logLine);
            Console.WriteLine(logLine);
        }
        
        public void Dispose()
        {
            _writer?.Close();
            _writer?.Dispose();
            Console.WriteLine("Логгер закрыт.");
        }
    }

    internal class Program
    {
        static void ChangeTaskInfo(TaskInfo info)
        {
            info.Id = 999;
            Console.WriteLine($"Внутри метода ID = {info.Id}");
        }

        static void Main(string[] args)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string logPath = Path.Combine(desktopPath, "pipeline.log");

            var logger = new Logger(logPath);
                logger.Log("Запуск Pipeline...");

                var task1 = new PipelineTask(1, "Build Solution");

                Console.WriteLine($"До вызова: task1.Info.Id = {task1.Info.Id}");
                ChangeTaskInfo(task1.Info);
                Console.WriteLine($"После вызова: task1.Info.Id = {task1.Info.Id}");

                var task2 = new PipelineTask(2, "Run Tests");

                logger.Log($"Задача: {task1.Info.Name}");
                task1.Execute();
                logger.Log($"Статус: {task1.Status}");

                logger.Log($"Задача: {task2.Info.Name}");
                task2 .Execute();
                logger.Log($"Статус: {task2.Status}");

                logger.Log("Pipeline завершен.");

            logger.Dispose();
            Console.ReadKey();
        }
    }
}
