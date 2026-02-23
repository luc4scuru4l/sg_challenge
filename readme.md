# SG Financial Technology - API de Gestión de Cuentas

API orientada a servicios para la gestión segura de cuentas, permitiendo a los usuarios registrarse, autenticarse mediante JWT y realizar operaciones financieras básicas (depósitos, retiros y consulta de saldos).

## Stack Tecnológico

* **Framework:** .NET 8 con C#.
* **Gateway:** YARP.
* **Base de Datos:** SQL Server y Entity Framework Core.
* **Seguridad:** Autenticación basada en JWT.

## Decisiones de Arquitectura

* **Arquitectura Orientada a Servicios:** Decidí separar la lógica en dos microservicios principales para garantizar alta cohesión y bajo acoplamiento:
    * `AuthService`: Responsable de la identidad y emisión de tokens JWT.
    * `AccountService`: Responsable de las transacciones y el core financiero.
* **API Gateway:** Punto de entrada único. Es el encargado de enrutar las peticiones al servicio correspondiente, simplificando el consumo para los clientes.
* **Database per Service:** Cada servicio administra sus propios datos en SQL Server.
* **Clean Architecture:** Implementada en los microservicios para separar la lógica de negocio y casos de uso de la infraestructura y los controladores.

## Configuración
La aplicación está diseñada para ser ejecutada y probada en un entorno local.

**1. Clonar el repositorio:** `git clone https://github.com/luc4scuru4l/sg_challenge.git` y pararse en la raíz.

**2. Preparar las variables de entorno**

Copiá el archivo de ejemplo para generar su configuración local:
`cp .env.example .env`

Asegurate de que el archivo `.env` contenga los siguientes valores:

* `DB_PASSWORD`: Contraseña del usuario SA de SQL Server (Ej: SGFinancial2026!).
* `JWT_SECRET_KEY`: Clave privada para la firma de tokens.
* `GATEWAY_PORT`: Puerto del API Gateway (Por defecto: 5000).
* `SQL_PORT`: Puerto de SQL Server (Por defecto: 1433).

**3. Levantar la infraestructura**

Ejecutá el orquestador en segundo plano:

`docker compose up -d`

*Nota: Si experimentas conflictos de puertos, modificá `GATEWAY_PORT` o `SQL_PORT` en su archivo `.env`.*

**4. Inicializar las Bases de Datos**

Una vez que el contenedor de SQL Server esté operativo, Hay que inyectar los esquemas de datos ejecutando los siguientes comandos
en la terminal. **Reemplazá "SGFinancial2026!" si modificaste la contraseña en el paso 1.**

```bash
docker exec -i sg_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SGFinancial2026!" -C < ./scripts/01_AuthDb.sql

docker exec -i sg_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SGFinancial2026!" -C < ./scripts/02_AccountDb.sql
```

## Ejecución

### Autenticación
Para garantizar la seguridad, todos los endpoints del servicio de cuentas requieren autenticación.

*Nota: De acá a lo que sigue de la documentación se asume que el API Gateway está escuchando en el puerto 5000*

##### **¿Cómo obtener un JWT?**
1. Registrarse con el endpoint `http://localhost:5000/api/auth/register` 
2. Loguearse con el endpoint `http://localhost:5000/api/auth/login` utilizando las credenciales del punto 1.  
3. Utilizar el token obtenido en el punto 2 para acceder a los endpoints del servicio de cuentas. 
El token debe ser incluído en la cabecera de las peticiones de la siguiente forma: `Authorization: Bearer <TU_JWT>`

## Endpoints Disponibles
El API Gateway se ejecuta en el puerto `5000`. Todas las peticiones deben dirigirse a este puerto.

### Autenticación (Ruteado a AuthService.API)
* `POST http://localhost:5000/api/auth/register` - Crea un nuevo usuario.
* `POST http://localhost:5000/api/auth/login` - Autentica y devuelve el JWT.

### Cuentas (Ruteado a AccountService.API)
* `POST http://localhost:5000/api/accounts` - Crea una cuenta.
* `GET http://localhost:5000/api/accounts/{id}/balance` - Consulta el saldo de una cuenta.
* `POST http://localhost:5000/api/accounts/{id}/deposit` - Realiza un depósito en una cuenta.
* `POST http://localhost:5000/api/accounts/{id}/withdraw` - Realiza un retiro en una cuenta.

Para más información respecto a los DTO's de entrada y salida para los endpoints se puede consultar:
1. Documentación en Swagger
   1. **Servicio de autenticación:** `http://localhost:5000/auth-docs/`
   2. **Servicio de cuentas:** `http://localhost:5000/accounts-docs/`
2. Colección de postman presente en la raíz del proyecto. Contiene los endpoints y ejemplos.
3. Consultar el archivo `api.md` presente en la raíz del proyecto. 

## Bibliografía consultada
1. Building Microservices - Sam Newman
2. Clean Architecture - Robert C. Martin
3. Designing Web APIs - Brenda Jin et al.