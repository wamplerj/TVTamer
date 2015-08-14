@ECHO OFF

cls
"packages\FAKE.4.1.1\tools\Fake.exe" build.fsx
REM pause


REM "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild" build.proj /t:build /p:Configuration=Release

