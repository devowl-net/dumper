namespace Dumper
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Dumps;
    
    internal static class Program
    {
        private static bool _stopThread;

        /// <summary>
        /// Entry point.
        /// </summary>
        static void Main(string[] args)
        {
            Directory.GetFiles(DumpUtils.DumpDirectory).ToList().ForEach(File.Delete);
            CreateThread();
            TestMethod();
            _stopThread = true;
        }

        private static void CreateThread()
        {
            new Thread(() =>
            {
                while (!_stopThread)
                {
                    Console.WriteLine("while(true)");
                }
            })
            {
                Name = "While(true) thread"
            }.Start();
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static void TestMethod()
        {
            var dateTime = DateTime.Now;
            try
            {
                Console.WriteLine(dateTime);

                int someDouble = 0;
                someDouble = 10 / someDouble;
            }
            catch (Exception)
            {
                DumpUtils.WriteDump();
            }
        }
    }
}
