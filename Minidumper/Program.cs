using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Minidumper
{
    public class Program
    {
        [DllImport("dbghelp.dll")]
        private static extern bool MiniDumpWriteDump(IntPtr hProcess, int processId, IntPtr hFile, int dumpType,
            IntPtr exceptionParam, IntPtr userStreamParam, IntPtr callStackParam);
        
        public static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Not enough arguments. 1) process handler 2) processId 3) filePath 4) minidumpType");
                return;
            }

            try
            {
                var handler = new IntPtr(int.Parse(args[0]));
                var processId = int.Parse(args[1]);
                var filePath = args[2];
                var minidumpType = int.Parse(args[3]);

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
