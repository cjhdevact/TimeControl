@echo off 
set apath=%1
set apath=%apath:"=%
::echo %apath%
set current_dir=%WINDIR%\System32
pushd %current_dir%
for /f "tokens=1,2,*" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v "Userinit" ^|findstr /i "Userinit"') do (
    set value=%%c
)
set value=%value:"=%
setlocal enabledelayedexpansion
set value=!value:%apath%,=!
::echo %value%

reg add "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v "Userinit" /t REG_SZ /d "%value%" /f 