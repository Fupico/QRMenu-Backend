using Microsoft.Extensions.FileProviders;

namespace QRMenuBackend.Infrastructure;

public static class PersistentWebRootInitializer
{
    public static void Initialize(IWebHostEnvironment environment, IConfiguration configuration, ILogger logger)
    {
        var targetWebRoot = environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(targetWebRoot))
        {
            targetWebRoot = Path.Combine(environment.ContentRootPath, "wwwroot");
            environment.WebRootPath = targetWebRoot;
            environment.WebRootFileProvider = new PhysicalFileProvider(targetWebRoot);
        }

        Directory.CreateDirectory(targetWebRoot);
        Directory.CreateDirectory(Path.Combine(targetWebRoot, "images"));

        var seedWebRoot = configuration["APP_DEFAULT_WEB_ROOT_PATH"];
        if (string.IsNullOrWhiteSpace(seedWebRoot))
        {
            seedWebRoot = Path.Combine(environment.ContentRootPath, "default-wwwroot");
        }

        if (!Directory.Exists(seedWebRoot))
        {
            logger.LogWarning("Default static content directory was not found at {SeedWebRoot}.", seedWebRoot);
            return;
        }

        var seedFullPath = Path.GetFullPath(seedWebRoot);
        var targetFullPath = Path.GetFullPath(targetWebRoot);

        if (string.Equals(seedFullPath, targetFullPath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        CopyMissingFiles(seedFullPath, targetFullPath);
    }

    private static void CopyMissingFiles(string sourceDirectory, string targetDirectory)
    {
        foreach (var directory in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceDirectory, directory);
            Directory.CreateDirectory(Path.Combine(targetDirectory, relativePath));
        }

        foreach (var file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceDirectory, file);
            var destination = Path.Combine(targetDirectory, relativePath);
            var destinationDirectory = Path.GetDirectoryName(destination);

            if (!string.IsNullOrWhiteSpace(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            if (!File.Exists(destination))
            {
                File.Copy(file, destination);
            }
        }
    }
}
