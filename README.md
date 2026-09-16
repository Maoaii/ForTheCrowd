## Traces
```bash
dotnet-trace collect --providers Microsoft-DotNETCore-SampleProfiler,Microsoft-Windows-DotNETRuntime:0x1F000080018:4 --format Speedscope -o /home/Maoaii/Documents/Game\ Dev/ForTheCrowd/profiles/trace.nettrace -- /home/Maoaii/Apps/godot/godot-4.6-mono/Godot_v4.6-stable_mono_linux.x86_64 --path /home/Maoaii/Documents/Game\ Dev/ForTheCrowd
```