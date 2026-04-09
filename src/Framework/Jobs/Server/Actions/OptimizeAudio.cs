using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using CliWrap;
using CliWrap.Buffered;
using Crudspa.Framework.Jobs.Server.Extensions;

namespace Crudspa.Framework.Jobs.Server.Actions;

public class OptimizeAudio(
    ILogger<OptimizeAudio> logger,
    IFrameworkActionService frameworkActionService,
    IBlobService blobService)
    : IJobAction
{
    public OptimizeAudioConfig? Config { get; set; }
    private Guid? _sessionId;

    private enum AudioProfile { Speech, Music }

    private const String RootFolder = @"C:\data\temp\audio";
    private static String FfmpegPath => Path.Combine(RootFolder, "ffmpeg.exe");
    private static String FfprobePath => Path.Combine(RootFolder, "ffprobe.exe");
    private const Int32 TpCeiling = -2;

    public void Configure(Guid? sessionId, String json)
    {
        Config = json.FromJson<OptimizeAudioConfig>();
        _sessionId = sessionId;
    }

    public async Task<Boolean> Run(Guid? jobId)
    {
        try
        {
            await OptimizeAudioFiles();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while optimizing audio.");
            return false;
        }
    }

    private async Task OptimizeAudioFiles()
    {
        logger.LogInformation("Fetching audio files needing optimization...");

        var response = await frameworkActionService.FetchAudioForOptimization(new(_sessionId));

        if (!response.Ok)
            throw new($"Call to audioFileService.FetchForOptimization() failed. {response.ErrorMessages}");

        var audioFiles = response.Value;

        logger.LogInformation("Found {audioFilesCount} audio files to optimize.", audioFiles.Count);

        if (audioFiles.Count == 0)
            return;

        logger.LogInformation("Ensuring ffmpeg/ffprobe available in {folder}...", RootFolder);
        await FfmpegHelpers.InstallTools(RootFolder);

        var downloadFolder = Path.Combine(RootFolder, "download");
        var processedFolder = Path.Combine(RootFolder, "processed");
        var diagnosticsFolder = Path.Combine(RootFolder, "diagnostics");

        Directory.CreateDirectory(downloadFolder);
        Directory.CreateDirectory(processedFolder);
        Directory.CreateDirectory(diagnosticsFolder);

        var attempts = 0;
        var successes = 0;

        foreach (var audioFile in audioFiles)
        {
            attempts++;

            var fileBase = audioFile.Id!.Value.ToString("D");
            var downloadFilePath = Path.Combine(downloadFolder, fileBase + ".bin");
            var processedFilePath = Path.Combine(processedFolder, fileBase + ".mp3");
            var probeJsonPath = Path.Combine(diagnosticsFolder, fileBase + ".probe.json");
            var outProbeJsonPath = Path.Combine(diagnosticsFolder, fileBase + ".out.probe.json");

            try
            {
                logger.LogInformation("File #{index}: downloading blob {blobId} to {downloadFilePath}.", attempts, audioFile.BlobId, downloadFilePath);

                await DownloadBlob(audioFile.BlobId!.Value, downloadFilePath);

                logger.LogInformation("Probing {downloadFilePath}...", downloadFilePath);

                var inputProbe = await Probe(downloadFilePath);

                logger.LogInformation("Writing probe results to {probeFilePath}...", probeJsonPath);

                await WriteJson(probeJsonPath, inputProbe);

                logger.LogInformation("Validating probed file {downloadFilePath} has audio...", downloadFilePath);

                ValidateProbeHasAudio(inputProbe, audioFile.Id!.Value, downloadFilePath);

                var isLikelyWav = (inputProbe.ContainerFormat?.Contains("wav", StringComparison.OrdinalIgnoreCase) ?? false)
                    || (inputProbe.AudioCodecName?.Contains("adpcm_ima_wav", StringComparison.OrdinalIgnoreCase) ?? false);

                logger.LogInformation("Attempting quick decode for {downloadFilePath}...", downloadFilePath);

                if (!await CanDecodeFewSeconds(downloadFilePath, isLikelyWav))
                    logger.LogWarning("Quick decode probe was inconclusive for file {audioFileId}, proceeding to full transcode.", audioFile.Id);

                logger.LogInformation("Determining audio profile for {downloadFilePath}...", downloadFilePath);

                var profile = DetermineProfile(inputProbe);

                logger.LogInformation(
                    "File {audioFileId}: codec={codec}, channels={channels}, sampleRate={sampleRate}, durationSec={durationSec}. Using {profile} profile.",
                    audioFile.Id, inputProbe.AudioCodecName ?? "(unknown)", inputProbe.AudioChannels?.ToString() ?? "(unknown)",
                    inputProbe.AudioSampleRate?.ToString() ?? "(unknown)", inputProbe.DurationSeconds?.ToString("0.00") ?? "(unknown)", profile.ToString()
                );

                var success = await Transcode(downloadFilePath, processedFilePath, profile, inputProbe, fileBase, isLikelyWav);

                if (!success)
                    throw new($"All transcoding attempts failed for input {downloadFilePath}.");

                logger.LogInformation("Validating optimized file {processedFilePath}...", processedFilePath);

                var outputProbe = await Probe(processedFilePath);

                ValidateProbeHasAudio(outputProbe, audioFile.Id!.Value, processedFilePath);

                await WriteJson(outProbeJsonPath, outputProbe);

                var blobId = await AddBlob(processedFilePath);

                audioFile.OptimizedBlobId = blobId;
                audioFile.OptimizedFormat = ".mp3";
                audioFile.OptimizedStatus = AudioFile.OptimizationStatus.Succeeded;

                await frameworkActionService.SaveAudioOptimizationStatus(new(_sessionId, audioFile));

                logger.LogInformation("Audio optimization succeeded for {audioFileId}. outputBlobId={outputBlobId}", audioFile.Id, blobId);

                successes++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while optimizing audio file {audioFileId}.", audioFile.Id);

                audioFile.OptimizedStatus = AudioFile.OptimizationStatus.Failed;

                await frameworkActionService.SaveAudioOptimizationStatus(new(_sessionId, audioFile));
            }
            finally
            {
                new FileInfo(downloadFilePath).SafeDelete();
                new FileInfo(processedFilePath).SafeDelete();
                new FileInfo(probeJsonPath).SafeDelete();
                new FileInfo(outProbeJsonPath).SafeDelete();
            }
        }

        logger.LogInformation("OptimizeAudio complete. {attempts} file(s) processed, {successes} succeeded.", attempts, successes);
    }

    private async Task<Boolean> Transcode(String inputPath, String outputPath, AudioProfile profile, ProbeSummary probe, String fileBase, Boolean isLikelyWav)
    {
        logger.LogInformation("Transcoding {inputPath} -> {outputPath} using requested profile {profile}.", inputPath, outputPath, profile);

        new FileInfo(outputPath).SafeDelete();

        SelectedAudioStream selected;
        if (probe.AudioStreamCount == 1)
            selected = new(0, probe.AudioChannels ?? 1, probe.AudioSampleRate, probe.AudioBitrate, probe.AudioCodecName, probe.DurationSeconds);
        else
            selected = await PickBestAudioStream(inputPath);

        logger.LogInformation("Selected stream for {inputPath}: index={streamIndex}, codec={codec}, channels={channels}, sampleRate={sampleRate}, estDurationSec={durationSec}.",
            inputPath, selected.StreamIndex, selected.CodecName ?? "(unknown)", selected.Channels, selected.SampleRate?.ToString() ?? "(unknown)", selected.DurationSec?.ToString("0.00") ?? "(unknown)");

        var isWav = probe.ContainerFormat.IsBasically("wav") || probe.ContainerFormat.Has("wav");
        var slowAdpcm = IsSlowAdpcm(probe);
        var demuxerPrefix = isWav && slowAdpcm ? "-f wav -ignore_length 1 -guess_layout_max 0 " : "";
        var map = selected.StreamIndex >= 0 ? $"0:{selected.StreamIndex}" : "0:a:0";

        var baseInput =
            $"-nostdin -hide_banner -loglevel error -nostats -y -analyzeduration 10M -probesize 50M " +
            $"-fflags +genpts+igndts+discardcorrupt -err_detect ignore_err -ignore_unknown " +
            $"{demuxerPrefix}-i \"{inputPath}\" -map {map} -vn -sn -dn -threads 0 -filter_threads 2";

        var actualProfile = DetermineProfile(selected.Channels, selected.SampleRate, selected.Bitrate ?? probe.AudioBitrate ?? probe.TotalBitrate, selected.CodecName);

        if (actualProfile != profile)
            logger.LogInformation("Requested profile {requested} adjusted to {actual} based on selected stream.", profile, actualProfile);

        var isSpeech = actualProfile == AudioProfile.Speech;

        var targetAr = ChooseSampleRate(isSpeech, selected.SampleRate);
        var targetAc = isSpeech ? 1 : Math.Min(2, selected.Channels);

        var (preEqChain, finalSampleFormat) = BuildFilter(isSpeech, slowAdpcm, targetAc);

        var iTarget = isSpeech ? -18 : -16;
        var lraTarget = isSpeech ? 7 : 11;

        logger.LogInformation("Running loudness analysis for {inputPath} with targets I={iTarget}, LRA={lraTarget}...", inputPath, iTarget, lraTarget);

        var analysis = slowAdpcm
            ? null
            : await AnalyzeLoudness(inputPath, iTarget, lraTarget, selected.StreamIndex, probe.DurationSeconds, demuxerPrefix, preEqChain);

        if (analysis is null)
            logger.LogInformation("Loudness analysis unavailable for {inputPath}. Proceeding with single-pass loudnorm.", inputPath);
        else
            logger.LogInformation("Loudness analysis complete for {inputPath}. Using measured values (I={I}, TP={TP}, LRA={LRA}, Thresh={Thresh}, Offset={Offset}).",
                inputPath, analysis.InputI, analysis.InputTp, analysis.InputLra, analysis.InputThresh, analysis.TargetOffset);

        var loudnormSecondPass = analysis is null
            ? $"loudnorm=I={iTarget}:LRA={lraTarget}:TP={TpCeiling}"
            : $"loudnorm=I={iTarget}:LRA={lraTarget}:TP={TpCeiling}:measured_I={analysis.InputI}:measured_TP={analysis.InputTp}:measured_LRA={analysis.InputLra}:measured_thresh={analysis.InputThresh}:offset={analysis.TargetOffset}:linear=true";

        var filterGraph = $"{preEqChain},{loudnormSecondPass},alimiter=level_in=1:level_out=1,{finalSampleFormat}";

        const String lameReservoir = "-reservoir 1";
        const String lameJointStereo = "-joint_stereo 1";

        var vbrQualityArg = isSpeech ? "-q:a 5" : "-q:a 2";

        var preferCbrFirst = slowAdpcm || (isSpeech && (probe.DurationSeconds ?? 0) >= 1800);
        var cbrBitrate = isSpeech ? "128k" : "192k";

        var argsVbr = $"{baseInput} -filter:a \"{filterGraph}\" -c:a libmp3lame -ar {targetAr} -ac {targetAc} {vbrQualityArg} {lameJointStereo} {lameReservoir} -id3v2_version 3 -write_xing 1 -map_metadata -1 -map_chapters -1 \"{outputPath}\"";
        var argsCbr = $"{baseInput} -filter:a \"{filterGraph}\" -c:a libmp3lame -ar {targetAr} -ac {targetAc} -b:a {cbrBitrate} {lameJointStereo} {lameReservoir} -id3v2_version 3 -write_xing 1 -map_metadata -1 -map_chapters -1 \"{outputPath}\"";

        logger.LogInformation("Attempting {mode} transcoding for {inputPath}...", preferCbrFirst ? "CBR" : "VBR", inputPath);

        var firstArgs = preferCbrFirst ? argsCbr : argsVbr;
        var secondArgs = preferCbrFirst ? argsVbr : argsCbr;

        var sw1 = Stopwatch.StartNew();
        var first = await Cli
            .Wrap(FfmpegPath)
            .WithArguments(firstArgs)
            .WithStandardOutputPipe(PipeTarget.Null)
            .WithStandardErrorPipe(PipeTarget.Null)
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();
        sw1.Stop();

        logger.LogInformation("{mode} attempt finished for {inputPath} with exitCode={exitCode} in {elapsedMs} ms.", preferCbrFirst ? "CBR" : "VBR", inputPath, first.ExitCode, sw1.ElapsedMilliseconds);

        var success = first.ExitCode == 0 && await OutputLooksGood(outputPath, targetAr, targetAc, probe.DurationSeconds, isLikelyWav);

        if (success)
            return true;

        logger.LogWarning("{mode} attempt did NOT produce acceptable output for {inputPath}.", preferCbrFirst ? "CBR" : "VBR", inputPath);

        new FileInfo(outputPath).SafeDelete();

        logger.LogInformation("Attempting {mode} transcoding for {inputPath}...", preferCbrFirst ? "VBR" : "CBR", inputPath);

        var sw2 = Stopwatch.StartNew();
        var second = await Cli
            .Wrap(FfmpegPath)
            .WithArguments(secondArgs)
            .WithStandardOutputPipe(PipeTarget.Null)
            .WithStandardErrorPipe(PipeTarget.Null)
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();
        sw2.Stop();

        logger.LogInformation("{mode} attempt finished for {inputPath} with exitCode={exitCode} in {elapsedMs} ms.", preferCbrFirst ? "VBR" : "CBR", inputPath, second.ExitCode, sw2.ElapsedMilliseconds);

        success = second.ExitCode == 0 && await OutputLooksGood(outputPath, targetAr, targetAc, probe.DurationSeconds, isLikelyWav);

        if (success)
            return true;

        logger.LogWarning("{mode} attempt did NOT produce acceptable output for {inputPath}.", preferCbrFirst ? "VBR" : "CBR", inputPath);

        new FileInfo(outputPath).SafeDelete();

        var intermediateWav = Path.Combine(Path.GetDirectoryName(outputPath)!, fileBase + ".intermediate.wav");

        try
        {
            var argsWav = $"{baseInput} -rf64 always -ar {targetAr} -ac {targetAc} -c:a pcm_s16le -map_metadata -1 \"{intermediateWav}\"";

            logger.LogInformation("Creating intermediate WAV (pcm_s16le) for {inputPath} at {intermediateWavPath}...", inputPath, intermediateWav);

            var wav1Sw = Stopwatch.StartNew();
            var wav1 = await Cli
                .Wrap(FfmpegPath)
                .WithArguments(argsWav)
                .WithStandardOutputPipe(PipeTarget.Null)
                .WithStandardErrorPipe(PipeTarget.Null)
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();
            wav1Sw.Stop();

            logger.LogInformation("Intermediate WAV (pcm_s16le) finished for {inputPath} with exitCode={exitCode} in {elapsedMs} ms.", inputPath, wav1.ExitCode, wav1Sw.ElapsedMilliseconds);

            var wavOk = wav1.ExitCode == 0 && File.Exists(intermediateWav);

            if (!wavOk)
            {
                argsWav = $"{baseInput} -rf64 always -ar {targetAr} -ac {targetAc} -c:a pcm_f32le -map_metadata -1 \"{intermediateWav}\"";

                logger.LogInformation("Creating intermediate WAV (pcm_f32le) for {inputPath} at {intermediateWavPath}...", inputPath, intermediateWav);

                var wav2Sw = Stopwatch.StartNew();
                var wav2 = await Cli
                    .Wrap(FfmpegPath)
                    .WithArguments(argsWav)
                    .WithStandardOutputPipe(PipeTarget.Null)
                    .WithStandardErrorPipe(PipeTarget.Null)
                    .WithValidation(CommandResultValidation.None)
                    .ExecuteAsync();
                wav2Sw.Stop();

                logger.LogInformation("Intermediate WAV (pcm_f32le) finished for {inputPath} with exitCode={exitCode} in {elapsedMs} ms.", inputPath, wav2.ExitCode, wav2Sw.ElapsedMilliseconds);

                if (wav2.ExitCode != 0 || !File.Exists(intermediateWav))
                {
                    new FileInfo(intermediateWav).SafeDelete();
                    return false;
                }
            }

            var finalArgs =
                $"-nostdin -hide_banner -loglevel error -nostats -y -i \"{intermediateWav}\" -map 0:a:0 -vn -sn -dn -threads 0 " +
                $"-filter:a \"{filterGraph}\" -c:a libmp3lame -ar {targetAr} -ac {targetAc} -b:a {cbrBitrate} {lameJointStereo} {lameReservoir} -id3v2_version 3 -write_xing 1 -map_metadata -1 -map_chapters -1 \"{outputPath}\"";

            logger.LogInformation("Attempting to transcode to {outputPath} from intermediate {intermediateWavPath}...", outputPath, intermediateWav);

            var mp3Sw = Stopwatch.StartNew();
            var mp3 = await Cli
                .Wrap(FfmpegPath)
                .WithArguments(finalArgs)
                .WithStandardOutputPipe(PipeTarget.Null)
                .WithStandardErrorPipe(PipeTarget.Null)
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();
            mp3Sw.Stop();

            logger.LogInformation("Transcode attempt finished for {inputPath} with exitCode={exitCode} in {elapsedMs} ms.", inputPath, mp3.ExitCode, mp3Sw.ElapsedMilliseconds);

            if (mp3.ExitCode == 0 && await OutputLooksGood(outputPath, targetAr, targetAc, probe.DurationSeconds, isLikelyWav))
                return true;

            logger.LogWarning("Final transcode attempt did NOT produce acceptable output for {inputPath}.", inputPath);

            new FileInfo(outputPath).SafeDelete();
        }
        finally
        {
            new FileInfo(intermediateWav).SafeDelete();
        }

        return false;
    }

    private static Int32 ChooseSampleRate(Boolean isSpeech, Int32? srcRate)
    {
        if (srcRate is not > 0)
            return isSpeech ? 32000 : 44100;

        if (isSpeech)
        {
            if (srcRate <= 16000)
                return 24000;

            return 32000;
        }

        if (Math.Abs(srcRate.Value - 48000) <= 200)
            return 48000;

        return 44100;
    }

    private static async Task<LoudnormAnalysis?> AnalyzeLoudness(String inputPath, Int32 targetI, Int32 targetLra, Int32 streamIdx, Double? durationSec, String demuxerPrefix, String preEqChain)
    {
        if (durationSec is null or <= 2.0)
            return null;

        if (durationSec <= 1200)
            return await AnalyzeWindow(inputPath, streamIdx, targetI, targetLra, null, null, demuxerPrefix, preEqChain);

        var windows = new (Double ss, Double t)[]
        {
            (Math.Max(0, durationSec.Value * 0.10), 30),
            (Math.Max(0, durationSec.Value * 0.50), 30),
            (Math.Max(0, durationSec.Value * 0.90 - 30), 30),
        };

        var results = new List<LoudnormAnalysis>();

        foreach (var window in windows)
        {
            var analysis = await AnalyzeWindow(inputPath, streamIdx, targetI, targetLra, window.ss, window.t, demuxerPrefix, preEqChain);
            if (analysis is not null) results.Add(analysis);
        }

        if (results.Count == 0) return null;

        var avgI = results.Average(x => Parse(x.InputI, -23));
        var maxTp = results.Max(x => Parse(x.InputTp, -2));
        var avgLra = results.Average(x => Parse(x.InputLra, 7));
        var avgThresh = results.Average(x => Parse(x.InputThresh, -70));
        var avgOff = results.Average(x => Parse(x.TargetOffset, 0));

        return new()
        {
            InputI = avgI.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
            InputTp = maxTp.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
            InputLra = avgLra.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
            InputThresh = avgThresh.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
            TargetOffset = avgOff.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
        };
    }

    private static async Task<LoudnormAnalysis?> AnalyzeWindow(String inputPath, Int32 streamIndex, Int32 targetI, Int32 targetLra, Double? ss, Double? t, String demuxerPrefix, String preEqChain)
    {
        var seek = ss is null ? "" : $"-ss {ss.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)} ";
        var dur = t is null ? "" : $"-t {t.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)} ";

        var map = streamIndex >= 0 ? $"0:{streamIndex}" : "0:a:0";
        var args =
            $"-nostdin -hide_banner -loglevel info -nostats -y " +
            $"-analyzeduration 50M -probesize 150M " +
            $"-fflags +genpts+igndts+discardcorrupt -err_detect ignore_err -ignore_unknown " +
            $"{seek}{demuxerPrefix}-i \"{inputPath}\" -map {map} -vn -sn -dn " +
            $"{dur}-filter:a \"{preEqChain},loudnorm=I={targetI}:LRA={targetLra}:TP={TpCeiling}:print_format=json\" -f null NUL";

        try
        {
            var result = await Cli
                .Wrap(FfmpegPath)
                .WithArguments(args)
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();

            var text = result.StandardOutput.Trim();
            if (text.Length == 0) text = result.StandardError.Trim();

            var lastBrace = text.LastIndexOf('{');
            var endBrace = text.LastIndexOf('}');
            if (lastBrace < 0 || endBrace <= lastBrace) return null;

            var json = text.Substring(lastBrace, endBrace - lastBrace + 1);
            if (!json.Contains("\"input_i\"", StringComparison.OrdinalIgnoreCase))
                return null;

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var analysis = new LoudnormAnalysis
            {
                InputI = Get("input_i", root),
                InputTp = Get("input_tp", root),
                InputLra = Get("input_lra", root),
                InputThresh = Get("input_thresh", root),
                TargetOffset = Get("target_offset", root),
            };

            if (analysis.InputI.Length == 0) return null;

            return analysis;
        }
        catch
        {
            return null;
        }
    }

    private async Task<Boolean> OutputLooksGood(String filePath, Int32 expectedAr, Int32 expectedAc, Double? expectedInputDurationSec, Boolean isLikelyWav)
    {
        try
        {
            var file = new FileInfo(filePath);

            if (!file.Exists || file.Length < 1024)
            {
                logger.LogDebug("OutputLooksGood=false for {filePath} reason={reason} details={lengthBytes}", filePath, "output_missing_or_too_small", file.Length);
                return false;
            }

            var probe = await Probe(filePath);

            if (probe.AudioStreamCount <= 0)
            {
                logger.LogDebug("OutputLooksGood=false for {filePath} reason={reason}", filePath, "no_audio_streams");
                return false;
            }

            if (probe.DurationSeconds is null or <= 0.1)
            {
                logger.LogDebug("OutputLooksGood=false for {filePath} reason={reason} details={durationSec}", filePath, "bad_duration", probe.DurationSeconds);
                return false;
            }

            if (!probe.AudioCodecName.IsBasically("mp3") && !probe.AudioCodecName.IsBasically("mp3float"))
            {
                logger.LogDebug("OutputLooksGood=false for {filePath} reason={reason} details={codec}", filePath, "wrong_codec", probe.AudioCodecName);
                return false;
            }

            if (probe.AudioChannels is null or < 1 or > 2)
            {
                logger.LogDebug("OutputLooksGood=false for {filePath} reason={reason} details={channels}", filePath, "bad_channels", probe.AudioChannels);
                return false;
            }

            if (probe.AudioSampleRate is null or <= 0)
            {
                logger.LogDebug("OutputLooksGood=false for {filePath} reason={reason} details={sampleRate}", filePath, "bad_samplerate", probe.AudioSampleRate);
                return false;
            }

            if (Math.Abs(probe.AudioSampleRate.Value - expectedAr) > 100)
            {
                logger.LogDebug("OutputLooksGood=false for {filePath} reason={reason} details={actualAr} expectedAr={expectedAr}", filePath, "samplerate_mismatch", probe.AudioSampleRate, expectedAr);
                return false;
            }

            if (probe.AudioChannels.Value != expectedAc)
            {
                logger.LogDebug("OutputLooksGood=false for {filePath} reason={reason} details={actualAc} expectedAc={expectedAc}", filePath, "channels_mismatch", probe.AudioChannels, expectedAc);
                return false;
            }

            if (expectedInputDurationSec is > 1 && probe.DurationSeconds is > 0)
            {
                var inDur = expectedInputDurationSec.Value;
                var outDur = probe.DurationSeconds.Value;

                var slowAdpcm = IsSlowAdpcm(probe);
                var isLongDamaged = isLikelyWav && inDur >= 900;
                var percentTol = isLongDamaged || slowAdpcm ? 0.085 : 0.02;
                var minTol = isLongDamaged || slowAdpcm ? 15.0 : 3.0;
                var tol = Math.Max(minTol, inDur * percentTol);
                const Double epsilon = 0.10;

                if (outDur + tol + epsilon < Math.Min(inDur, 24 * 60 * 60))
                {
                    logger.LogDebug("OutputLooksGood=false for {filePath} reason={reason} details={outDur} inputDur={inDur} tol={tol}", filePath, "duration_short", outDur, inDur, tol);
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "OutputLooksGood=false for {filePath} reason={reason}", filePath, "probe_exception");
            return false;
        }
    }

    private static async Task<ProbeSummary> Probe(String filePath)
    {
        var baseArgsSmall =
            $"-hide_banner -loglevel error -print_format json -show_format -show_streams " +
            $"-analyzeduration 5M -probesize 10M -i \"{filePath}\"";

        var root = await RunProbe(baseArgsSmall);

        if (root is not null)
            return ProbeSummary.From(root);

        var baseArgsLarge =
            $"-hide_banner -loglevel error -print_format json -show_format -show_streams " +
            $"-analyzeduration 100M -probesize 200M -i \"{filePath}\"";

        root = await RunProbe(baseArgsLarge);

        if (root is not null)
            return ProbeSummary.From(root);

        var wavArgs =
            $"-hide_banner -loglevel error -print_format json -show_format -show_streams " +
            $"-analyzeduration 100M -probesize 200M -f wav -ignore_length 1 -guess_layout_max 0 -i \"{filePath}\"";

        root = await RunProbe(wavArgs);

        if (root is not null)
            return ProbeSummary.From(root);

        throw new($"ffprobe failed for {filePath}.");
    }

    private static async Task<FfprobeRoot?> RunProbe(String args)
    {
        var result = await Cli
            .Wrap(FfprobePath)
            .WithArguments(args)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();

        if (result.ExitCode != 0) return null;

        return JsonSerializer.Deserialize<FfprobeRoot>(result.StandardOutput, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private static void ValidateProbeHasAudio(ProbeSummary probe, Guid audioFileId, String path)
    {
        if (probe.AudioStreamCount <= 0)
            throw new($"No decodable audio streams detected for file {audioFileId} at {path}.");

        if (!probe.DurationSeconds.HasValue || !Double.IsFinite(probe.DurationSeconds.Value) || probe.DurationSeconds.Value <= 0.1 || probe.DurationSeconds.Value > 24 * 60 * 60)
            throw new($"No valid duration for {audioFileId} at {path}.");
    }

    private static AudioProfile DetermineProfile(ProbeSummary probe)
    {
        var channels = probe.AudioChannels ?? 1;
        var rate = probe.AudioSampleRate ?? 16000;
        var bitrate = probe.AudioBitrate ?? probe.TotalBitrate ?? 64000;

        var narrowbandCodec = probe.AudioCodecName is not null &&
            (probe.AudioCodecName.Contains("amr", StringComparison.OrdinalIgnoreCase)
                || probe.AudioCodecName.Contains("g7", StringComparison.OrdinalIgnoreCase)
                || probe.AudioCodecName.Contains("adpcm", StringComparison.OrdinalIgnoreCase)
                || probe.AudioCodecName.Contains("speex", StringComparison.OrdinalIgnoreCase));

        if (channels <= 1 || rate <= 24000 || bitrate <= 96000 || narrowbandCodec)
            return AudioProfile.Speech;

        return AudioProfile.Music;
    }

    private static AudioProfile DetermineProfile(Int32 channels, Int32? sampleRate, Int64? bitrate, String? codecName)
    {
        var ch = channels;
        var rate = sampleRate ?? 16000;
        var br = bitrate ?? 64000;

        var narrowbandCodec =
            codecName is not null &&
            (codecName.Contains("amr", StringComparison.OrdinalIgnoreCase)
                || codecName.Contains("g7", StringComparison.OrdinalIgnoreCase)
                || codecName.Contains("adpcm", StringComparison.OrdinalIgnoreCase)
                || codecName.Contains("speex", StringComparison.OrdinalIgnoreCase));

        if (ch <= 1 || rate <= 24000 || br <= 96000 || narrowbandCodec)
            return AudioProfile.Speech;

        return AudioProfile.Music;
    }

    private async Task<Boolean> CanDecodeFewSeconds(String path, Boolean isLikelyWav)
    {
        var relax = isLikelyWav ? "-f wav -ignore_length 1 -guess_layout_max 0 " : "";

        const String common = "-nostdin -hide_banner -loglevel warning -y -analyzeduration 10M -probesize 50M -fflags +discardcorrupt -err_detect ignore_err -ignore_unknown ";

        var argsList = new List<String>
        {
            $"{common}{relax}-i \"{path}\" -t 1 -f null NUL",
            $"{common}{relax}-ss 3 -i \"{path}\" -t 2 -f null NUL",
            $"{common}{relax}-ss 10 -i \"{path}\" -t 2 -f null NUL",
        };

        if (isLikelyWav)
            foreach (var skip in new[] { 44, 512, 4096, 8192 })
                argsList.Add($"{common}{relax}-skip_initial_bytes {skip} -i \"{path}\" -t 1 -f null NUL");

        foreach (var args in argsList)
            if (await Cli.Wrap(FfmpegPath).WithArguments(args).WithValidation(CommandResultValidation.None).RunAndLog(logger))
                return true;

        return false;
    }

    private static async Task<SelectedAudioStream> PickBestAudioStream(String filePath)
    {
        var args = "-hide_banner -loglevel error -print_format json -show_streams -select_streams a -i \"" + filePath + "\"";

        var result = await Cli
            .Wrap(FfprobePath)
            .WithArguments(args)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();

        if (result.ExitCode != 0)
            return new(-1, 1, null, null, null, null);

        var root = JsonSerializer.Deserialize<FfprobeRoot>(result.StandardOutput, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var streams = root?.Streams ?? [];

        if (streams.Count == 0)
            return new(-1, 1, null, null, null, null);

        var ranked = streams
            .Select((s, i) => new
            {
                StreamIndex = s.Index ?? i,
                Channels = s.Channels ?? 0,
                Bitrate = ParseBitrate(s.BitRate),
                HasSampleRate = s.SampleRate.HasSomething(),
                SampleRate = s.SampleRate.HasSomething() && Int32.TryParse(s.SampleRate, out var sr) ? sr : (Int32?)null,
                s.CodecName,
                Duration = ParseDurationSeconds(s.Duration),
                Default = s.Disposition is not null && s.Disposition.TryGetValue("default", out var d) && d == 1,
            })
            .OrderByDescending(x => x.Default)
            .ThenByDescending(x => x.Duration)
            .ThenByDescending(x => x.Channels)
            .ThenByDescending(x => x.Bitrate)
            .ThenByDescending(x => x.HasSampleRate)
            .ToList();

        var best = ranked[0];

        return new(
            best.StreamIndex,
            Math.Max(1, best.Channels),
            best.SampleRate,
            best.Bitrate,
            best.CodecName,
            best.Duration
        );
    }

    private async Task DownloadBlob(Guid blobId, String filePath)
    {
        await using var source = await blobService.FetchStream(blobId);
        await using var destination = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20, useAsync: true);

        await source.CopyToAsync(destination);
        await destination.FlushAsync();

        if (new FileInfo(filePath).Length < 512)
            throw new($"Downloaded blob {blobId} is too small to contain valid audio ({new FileInfo(filePath).Length} bytes).");
    }

    private async Task<Guid> AddBlob(String fileName)
    {
        var blobId = Guid.NewGuid();

        await using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
        await blobService.AddStream(blobId, fileStream);

        return blobId;
    }

    private static String Get(String name, JsonElement root) => root.TryGetProperty(name, out var element) ? element.GetString() ?? "" : "";

    private static Double Parse(String input, Double defaultValue) => Double.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var outputValue) ? outputValue : defaultValue;

    private static Int64 ParseBitrate(String? input) => Int64.TryParse(input, out var v) ? v : 0;

    private static Double ParseDurationSeconds(String? input)
    {
        if (input.HasNothing())
            return 0.0;

        return Double.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0.0;
    }

    private static async Task WriteJson(String path, ProbeSummary probe)
    {
        var json = JsonSerializer.Serialize(probe, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
    }

    private static Boolean IsSlowAdpcm(ProbeSummary p)
    {
        return (p.AudioCodecName?.Has("adpcm_ima_wav") ?? false)
            && (p.AudioSampleRate ?? 0) >= 48000
            && (p.AudioChannels ?? 0) <= 1
            && (p.DurationSeconds ?? 0) >= 900;
    }

    private static (String PreEqChain, String FinalSampleFmt) BuildFilter(Boolean isSpeech, Boolean slowAdpcm, Int32 targetAc)
    {
        var layout = targetAc == 1 ? "aformat=channel_layouts=mono" : "aformat=channel_layouts=stereo";

        var resample = slowAdpcm
            ? "asetpts=PTS-STARTPTS,aresample=resampler=swr:filter_size=8:cutoff=0.97:async=1000:first_pts=0:min_hard_comp=0.10"
            : "asetpts=PTS-STARTPTS,aresample=resampler=soxr:precision=20:dither_method=triangular:async=250:min_hard_comp=0.10";

        const String musicEq = "adeclick,dcshift=shift=0:limitergain=0,highpass=f=30,lowpass=f=21000";
        const String speechStd = "adeclick,dcshift=shift=0:limitergain=0,afftdn=nr=8:nt=w:om=o,agate=threshold=-50dB:ratio=2:attack=10:release=150,acompressor=threshold=-24dB:ratio=2:attack=50:release=200,highpass=f=80,lowpass=f=16000";
        const String speechLight = "highpass=f=120,lowpass=f=12000";

        var pre = isSpeech ? slowAdpcm ? speechLight : speechStd : musicEq;
        return ($"{layout},{resample},{pre}", "aformat=sample_fmts=fltp|flt");
    }

    private sealed class LoudnormAnalysis
    {
        public String InputI { get; init; } = "-23.0";
        public String InputTp { get; init; } = "-2.0";
        public String InputLra { get; init; } = "0.0";
        public String InputThresh { get; init; } = "-70.0";
        public String TargetOffset { get; init; } = "0.0";
    }

    private sealed class FfprobeRoot
    {
        [JsonPropertyName("format")] public FfprobeFormat? Format { get; init; }
        [JsonPropertyName("streams")] public List<FfprobeStream>? Streams { get; init; }
    }

    private sealed class FfprobeFormat
    {
        [JsonPropertyName("format_name")] public String? FormatName { get; set; }
        [JsonPropertyName("duration")] public String? Duration { get; set; }
        [JsonPropertyName("bit_rate")] public String? BitRate { get; set; }
        [JsonPropertyName("size")] public String? Size { get; set; }
    }

    private sealed class FfprobeStream
    {
        [JsonPropertyName("codec_type")] public String? CodecType { get; set; }
        [JsonPropertyName("codec_name")] public String? CodecName { get; set; }
        [JsonPropertyName("sample_rate")] public String? SampleRate { get; set; }
        [JsonPropertyName("channels")] public Int32? Channels { get; set; }
        [JsonPropertyName("bit_rate")] public String? BitRate { get; set; }
        [JsonPropertyName("duration")] public String? Duration { get; set; }
        [JsonPropertyName("disposition")] public Dictionary<String, Int32>? Disposition { get; set; }
        [JsonPropertyName("index")] public Int32? Index { get; set; }
    }

    private sealed record SelectedAudioStream(
        Int32 StreamIndex,
        Int32 Channels,
        Int32? SampleRate,
        Int64? Bitrate,
        String? CodecName,
        Double? DurationSec
    );

    private sealed class ProbeSummary
    {
        public String? ContainerFormat { get; private init; }
        public String? AudioCodecName { get; private init; }
        public Int32? AudioChannels { get; private init; }
        public Int32? AudioSampleRate { get; private init; }
        public Int64? AudioBitrate { get; private init; }
        public Int64? TotalBitrate { get; private init; }
        public Double? DurationSeconds { get; private init; }
        public Int64? SizeBytes { get; private init; }
        public Int32 AudioStreamCount { get; private init; }

        public static ProbeSummary From(FfprobeRoot? root)
        {
            if (root is null)
                return new();

            var audio = root.Streams?.FirstOrDefault(x => x.CodecType.IsBasically("audio"));

            Int32? sampleRate = null;
            if (audio?.SampleRate is { Length: > 0 } srStr && Int32.TryParse(srStr, out var srVal))
                sampleRate = srVal;

            Int64? containerBitrate = null;
            if (root.Format?.BitRate is { Length: > 0 } brStr && Int64.TryParse(brStr, out var brVal))
                containerBitrate = brVal;

            Int64? audioBitrate = null;
            if (audio?.BitRate is { Length: > 0 } abStr && Int64.TryParse(abStr, out var abVal))
                audioBitrate = abVal;

            Double? durationSeconds = null;
            var durStr = audio?.Duration ?? root.Format?.Duration;
            if (durStr is { Length: > 0 } && Double.TryParse(durStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var dval))
                durationSeconds = dval;

            Int64? sizeBytes = null;
            if (root.Format?.Size is { Length: > 0 } sizeStr && Int64.TryParse(sizeStr, out var sizeVal))
                sizeBytes = sizeVal;

            return new()
            {
                ContainerFormat = root.Format?.FormatName,
                AudioCodecName = audio?.CodecName,
                AudioChannels = audio?.Channels,
                AudioSampleRate = sampleRate,
                AudioBitrate = audioBitrate,
                TotalBitrate = containerBitrate,
                DurationSeconds = durationSeconds,
                SizeBytes = sizeBytes,
                AudioStreamCount = root.Streams?.Count(x => x.CodecType.IsBasically("audio")) ?? 0,
            };
        }
    }
}