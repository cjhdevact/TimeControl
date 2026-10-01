@echo off 
set apath=%1
set apath=%apath:"=%
::echo %apath%
set current_dir=%WINDIR%\System32
pushd %current_dir%
for /f "tokens=1,2,*" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v "Userinit" ^|findstr /i "Userinit"') do (
    set value=%%c
)
::echo %value%
set value=%value:"=%
set "bat_value=%value%%apath%,"
echo %bat_value%

reg add "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v "Userinit" /t REG_SZ /d "%bat_value%" /f 