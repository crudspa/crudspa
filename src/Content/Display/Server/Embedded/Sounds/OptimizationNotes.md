The files in this directory were optimized using the following command:

```powershell
Get-ChildItem .\download -Filter *.mp3 | ForEach-Object { .\ffmpeg.exe -loglevel error -y -i "$($_.FullName)" -c:a libmp3lame -filter_complex "loudnorm=I=-24:LRA=9:TP=-3" -ar 44100 -ac 2 -q:a 2 ".\processed\$($_.BaseName).mp3" }
```
