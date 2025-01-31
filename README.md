### API de Gestión de Cursos con ASP.NET Core ###
Por Arturo Carrasco González
Descripción General
Este es un proyecto de API robusta desarrollado con ASP.NET Core que proporciona funcionalidades integrales para la gestión de categorías, cursos y usuarios. La aplicación sigue una arquitectura en capas y utiliza una variedad de tecnologías y herramientas para ofrecer una solución completa y escalable.
Características Clave
Gestión de Categorías

Permite crear, consultar, actualizar y eliminar categorías
Las categorías se utilizan para organizar y clasificar los cursos disponibles

Gestión de Cursos

Permite crear, consultar, actualizar y eliminar cursos
Cada curso está asociado a una categoría específica
Proporciona funcionalidades avanzadas de búsqueda y filtrado de cursos

Gestión de Usuarios

Permite el registro de nuevos usuarios
Soporta la autenticación de usuarios a través de un sistema de inicio de sesión
Ofrece endpoints para recuperar información de los usuarios

Autenticación y Autorización

Utiliza .NET Identity para la autenticación de usuarios y el control de acceso basado en roles
Administra los permisos y roles de los usuarios para restringir el acceso a los recursos de la API

Versionado de la API

Implementa un sistema de versionado para admitir múltiples versiones de la API de forma simultánea
Garantiza la compatibilidad hacia atrás y una migración gradual para los clientes

Caché

Aprovecha el caché en memoria para mejorar los tiempos de respuesta de la API
Reduce la carga en la base de datos
Configura perfiles de caché para controlar el comportamiento de almacenamiento en caché en diferentes endpoints

Documentación y Pruebas

Genera documentación interactiva de la API utilizando Swagger/OpenAPI
Permite a los desarrolladores explorar y probar fácilmente los endpoints disponibles

Subida de Imágenes

Admite la carga y gestión de imágenes de los cursos
Los usuarios pueden cargar imágenes relacionadas con los cursos
La API se encarga de almacenarlas y asociarlas correctamente
