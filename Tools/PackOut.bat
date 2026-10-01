@echo off
echo [PACKOUT] 正在打包程序
if exist "%~dp0TimeControl-Bin" rd /s /q "%~dp0TimeControl-Bin"
md "%~dp0TimeControl-Bin"
copy "%~dp0..\Src\TimeControl\files\1-安装.bat" "%~dp0TimeControl-Bin\1-安装.bat"
copy "%~dp0..\Src\TimeControl\files\2-卸载.bat" "%~dp0TimeControl-Bin\2-卸载.bat"
copy "%~dp0..\Src\TimeControl\files\TimeControlAdmxs.exe" "%~dp0TimeControl-Bin\TimeControlAdmxs.exe"
copy "%~dp0..\Src\TimeControl\files\TimeControl.adm" "%~dp0TimeControl-Bin\TimeControl.adm"
copy "%~dp0..\Src\TimeControl\files\TimeControl.xml" "%~dp0TimeControl-Bin\TimeControl.xml"
copy "%~dp0..\Src\TimeControl\bin\Release\TimeControl.exe" "%~dp0TimeControl-Bin\TimeControl.exe"
copy "%~dp0..\Src\TimeControl\bin\x64\Release\TimeControl.exe" "%~dp0TimeControl-Bin\TimeControl64.exe"
copy "%~dp0..\Src\TimeControl\files\certmgr.exe" "%~dp0TimeControl-Bin\certmgr.exe"
copy "%~dp0..\Src\TimeControl\files\rootcert.cer" "%~dp0TimeControl-Bin\rootcert.cer"
echo [PACKOUT] 打包完成
echo.