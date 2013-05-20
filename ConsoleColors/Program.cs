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
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;

    class Program
    {
        private static readonly object lockObj = new object();

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Clear();

            var worker = new Thread(Run);
            worker.Start();
            worker.Join();
        }

        private static void WriteLine(string line)
        {
            lock (lockObj)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.CursorLeft = 0;
                Console.CursorTop = Console.BufferHeight - 1;

                Console.WriteLine(line);
            }
        }

        private static void Run()
        {
            //Console.WindowWidth = 160;
            //Console.WindowHeight = 70;

            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Console.WriteLine("Press 'q' to stop. Press any other key to start/restart.");
            var key = Console.ReadKey();

            while (!key.KeyChar.Equals('q'))
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();

                var writers = GetWriters();
                var workers = writers.Select(w => StartThread(w)).ToList();

                key = Console.ReadKey();
                writers.ForEach(w => w.Run = false);
                workers.ForEach(w => w.Join());
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorLeft = 0;
            Console.CursorTop = Console.BufferHeight - 1;
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        }

        private static List<ConsoleWriterBase> GetWriters()
        {
            var writers = new List<ConsoleWriterBase>();
            //writers.Add(new CrazyColorsConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 10, false));
            //writers.Add(new SineWaveColorsConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 1));
            writers.Add(new SineWaveColorsConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 5));
            writers.Add(new SineWaveColorsConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 10));
            writers.Add(new SineWaveColorsConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 20));
            writers.Add(new SineWaveColorsConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 30));
            writers.Add(new SineWaveColorsConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 40));
            writers.Add(new SineWaveColorsConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 50));
            writers.Add(new ColorCirclesConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 5));
            writers.Add(new ColorCirclesConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 10));
            writers.Add(new ColorCirclesConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 20));
            writers.Add(new ColorCirclesConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 30));
            writers.Add(new ColorCirclesConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 40));
            writers.Add(new ColorCirclesConsoleWriter(lockObj, Console.BufferWidth - 1, Console.BufferHeight - 1, 50));
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
