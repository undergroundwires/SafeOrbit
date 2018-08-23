@echo off
:: %1	: Current configuration
:: %2	: Requested configuration (will only create package on this mode)
:: %3  : Target destination

echo ------Nuget variables-------
echo Current configuration   : %1
echo Requested configuration : %2
echo Target destination : %3
echo ----------------------------


IF "%1" == "%2" (
	dotnet pack --no-build --configuration %1 -o %3
) ELSE (
	echo ----------------------------
	echo Nuget package is not created as current configuration does not match the requested configuration.
)