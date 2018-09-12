@echo OFF

echo ======================================
echo    beginning delete nupkgs package   
echo ======================================
del /f /s /q nupkgs\*.nupkg

echo:
echo ======================================
echo           beginning build     
echo ======================================
dotnet build  --configuration Release

echo:
echo:
echo ======================================
echo         beginning package       
echo ======================================
dotnet pack --output nupkgs --configuration Release 

echo:
echo:
echo ======================================
echo          beginning publish        
echo ======================================
dotnet nuget push nupkgs\*.nupkg -k xiaokeduo.com -s http://192.168.12.20:8099/nuget

if %errorlevel% == 1 (pause)