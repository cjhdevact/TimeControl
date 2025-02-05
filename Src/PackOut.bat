@echo off
echo 任意键打包 时间小工具（TimeControl）...
pause > nul
if exist "%~dp0TimeControl-Bin" rd /s /q "%~dp0TimeControl-Bin"
md "%~dp0TimeControl-Bin"
copy "%~dp0TimeControl\files\1-安装.bat" "%~dp0TimeControl-Bin\1-安装.bat"
copy "%~dp0TimeControl\files\2-卸载.bat" "%~dp0TimeControl-Bin\2-卸载.bat"
copy "%~dp0TimeControl\files\TimeControlAdmxs.exe" "%~dp0TimeControl-Bin\TimeControlAdmxs.exe"
copy "%~dp0TimeControl\files\TimeControl.adm" "%~dp0TimeControl-Bin\TimeControl.adm"
copy "%~dp0TimeControl\files\TimeControl.xml" "%~dp0TimeControl-Bin\TimeControl.xml"
copy "%~dp0TimeControl\files\UserinitBootInstall.bat" "%~dp0TimeControl-Bin\UserinitBootInstall.bat"
copy "%~dp0TimeControl\files\UserinitBootUnInstall.bat" "%~dp0TimeControl-Bin\UserinitBootUnInstall.bat"
copy "%~dp0TimeControl\bin\Release\TimeControl.exe" "%~dp0TimeControl-Bin\TimeControl.exe"
copy "%~dp0TimeControl\bin\x64\Release\TimeControl.exe" "%~dp0TimeControl-Bin\TimeControl64.exe"
copy "%~dp0TimeControl\files\certmgr.exe" "%~dp0TimeControl-Bin\certmgr.exe"
copy "%~dp0TimeControl\files\rootcert.cer" "%~dp0TimeControl-Bin\rootcert.cer"
echo.
echo 完成！
echo 任意键退出...
pause > nul