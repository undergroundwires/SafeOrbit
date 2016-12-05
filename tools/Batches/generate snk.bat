@echo off
@break off
@color 0a
@cls

REM Generates a new RSACryptoServiceProvider key of the specified size and writes it to the specified file. Both a public and private key are written to the file. 

REM variables
set fileName=signature-key-pair
set sn="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe"
set local=%~dp0

call :drawLine

REM creates a new, random key pair and stores it in %fileName%.snk. 
%sn% -k %fileName%.snk

call :drawLine

echo A new random key pair is created at %local%\%fileName%.snk
echo.
echo It holds both "private" and "public" key you can extract public key and token using "extract public key and token.bat"

call :drawLine

pause
exit /b

:drawLine 
	echo -------------------------
	exit /b