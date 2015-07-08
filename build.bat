
@ECHO OFF
echo Building tvtamer solution
"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild" build.proj /t:build /p:Configuration=Release

