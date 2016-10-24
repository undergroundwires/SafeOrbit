@echo off
@break off
@color 0a
@cls

REM Fill the following
set outputSnkName= REM UnitTests-public-key.snk
set outputTokenName= REM UnitTests-public-key-token.txt
set filePath= REM ..\docs\signature_keys\private\UnitTests-key.snk

set sn="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe"

REM Extract Public Key from filePath and write it to outputName
%sn% -p %filePath% %outputSnkName%

REM Extract Publick Key Token from Token.snk and write it to PublicToken.txt
%sn% -t %outputSnkName% > %outputTokenName%

pause