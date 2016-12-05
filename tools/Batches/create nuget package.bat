@echo off
@break off
@color 0a
@cls
set local=%~dp0
REM --------- Init
REM -- Project
set sourceDir=..\src\SafeOrbit
set projectFileName=SafeOrbit.xproj
REM -- System
set nuget=%local%external\nuget.exe
REM -- Log Variables
echo Create nuget package
echo --------------------
echo User :
echo  SourceDir=%sourceDir%
echo  ProjectFileName=%projectFileName%
echo System : 
echo Nuget=%nuget%
echo --------------------
REM --------- Act
cd %sourceDir%
%nuget% pack %projectFileName% -Prop Configuration=Release
REM --------- End
echo --------------------
pause