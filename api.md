# Documentación de los endpoints

Todos los enpoints pueden recibir y retornar JSON.

## AuthService

Microservicio encargado de la gestión de identidades, registro de usuarios y emisión de tokens JWT para el ecosistema
financiero SG.
Este servicio permite:

1. Registrar un usuario
2. Loguear un usuario

### Endpoints

#### 1. `POST /api/auth/register`

Registra un nuevo usuario en el sistema.
Las credenciales a registrar deben cumplir con la política de seguridad:

**Constraseña:**

    Mínimo 8 caracteres.
    Máximo 32 caracteres.
    Al menos un número.

**Nombre de usuario**

    Menos de 50 caracteres
    Más de 4 caracteres

*Tener en cuenta que el nombre de usuario es case insensitive*

##### Body
| Campo      | Tipo   | Requerido | Descripción       |
|:-----------|:-------| :--- |:------------------|
| `userName` | string | **Sí** | Nombre de usuario |
| `password` | string | **Sí** | Contraseña        |

##### Response

1. `201 Created`

        Sin cuerpo
2. `400 Bad Request`: Alguna validación en cuanto a las politicas de seguridad de las credenciales no fue superada.
    ```json
    {
        "title": "Invalid Password",
        "status": 400,
        "detail": "La contraseña no puede tener menos de 8 caracteres.",
        "instance": "/api/auth/register"
    }
    ```
3. `409 Conflict`: Ya existe un usuario con ese nombre de usuario.
    ```json
    {
        "title": "Conflict User",
        "status": 409,
        "detail": "El nombre de usuario 'Morfi' ya está en uso.",
        "instance": "/api/auth/register"
    }
    ```

#### 2. `POST /api/auth/login`

Crea y retorna un token de acceso firmado para un usuario registrado. El token expira a los 20 minutos.

##### Body

| Campo      | Tipo   | Requerido | Descripción       |
|:-----------|:-------| :--- |:------------------|
| `userName` | string | **Sí** | Nombre de usuario |
| `password` | string | **Sí** | Contraseña        |

##### Response

1. `200 Ok`: Inicio de sesión exitoso.
    ```json
    {
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1MWJmZTljMy02YjkwLTQ3MGUtYjE4Ny03MTYzOTJkNTEzMmQiLCJuYW1lIjoiTHVxdWkiLCJqdGkiOiJlYWNiOWExMC00NTViLTQ0NTUtYWVkOS1iNzM3NDJmMWY5MmQiLCJleHAiOjE3NzE4MTY2NDIsImlzcyI6IlNHLkF1dGhTZXJ2aWNlIiwiYXVkIjoiU0cuTWljcm9zZXJ2aWNlcyJ9.Sa1uaqIXipvP12HsbahU2SeOUMAAoPpy8h1rLedMb14"
    }
    ```
2. `401 Unauthorized`: Usuario inexistente o credenciales inválidas.
   ```json
   {
      "title": "Unauthorized",
      "status": 401,
      "detail": "Credenciales inválidas.",
      "instance": "/api/auth/login"
   }
   ```

## AccountService

Microservicio encargado de la gestión de cuentas para el ecosistema financiero SG.
Todos los endpoints están segurizados mediante JWT.
Por lo que todos los endpoints deben incluir la siguiente cabecera:

`Authorization: Bearer <TU_JWT>`

Este servicio permite:

1. Crear una cuenta.
2. Consultar el balance de una cuenta.
3. Realizar un depósito en una cuenta.
4. Realizar un retiro en una cuenta.

### Endpoints

*Todos pueden devolver `401 Unauthorized` sin cuerpo si el token es inválido, está vencido*

#### 1. `POST /api/accounts`

Crea una cuenta con balance 0.

##### Body

No recibe

##### Response

1. `201 Created`: Cuenta creada exitosamente
   ```json
   {
      "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "balance": 0
   }
   ```

#### 2. `GET /api/accounts/{id}/balance`

Consulta el balance de una cuenta existente.

##### Body

No recibe

##### Response

1. `200 Ok`
   ```json
   {
       "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
       "balance": 1111.2100
   }
   ```
2. `404 Not Found`: Cuenta inexistente
   ```json
   {
      "title": "Resource Not Found",
      "status": 404,
      "detail": "No se encontró la cuenta con el ID especificado: 1c14fe81-dbec-4f74-8f45-63f15e21a74a",
      "instance": "/api/accounts/1c14fe81-dbec-4f74-8f45-63f15e21a74a/balance"
   }
   ```

#### 3. `POST /api/accounts/{id}/deposit`

Realiza un deposito en una cuenta. El monto no puede ser negativo o tener más de 3 decimales.

##### Body

| Campo      | Tipo    | Requerido | Descripción       |
|:-----------|:--------| :--- |:------------------|
| `amount`   | numeric | **Sí** | Monto a depositar |

##### Response

1. `200 OK`: Deposito exitoso.
   ```json
   {
      "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "balance": 100
   }
   ```

2. `400 Bad Request`: El monto tenía más de dos decimales o era negativo
   ```json
   {
       "title": "Invalid Money Amount",
       "status": 400,
       "detail": "El monto no puede tener más de dos decimales.",
       "instance": "/api/accounts/cdef4f1f-a540-4331-832b-c5e703e23ade/deposit"
   }
   ```
   
3. `404 Not Found`: Cuenta inexistente o no asociada al userId del token.
   ```json
   {
      "title": "Resource Not Found",
      "status": 404,
      "detail": "No se encontró la cuenta con el ID especificado: cdef4f1f-a540-4331-832b-c5e703e23ad3",
      "instance": "/api/accounts/cdef4f1f-a540-4331-832b-c5e703e23ad3/deposit"
   }
   ```

#### 4. `POST /api/accounts/{id}/withdraw`

Realiza un retiro en una cuenta existente. El monto a retirar no puede exceder el saldo actual de la cuenta ni ser negativo o tenes más de dos decimales.

##### Body

| Campo      | Tipo    | Requerido | Descripción     |
|:-----------|:--------| :--- |:----------------|
| `amount`   | numeric | **Sí** | Monto a retirar |

##### Response

1. `200 OK`: Retiro exitoso.
   ```json
   {
      "accountId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "balance": 100
   }
   ```

2. `400 Bad Request`: El monto tenía más de dos decimales o era negativo
   ```json
   {
       "title": "Invalid Money Amount",
       "status": 400,
       "detail": "El monto no puede tener más de dos decimales.",
       "instance": "/api/accounts/cdef4f1f-a540-4331-832b-c5e703e23ade/deposit"
   }
   ```

3. `404 Not Found`: Cuenta inexistente o no asociada al userId del token.
   ```json
   {
      "title": "Resource Not Found",
      "status": 404,
      "detail": "No se encontró la cuenta con el ID especificado: cdef4f1f-a540-4331-832b-c5e703e23ad3",
      "instance": "/api/accounts/cdef4f1f-a540-4331-832b-c5e703e23ad3/deposit"
   }
   ```
4. `422 Unprocessable Content`: El monto a retirar es mayor que el balance de la cuenta.
   ```json
   {
    "title": "Business Validation Error",
    "status": 422,
    "detail": "Fondos insuficientes para realizar la operación.",
    "instance": "/api/accounts/09a57768-5cbd-407e-b5e8-b47222ff4b23/withdraw"
   }
   ```