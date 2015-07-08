
@ECHO OFF
IF [%1] == [] GOTO EXIT

echo Building tvtamer solution
"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild" build.proj /t:build /p:Configuration=Release

echo Copying Files to %1TVCleaner\

xcopy TvCleaner\bin\Release\TVCleaner.exe %1TVCleaner\ /Y
xcopy TvCleaner\bin\Release\*.dll %1TVCleaner\ /Y
xcopy TvCleaner\bin\Release\TVCleaner.exe.config %1TVCleaner\ /Y
xcopy TvCleaner\bin\Release\nlog.config %1TVCleaner\ /Y

echo Copying Files to %1TVTamer\

xcopy TvTamer\bin\Release\*.exe %1TVTamer\ /Y
xcopy TvTamer\bin\Release\*.dll %1TVTamer\ /Y
xcopy TvTamer\bin\Release\*.exe.config %1TVTamer\ /Y
xcopy TvTamer\bin\Release\nlog.config %1TVTamer\ /Y

:EXIT
