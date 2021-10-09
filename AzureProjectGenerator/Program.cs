using AzureProjectGenerator.Utility;
using Microsoft.Extensions.Logging;
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
            internal ILogger logger;
            public OsConfig(ILogger _logger)
            {
                isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                logger = _logger;
            }
        }

        private static void Main(string[] args)
        {
            var logFactory = new LoggerFactory();

            var logger = logFactory.CreateLogger<Type>();

            var osConfig = new OsConfig(logger);
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
            string templatesPath = Directory.GetCurrentDirectory();
            templatesPath = templatesPath.Split("bin")[0] + "Templates";


            if (os.isWindows)
            {
                string script = $@"cd {templatesPath}
                        dotnet pack
                        dotnet new -i .\bin\Debug\AzureProjectTemplate.1.0.0.nupkg
                        cd {output}
                        mkdir {name}.API
                        cd {name}.API
                        dotnet new azureprojectapi -n {name}
                        cd ..
                        mkdir {name}.Domain
                        cd {name}.Domain
                        dotnet new azureprojectdomain -n {name}
                        cd ..
                        mkdir {name}.Infra
                        cd {name}.Infra
                        dotnet new azureprojectinfra -n {name}
                        cd ..
                        dotnet new sln -n {name}
                        dotnet sln add {name}.API/{name}.API.csproj
                        dotnet sln add {name}.Domain/{name}.Domain.csproj
                        dotnet sln add {name}.Infra/{name}.Infra.csproj";

                PowerShell ps = PowerShell.Create();

                ps.AddScript(script);
                ps.Invoke();
            }
            else
            {
                string script = $@"cd {templatesPath}                                \
                        dotnet pack                                                  \
                        dotnet new -i ./bin/Debug/AzureProjectTemplate.1.0.0.nupkg   \
                        cd {output}                                                  \
                        mkdir {name}.API                                             \
                        cd {name}.API                                                \
                        dotnet new azureprojectapi -n {name}                         \
                        cd ..                                                        \
                        mkdir {name}.Domain                                          \
                        cd {name}.Domain                                             \
                        dotnet new azureprojectdomain -n {name}                      \
                        cd ..                                                        \
                        mkdir {name}.Infra                                           \
                        cd {name}.Infra                                              \
                        dotnet new azureprojectinfra -n {name}                       \
                        cd ..                                                        \
                        dotnet new sln -n {name}                                     \
                        dotnet sln add {name}.API/{name}.API.csproj                  \
                        dotnet sln add {name}.Domain/{name}.Domain.csproj            \
                        dotnet sln add {name}.Infra/{name}.Infra.csproj";

                _ = script.Bash(os.logger).Result;
            } 
        }
    }
}