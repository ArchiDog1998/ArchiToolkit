using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Octokit;

namespace NukeBuilder;

internal class ContributorInformation
{
    public List<string> PullRequests { get; set; } = [];
    public List<string> Commits { get; set; } = [];

    public override string ToString()
    {
        var prs = string.Join(", ", PullRequests);
        var commits = string.Join(", ", Commits);
        string[] items = [prs, commits];
        return string.Join(", ", items.Where(s => !string.IsNullOrEmpty(s)));
    }
}

public class ContributorManager
{
    private readonly Dictionary<string, ContributorInformation> Data = [];

    public bool Add(GitHubCommit commit)
    {
        if (commit.Author is not { } author) return false;
        ref var item = ref GetItem(author.Login);
        item.Commits.Add(commit.Sha);
        return true;
    }

    public bool Add(PullRequest pullRequest)
    {
        if (pullRequest.User is not { } user) return false;
        ref var item = ref GetItem(user.Login);
        item.PullRequests.Add($"#{pullRequest.Number}");
        return true;
    }

    private ref ContributorInformation GetItem(string login)
    {
        ref var item = ref CollectionsMarshal.GetValueRefOrAddDefault(Data, login, out var exists);
        if (!exists) item = new ContributorInformation();
        return ref item;
    }

    public StringBuilder ToStringBuilder()
    {
        var builder = new StringBuilder();
        builder.AppendLine("## Thank You to Our Contributors! 🎉");
        builder.AppendLine(
            "We'd like to extend our heartfelt thanks to the following contributors for their valuable contributions to this release:");
        foreach (var contributor in Data)
        {
            builder.AppendLine($"- @{contributor.Key} - {contributor.Value}");
        }

        builder.AppendLine(
            "\nYour hard work and dedication helped improve this project, and we truly appreciate all the time and effort you've invested.");
        builder.AppendLine("Thank you for being a part of this amazing community! 🙏");
        return builder;
    }
}