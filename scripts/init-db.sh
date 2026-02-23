#!/bin/bash

echo "Inicializando bases de datos en SQL Server..."

docker exec -i sg_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SGFinancial2026!" -C < ./scripts/01_AuthDb.sql
if [ $? -ne 0 ]; then
    echo "Error crítico: Falló la inicialización de AuthDb. Abortando proceso."
    exit 1
fi

docker exec -i sg_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SGFinancial2026!" -C < ./scripts/02_AccountDb.sql
if [ $? -ne 0 ]; then
    echo "Error crítico: Falló la inicialización de AccountDb. Abortando proceso."
    exit 1
fi

echo ""
echo "Ejecución finalizada correctamente."
