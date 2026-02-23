@echo off
echo Inicializando bases de datos en SQL Server...

docker exec -i sg_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SGFinancial2026!" -C < ./scripts/01_AuthDb.sql
if %errorlevel% neq 0 exit /b %errorlevel%

docker exec -i sg_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SGFinancial2026!" -C < ./scripts/02_AccountDb.sql
if %errorlevel% neq 0 exit /b %errorlevel%

echo.
echo Ejecucion finalizada correctamente.
pause
