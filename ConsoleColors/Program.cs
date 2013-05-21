// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoleColors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class Program
    {
        private static readonly object LockObj = new object();

        public static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Clear();

            var worker = new Thread(Run);
            worker.Start();
            worker.Join();
        }

        private static void Run()
        {
            Console.WindowWidth = 160;
            Console.WindowHeight = 70;

            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Console.WriteLine("Press 'q' to stop. Press any other key to start/restart.");
            var key = Console.ReadKey();

            while (!key.KeyChar.Equals('q'))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                var writers = GetWriters();
                var workers = writers.Select(StartThread).ToList();

                key = Console.ReadKey();
                writers.ForEach(w => w.Run = false);
                workers.ForEach(w => w.Join());
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorLeft = 0;
            Console.CursorTop = Console.BufferHeight - 1;
            Console.Write("Press any key to quit.");
            Console.ReadKey();
        }

        private static List<ConsoleWriterBase> GetWriters()
        {
            var writers = new List<ConsoleWriterBase>();
            writers.Add(new CrazyColorsConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 10, true));
            writers.Add(new SineWaveColorsConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 5));
            writers.Add(new SineWaveColorsConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 10));
            writers.Add(new SineWaveColorsConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 20));
            writers.Add(new SineWaveColorsConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 30));
            writers.Add(new SineWaveColorsConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 40));
            writers.Add(new SineWaveColorsConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 50));
            writers.Add(new ColorCirclesConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 5, 6, false));
            writers.Add(new ColorCirclesConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 10, 6, false));
            writers.Add(new ColorCirclesConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 20, 5, true));
            writers.Add(new ColorCirclesConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 30, 16, false));
            writers.Add(new ColorCirclesConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 40, 10, true));
            writers.Add(new ColorCirclesConsoleWriter(LockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 50, 6, true));
            return writers;
        }

        private static Thread StartThread(ConsoleWriterBase writer)
        {
            var worker = new Thread(writer.Go);
            worker.Start();
            return worker;
        }
    }
}
