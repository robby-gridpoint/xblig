# Flappy Monkey PC Compatibility

| Area | Status | Notes |
| --- | --- | --- |
| Recovered source | Compilable | The recovered C# project builds with the installed .NET SDK. |
| Framework | FNA + .NET 10 | Replaces the Xbox XNA runtime with FNA's desktop implementation. |
| Saves | Supported | Xbox storage containers are redirected under `%LocalAppData%\FlappyMonkey\Minotaur`. |
| Crash reporting | Supported for managed exceptions | `robbyPort-crash.log` is written beside the executable and a Windows error dialog is displayed. Native hangs/faults still require Windows Event Viewer. |

## Requirements

- Windows 10 or newer
- .NET 10 x86 runtime
- A Vulkan-capable graphics driver, or another renderer supported by FNA3D
- The included x86 runtime libraries: `SDL3.dll`, `FNA3D.dll`, `FAudio.dll`, and `libtheorafile.dll`

## Audio Notes

The Xbox XMA2 sound effects are not played through FAudio's normal `SoundEffect.Play` path because repeated cue playback can hang this port. Converted WAV effects use a Windows asynchronous cue backend that rejects overlapping cues. Music follows the same Windows WAV playback backend.