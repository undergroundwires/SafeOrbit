@echo off
@break off
@color 0a
@cls


REM Fill the following
set outputSnkName=REM IntegrationTests-public-key.snk
set outputTxtName=REM IntegrationTests-public-key.txt
set filePath=REM ..\docs\signature_keys\private\IntegrationTests-key.snk

set sn="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe"
set local=%~dp0
REM Extract Public Key from filePath and write it to outputName
%sn% -p %filePath% %outputSnkName%

REM Public key : This outputs a very long key over five lines. Copy it and add the lines together (e.g. via notepad)
REM Public key token : This outputs  public key token as well.
%sn% -tp %outputSnkName% > %outputTxtName%

 
pause