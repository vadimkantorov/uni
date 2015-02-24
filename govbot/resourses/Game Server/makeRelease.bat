del release\*.* /Q
md release
xcopy ..\SwarmIntellect\bin\debug\DCIMAP.GANS.Basic.dll release\
xcopy ..\SwarmIntellect\bin\debug\DCIMAP.GANS.SwarmIntellect.dll release\
xcopy bin\Debug\Game.exe release\
xcopy world.bmp release\
xcopy runme.bat release\
xcopy ..\TestPlayer\bin\debug\TestPlayer.exe release\
copy ..\TestPlayer\Program.cs release\TestPlayer.cs
"C:\Program Files\Winrar\rar" a -r game.rar release\*

