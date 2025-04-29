@echo off

docker-compose up -d --build

docker stop mainservice fileservice notificationservice

cd /d "%~dp0Backend"
call dbscript.cmd

cd /d "%~dp0FileService"
call dbscript.cmd

docker start mainservice fileservice notificationservice

echo Готово!
pause