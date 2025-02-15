using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Serilog;

[GitHubActions("nuke", GitHubActionsImage.WindowsLatest,
    On = [GitHubActionsTrigger.Push],
    ImportSecrets = [nameof(NuGetApiKey)],
    InvokedTargets = [nameof(PushNugetPackages)])]
class Build : NukeBuild
{
    [Parameter] readonly string NuGetApiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");
    [Parameter] readonly string NuGetSource = "https://api.nuget.org/v3/index.json";
    [Solution(GenerateProjects = true)] readonly Solution Solution;

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

    Target Test => d => d
        .DependsOn(Compile)
        .Executes(() =>
        {
            TestProject(Solution.tests.ArchiToolkit_Assertions_Tests);
            TestProject(Solution.tests.ArchiToolkit_PureConst_Tests);
        });

    Target CopyNuGetPackages => d => d
        .DependsOn(Test)
        .Executes(() =>
        {
            OutputDirectory.CreateOrCleanDirectory();
            var packageFiles = RootDirectory.GlobFiles("**/*.nupkg");

            foreach (var package in packageFiles)
            {
                package.Copy(OutputDirectory / package.Name, ExistsPolicy.FileOverwriteIfNewer);
                package.DeleteFile();
            }
        });

    Target PushNugetPackages => d => d
        .DependsOn(CopyNuGetPackages)
        .OnlyWhenStatic(() => NuGetApiKey != null)
        .Executes(() =>
        {
            foreach (var package in OutputDirectory.GlobFiles("*.nupkg"))
            {
                var existVersions = GetPackageVersions(package, out var packageId, out var version);
                foreach (var deletingVersion in VersionsShouldDelete(existVersions))
                    DeletePackage(packageId, deletingVersion);

                if (existVersions.Contains(version)) continue;

                PushPackage(package);
            }
        });

    private static void TestProject(Project project) =>
        DotNetTasks.DotNetTest(s => s
            .SetProjectFile(project)
            .SetConfiguration(Configuration.Release)
            .EnableNoRestore()
            .EnableNoBuild()
            .SetLoggers("trx"));

    public static int Main() => Execute<Build>(x => x.PushNugetPackages);

    IEnumerable<Version> VersionsShouldDelete(Version[] versions)
    {
        var canDelete = versions.OrderDescending().Skip(10).ToArray();

        foreach (var version in canDelete.GroupBy(v => new { v.Major, v.Minor })
                     .SelectMany(v => v.OrderDescending().Skip(1)))
            yield return version;
    }

    void DeletePackage(string packageId, Version version)
    {
        try
        {
            DotNetTasks.DotNetNuGetDelete(s => s
                .SetSource(NuGetSource)
                .SetApiKey(NuGetApiKey)
                .SetPackageId(packageId)
                .SetPackageVersion(version.ToString()));
            Log.Information($"Deleted the nuget package {packageId} {version}");
        }
        catch
        {
            Log.Warning($"Failed to delete the nuget package {packageId} {version}");
        }
    }

    void PushPackage(AbsolutePath package)
    {
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
            Log.Warning($"Failed to push the nuget package {package}");
        }
    }

    Version[] GetPackageVersions(AbsolutePath packagePath, out string packageName, out Version version)
    {
        var packageNameVersion = packagePath.NameWithoutExtension;
        var parts = packageNameVersion.Split('.');
        if (parts.Length < 2)
        {
            packageName = string.Empty;
            version = null!;
            return [];
        }

        packageName = string.Join(".", parts.TakeWhile(p => !uint.TryParse(p, out _))).ToLower();
        if (!Version.TryParse(string.Join(".", parts.SkipWhile(p => !uint.TryParse(p, out _))), out version)) return [];

        var nugetCheckUrl = $"https://api.nuget.org/v3-flatcontainer/{packageName}/index.json";

        try
        {
            using var client = new HttpClient();
            var response = client.GetStringAsync(nugetCheckUrl).Result;

            using var jsonDoc = JsonDocument.Parse(response);
            return jsonDoc.RootElement.GetProperty("versions").Deserialize<Version[]>();
        }
        catch
        {
            Log.Warning($"Failed to check NuGet for {packageName} {version}. Assuming it doesn't exist.");
            return [];
        }
    }
}