REM -- Project variables
set sourceDir=..\src\SafeOrbit
set projectFileName=SafeOrbit.xproj
REM -- Nuget
set local=%~dp0
set nuget=%local%nuget.exe

cd %sourceDir%

%nuget% pack %projectFileName% -Prop Configuration=Release
 pause