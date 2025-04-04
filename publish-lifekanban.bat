@echo off
echo Publishing LifeKanban application...

:: Publish the API
dotnet publish LifeKanbanApi -c Release -o ./publish/api

:: Publish the Web App
dotnet publish LifeKanban -c Release -o ./publish/web

echo LifeKanban has been published to the ./publish directory