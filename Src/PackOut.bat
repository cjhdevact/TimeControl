@echo off
echo �������� ʱ��С���ߣ�TimeControl��...
pause > nul
if exist "%~dp0TimeControl-Bin" rd /s /q "%~dp0TimeControl-Bin"
md "%~dp0TimeControl-Bin"
copy "%~dp0TimeControl\files\1-��װ.bat" "%~dp0TimeControl-Bin\1-��װ.bat"
copy "%~dp0TimeControl\files\2-ж��.bat" "%~dp0TimeControl-Bin\2-ж��.bat"
copy "%~dp0TimeControl\files\TimeControlAdmxs.exe" "%~dp0TimeControl-Bin\TimeControlAdmxs.exe"
copy "%~dp0TimeControl\files\UserinitBootInstall.bat" "%~dp0TimeControl-Bin\UserinitBootInstall.bat"
copy "%~dp0TimeControl\files\UserinitBootUnInstall.bat" "%~dp0TimeControl-Bin\UserinitBootUnInstall.bat"
copy "%~dp0TimeControl\bin\Release\TimeControl.exe" "%~dp0TimeControl-Bin\TimeControl.exe"
copy "%~dp0TimeControl\bin\x64\Release\TimeControl.exe" "%~dp0TimeControl-Bin\TimeControl64.exe"
echo.
echo ��ɣ�
echo ������˳�...
pause > nul