[CmdletBinding(SupportsShouldProcess)]
param(
    [String]$SourceDirectory = (Join-Path $PSScriptRoot "Blobs"),
    [String]$LocalBlobRoot = "C:\data\temp\blobs\local",
    [String[]]$SampleName
)

# Keep the readable blob filenames in git, then project them into BlobServiceLocal's
# bare-guid file layout only when a developer explicitly seeds local sample media.
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Get-SampleHostConfig
{
    param(
        [String]$RepoRoot,
        [String[]]$SampleName
    )

    $samplesRoot = Join-Path $RepoRoot "src\Samples"
    if (-not (Test-Path -LiteralPath $samplesRoot))
    {
        throw "Could not find the samples folder at '$samplesRoot'."
    }

    $sampleFilter = if ($SampleName)
    {
        $SampleName | ForEach-Object { $_.ToLowerInvariant() }
    }
    else
    {
        $null
    }

    foreach ($sampleDirectory in Get-ChildItem -LiteralPath $samplesRoot -Directory | Sort-Object Name)
    {
        if ($sampleFilter -and $sampleDirectory.Name.ToLowerInvariant() -notin $sampleFilter)
        {
            continue
        }

        foreach ($hostFolder in "Server", "Engine")
        {
            $configPath = Join-Path $sampleDirectory.FullName "$hostFolder\appsettings.json"
            if (-not (Test-Path -LiteralPath $configPath))
            {
                continue
            }

            # Discover the local blob targets from checked-in appsettings so new samples
            # can join this flow without editing the script.
            $config = Get-Content -LiteralPath $configPath -Raw | ConvertFrom-Json
            $blobService = $config."Crudspa.Framework.Core.Server.BlobService"
            $storageContainer = $config."Crudspa.Framework.Core.Server.StorageContainer"

            if ([String]::IsNullOrWhiteSpace($blobService) -or $blobService -notlike "*BlobServiceLocal*")
            {
                continue
            }

            if ([String]::IsNullOrWhiteSpace($storageContainer))
            {
                throw "Sample '$($sampleDirectory.Name)' is missing Crudspa.Framework.Core.Server.StorageContainer in '$configPath'."
            }

            [PSCustomObject]@{
                SampleName = $sampleDirectory.Name
                HostFolder = $hostFolder
                ConfigPath = $configPath
                StorageContainer = $storageContainer
            }
        }
    }
}

function Get-SourceBlob
{
    param([String]$SourceDirectory)

    if (-not (Test-Path -LiteralPath $SourceDirectory))
    {
        throw "Could not find the blob source folder at '$SourceDirectory'."
    }

    $blobFiles = foreach ($file in Get-ChildItem -LiteralPath $SourceDirectory -File -Recurse | Sort-Object FullName)
    {
        # Source filenames keep a descriptive suffix for humans, but the destination key is
        # always the leading blob id because BlobServiceLocal stores files by guid alone.
        if ($file.Name -notmatch "^(?<BlobId>[0-9a-fA-F]{8}(?:-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12})(?:-|$)")
        {
            Write-Warning "Skipping '$($file.FullName)' because its name does not start with a blob id."
            continue
        }

        [PSCustomObject]@{
            BlobId = [Guid]$Matches.BlobId
            BlobKey = ([Guid]$Matches.BlobId).ToString("D")
            SourcePath = $file.FullName
            SourceName = $file.Name
        }
    }

    $duplicates = $blobFiles |
        Group-Object BlobKey |
        Where-Object Count -gt 1

    if ($duplicates)
    {
        $duplicateText = [String]::Join(", ", ($duplicates |
            ForEach-Object { $_.Name } |
            Sort-Object))

        throw "Found duplicate blob ids in '$SourceDirectory': $duplicateText"
    }

    return @($blobFiles)
}

function Test-FileContentMatches
{
    param(
        [String]$SourcePath,
        [String]$DestinationPath
    )

    $sourceItem = Get-Item -LiteralPath $SourcePath
    $destinationItem = Get-Item -LiteralPath $DestinationPath

    if ($sourceItem.Length -ne $destinationItem.Length)
    {
        return $false
    }

    $sourceHash = Get-FileHash -LiteralPath $SourcePath -Algorithm SHA256
    $destinationHash = Get-FileHash -LiteralPath $DestinationPath -Algorithm SHA256

    return $sourceHash.Hash -eq $destinationHash.Hash
}

function Sync-BlobContainer
{
    param(
        [Object[]]$SourceBlobs,
        [String]$DestinationDirectory
    )

    if (-not (Test-Path -LiteralPath $DestinationDirectory))
    {
        if ($PSCmdlet.ShouldProcess($DestinationDirectory, "Create local blob directory"))
        {
            New-Item -ItemType Directory -Path $DestinationDirectory -Force | Out-Null
        }
    }

    $copied = 0
    $updated = 0
    $unchanged = 0

    foreach ($blob in $SourceBlobs)
    {
        $destinationPath = Join-Path $DestinationDirectory $blob.BlobKey

        if (-not (Test-Path -LiteralPath $destinationPath))
        {
            if ($PSCmdlet.ShouldProcess($destinationPath, "Copy sample blob '$($blob.SourceName)'"))
            {
                Copy-Item -LiteralPath $blob.SourcePath -Destination $destinationPath
                $copied++
            }

            continue
        }

        if (Test-FileContentMatches -SourcePath $blob.SourcePath -DestinationPath $destinationPath)
        {
            $unchanged++
            continue
        }

        if ($PSCmdlet.ShouldProcess($destinationPath, "Refresh sample blob '$($blob.SourceName)'"))
        {
            Copy-Item -LiteralPath $blob.SourcePath -Destination $destinationPath -Force
            $updated++
        }
    }

    [PSCustomObject]@{
        Copied = $copied
        Updated = $updated
        Unchanged = $unchanged
    }
}

$targets = Get-SampleHostConfig -RepoRoot $repoRoot -SampleName $SampleName
if (-not $targets)
{
    throw "No sample hosts configured for BlobServiceLocal were found."
}

$sourceBlobs = Get-SourceBlob -SourceDirectory $SourceDirectory
if (-not $sourceBlobs)
{
    Write-Host "No source blobs were found under '$SourceDirectory'."
    return
}

$targetsByContainer = $targets | Group-Object StorageContainer | Sort-Object Name

foreach ($targetGroup in $targetsByContainer)
{
    # Several samples can share one BlobServiceLocal container. Seed each distinct
    # container once, then report which sample hosts point at it.
    $sampleLabels = $targetGroup.Group |
        Select-Object -ExpandProperty SampleName |
        Sort-Object -Unique

    $destinationDirectory = Join-Path $LocalBlobRoot $targetGroup.Name
    $result = Sync-BlobContainer -SourceBlobs $sourceBlobs -DestinationDirectory $destinationDirectory

    Write-Host ("[{0}] copied {1}, refreshed {2}, unchanged {3} -> {4}" -f ($sampleLabels -join ", "), $result.Copied, $result.Updated, $result.Unchanged, $destinationDirectory)
}
