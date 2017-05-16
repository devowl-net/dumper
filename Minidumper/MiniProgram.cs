using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Minidumper
{
    internal static class MiniProgram
    {
        [DllImport("dbghelp.dll")]
        private static extern bool MiniDumpWriteDump(IntPtr hProcess, int processId, IntPtr hFile, int dumpType,
            IntPtr exceptionParam, IntPtr userStreamParam, IntPtr callStackParam);

        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Not enough arguments. 1) ProcessId 2) minidumpType 2) Minidump file path");
                return;
            }

            try
            {
                var processId = int.Parse(args[0]);
                var minidumpType = int.Parse(args[1]);
                var filePath = args[2];
                var handler = Process.GetProcessById(processId).Handle;

                using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    MiniDumpWriteDump(
                        handler,
                        processId,
                        fileStream.SafeFileHandle.DangerousGetHandle(),
                        minidumpType,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("Arguments: " + string.Join(" ", args));
            }
        }
    }
}