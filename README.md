# RealEstateMillion.API — DESARROLLADA POR ROINER GOMEZ

Este repositorio contiene la solución **RealEstateMillion.API**, una API RESTful desarrollada en **.NET 9** para la gestión de propiedades inmobiliarias. El proyecto sigue un enfoque de arquitectura limpia y modular, con separación clara de responsabilidades entre capas.

---

## Enfoque arquitectónico

La solución está organizada en múltiples proyectos que representan capas funcionales independientes:

| Proyecto                      | Rol principal                                                                 |
|------------------------------|------------------------------------------------------------------------------|
| **RealEstateMillion.API**     | Punto de entrada de la API. Contiene controladores, filtros, middleware y configuración. |
| **RealEstateMillion.Application** | Lógica de negocio, servicios de aplicación, DTOs y mapeos.                         |
| **RealEstateMillion.Domain**      | Modelos de dominio, entidades, interfaces y contratos.                           |
| **RealEstateMillion.Infrastructure** | Implementaciones concretas de servicios, acceso a datos, y lógica persistente.     |
| **RealEstateMillion.Tests**       | Proyecto de pruebas unitarias y helpers para testeo.                             |
| **RealEstateMillion.API.http**    | Configuración auxiliar para pruebas HTTP (Postman, etc).                          |

Este enfoque permite mantener una arquitectura desacoplada, escalable y fácil de testear.

---

## Estructura destacada

### 🔹 RealEstateMillion.API
- **Controllers/**: Endpoints públicos de la API.
- **Filters/**: Filtros globales para validaciones y manejo de errores.
- **Attributes/**: Decoradores personalizados.
- **Middleware/**: Lógica transversal (ej. logging, autenticación).
- **wwwroot/**: Imagenes cargadas
- **appsettings.json**: Configuración de entorno.
- **Dockerfile**: Configuración para despliegue en contenedores.

### 🔹 RealEstateMillion.Application
- **DTOs/**: Objetos de transferencia entre capas.
- **Mappings/**: Configuración de AutoMapper.
- **Services/**: Lógica de negocio desacoplada del controlador.

### 🔹 RealEstateMillion.Domain
- **Entities/**: Modelos del negocio (ej. Property, Owner).
- **Interfaces/**: Contratos para repositorios y servicios.
- **Enums/**: Tipos definidos para el dominio.

### 🔹 RealEstateMillion.Infrastructure
- **Services/**: Implementaciones concretas de interfaces del dominio.
- **Common/**: Utilidades compartidas para persistencia y configuración.

### 🔹 RealEstateMillion.Tests
- **TestHelpers/**: Mocks, datos de prueba y configuraciones para testeo unitario.

---

##  Pruebas y entorno Postman

Para facilitar el testeo, se incluyen dos archivos:

- `realstate.json`: colección de endpoints para importar en Postman.
- `data.json`: ejemplos de payloads para crear propiedades y aplicar filtros.

> Recuerda iniciar sesión con el endpoint `/api/v1/auth/login` antes de consumir endpoints protegidos.

---

## enfoque

- **Escalabilidad**: cada módulo puede evolucionar de forma independiente.
- **Testabilidad**: separación clara permite pruebas unitarias efectivas.
- **Mantenibilidad**: estructura limpia facilita refactorizaciones y nuevas integraciones.
- **Cumplimiento**: permite aplicar validaciones legales y trazabilidad sin acoplar la lógica.

---

##  Recomendaciones para contribuir

- Mantén la separación de capas: no mezcles lógica de negocio en controladores.
- Usa interfaces para desacoplar dependencias.
- Agrega pruebas unitarias para cada nuevo servicio.
- Documenta los endpoints en Swagger y mantén actualizados los archivos de Postman.

---

## Practias implementadas
- Architecture
- Structure
- Documentation Code
- Best Practices
- Manage Performance
- Unit Test
- Security

## Se gestionó de forma eficiente
- Create Property Building
- Add Image from property
- Change Price
- Update property
- List property with filters

---

##  Requisitos previos

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Postman](https://www.postman.com/downloads/)
- Git

---

##  Instalación

```bash
git clone  https://github.com/RoinerGomez98/RealEstateMillion.API.git
cd million-real-estate-api
dotnet restore
dotnet build
