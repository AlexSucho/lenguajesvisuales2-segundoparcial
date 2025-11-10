# ✅ Gestor de Clientes API

API REST creada con **ASP.NET Core (.NET 8)** para la gestión de clientes.  
Permite registrar, listar, actualizar, eliminar clientes y subir fotografías asociadas.  
Documentación de endpoints disponible mediante **Swagger**.

---

## 🚀 Tecnologías utilizadas
- ASP.NET Core 8 – Web API
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- Carga de imágenes en servidor local
- Patrón REST

---

## ✅ Funcionalidades principales
✔ Registrar clientes  
✔ Listar clientes  
✔ Obtener cliente por ID  
✔ Editar y actualizar datos  
✔ Eliminar clientes  
✔ Subida de imágenes (.jpg/.png)  
✔ Respuestas JSON  
✔ Validaciones en modelos  
✔ Uso de DTOs para transferencia de datos  

---

## ⚙️ Arquitectura del proyecto

- **Controllers** → Controladores con endpoints REST  
- **Models** → Entidades del sistema  
- **Data** → DbContext y migraciones EF  
- **Middlewares** → Manejo centralizado de excepciones  
- **wwwroot/uploads** → Carpeta donde se almacenan las imágenes  
- **appsettings.json** → Configuración del proyecto  
- **Program.cs** → Configuración principal de servicios y pipeline  

---

## 🛠 Base de datos
Base de datos generada con **Entity Framework Core Code First**.  
Se puede crear automáticamente con:

```powershell
Add-Migration Init
Update-Database
