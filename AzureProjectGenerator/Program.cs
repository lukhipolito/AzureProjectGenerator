using CShellNet;
using Medallion.Shell;
using Microsoft.Extensions.Logging;
using System;
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
            string azureUser = string.Empty;
            string azurePassword = string.Empty;
            string azureRegion = string.Empty;
            string azureSubscriptionId = string.Empty;
            string azureResourceGroup = string.Empty;
            string azureAppServiceName = string.Empty;

            var logFactory = new LoggerFactory();

            var logger = logFactory.CreateLogger<Type>();

            var osConfig = new OsConfig(logger);
            string name = string.Empty;
            string output = string.Empty;
            for (int i = 0; i < args.Length; i++)
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

            if (string.IsNullOrEmpty(name)
                || string.IsNullOrEmpty(output))
            {
                return;
            }

            Console.WriteLine("Current OS: " + RuntimeInformation.OSDescription);
            Console.WriteLine($"Is windows: {osConfig.isWindows}");
            Console.WriteLine($"Is linux: {osConfig.isLinux}");
            Console.WriteLine($"Is mac: {osConfig.isMac}");
            Console.WriteLine("");

            var input = string.Empty;
            Console.WriteLine("Please enter the following:");
            //Console.WriteLine("Azure account user: ");
            //input =  Console.ReadLine();
            //while (string.IsNullOrEmpty(input))
            //{
            //    Console.WriteLine("please provide a valid username");
            //    input = Console.ReadLine();
            //}

            //azureUser = input;
            //Console.WriteLine("");

            //Console.WriteLine("Azure account password: ");
            //input = Console.ReadLine();
            //while (string.IsNullOrEmpty(input))
            //{
            //    Console.WriteLine("please provide a valid password");
            //    input = Console.ReadLine();
            //}

            //azurePassword = input;
            //Console.WriteLine("");

            //Console.WriteLine("Region: ");
            //input = Console.ReadLine();
            //while (string.IsNullOrEmpty(input))
            //{
            //    Console.WriteLine("please provide a valid region");
            //    input = Console.ReadLine();
            //}

            //azureRegion = input;
            //Console.WriteLine("");

            Console.WriteLine("Subscription ID: ");
            input = Console.ReadLine();
            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("please provide a valid Subscription ID");
                input = Console.ReadLine();
            }

            azureSubscriptionId = input;
            Console.WriteLine("");

            Console.WriteLine("Resource group name: ");
            input = Console.ReadLine();
            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("please provide a valid resource group name");
                input = Console.ReadLine();
            }

            azureResourceGroup = input;
            Console.WriteLine("");

            Console.WriteLine("App Service instance name: ");
            input = Console.ReadLine();
            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("please provide a valid App Service instance name");
                input = Console.ReadLine();
            }

            azureAppServiceName = input;

            Console.WriteLine("");

            Console.WriteLine($"Creating '{name}' project at path: {output}");
            GeneratePack(osConfig, name, output, azureSubscriptionId, azureAppServiceName, azureResourceGroup);
            Console.WriteLine("Project created!");
            Console.WriteLine("");
            Console.WriteLine("Creating App Service");
            Console.WriteLine("App Service Ready!");
            Console.WriteLine("");
            Console.WriteLine("Configuring Azure Pipeline");
            Console.WriteLine("Pipeline ready!");
            Console.WriteLine("");
        }

        private static void GeneratePack(OsConfig os, string name, string output, 
            string azureSubscriptionId, string azureAppServiceName, string azureResourceGroup)
        {
            string templatesPath = Directory.GetCurrentDirectory();
            string azurePipelinesPath = string.Empty;

            if (os.isWindows)
            {
                templatesPath = templatesPath.Split("bin")[0] + "Templates";

                string script_pack = $@"cd {templatesPath}
                        dotnet pack";

                string script_install = "dotnet new -i .\\bin\\Debug\\AzureProjectTemplate.1.0.0.nupkg";

                string script = $@"
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
                        mkdir pipeline
                        cd pipeline
                        dotnet new azureprojectpipeline -n {name}
                        cd ..
                        dotnet new azureprojectpipelineyml -n {name}
                        dotnet new sln -n {name}
                        dotnet sln add {name}.API/{name}.API.csproj
                        dotnet sln add {name}.Domain/{name}.Domain.csproj
                        dotnet sln add {name}.Infra/{name}.Infra.csproj";

                PowerShell ps = PowerShell.Create();

                ps.AddScript(script_pack);
                ps.Invoke();

                ps.AddScript(script_install);
                ps.Invoke();

                ps.AddScript(script);
                ps.Invoke();

                azurePipelinesPath = $"{output}\\azure-pipelines.yml";
                
            }
            else
            {
                templatesPath = templatesPath.Split("bin")[0] + "/Templates";
                var shell = new CShell();
                Console.WriteLine("templatesPath = " + templatesPath);

                _ = shell.ChangeFolder(templatesPath);

                _ = shell.Run("dotnet", "pack")
                    .AsResult().Result;

                _ = shell.Run("dotnet", "new", "-i", "./bin/Debug/AzureProjectTemplate.1.0.0.nupkg")
                    .AsResult().Result;

                _ = shell.ChangeFolder(output);
                
                _ = shell.Run("mkdir", $"{name}.API" )
                    .AsResult().Result;

                _ = shell.ChangeFolder($"{name}.API");

                _ = shell.Run("dotnet", "new", "azureprojectapi", "-n", name)
                    .AsResult().Result;

                _ = shell.ChangeFolder("..");

                _ = shell.Run("mkdir", $"{name}.Domain")
                    .AsResult().Result;

                _ = shell.ChangeFolder($"{name}.Domain");

                _ = shell.Run("dotnet", "new", "azureprojectdomain", "-n", name)
                    .AsResult().Result;

                _ = shell.ChangeFolder("..");

                _ = shell.Run("mkdir", $"{name}.Infra")
                    .AsResult().Result;

                _ = shell.ChangeFolder($"{name}.Infra");

                _ = shell.Run("dotnet", "new", "azureprojectinfra", "-n", name)
                    .AsResult().Result;

                _ = shell.ChangeFolder("..");

                _ = shell.Run("mkdir", "pipeline")
                    .AsResult().Result;

                _ = shell.ChangeFolder("pipeline");

                _ = shell.Run("dotnet", "new", "azureprojectpipeline", "-n", name)
                    .AsResult().Result;

                _ = shell.ChangeFolder("..");

                _ = shell.Run("dotnet", "new", "azureprojectpipelineyml", "-n", name)
                    .AsResult().Result;                            

                _ = shell.Run("dotnet", "new", "sln", "-n", name)
                    .AsResult().Result;

                _ = shell.Run("dotnet", "sln", "add", $"{name}.API/{name}.API.csproj")
                    .AsResult().Result;

                _ = shell.Run("dotnet", "sln", "add", $"{name}.Domain/{name}.Domain.csproj")
                    .AsResult().Result;

                _ = shell.Run("dotnet", "sln", "add", $"{name}.Infra/{name}.Infra.csproj")
                    .AsResult().Result;

                azurePipelinesPath = $"{output}/azure-pipelines.yml";
            }

            StreamReader reader = new StreamReader(File.OpenRead(azurePipelinesPath));
            string fileContent = reader.ReadToEnd();
            reader.Close();
            fileContent = fileContent.Replace("{{azResourceName}}", azureResourceGroup);
            fileContent = fileContent.Replace("{{azWebAppName}}", azureAppServiceName);
            fileContent = fileContent.Replace("{{azSubscription}}", azureSubscriptionId);
            StreamWriter writer = new StreamWriter(File.OpenWrite(azurePipelinesPath));
            writer.Write(fileContent);
            writer.Close();
        }


    }
}