using CommandLine;
using System;
using System.Runtime.InteropServices;

namespace AzureProjectGenerator
{
    internal class Program
    {
        public interface ICommand
        {
            void Execute();
        }

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
        }

        [Verb("generate", isDefault:true, HelpText = "Generates the template for the web project to be published on Azure")]
        public class Generate : ICommand
        {
            [Option('n', "name", Required = true, HelpText = "The name that the projects will have")]
            public string Name { get; set; }
            public void Execute()
            {
                var osConfig = new OsConfig();
                Console.WriteLine("Executing Push");
            }
        }

        private void GeneratePack(OsConfig os)
        {
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