@echo off
setlocal

powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0Uninstall-Template.ps1"
set "UNINSTALL_EXIT_CODE=%ERRORLEVEL%"

echo.
if not "%UNINSTALL_EXIT_CODE%"=="0" (
    echo Uninstallation failed. Review the error above.
)
pause
exit /b %UNINSTALL_EXIT_CODE%

