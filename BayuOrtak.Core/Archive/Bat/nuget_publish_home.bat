@echo off
set /p userInput="Yapilacak islemi onayliyor musunuz? [y/n]: "
if /i "%userInput%" neq "y" (
    echo Islem iptal edildi.
    exit /b
)
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Yonetici yetkisi gerekiyor, yeniden baslatiliyor...
    powershell -Command "Start-Process '%~0' -Verb RunAs"
    exit /b
)
cd /d C:\Dosyalar\Projeler\library-csharp-core-bayuortak\BayuOrtak.Core
nuget pack BayuOrtak.Core.nuspec -OutputDirectory "C:\Dosyalar\Publish"
echo Islem tamamlandi. Pencere 5 saniye icinde kapanacak...
for /l %%x in (5,-1,1) do (
    echo %%x saniye...
    timeout /t 1 >nul
)
exit