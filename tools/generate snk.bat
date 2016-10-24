set fileName=signature-key.snk

set sn="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe"

%sn% -k %fileName%

pause