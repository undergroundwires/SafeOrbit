REM -- Project variables
set sourceDir=..\src\SafeOrbit
set projectFileName=SafeOrbit.csproj
REM -- Nuget
set local=%~dp0
set nuget=%local%nuget.exe

cd %sourceDir%

%nuget% pack SafeOrbit.csproj -Symbols -Prop Configuration=Release
 pause