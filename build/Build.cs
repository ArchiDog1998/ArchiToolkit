using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Serilog;

class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(GenerateProjects = true)] readonly Solution Solution;

    [Parameter] readonly string NuGetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");
    [Parameter] readonly string NuGetSource = "https://api.nuget.org/v3/index.json";

    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => d => d
        .Before(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(s => s.SetProject(Solution));
        });

    Target Restore => d => d
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(s => s
                .SetProjectFile(Solution)
            );
        });

    Target Compile => d => d
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration.Release)
                .EnableNoRestore());
        });

    Target CopyNuGetPackages => d => d
        .DependsOn(Compile)
        .Executes(() =>
        {
            var packageFiles = RootDirectory.GlobFiles("**/*.nupkg");

            if (OutputDirectory.Exists())
            {
                OutputDirectory.DeleteDirectory();
            }
            OutputDirectory.CreateDirectory();

            foreach (var package in packageFiles)
            {
                package.Copy(OutputDirectory / package.Name);
                package.DeleteFile();
            }
        });

    Target Push => d => d
        .DependsOn(CopyNuGetPackages)
        .OnlyWhenStatic(() => NuGetApiKey != null)
        .Executes(() =>
        {
            foreach (var package in OutputDirectory.GlobFiles("*.nupkg"))
            {
                if (IsPackageVersionOnNuGet(package)) continue;

                try
                {
                    DotNetTasks.DotNetNuGetPush(s => s
                        .SetTargetPath(package)
                        .SetSource(NuGetSource)
                        .SetApiKey(NuGetApiKey)
                    );
                }
                catch
                {
                    // ignored
                }
            }
        });

    public static int Main() => Execute<Build>(x => x.Push);

    bool IsPackageVersionOnNuGet(AbsolutePath packagePath)
    {
        var packageNameVersion = packagePath.NameWithoutExtension; // "MyPackage.1.0.0"
        var parts = packageNameVersion.Split('.');
        if (parts.Length < 2) return false;

        var packageName = string.Join(".", parts.TakeWhile(p => !uint.TryParse(p, out _))); // Extract "MyPackage"
        var packageVersion = string.Join(".", parts.SkipWhile(p => !uint.TryParse(p, out _))); // Extract "1.0.0"

        var nugetCheckUrl = $"https://api.nuget.org/v3-flatcontainer/{packageName.ToLower()}/index.json";

        try
        {
            using var client = new HttpClient();
            var response = client.GetStringAsync(nugetCheckUrl).Result;

            return response.Contains($"\"{packageVersion}\"");
        }
        catch
        {
            Log.Warning($"Failed to check NuGet for {packageName} {packageVersion}. Assuming it doesn't exist.");
            return false;
        }
    }
}