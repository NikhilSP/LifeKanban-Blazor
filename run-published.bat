@echo off
echo Starting LifeKanban application...

:: Start the API (in a new window)
start cmd /k "cd LifeKanbanApi && dotnet run --urls=http://localhost:5190"

:: Wait a moment for the API to initialize
timeout /t 5

:: Start the Web App
start cmd /k "cd LifeKanban && dotnet run --urls=http://localhost:5119"

:: Wait another moment and launch browser
timeout /t 3
start http://localhost:5119

echo LifeKanban is running!
echo API: http://localhost:5190
echo Web App: http://localhost:5119