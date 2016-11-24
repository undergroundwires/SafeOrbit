REM --------- Prepare
REM -- Project variables
set sourceDir=..\src\SafeOrbit.xproj
REM -- Nuget
set local=%~dp0
set nuget=%local%nuget.exe
REM --------- Act
cd %sourceDir%
%nuget% spec 

pause