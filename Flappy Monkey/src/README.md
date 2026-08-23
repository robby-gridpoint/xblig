# Flappy Monkey PC Port

This folder contains the recovered XNA 4 source retargeted to FNA and `.NET 10`.

Build from the extraction root:

```powershell
dotnet build .\recovered-fna\FlappyMonkey.csproj
```

Run:

```powershell
.\recovered-fna\bin\Debug\net10.0\FlappyMonkey.exe
```

`robbyPort.cs` is the single home for PC-port compatibility code: Xbox GamerServices, System Link/session stubs, PC storage, packet compatibility, and crash reporting. Keep future Xbox-to-PC helpers there rather than spreading them across recovered gameplay files.

The port is local-controller/single-player only. Xbox Live, System Link, marketplace, and invite paths are intentionally inert.

Converted audio is kept under `../port-audio` and copied to output by the project file. For the reusable recovery, conversion, and validation workflow, use `/xbox-xna-pc-port`.