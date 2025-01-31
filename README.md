# API de Gestión de Cursos con ASP.NET Core

## Por Arturo Carrasco González

## Descripción General

Este es un proyecto de API robusta desarrollado con ASP.NET Core que proporciona funcionalidades integrales para la gestión de categorías, cursos y usuarios. La aplicación sigue una arquitectura en capas y utiliza una variedad de tecnologías y herramientas para ofrecer una solución completa y escalable.

Además, la idea detrás de este proyecto era crear una API cuyas funcionalidades pudieran servir como plantilla para proyectos más grandes, facilitando la reutilización de código y la implementación de buenas prácticas en el desarrollo de APIs.

## Características Clave

### Gestión de Categorías

- Permite crear, consultar, actualizar y eliminar categorías.
- Las categorías se utilizan para organizar y clasificar los cursos disponibles.

### Gestión de Cursos

- Permite crear, consultar, actualizar y eliminar cursos.
- Cada curso está asociado a una categoría específica.
- Proporciona funcionalidades avanzadas de búsqueda y filtrado de cursos.

### Gestión de Usuarios

- Permite el registro de nuevos usuarios.
- Soporta la autenticación de usuarios a través de un sistema de inicio de sesión.
- Ofrece endpoints para recuperar información de los usuarios.

### Autenticación y Autorización

- Utiliza .NET Identity para la autenticación de usuarios y el control de acceso basado en roles.
- Administra los permisos y roles de los usuarios para restringir el acceso a los recursos de la API.

### Versionado de la API

- Implementa un sistema de versionado para admitir múltiples versiones de la API de forma simultánea.
- Garantiza la compatibilidad hacia atrás y una migración gradual para los clientes.

### Paginación

- Implementa un sistema de paginación en las consultas de listas de elementos.
- Mejora el rendimiento y evita la sobrecarga del servidor al devolver los datos en bloques manejables.
- Permite a los clientes de la API navegar eficientemente por grandes volúmenes de datos.

### Caché

- Aprovecha el caché en memoria para mejorar los tiempos de respuesta de la API.
- Reduce la carga en la base de datos.
- Configura perfiles de caché para controlar el comportamiento de almacenamiento en caché en diferentes endpoints.

### Documentación y Pruebas

- Genera documentación interactiva de la API utilizando Swagger/OpenAPI.
- Permite a los desarrolladores explorar y probar fácilmente los endpoints disponibles.

### Subida de Imágenes

- Admite la carga y gestión de imágenes de los cursos.
- Los usuarios pueden cargar imágenes relacionadas con los cursos.
- La API se encarga de almacenarlas y asociarlas correctamente.

---

## Tecnologías y Herramientas Utilizadas

- **ASP.NET Core**: La última versión del framework web de Microsoft.
- **Entity Framework Core**: Biblioteca de Mapeo Objeto-Relacional (ORM) para el acceso a datos.
- **SQL Server**: Base de datos relacional utilizada para almacenar los datos de la aplicación.
- **AutoMapper**: Librería para mapear objetos entre diferentes modelos de datos.
- **Swagger/OpenAPI**: Herramienta para la generación de documentación interactiva de la API.
- **.NET Identity**: Framework para manejar la autenticación y autorización de usuarios.
- **Redis**: Almacén de datos en memoria utilizado para implementar la funcionalidad de caché.

---

## Configuración y Ejecución

### Prerrequisitos

Asegúrate de tener instalado el último SDK de .NET Core.

### Configuración de la Base de Datos

1. Configura la cadena de conexión a tu servidor SQL Server en el archivo appsettings.json.
2. Ejecuta las migraciones de la base de datos para crear las tablas necesarias:

   dotnet ef migrations add InitialCreate  
   dotnet ef database update  

### Secretos de Usuario

Agrega el siguiente secreto de usuario a tu proyecto:

   {  
     "ApiSettings:secretKey": "API Cursos con .net 8 completa y muy enriquecedora a nivel de conocimientos."  
   }

Puedes agregar este secreto utilizando herramientas como:

   dotnet user-secrets set "ApiSettings:secretKey" "API Cursos con .net 8 completa y muy enriquecedora a nivel de conocimientos."

O editando manualmente el archivo secrets.json.

### Ejecución de la Aplicación

1. Inicia la aplicación con:

   dotnet run  

2. Visita la documentación de Swagger en la siguiente ruta:

   http://localhost:5000/swagger  
