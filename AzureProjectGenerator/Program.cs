using System;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace AzureProjectGenerator
{
    internal class Program
    {
        private class OsConfig
        {
            internal bool isWindows;
            internal bool isMac;
            internal bool isLinux;
            public OsConfig()
            {
                isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            }
        }

        private static void Main(string[] args)
        {

            var osConfig = new OsConfig();
            Console.WriteLine("Current OS: " + RuntimeInformation.OSDescription);
            Console.WriteLine($"Is windows: {osConfig.isWindows}");
            Console.WriteLine($"Is linux: {osConfig.isLinux}");
            Console.WriteLine($"Is mac: {osConfig.isMac}");

            string name = string.Empty;
            string output = string.Empty;
            for (int i=0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg)
                {
                    case "-n":
                    case "--name":
                        name = args[i + 1];
                        break;
                    case "-o":
                    case "--output":
                        output = args[i + 1];
                        break;
                    default:
                        break;
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("'Name' is required");
            }

            if (string.IsNullOrEmpty(output))
            {
                Console.WriteLine("'Output' is required");
            }

            if (string.IsNullOrEmpty(name)
                && string.IsNullOrEmpty(output))
            {
                Console.WriteLine("No valid arguments were found");
                return;
            }

            if(string.IsNullOrEmpty(name) 
                || string.IsNullOrEmpty(output))
            {
                return;
            }

            GeneratePack(osConfig, name, output);
        }

        private static void GeneratePack(OsConfig os, string name, string output)
        {
            string path = Directory.GetCurrentDirectory();
            path = path.Split("bin")[0] + "Templates";

            if (os.isWindows)
            {
                PowerShell ps = PowerShell.Create();
                ps.AddCommand("Get-Process");
                ps.Invoke();
            }
            else
            {
                var psi = new ProcessStartInfo();
                psi.FileName = "/bin/bash";
                psi.Arguments = "";
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;

                using var process = Process.Start(psi);
            }
            /*
             * on the template folder
             * 
             * WINDOWS:
             * dotnet new -i .\
             * 
             * 
             * LINUX:
             * dotnet new -i ./
             * 
             * on the parent of templates folder
             * dotnet new console -n {name} -o .
             * dotnet pack
             * dotnet new -i PATH_TO_NUPKG_FILE
             */
        }
    }
}