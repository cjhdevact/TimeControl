::===================================
::   TimeControl UserInit Boot - Batch Install Script
::===================================
@echo off 
::set path=%1
::set path=%path:"=%
set mpath=%windir%\CJH\TimeControl\TimeControl.exe
::echo %path%
set current_dir=%WINDIR%\System32
pushd %current_dir%
for /f "tokens=1,2,*" %%a in ('reg query "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v "Userinit" ^|findstr /i "Userinit"') do (
    set value=%%c
)
echo The default UserInit value:
echo %value% 
echo.
set value=%value:"=%
set "bat_value=%value%%mpath%,"
echo The value will write in UserInit:
echo %bat_value%
echo.
reg add "HKLM\Software\Microsoft\Windows NT\CurrentVersion\Winlogon" /v "Userinit" /t REG_SZ /d "%bat_value%" /f 

::exit