using ImageMagick;

namespace Crudspa.Framework.Jobs.Server.Services;

public sealed record ImageOptimizationResult(
    Dictionary<String, String> Files,
    String ProfileName,
    Int32 Width,
    Int32 Height,
    String Format);

public static class ImageOptimizer
{
    private sealed record WebpProfile(
        String Name,
        Boolean Lossless,
        Int32 MasterQuality,
        Int32 ResizedQuality,
        Boolean UseUnsharp,
        Boolean Exact,
        Int32 AlphaQuality);

    private static readonly Int32[] TargetWidths = [96, 192, 360, 540, 720, 1080, 1440, 1920, 3840];
    private static readonly WebpProfile PhotoProfile = new("photo", false, 78, 82, true, false, 85);
    private static readonly WebpProfile GraphicProfile = new("graphic", true, 100, 100, false, true, 100);

    public static async Task<ImageOptimizationResult> OptimizeAndResize(String inputFilePath, String outputFolder, String? format)
    {
        Directory.CreateDirectory(outputFolder);

        var results = new Dictionary<String, String>();
        var fileName = Path.GetFileNameWithoutExtension(inputFilePath);
        var masterPath = Path.Combine(outputFolder, $"{fileName}-optimized.webp");

        using var original = await ToRaster(inputFilePath, format, targetLargestWidth: 3840);

        var width = (Int32)original.Width;
        var height = (Int32)original.Height;

        Prepare(original);
        var profile = SelectProfile(format, original);

        using (var master = new MagickImage(original))
        {
            ApplyProfile(master, profile, isMaster: true);
            await master.WriteAsync(masterPath);
            results["optimized"] = masterPath;
        }

        foreach (var widthTarget in TargetWidths)
        {
            if (original.Width <= (UInt32)widthTarget) continue;

            using var resized = new MagickImage(original);

            resized.Resize((UInt32)widthTarget, 0);
            var resizedPath = Path.Combine(outputFolder, $"{fileName}-{widthTarget}.webp");

            ApplyProfile(resized, profile, isMaster: false);
            await resized.WriteAsync(resizedPath);
            results[widthTarget.ToString()] = resizedPath;
        }

        return new(results, profile.Name, width, height, ".webp");
    }

    private static async Task<MagickImage> ToRaster(String inputFilePath, String? format, Int32 targetLargestWidth)
    {
        if (!format.IsBasically(".svg"))
            return new(inputFilePath);

        var settings = new MagickReadSettings
        {
            BackgroundColor = MagickColors.Transparent,
            Width = (UInt32?)targetLargestWidth,
        };

        var image = new MagickImage();

        await image.ReadAsync(inputFilePath, settings);

        return image;
    }

    private static void Prepare(MagickImage image)
    {
        if (image.HasProfile("icc"))
            image.TransformColorSpace(ColorProfiles.SRGB);
        else
            image.ColorSpace = ColorSpace.sRGB;

        image.AutoOrient();
        image.Strip();
        image.Depth = 8;
    }

    private static WebpProfile SelectProfile(String? format, MagickImage image)
    {
        if (format.IsBasically(".svg"))
            return GraphicProfile;

        var totalColors = image.TotalColors;
        var colorDensity = totalColors / Math.Max(1D, image.Width * (Double)image.Height);

        return totalColors <= 4096 || (totalColors <= 32768 && colorDensity <= .01D)
            ? GraphicProfile
            : PhotoProfile;
    }

    private static void ApplyProfile(MagickImage image, WebpProfile profile, Boolean isMaster)
    {
        image.Format = MagickFormat.WebP;
        image.FilterType = FilterType.Lanczos;
        image.Quality = (UInt32)(isMaster ? profile.MasterQuality : profile.ResizedQuality);

        if (profile.UseUnsharp)
            image.UnsharpMask(0.0, 0.8, isMaster ? 0.3 : 0.4, 0.02);

        SetDefines(image, profile);
    }

    private static void SetDefines(MagickImage image, WebpProfile profile)
    {
        image.Settings.SetDefine("webp:method", "6");
        image.Settings.SetDefine("webp:auto-filter", "true");
        image.Settings.SetDefine("webp:lossless", profile.Lossless ? "true" : "false");
        image.Settings.SetDefine("webp:use-sharp-yuv", profile.Lossless ? "false" : "true");

        if (profile.Exact)
            image.Settings.SetDefine("webp:exact", "true");

        if (image.HasAlpha)
        {
            image.Settings.SetDefine("webp:alpha-compression", "1");
            image.Settings.SetDefine("webp:alpha-quality", $"{profile.AlphaQuality:D}");
        }
    }
}