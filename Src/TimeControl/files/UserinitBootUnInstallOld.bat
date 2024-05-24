::===================================
::   TimeControl UserInit Boot - Batch UnInstall Script (Old)
::===================================
@echo off 
::set path=%1
::set path=%path:"=%
set mpath=%windir%\TimeControl.exe
::echo %path%
set current_dir=%WINDIR%\System32
pushd %current_dir%
for /f "tokens=1,2,*" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v "Userinit" ^|findstr /i "Userinit"') do (
    set value=%%c
)
set value=%value:"=%
setlocal enabledelayedexpansion
set value=!value:%mpath%,=!
::echo %value%
echo The value will write in UserInit:
echo %value%
echo.

reg add "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v "Userinit" /t REG_SZ /d "%value%" /f 

::exit
