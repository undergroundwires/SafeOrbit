@echo off
REM %1	: Current configuration
REM %2	: Requested configuration (will only create package on this mode)
REM %3	: Requested configuration (will only create package on this mode)


IF "%1" == "%2" dotnet pack --no-build --configuration %1 -o ../%3