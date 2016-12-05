set dir=H:\Dev\SafeOrbit\src\UnitTests\bin\Release\netstandard1.6
set dllName=UnitTests.dll
set sn="C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\sn.exe"

cd %dir%
%sn% -T %dllName%

pause