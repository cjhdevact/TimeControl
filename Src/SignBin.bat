::Tips Set the CSIGNCERT as your path.
@echo off
path D:\ProjectsTmp\SignPack;%path%
echo �����ǩ�� ʱ��С���ߣ�TimeControl��...
pause > nul
cmd.exe /c signcmd.cmd "%CSIGNCERT%" "%~dp0TimeControl-Bin\TimeControl.exe"
cmd.exe /c signcmd.cmd "%CSIGNCERT%" "%~dp0TimeControl-Bin\TimeControl64.exe"
cmd.exe /c signcmd.cmd "%CSIGNCERT%" "%~dp0TimeControl-Bin\TimeControlAdmxs.exe"
echo.
echo ��ɣ�
echo ������˳�...
pause > nul