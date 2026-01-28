@echo off
setlocal

REM ==========================
REM Variables
REM ==========================
set "DOCKER_IMAGE=longdong123/swd392-iamms:latest"
set "LOCAL_PATH=C:\swd392"
set "SERVER_USER=admin"
set "SERVER_IP=26.92.115.30"
set "CONTAINER_NAME=iamms"
set "SERVER_PORT=5000"

REM ==========================
REM 1️⃣ Build Docker image locally
REM ==========================
echo Building Docker image...
cd /d "%LOCAL_PATH%"
docker build -t %DOCKER_IMAGE% .
IF ERRORLEVEL 1 (
    echo Docker build failed. Aborting.
    exit /b 1
)

REM ==========================
REM 2️⃣ Push Docker image to Docker Hub
REM ==========================
echo Pushing Docker image to Docker Hub...
docker push %DOCKER_IMAGE%
IF ERRORLEVEL 1 (
    echo Docker push failed. Aborting.
    exit /b 1
)

REM ==========================
REM 3️⃣ SSH into server & update container
REM ==========================
echo Connecting to server and updating container...
ssh %SERVER_USER%@%SERVER_IP% "docker pull %DOCKER_IMAGE% && docker stop %CONTAINER_NAME% || true && docker rm %CONTAINER_NAME% || true && docker run -d --name %CONTAINER_NAME% -p %SERVER_PORT%:%SERVER_PORT% %DOCKER_IMAGE%"

IF ERRORLEVEL 1 (
    echo SSH command failed. Check connection or credentials.
    exit /b 1
)

echo Deployment finished successfully!
pause
endlocal
