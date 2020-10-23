using System.Diagnostics;

namespace Classroom_Client
{
    internal static partial class Utils
    {
        public static string ExecuteBashCommand(string command)
        {
            System.Console.WriteLine($"Executing command: {command}");
            command = command.Replace("\"", "\"\"");

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }

        public static string Notify(string text) => ExecuteBashCommand($"zenity --notification --text=\"{text}\"");
    }
}