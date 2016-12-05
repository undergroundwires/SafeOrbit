set local=%~dp0
REM --------- Init
REM -- Variables
set sourceDir=..\src\SafeOrbit.xproj
set nuget=%local%external\nuget.exe

REM -- Log variables
echo Create nuget specs file
echo --------------------
echo User :
echo  OutputSnkName=%outputSnkName%
echo  OutputTxtName=%outputTxtName%
echo  FilePath=%filePath%
echo System : 
echo  SN=%sn%
echo --------------------

REM --------- Act
cd %sourceDir%
%nuget% spec 

REM --------- End
echo --------------------
pause