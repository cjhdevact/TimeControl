::Tips:
::Set the CSIGNTOOL as your SignPackPath
@echo off
path %CSIGNTOOL%;%path%
echo [GetSHA256] 正在获取 SHA-256 校验
if exist "%~dp0TimeControl-Bin.7z.sha256" del /q "%~dp0TimeControl-Bin.7z.sha256"
cmd.exe /c gesha256.cmd "%~dp0TimeControl-Bin.7z"
echo [GetSHA256] SHA-256 校验结束
echo.