::Tips Set the CSIGNCERT as your path.
@echo off
path D:\ProjectsTmp\SignPack;%path%
echo 任意键签名 时间小工具（TimeControl）...
pause > nul
cmd.exe /c signcmd.cmd "%CSIGNCERT%" "%~dp0TimeControl-Bin\TimeControl.exe"
cmd.exe /c signcmd.cmd "%CSIGNCERT%" "%~dp0TimeControl-Bin\TimeControl64.exe"
cmd.exe /c signcmd.cmd "%CSIGNCERT%" "%~dp0TimeControl-Bin\TimeControlAdmxs.exe"
echo.
echo 完成！
echo 任意键退出...
pause > nul