
@ECHO OFF
IF [%1] == [] GOTO EXIT

echo Building tvtamer solution
"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild" tvtamer.sln /t:build /p:Configuration=Release

echo Copying Files to %1

xcopy TvCleaner\bin\Release\TVCleaner.exe %1 /Y
xcopy TvCleaner\bin\Release\*.dll %1 /Y
xcopy TvCleaner\bin\Release\TVCleaner.exe.config %1 /Y
xcopy TvCleaner\bin\Release\nlog.config %1 /Y

:EXIT
