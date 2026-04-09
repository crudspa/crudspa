using System.IO.Compression;

namespace Crudspa.Framework.Jobs.Server.Extensions;

public static class FfmpegHelpers
{
    public static async Task InstallTools(String rootFolder)
    {
        var ffmpegPath = Path.Combine(rootFolder, "ffmpeg.exe");
        var ffprobePath = Path.Combine(rootFolder, "ffprobe.exe");

        if (File.Exists(ffmpegPath) && File.Exists(ffprobePath))
            return;

        var dir = new DirectoryInfo(rootFolder);
        dir.EnsureExists();

        var zipPath = Path.Combine(rootFolder, "ffmpeg.zip");

        await DownloadFfmpeg(zipPath);

        Extract(zipPath, "ffmpeg.exe", rootFolder);
        Extract(zipPath, "ffprobe.exe", rootFolder);

        TryDelete(zipPath);
    }

    public static async Task DownloadFfmpeg(String zipFilePath)
    {
        const String url = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip";

        var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(5);

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        await using FileStream stream = new(zipFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await response.Content.CopyToAsync(stream);
    }

    public static void Extract(String zipPath, String fileName, String outputFolderPath)
    {
        using var archive = ZipFile.OpenRead(zipPath);

        foreach (var entry in archive.Entries)
            if (entry.FullName.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
            {
                var destinationPath = Path.Combine(outputFolderPath, fileName);
                entry.ExtractToFile(destinationPath, true);
                break;
            }
    }

    private static void TryDelete(String path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            //
        }
    }
}