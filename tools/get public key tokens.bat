@echo off
@break off
@color 0a
@cls

REM Path of the sn tool
set sn="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe"

REM call :printPublicToken "SafeOrbit" ".\src\SafeOrbit\bin\Release\SafeOrbit.dll"
call :printPublicToken "TestApi" ".\src\tests\TestApi\bin\Release\SafeOrbit.Tests.dll"
REM call :printPublicToken "UnitTests" ".\src\tests\UnitTests\bin\Release\XXXXXXXXX.dll"
REM call :printPublicToken "PerformanceTests" ".\src\tests\PerformanceTests\bin\Release\XXXXXXXXX.dll"
REM call :printPublicToken "IntegrationTests" ".\src\tests\PerformanceTests\bin\Release\XXXXXXXXX.dll"



pause
exit /b


REM Prints the public token of the file.
REM Argument 1 : Path of the file.
REM Argument 2 : Name of the file.

:printPublicToken
set name=%1
set path=%2
echo --------------------------------------
echo --------------------------------------
echo ----- %name%
echo --------------------------------------
%sn% -T %path%
exit /b
