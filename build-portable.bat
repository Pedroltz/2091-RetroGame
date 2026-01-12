@echo off
REM Script alternativo em Batch para build portátil
REM Executa o script PowerShell

title 2091 - Builder Portátil

echo.
echo ========================================
echo   Executando Builder de Aplicativo
echo ========================================
echo.

powershell -ExecutionPolicy Bypass -File "%~dp0build-portable.ps1"

if errorlevel 1 (
    echo.
    echo ERRO: Falha ao executar o script PowerShell
    echo.
    pause
    exit /b 1
)

exit /b 0
