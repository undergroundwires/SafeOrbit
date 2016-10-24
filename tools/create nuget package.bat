REM --------- Prepare
REM -- Project variables
set sourceDir=..\src\SafeOrbit
set projectFileName
REM -- Nuget
set local=%~dp0
set nuget=%local%nuget.exe
REM --------- Act
cd %sourceDir%
%nuget% pack SafeOrbit.csproj -Prop Configuration=Release

pause