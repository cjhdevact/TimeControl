::/*****************************************************\
::
::     TimeControl - ж��.cmd
::
::     ��Ȩ����(C) 2022-2024 CJH��
::
::     ��װ������
::
::\*****************************************************/
@echo off
if "%1" == "/noadm" goto main
fltmc 1>nul 2>nul&& goto main
title ʱ��С���߰�װ����
echo ���ڻ�ȡ����ԱȨ��...
echo.
echo ����ʹ�� %0 /noadm ����Bat��Ȩ�������ֶ��Թ���Ա�������
echo �����ǰ��������ѭ�����֣�����δ�ɹ���ȡ����ԱȨ�ޣ���ע����ǰ�û����������ԣ�
echo Ȼ���Թ���Ա�û��˺����л��ֶ��Թ���Ա������С�
if "%1" == "/mshtaadm" goto mshtaAdmin
if "%2" == "/mshtaadm" goto mshtaAdmin
if "%1" == "/psadm" goto powershellAdmin
if "%2" == "/psadm" goto powershellAdmin
ver | findstr "10\.[0-9]\.[0-9]*" >nul && goto powershellAdmin
:mshtaAdmin
rem ԭ��������mshta����vbscript�ű���bat�ļ���Ȩ
rem ����ʹ����ǰ������ŵ�%~dpnx0����ʾ��ǰ�ű�����ԭ��Ķ��ļ���%~s0���ɿ�
rem ����ʹ��������Net session���ڶ����Ǽ���Ƿ���Ȩ�ɹ��������Ȩʧ������ת��failed��ǩ
rem ����Ч��������Ȩʧ��֮��bat�ļ�����ִ�е�����
::Net session >nul 2>&1 || mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c ""%~dpnx0""","","runas",1)(window.close)&&exit
set parameters=
:parameter
@if not "%~1"=="" ( set parameters=%parameters% %~1& shift /1& goto :parameter)
set parameters="%parameters:~1%"
mshta vbscript:createobject("shell.application").shellexecute("%~dpnx0",%parameters%,"","runas",1)(window.close)&exit
cd /d "%~dp0"
Net session >nul 2>&1 || goto failed
goto main

:powershellAdmin
rem ԭ��������powershell��bat�ļ���Ȩ
rem ����ʹ��������Net session���ڶ����Ǽ���Ƿ���Ȩ�ɹ��������Ȩʧ������ת��failed��ǩ
rem ����Ч��������Ȩʧ��֮��bat�ļ�����ִ�е�����
Net session >nul 2>&1 || powershell start-process \"%0\" -argumentlist \"%1 %2\" -verb runas && exit
Net session >nul 2>&1 || goto failed
goto main

:failed
cls
echo.
echo ��ǰδ�Թ���Ա������С����ֶ��Թ���Ա������б�����
echo.
echo ������ر�... & pause > NUL
goto enda

:main
cd /d "%~dp0"
title ʱ��С���߰�װ����
cls
echo.
echo ====================================================
echo                   ʱ��С���߰�װ����
echo ====================================================
echo.
echo ��Ȩ����(C) 2022-2024 CJH��
echo.
echo ��װǰ����ر�ɱ������Լ���UAC����������UAC�ȼ�Ϊ��ͣ������ڰ�װ����������ѡ��д���Զ�������ᱻ���ص��°�װʧ�ܡ�
echo.
echo �������ʼ��װ... & pause >nul

cls
echo.
echo ====================================================
echo                   ʱ��С���߰�װ����
echo ====================================================
echo.
echo ���ڰ�װ��...
echo.
taskkill /f /im TimeControl.exe
::ver|findstr "\<10\.[0-9]\.[0-9][0-9]*\>" > nul && (set netv=4)
ver|findstr "\<6\.[0-1]\.[0-9][0-9]*\>" > nul && (set netv=4c)
ver|findstr "\<6\.[2-9]\.[0-9][0-9]*\>" > nul && (set netv=4c)
ver|findstr "\<5\.[0-9]\.[0-9][0-9]*\>" > nul && (set netv=4c)

if "%PROCESSOR_ARCHITECTURE%"=="x86" goto x86
if "%PROCESSOR_ARCHITECTURE%"=="AMD64" goto x64

:x86
echo.
if exist "%windir%\TimeControl.exe" choice /C YN /T 5 /D Y /M "��⵽��ǰϵͳ���ھɰ汾ʱ��С���ߡ���(Y)��(N)Ҫɾ���ɰ�ʱ��С���ߣ�5����Զ�ѡ��Y��"
if errorlevel 1 set a=1
if errorlevel 2 set a=2
if exist "%windir%\TimeControl.exe" if "%a%" == "1" echo.
if exist "%windir%\TimeControl.exe" if "%a%" == "1" echo �����ʱ��ͣ���ڴ˲����������Ƿ�ɱ��������ء�
if exist "%windir%\TimeControl.exe" if "%a%" == "1" echo.
if exist "%windir%\TimeControl.exe" if "%a%" == "1" call "%~dp0UserinitBootUnInstallOld.bat"
if exist "%windir%\TimeControl.exe" if "%a%" == "1" Reg delete HKLM\Software\Microsoft\Windows\CurrentVersion\run /v TimeControl /f
if exist "%windir%\TimeControl.exe" if "%a%" == "1" del /q "%windir%\TimeControl.exe"
if exist "%windir%\TimeControl.exe" if "%a%" == "1" del /q "%windir%\TimeControlm.exe"

if not exist "%windir%\CJH\TimeControl" md "%windir%\CJH\TimeControl"
copy "%~dp0TimeControl.exe" "%windir%\CJH\TimeControl\TimeControl.exe"
echo.
choice /C YN /T 5 /D Y /M "��(Y)��(N)Ҫ����Զ������5����Զ�ѡ��Y��"
if errorlevel 1 set aa=1
if errorlevel 2 set aa=2
if "%aa%" == "1" echo.
if "%aa%" == "1" echo �����ʱ��ͣ���ڴ˲����������Ƿ�ɱ��������ء�
if "%aa%" == "1" echo.
if "%aa%" == "1" Reg add HKLM\Software\Microsoft\Windows\CurrentVersion\run /v TimeControl /t REG_SZ /d "%windir%\CJH\TimeControl\TimeControl.exe" /f
echo.
choice /C YN /T 5 /D Y /M "��(Y)��(N)Ҫ���Userinit���Զ������5����Զ�ѡ��Y��"
if errorlevel 1 set ab=1
if errorlevel 2 set ab=2
if "%ab%" == "1" echo.
if "%ab%" == "1" echo �����ʱ��ͣ���ڴ˲����������Ƿ�ɱ��������ء�
if "%ab%" == "1" echo.
if "%ab%" == "1" call "%~dp0UserinitBootInstall.bat"
echo.
choice /C YN /T 5 /D Y /M "��(Y)��(N)Ҫ��װ���Ե���ǰϵͳ����װ�����ʹ������Ա༭ʱ��С���ߵĲ��ԣ�����Windows Vista���ϰ汾֧�֣���5����Զ�ѡ��Y��"
if errorlevel 1 set ac=1
if errorlevel 2 set ac=2
if "%ac%" == "1" if exist "%windir%\PolicyDefinitions\*.admx" call "%~dp0TimeControlAdmxs.exe"

echo.
choice /C YN /T 5 /D Y /M "��(Y)��(N)Ҫ������ݷ�ʽ����ʼ�˵���5����Զ�ѡ��Y��"
if errorlevel 1 set ac=1
if errorlevel 2 set ac=2
if "%ad%" == "1" if not exist "%systemdrive%\ProgramData\Microsoft\Windows\Start Menu\Programs\CJH\ʱ��С����" md "%systemdrive%\ProgramData\Microsoft\Windows\Start Menu\Programs\CJH\ʱ��С����"
if "%ad%" == "1" call mshta VBScript:Execute("Set a=CreateObject(""WScript.Shell""):Set b=a.CreateShortcut(""%systemdrive%\ProgramData\Microsoft\Windows\Start Menu\Programs\CJH\ʱ��С����\ʱ��С����.lnk""):b.TargetPath=""%windir%\CJH\TimeControl\TimeControl.exe"":b.WorkingDirectory=""%windir%\CJH\TimeControl"":b.Save:close")

copy /y "%~dp0ж��.bat" "%windir%\CJH\TimeControl\Uninstall.bat"

echo.
choice /C YN /T 5 /D Y /M "��(Y)��(N)���ж�س����б�5����Զ�ѡ��Y��"
if errorlevel 1 set ae=1
if errorlevel 2 set ae=2
if "%ae%" == "1" Reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TimeControl /v DisplayIcon /t REG_SZ /d "%windir%\CJH\TimeControl\TimeControl.exe" /f
if "%ae%" == "1" Reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TimeControl /v DisplayName /t REG_SZ /d "ʱ��С���ߣ�TimeControl��" /f
if "%ae%" == "1" Reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TimeControl /v Publisher /t REG_SZ /d "CJH" /f
if "%ae%" == "1" Reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TimeControl /v UninstallString /t REG_SZ /d "%windir%\CJH\TimeControl\Uninstall.bat" /f

start %windir%\CJH\TimeControl\TimeControl.exe

echo.
cls

echo.
echo ====================================================
echo                   ʱ��С���߰�װ����
echo ====================================================
echo.
echo ��װ��ɣ�������˳�... & pause > nul
goto enda

:x64
echo.
if exist "%windir%\TimeControl.exe" choice /C YN /T 5 /D Y /M "��⵽��ǰϵͳ���ھɰ汾ʱ��С���ߡ���(Y)��(N)Ҫɾ���ɰ�ʱ��С���ߣ�5����Զ�ѡ��Y��"
if errorlevel 1 set a=1
if errorlevel 2 set a=2
if exist "%windir%\TimeControl.exe" if "%a%" == "1" echo.
if exist "%windir%\TimeControl.exe" if "%a%" == "1" echo �����ʱ��ͣ���ڴ˲����������Ƿ�ɱ��������ء�
if exist "%windir%\TimeControl.exe" if "%a%" == "1" echo.
if exist "%windir%\TimeControl.exe" if "%a%" == "1" call "%~dp0UserinitBootUnInstallOld.bat"
if exist "%windir%\TimeControl.exe" if "%a%" == "1" Reg delete HKLM\Software\Microsoft\Windows\CurrentVersion\run /v TimeControl /f
if exist "%windir%\TimeControl.exe" if "%a%" == "1" del /q "%windir%\TimeControl.exe"
if exist "%windir%\TimeControl.exe" if "%a%" == "1" del /q "%windir%\TimeControlm.exe"
if exist "%windir%\TimeControl.exe" if "%a%" == "1" del /q "%windir%\syswow64\TimeControl.exe"
if exist "%windir%\TimeControl.exe" if "%a%" == "1" del /q "%windir%\syswow64\TimeControlm.exe"

if not exist "%windir%\CJH\TimeControl" md "%windir%\CJH\TimeControl"
if not exist "%windir%\CJH\TimeControl\x86" md "%windir%\CJH\TimeControl\x86"
copy "%~dp0TimeControl64.exe" "%windir%\CJH\TimeControl\TimeControl.exe"
copy "%~dp0TimeControl.exe" "%windir%\CJH\TimeControl\x86\TimeControl.exe"
echo.
choice /C YN /T 5 /D Y /M "��(Y)��(N)Ҫ����Զ������5����Զ�ѡ��Y��"
if errorlevel 1 set aa=1
if errorlevel 2 set aa=2
if "%aa%" == "1" echo.
if "%aa%" == "1" echo �����ʱ��ͣ���ڴ˲����������Ƿ�ɱ��������ء�
if "%aa%" == "1" echo.
if "%aa%" == "1" Reg add HKLM\Software\Microsoft\Windows\CurrentVersion\run /v TimeControl /t REG_SZ /d "%windir%\CJH\TimeControl\TimeControl.exe" /f
echo.
choice /C YN /T 5 /D Y /M "��(Y)��(N)Ҫ���Userinit���Զ������5����Զ�ѡ��Y��"
if errorlevel 1 set ab=1
if errorlevel 2 set ab=2
if "%ab%" == "1" echo.
if "%ab%" == "1" echo �����ʱ��ͣ���ڴ˲����������Ƿ�ɱ��������ء�
if "%ab%" == "1" echo.
if "%ab%" == "1" call "%~dp0UserinitBootInstall.bat"
echo.
choice /C YN /T 5 /D Y /M "��(Y)��(N)Ҫ��װ���Ե���ǰϵͳ����װ�����ʹ������Ա༭ʱ��С���ߵĲ��ԣ�����Windows Vista���ϰ汾֧�֣���5����Զ�ѡ��Y��"
if errorlevel 1 set ac=1
if errorlevel 2 set ac=2
if "%ac%" == "1" if exist "%windir%\PolicyDefinitions\*.admx" call "%~dp0TimeControlAdmxs.exe"

echo.
choice /C YN /T 5 /D Y /M "��(Y)��(N)Ҫ������ݷ�ʽ����ʼ�˵���5����Զ�ѡ��Y��"
if errorlevel 1 set ad=1
if errorlevel 2 set ad=2
if "%ad%" == "1" if not exist "%systemdrive%\ProgramData\Microsoft\Windows\Start Menu\Programs\CJH\ʱ��С����" md "%systemdrive%\ProgramData\Microsoft\Windows\Start Menu\Programs\CJH\ʱ��С����"
if "%ad%" == "1" call mshta VBScript:Execute("Set a=CreateObject(""WScript.Shell""):Set b=a.CreateShortcut(""%systemdrive%\ProgramData\Microsoft\Windows\Start Menu\Programs\CJH\ʱ��С����\ʱ��С����.lnk""):b.TargetPath=""%windir%\CJH\TimeControl\TimeControl.exe"":b.WorkingDirectory=""%windir%\CJH\TimeControl"":b.Save:close")
if "%ad%" == "1" call mshta VBScript:Execute("Set a=CreateObject(""WScript.Shell""):Set b=a.CreateShortcut(""%systemdrive%\ProgramData\Microsoft\Windows\Start Menu\Programs\CJH\ʱ��С����\ʱ��С���ߣ�32λ��.lnk""):b.TargetPath=""%windir%\CJH\TimeControl\x86\TimeControl.exe"":b.WorkingDirectory=""%windir%\CJH\TimeControl\x86"":b.Save:close")

copy /y "%~dp0ж��.bat" "%windir%\CJH\TimeControl\Uninstall.bat"

echo.
choice /C YN /T 5 /D Y /M "��(Y)��(N)���ж�س����б�5����Զ�ѡ��Y��"
if errorlevel 1 set ae=1
if errorlevel 2 set ae=2
if "%ae%" == "1" Reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TimeControl /v DisplayIcon /t REG_SZ /d "%windir%\CJH\TimeControl\TimeControl.exe" /f
if "%ae%" == "1" Reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TimeControl /v DisplayName /t REG_SZ /d "ʱ��С���ߣ�TimeControl��" /f
if "%ae%" == "1" Reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TimeControl /v Publisher /t REG_SZ /d "CJH" /f
if "%ae%" == "1" Reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\TimeControl /v UninstallString /t REG_SZ /d "%windir%\CJH\TimeControl\Uninstall.bat" /f

start %windir%\CJH\TimeControl\TimeControl.exe

echo.
cls

echo.
echo ====================================================
echo                   ʱ��С���߰�װ����
echo ====================================================
echo.
echo ��װ��ɣ�������˳�... & pause > nul
goto enda

:enda