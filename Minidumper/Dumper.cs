using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Dumper
{
    /// <summary>
    /// Create current process minidumps.
    /// </summary>
    public static class Dumper
    {
        private static readonly string MinidumperExe =
            "TVqQAAMAAAAEAAAA//8AALgAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAgAAAAA4fug4AtAnNIbgBTM0hVGhpcyBwcm9ncmFtIGNhbm5vdCBiZSBy" +
            "dW4gaW4gRE9TIG1vZGUuDQ0KJAAAAAAAAABQRQAATAEDAL+VC1kAAAAAAAAAAOAAIgAL" +
            "ATAAAAwAAAAGAAAAAAAALisAAAAgAAAAQAAAAABAAAAgAAAAAgAABAAAAAAAAAAEAAAA" +
            "AAAAAACAAAAAAgAAAAAAAAMAQIUAABAAABAAAAAAEAAAEAAAAAAAABAAAAAAAAAAAAAA" +
            "ANwqAABPAAAAAEAAAIgDAAAAAAAAAAAAAAAAAAAAAAAAAGAAAAwAAACkKQAAHAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAACAAAAAAAAAAA" +
            "AAAACCAAAEgAAAAAAAAAAAAAAC50ZXh0AAAANAsAAAAgAAAADAAAAAIAAAAAAAAAAAAA" +
            "AAAAACAAAGAucnNyYwAAAIgDAAAAQAAAAAQAAAAOAAAAAAAAAAAAAAAAAABAAABALnJl" +
            "bG9jAAAMAAAAAGAAAAACAAAAEgAAAAAAAAAAAAAAAAAAQAAAQgAAAAAAAAAAAAAAAAAA" +
            "AAAQKwAAAAAAAEgAAAACAAUAECEAAJQIAAABAAAAAgAABgAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABswBwCQAAAAAQAAEQKOaRouC3IB" +
            "AABwKA8AAAoqABIAAhaaKBAAAAooEQAACgIXmigQAAAKCwIYmgIZmigQAAAKDBdzEgAA" +
            "Cg0GBwlvEwAACm8UAAAKCH4VAAAKfhUAAAp+FQAACigBAAAGJt4KCSwGCW8WAAAK3N4h" +
            "KBcAAApypgAAcHK+AABwAigYAAAKKBkAAAooDwAACt4AKgEcAAACAD0AJWIACgAAAAAA" +
            "ABIAXG4AIRIAAAEeAigaAAAKKkJTSkIBAAEAAAAAAAwAAAB2NC4wLjMwMzE5AAAAAAUA" +
            "bAAAAIwCAAAjfgAA+AIAAHwDAAAjU3RyaW5ncwAAAAB0BgAAxAAAACNVUwA4BwAAEAAA" +
            "ACNHVUlEAAAASAcAAEwBAAAjQmxvYgAAAAAAAAACAAABRxUCFAkAAAAA+gEzABYAAAEA" +
            "AAAaAAAAAgAAAAMAAAAIAAAAGgAAAA4AAAABAAAAAQAAAAEAAAABAAAAAQAAAAAABwIB" +
            "AAAAAAAGAHwBEgMGAOkBEgMGALAA4AIPADIDAAAGANgAlQIGAF8BlQIGAEABlQIGANAB" +
            "lQIGAJwBlQIGALUBlQIGAO8AlQIGAMQA8wIGAKIA8wIGACMBlQIGAAoBFgIGAHIDhAIG" +
            "AEMCEAAGAKcChAIGAHkAhAIGAAEAhAIGANkChAIGAC0AEAAGAFEAQQMGAEIA8wIGADYA" +
            "hAIGADAChAIAAAAABwAAAAAAAQABAAEAEAB8AsgCQQABAAEAAAAAAIAAkSC2AloAAQBQ" +
            "IAAAAACWAIsCZQAIAAghAAAAAIYY0wIGAAkAAAABAGIDAAACACMAAAADAHMAAAAEAIsA" +
            "AAAFAG0CAAAGAF0CAAAHAE4CAAABAF0DCQDTAgEAEQDTAgYAGQDTAgoAKQDTAhAAMQDT" +
            "AhAAOQDTAhAAQQDTAhAASQDTAhAAUQDTAhAAWQDTAhAAYQDTAhUAaQDTAhAAcQDTAhAA" +
            "eQDTAhAAmQCBACIAoQCcACcAqQDTAgEAiQDTAiwAiQBNADMAwQBgADgAqQCxAjwAyQCU" +
            "AAYAmQCBAD8A0QCQAkQA0QBrA0sAgQDTAgYALgALAGsALgATAHQALgAbAJMALgAjAJwA" +
            "LgArAKwALgAzAKwALgA7AKwALgBDAJwALgBLALIALgBTAKwALgBbAKwALgBjAMoALgBr" +
            "APQALgBzAAEBGgA3AgABAwC2AgEABIAAAAEAAAAAAAAAAAAAAAAAyAIAAAQAAAAAAAAA" +
            "AAAAAFEAGgAAAAAAAAAASW50MzIAPE1vZHVsZT4AU3lzdGVtLklPAG1zY29ybGliAHBy" +
            "b2Nlc3NJZABGaWxlTW9kZQBJRGlzcG9zYWJsZQBTYWZlSGFuZGxlAGdldF9TYWZlRmls" +
            "ZUhhbmRsZQBEYW5nZXJvdXNHZXRIYW5kbGUAaEZpbGUAQ29uc29sZQBXcml0ZUxpbmUA" +
            "ZHVtcFR5cGUARGlzcG9zZQBQYXJzZQBHdWlkQXR0cmlidXRlAERlYnVnZ2FibGVBdHRy" +
            "aWJ1dGUAQ29tVmlzaWJsZUF0dHJpYnV0ZQBBc3NlbWJseVRpdGxlQXR0cmlidXRlAEFz" +
            "c2VtYmx5VHJhZGVtYXJrQXR0cmlidXRlAFRhcmdldEZyYW1ld29ya0F0dHJpYnV0ZQBB" +
            "c3NlbWJseUZpbGVWZXJzaW9uQXR0cmlidXRlAEFzc2VtYmx5Q29uZmlndXJhdGlvbkF0" +
            "dHJpYnV0ZQBBc3NlbWJseURlc2NyaXB0aW9uQXR0cmlidXRlAENvbXBpbGF0aW9uUmVs" +
            "YXhhdGlvbnNBdHRyaWJ1dGUAQXNzZW1ibHlQcm9kdWN0QXR0cmlidXRlAEFzc2VtYmx5" +
            "Q29weXJpZ2h0QXR0cmlidXRlAEFzc2VtYmx5Q29tcGFueUF0dHJpYnV0ZQBSdW50aW1l" +
            "Q29tcGF0aWJpbGl0eUF0dHJpYnV0ZQBNaW5pZHVtcGVyLmV4ZQBTeXN0ZW0uUnVudGlt" +
            "ZS5WZXJzaW9uaW5nAFN0cmluZwBkYmdoZWxwLmRsbABGaWxlU3RyZWFtAGNhbGxTdGFj" +
            "a1BhcmFtAHVzZXJTdHJlYW1QYXJhbQBleGNlcHRpb25QYXJhbQBQcm9ncmFtAFN5c3Rl" +
            "bQBNYWluAEpvaW4AU3lzdGVtLlJlZmxlY3Rpb24ARXhjZXB0aW9uAFplcm8ATWluaUR1" +
            "bXBXcml0ZUR1bXAATWluaWR1bXBlcgAuY3RvcgBJbnRQdHIAU3lzdGVtLkRpYWdub3N0" +
            "aWNzAFN5c3RlbS5SdW50aW1lLkludGVyb3BTZXJ2aWNlcwBTeXN0ZW0uUnVudGltZS5D" +
            "b21waWxlclNlcnZpY2VzAERlYnVnZ2luZ01vZGVzAE1pY3Jvc29mdC5XaW4zMi5TYWZl" +
            "SGFuZGxlcwBhcmdzAGhQcm9jZXNzAENvbmNhdABPYmplY3QAAAAAAICjTgBvAHQAIABl" +
            "AG4AbwB1AGcAaAAgAGEAcgBnAHUAbQBlAG4AdABzAC4AIAAxACkAIABwAHIAbwBjAGUA" +
            "cwBzACAAaABhAG4AZABsAGUAcgAgADIAKQAgAHAAcgBvAGMAZQBzAHMASQBkACAAMwAp" +
            "ACAAZgBpAGwAZQBQAGEAdABoACAANAApACAAbQBpAG4AaQBkAHUAbQBwAFQAeQBwAGUA" +
            "ABdBAHIAZwB1AG0AZQBuAHQAcwA6ACAAAAMgAAAAAP2FrtO4J7JEh1OuvIWO9Y0ABCAB" +
            "AQgDIAABBSABARERBCABAQ4EIAEBAgcHBBgICBJFBAABAQ4EAAEIDgYgAgEOEVkEIAAS" +
            "XQMgABgCBhgEAAEBHAYAAg4OHQ4FAAIODg4It3pcVhk04IkKAAcCGAgYCBgYGAUAAQEd" +
            "DggBAAgAAAAAAB4BAAEAVAIWV3JhcE5vbkV4Y2VwdGlvblRocm93cwEIAQACAAAAAAAP" +
            "AQAKTWluaWR1bXBlcgAABQEAAAAAFwEAEkNvcHlyaWdodCDCqSAgMjAxNwAAKQEAJGRi" +
            "OTAxOTVlLTBlYWItNDIwYy1iOTUyLWQxNmE3ZTE1Y2E1MQAADAEABzEuMC4wLjAAAEcB" +
            "ABouTkVURnJhbWV3b3JrLFZlcnNpb249djQuMAEAVA4URnJhbWV3b3JrRGlzcGxheU5h" +
            "bWUQLk5FVCBGcmFtZXdvcmsgNAAAAAAAAAC/lQtZAAAAAAIAAAAcAQAAwCkAAMALAABS" +
            "U0RTd2N6HwcY3EqGQlehZH6pvwEAAABEOlwhSGFja1whTXlQcm9qZWN0c1xEdW1wZXJc" +
            "TWluaWR1bXBlclxvYmpcUmVsZWFzZVxNaW5pZHVtcGVyLnBkYgAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQrAAAAAAAAAAAAAB4rAAAAIAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAQKwAAAAAAAAAAAAAAAF9Db3JFeGVNYWluAG1zY29yZWUu" +
            "ZGxsAAAAAAD/JQAgQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAEAAAABgAAIAAAAAAAAAAAAAAAAAA" +
            "AAEAAQAAADAAAIAAAAAAAAAAAAAAAAAAAAEAAAAAAEgAAABYQAAALAMAAAAAAAAAAAAA" +
            "LAM0AAAAVgBTAF8AVgBFAFIAUwBJAE8ATgBfAEkATgBGAE8AAAAAAL0E7/4AAAEAAAAB" +
            "AAAAAAAAAAEAAAAAAD8AAAAAAAAABAAAAAEAAAAAAAAAAAAAAAAAAABEAAAAAQBWAGEA" +
            "cgBGAGkAbABlAEkAbgBmAG8AAAAAACQABAAAAFQAcgBhAG4AcwBsAGEAdABpAG8AbgAA" +
            "AAAAAACwBIwCAAABAFMAdAByAGkAbgBnAEYAaQBsAGUASQBuAGYAbwAAAGgCAAABADAA" +
            "MAAwADAAMAA0AGIAMAAAABoAAQABAEMAbwBtAG0AZQBuAHQAcwAAAAAAAAAiAAEAAQBD" +
            "AG8AbQBwAGEAbgB5AE4AYQBtAGUAAAAAAAAAAAA+AAsAAQBGAGkAbABlAEQAZQBzAGMA" +
            "cgBpAHAAdABpAG8AbgAAAAAATQBpAG4AaQBkAHUAbQBwAGUAcgAAAAAAMAAIAAEARgBp" +
            "AGwAZQBWAGUAcgBzAGkAbwBuAAAAAAAxAC4AMAAuADAALgAwAAAAPgAPAAEASQBuAHQA" +
            "ZQByAG4AYQBsAE4AYQBtAGUAAABNAGkAbgBpAGQAdQBtAHAAZQByAC4AZQB4AGUAAAAA" +
            "AEgAEgABAEwAZQBnAGEAbABDAG8AcAB5AHIAaQBnAGgAdAAAAEMAbwBwAHkAcgBpAGcA" +
            "aAB0ACAAqQAgACAAMgAwADEANwAAACoAAQABAEwAZQBnAGEAbABUAHIAYQBkAGUAbQBh" +
            "AHIAawBzAAAAAAAAAAAARgAPAAEATwByAGkAZwBpAG4AYQBsAEYAaQBsAGUAbgBhAG0A" +
            "ZQAAAE0AaQBuAGkAZAB1AG0AcABlAHIALgBlAHgAZQAAAAAANgALAAEAUAByAG8AZAB1" +
            "AGMAdABOAGEAbQBlAAAAAABNAGkAbgBpAGQAdQBtAHAAZQByAAAAAAA0AAgAAQBQAHIA" +
            "bwBkAHUAYwB0AFYAZQByAHMAaQBvAG4AAAAxAC4AMAAuADAALgAwAAAAOAAIAAEAQQBz" +
            "AHMAZQBtAGIAbAB5ACAAVgBlAHIAcwBpAG8AbgAAADEALgAwAC4AMAAuADAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAACAAAAwAAAAwOwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAA =";

        /// <summary>
        /// Folder for saved minidumps.
        /// </summary>
        public const string DumpDirectory = "Minidump";

        [DllImport("dbghelp.dll")]
        private static extern bool MiniDumpWriteDump(IntPtr hProcess, int processId, IntPtr hFile, int dumpType,
            IntPtr exceptionParam, IntPtr userStreamParam, IntPtr callStackParam);

        /// <summary>
        /// Write minidump to file.
        /// </summary>
        /// <param name="ownProcess">Current process as minidump maker or out of process application.</param>
        /// <param name="minidumpType">Minidump type.</param>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void WriteDump(bool ownProcess = true, MinidumpType minidumpType = MinidumpType.MiniDumpWithFullMemory)
        {
            if (!Directory.Exists(DumpDirectory))
            {
                Directory.CreateDirectory(DumpDirectory);
            }

            var currentProcess = Process.GetCurrentProcess();
            var fileName = string.Format("{0}_{1}_{2}.dmp", currentProcess.ProcessName,
                DateTime.Now.ToString("yyyy-dd-mm_HH-mm-ss"),
                Path.GetRandomFileName().Replace(".", ""));

            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(currentDir, DumpDirectory, fileName);
            var handler = currentProcess.Handle;
            var processId = currentProcess.Id;

            Console.WriteLine($"Handle = {handler} ProcessId = {processId}");
            if (ownProcess)
            {
                using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    MiniDumpWriteDump(
                        handler,
                        processId,
                        fileStream.SafeFileHandle.DangerousGetHandle(),
                        (int)minidumpType,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        IntPtr.Zero);
                }
            }
            else
            {
                var arguments = string.Join(" ", new object[]
                {
                    processId,
                    (int)minidumpType,
                    filePath
                }.Select(arg => string.Format("\"{0}\"", arg.ToString())));

                var minidumperPath = "Minidumper.exe";

                //var minidumperPath = CreateMinidumper();
                var process = Process.Start(minidumperPath, string.Join(" ", arguments));
                if (process != null)
                {
                    process.WaitForExit(10000);
                    //File.Delete(minidumperPath);
                }
            }
        }
        
        private static string CreateMinidumper()
        {
            var fileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".exe");
            File.WriteAllBytes(fileName, Convert.FromBase64String(MinidumperExe));
            return fileName;
        }
    }

    /// <summary>
    /// Identifies the type of information that will be written to the minidump file by the MiniDumpWriteDump function.
    /// </summary>
    /// <remarks>
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms680519(v=vs.85).aspx
    /// </remarks>
    [Flags]
    public enum MinidumpType
    {
        MiniDumpNormal = 0x00000000,
        MiniDumpWithDataSegs = 0x00000001,
        MiniDumpWithFullMemory = 0x00000002,
        MiniDumpWithHandleData = 0x00000004,
        MiniDumpFilterMemory = 0x00000008,
        MiniDumpScanMemory = 0x00000010,
        MiniDumpWithUnloadedModules = 0x00000020,
        MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
        MiniDumpFilterModulePaths = 0x00000080,
        MiniDumpWithProcessThreadData = 0x00000100,
        MiniDumpWithPrivateReadWriteMemory = 0x00000200,
        MiniDumpWithoutOptionalData = 0x00000400,
        MiniDumpWithFullMemoryInfo = 0x00000800,
        MiniDumpWithThreadInfo = 0x00001000,
        MiniDumpWithCodeSegs = 0x00002000,
        MiniDumpWithoutAuxiliaryState = 0x00004000,
        MiniDumpWithFullAuxiliaryState = 0x00008000,
        MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
        MiniDumpIgnoreInaccessibleMemory = 0x00020000,
        MiniDumpWithTokenInformation = 0x00040000
    };
}