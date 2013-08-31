using System;
using System.Diagnostics;
using System.IO;

namespace Bomberman
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ProcessArgs(args);

            using (BmGame game = new BmGame(args))
            {
                game.Run();
            }
        }

        [Conditional("DEBUG")]
        private static void ProcessArgs(String[] args)
        {
            String batch = FindArg(args, "-B");
            if (batch != null)
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = batch;
                p.Start();

                p.WaitForExit();
                if (p.ExitCode == 0)
                {
                    String exec = FindArg(args, "-R");
                    if (exec != null)
                    {
                        ProcessStartInfo info = new ProcessStartInfo();
                        info.WorkingDirectory = Path.GetDirectoryName(exec);
                        info.UseShellExecute = false;
                        info.FileName = exec;

                        p = Process.Start(info);
                    }
                }
            }
        }

        private static String FindArg(String[] args, String prefix)
        {
            foreach (String arg in args)
            {
                if (arg.StartsWith(prefix))
                {
                    return arg.Substring(prefix.Length);
                }
            }

            return null;
        }
    }
#endif
}

