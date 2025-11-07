@echo off
setlocal enabledelayedexpansion

REM ------------------------------------------------------------
REM  Simple launcher for Team-6 services (Release configuration)
REM  - Starts RabbitMQ in Docker (if not already running)
REM  - Launches compiled .NET executables in Release bin folders
REM  - Starts the frontend dev server
REM ------------------------------------------------------------

REM Navigate to repo root
pushd "%~dp0.."
set "REPO_ROOT=%CD%"

echo.
echo ===============================================
echo   Starting Team-6 services (Release mode)
echo ===============================================
echo.

REM --- Optional .NET build -------------------------------------
echo Building solution (Release)...
dotnet build "%REPO_ROOT%\Team-6.sln" --configuration Release
if errorlevel 1 (
    echo Build failed. Aborting startup.
    popd
    exit /b 1
)

REM --- RabbitMQ ------------------------------------------------
set "RABBIT_NAME=team6-rabbitmq"
echo Ensuring RabbitMQ container (%RABBIT_NAME%) is running...

set "RABBIT_EXISTS="
for /f "delims=" %%i in ('docker ps -aq --filter "name=%RABBIT_NAME%"') do set "RABBIT_EXISTS=%%i"

if not defined RABBIT_EXISTS (
    echo Container not found. Creating new instance...
    docker run -d --name %RABBIT_NAME% -p 5672:5672 -p 15672:15672 rabbitmq:3-management
) else (
    set "RABBIT_RUNNING="
    for /f "delims=" %%i in ('docker ps -q --filter "name=%RABBIT_NAME%"') do set "RABBIT_RUNNING=%%i"
    if defined RABBIT_RUNNING (
        echo RabbitMQ already running.
    ) else (
        echo Starting existing RabbitMQ container...
        docker start %RABBIT_NAME%
    )
)

echo.

REM --- Helper to start executable in a new window --------------
call :StartService "ApiGateway" "%REPO_ROOT%\ApiGateway\bin\Release\net9.0\ApiGateway.exe" "ASPNETCORE_URLS=http://localhost:56466"
call :StartService "Authorization.Api" "%REPO_ROOT%\AuthService\bin\Release\net9.0\Authorization.Api.exe" "ASPNETCORE_URLS=http://localhost:56468"
call :StartService "ConversationService.Api" "%REPO_ROOT%\ConversationService.Api\bin\Release\net9.0\ConversationService.Api.exe" "ASPNETCORE_URLS=http://localhost:55822"
call :StartService "OrchestratService.Application" "%REPO_ROOT%\OrchestratService.Application\bin\Release\net9.0\OrchestratService.Application.exe"
call :StartService "OnlineStatusService" "%REPO_ROOT%\OnlineStatusService\OnlineStatusService\bin\Release\net9.0\OnlineStatusService.exe"
call :StartService "ConversationDistributed" "%REPO_ROOT%\ConversationDistributed\bin\Release\net9.0\ConversationDistributed.exe"
call :StartService "MessageHubService.Application" "%REPO_ROOT%\MessageHubService.Application\bin\Release\net9.0\MessageHubService.Application.exe"
call :StartService "ChannelSettings" "%REPO_ROOT%\ChannelSettings\bin\Release\net9.0\ChannelSettings.exe" "ASPNETCORE_URLS=http://localhost:5014"

REM --- Frontend ------------------------------------------------
if exist "%REPO_ROOT%\frontend" (
    echo Starting frontend (npm run dev)...
    start "Frontend" cmd /k "cd /d %REPO_ROOT%\frontend && call npm.cmd run dev"
) else (
    echo Frontend directory not found, skipping frontend startup.
)

echo.
echo -------------------------------------------------------------
echo  All launch commands issued. Services run in their own windows.
echo  RabbitMQ UI: http://localhost:15672  (admin / admin)
echo -------------------------------------------------------------
echo.

popd
exit /b 0

:StartService
set "SERVICE_NAME=%~1"
set "EXEC_PATH=%~2"
set "SERVICE_ENV=%~3"

if not exist "%EXEC_PATH%" (
    echo WARNING: %SERVICE_NAME% executable not found at %EXEC_PATH%
    goto :EOF
)

set "SERVICE_DIR=%~dp2"
set "EXEC_FILE=%~nx2"
echo Starting %SERVICE_NAME%...

if defined SERVICE_ENV (
    start "Service - %SERVICE_NAME%" cmd /k "cd /d %SERVICE_DIR% && set %SERVICE_ENV% && %EXEC_FILE%"
) else (
    start "Service - %SERVICE_NAME%" cmd /k "cd /d %SERVICE_DIR% && %EXEC_FILE%"
)
goto :EOF

