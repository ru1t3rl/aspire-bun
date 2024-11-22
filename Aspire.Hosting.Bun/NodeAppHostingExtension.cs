using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Hosting;

namespace Aspire.Hosting.Bun;

public static class NodeAppHostingExtension
{
    public static IResourceBuilder<NodeAppResource> AddBunApp(
        this IDistributedApplicationBuilder builder,
        string name,
        string workingDirectory,
        string scriptName = "start",
        bool useBunx = true,
        string[]? args = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(workingDirectory);
        ArgumentNullException.ThrowIfNull(scriptName);

        string[] allArgs;

        if (useBunx)
        {
            allArgs = args is { Length: > 0 }
                ? ["--hot --bun bun run", scriptName, "--", .. args]
                : ["--hot --bun bun run", scriptName];
        }
        else
        {
            allArgs = args is { Length: > 0 }
                ? ["--hot --bun run", scriptName, "--", .. args]
                : ["--hot --bun run", scriptName];
        }

        workingDirectory =
            PathNormalizer.NormalizePathForCurrentPlatform(Path.Combine(builder.AppHostDirectory, workingDirectory));

        var resource = new NodeAppResource(name, useBunx
            ? "bunx"
            : "bun", workingDirectory);

        return builder.AddResource(resource)
            .WithNodeDefaults()
            .WithArgs(allArgs);
    }

    private static IResourceBuilder<NodeAppResource> WithNodeDefaults(this IResourceBuilder<NodeAppResource> builder) =>
        builder.WithOtlpExporter()
            .WithEnvironment("NODE_ENV", builder.ApplicationBuilder.Environment.IsDevelopment()
                ? "development"
                : "production");
}