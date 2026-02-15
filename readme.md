# SG Financial Technology - API de Gestión de Cuentas

API orientada a servicios para la gestión segura de cuentas, permitiendo a los usuarios registrarse, autenticarse mediante JWT y realizar operaciones financieras básicas (depósitos, retiros y consulta de saldos).

## Decisiones de Arquitectura
* **Arquitectura Orientada a Servicios:** Decidí separar la lógica en dos microservicios principales para garantizar alta cohesión y bajo acoplamiento:
    * `Auth.API`: Responsable de la identidad y emisión de tokens JWT.
    * `Account.API`: Responsable de las transacciones y el core financiero.
* **API Gateway:** Punto de entrada único. Es el encargado de enrutar las peticiones al servicio correspondiente, simplificando el consumo para los clientes.
* **Database per Service:** Cada servicio administra sus propios datos en SQL Server.
* **Clean Architecture:** Implementada en los microservicios para separar la lógica de negocio y casos de uso de la infraestructura y los controladores.
* **Comunicación Interna:** To do.

## Stack Tecnológico
* **Framework:** .NET 8 con C#.
* **Gateway:** YARP.
* **Base de Datos:** SQL Server y Entity Framework Core.
* **Seguridad:** Autenticación basada en JWT.

## Configuración y Ejecución
La aplicación está diseñada para ser ejecutada y probada en un entorno local.

1. **Clonar el repositorio:** `git clone https://github.com/luc4scuru4l/sg_challenge.git`

2. **Configuración de Base de Datos:**
    To do

3. **Ejecución:**
    To do

## Autenticación
Para garantizar la seguridad, todos los endpoints financieros requieren autenticación.

**¿Cómo obtener un JWT?**
Realizar un `POST` al endpoint de login (`http://localhost:XXXX/api/auth/login`) con credenciales válidas. La respuesta incluirá el JWT.

**Ejemplo de uso:**
Para consumir cualquier endpoint de cuentas, se debe incluir el JWT en los headers de las peticiones HTTP:
`Authorization: Bearer <TU_JWT>`

## Endpoints Disponibles
El API Gateway se ejecuta en el puerto `XXXX`. Todas las peticiones deben dirigirse a este puerto.

### Autenticación (Ruteado a Auth.API)
* `POST http://localhost:XXXX/api/auth/register` - Crea un nuevo usuario.
* `POST http://localhost:XXXX/api/auth/login` - Autentica y devuelve el JWT.

### Cuentas (Ruteado a Account.API)
* `POST http://localhost:XXXX/api/accounts` - Crea una cuenta.
* `GET http://localhost:XXXX/api/accounts/{id}/balance` - Consulta el saldo.
* `POST http://localhost:XXXX/api/accounts/{id}/deposit` - Realiza un depósito.
* `POST http://localhost:XXXX/api/accounts/{id}/withdraw` - Realiza un retiro.