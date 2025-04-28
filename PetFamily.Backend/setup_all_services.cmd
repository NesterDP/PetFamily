@echo off

docker-compose up -d --build

docker stop web fileservice notificationservice

cd /d "%~dp0Backend"
call dbscript.cmd

cd /d "%~dp0FileService"
call dbscript.cmd

docker start web fileservice notificationservice

echo Готово!
pause
