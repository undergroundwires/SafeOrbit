@echo off
@break off
@color 0a
@cls
set local=%~dp0
REM --------- Init
REM -- Variables
set outputSnkName=REM IntegrationTests-public-key.snk
set outputTxtName=REM IntegrationTests-public-key.txt
set filePath=REM ..\docs\signature_keys\private\IntegrationTests-key.snk
set sn="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe"
REM -- Log variables
echo Extract public key and token
echo --------------------
echo User :
echo  OutputSnkName=%outputSnkName%
echo  OutputTxtName=%outputTxtName%
echo  FilePath=%filePath%
echo System : 
echo  SN=%sn%
echo --------------------
REM --------- Act
REM Extract Public Key from filePath and write it to outputName
%sn% -p %filePath% %outputSnkName%
REM Public key : This outputs a very long key over five lines. Copy it and add the lines together (e.g. via notepad)
REM Public key token : This outputs  public key token as well.
%sn% -tp %outputSnkName% > %outputTxtName%
REM --------- End
echo --------------------
pause