# Property Management System

Property Management System is a web-based application developed for **PRN221**, designed to support **buying, selling, and renting real estate properties**.  
The system is built using **C#** with **ASP.NET Core Razor Pages** and follows the Page-based architectural pattern.

---

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [System Roles](#system-roles)
- [Technologies Used](#technologies-used)
- [System Requirements](#system-requirements)
- [Installation Guide](#installation-guide)
- [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [Future Improvements](#future-improvements)
- [License](#license)

---

## Overview

The **Property Management System** aims to provide a centralized platform where:
- Property owners can manage and publish their properties for rent or sale
- Users can search, view, and request to rent or buy properties
- Administrators can manage users, properties, and system data

This project focuses on applying **ASP.NET Core Razor Pages**, **Entity Framework Core**, and **SQL Server** in a real-world business scenario.

---

## Features

- User registration and authentication
- Role-based authorization
- Property listing (rent / sale)
- Property search and filtering
- Property management (CRUD)
- Contract and document management
- Admin management dashboard

---

## System Roles

### Admin
- Manage users and roles
- Manage all properties
- Monitor system data

### Landlord / Seller
- Create and manage property listings
- Upload related documents
- Manage rental or sale information

### User (Tenant / Buyer)
- Browse and search properties
- View property details
- Send rental or purchase requests

---

## Technologies Used

- **Language**: C#
- **Framework**: ASP.NET Core Razor Pages (.NET 6.0+)
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Frontend**: Razor Pages, HTML, CSS, JavaScript, Bootstrap
- **Authentication**: ASP.NET Core Identity

---

## System Requirements

Before running the project, make sure your environment meets the following requirements:

- .NET SDK 6.0 or later
- SQL Server / SQL Server Express
- Visual Studio 2022  
  (with **ASP.NET and web development** workload installed)
- Modern web browser (Chrome, Edge, Firefox)

---

## Installation Guide

### Step 1: Clone the repository

```bash
git clone https://github.com/Le-Giang-3003/property-management-system-ver2.git
cd property-management-system-ver2
```

### Step 2: Configure database connection

Open `appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=PropertyManagementDB;User Id=sa;Password=12345;TrustServerCertificate=True;"
}
```

### Step 3: Apply database migrations

- At navigation bar, open **Tools** → **NuGet Package Manager** → **Package Manager Console**.
- Enter `add-migration InitialCreate`, choose default project as the project that has `AppDbContext` (DataAccessLayer).
- Then enter `update-database`.

---

## Running the Application

### Using Visual Studio

1. Open the solution file (`.sln`) in Visual Studio 2022
2. Set the Web project as the Startup Project
3. Press `F5` or click **Run**
4. The application will be available at:

```
https://localhost:7206
```

### Using .NET CLI (optional)

```bash
dotnet restore
dotnet build
dotnet run --project PropertyManagementSystem.Web
```

---

## Admin Portal Login

- **Login Page**: Navigate to `/Login` (default landing page when not authenticated).
- **Default Admin Account (first run)**  
  If there is no Admin user in the database, the application will automatically create one during startup:
  - **Email**: `admin@localhost` (or configured in `appsettings.json` → `SeedAdmin:Email`)
  - **Password**: `Admin@123` (or configured in `appsettings.json` → `SeedAdmin:Password`)
- After logging in with the Admin account, the system will redirect to the **Admin Dashboard** (`/Admin/Index`). From the navigation menu, you can access: Manage Users, Create User, Roles & Permissions, System Settings, Audit Logs.
- **Create Additional Admins**: Login with an existing Admin → navigate to **Admin/CreateUser** → select Role **Admin** and create a new account.

---

## Project Structure

```
PropertyManagementSystem
│
├── PropertyManagementSystem.Web        (Presentation Layer - Razor Pages)
│   │
│   ├── Pages
│   │   ├── Shared
│   │   │   ├── _Layout.cshtml
│   │   │   └── _ValidationScriptsPartial.cshtml
│   │   │
│   │   ├── Index.cshtml / Index.cshtml.cs
│   │   ├── Login.cshtml / Login.cshtml.cs
│   │   ├── Register.cshtml / Register.cshtml.cs
│   │   │
│   │   ├── Properties
│   │   │   ├── Index.cshtml / Index.cshtml.cs
│   │   │   ├── Details.cshtml / Details.cshtml.cs
│   │   │   ├── Create.cshtml / Create.cshtml.cs
│   │   │   ├── Edit.cshtml / Edit.cshtml.cs
│   │   │   └── Delete.cshtml / Delete.cshtml.cs
│   │   │
│   │   ├── Admin
│   │   │   ├── Index.cshtml / Index.cshtml.cs
│   │   │   ├── ManageUsers.cshtml / ManageUsers.cshtml.cs
│   │   │   ├── CreateUser.cshtml / CreateUser.cshtml.cs
│   │   │   └── Settings.cshtml / Settings.cshtml.cs
│   │   │
│   │   └── Dashboard
│   │       └── Index.cshtml / Index.cshtml.cs
│   │
│   ├── wwwroot
│   │   ├── css
│   │   ├── js
│   │   ├── images
│   │   └── lib
│   │
│   ├── Program.cs
│   ├── appsettings.json
│   └── PropertyManagementSystem.Web.csproj
│
├── PropertyManagementSystem.BLL        (Business Logic Layer)
│   │
│   ├── Services
│   │   ├── Interfaces
│   │   │   ├── IPropertyService.cs
│   │   │   ├── IUserService.cs
│   │   │   └── IDocumentService.cs
│   │   │
│   │   └── Implementations
│   │       ├── PropertyService.cs
│   │       ├── UserService.cs
│   │       └── DocumentService.cs
│   │
│   ├── DTOs
│   │   ├── PropertyDTO.cs
│   │   ├── UserDTO.cs
│   │   └── ContractDTO.cs
│   │
│   └── PropertyManagementSystem.BLL.csproj
│
├── PropertyManagementSystem.DAL        (Data Access Layer)
│   │
│   ├── Entities
│   │   ├── Property.cs
│   │   ├── User.cs
│   │   ├── Lease.cs
│   │   └── Document.cs
│   │
│   ├── DbContext
│   │   └── AppDbContext.cs
│   │
│   ├── Repositories
│   │   ├── Interfaces
│   │   │   ├── IPropertyRepository.cs
│   │   │   └── IUserRepository.cs
│   │   │
│   │   └── Implementations
│   │       ├── PropertyRepository.cs
│   │       └── UserRepository.cs
│   │
│   ├── Migrations
│   │
│   └── PropertyManagementSystem.DAL.csproj
│
├── PropertyManagementSystem.sln
└── README.md
```

---

## Key Design Principles

### 1. Razor Pages Architecture
Clear separation of concerns with:
- **PageModel** (Code-behind handling logic and data binding)
- **Razor Page** (.cshtml file for UI rendering)
- Each page is self-contained with its own route

### 2. Layered Architecture
- **Presentation Layer** (Razor Pages, PageModels)
- **Business Logic Layer** (Services)
- **Data Access Layer** (EF Core, DbContext)

### 3. Dependency Injection
- Services are injected into PageModel constructors
- Improves testability and maintainability
- Follows ASP.NET Core DI patterns

### 4. Page-Based Routing
- Convention-based routing: `/Properties/Create` maps to `Pages/Properties/Create.cshtml`
- Handler methods: `OnGet()`, `OnPost()`, `OnPostAsync()`
- Built-in model binding and validation

---

## Security Considerations

- Authentication using ASP.NET Core Identity
- Role-based authorization using `[Authorize]` attributes and page-level authorization
- `AntiForgeryToken` validation on all POST requests
- Secure password hashing with Identity
- Server-side validation with Data Annotations
- Protection against common attacks (SQL Injection via EF Core, XSS via Razor encoding)

---

## Future Improvements

- Online payment integration (VNPay, Momo)
- Advanced search (map-based with Google Maps API, price range filters, location-based search)
- Notification system (email via SendGrid / in-app notifications)
- Report and analytics dashboard with charts
- Responsive mobile-friendly UI
- API layer for mobile application integration
- Real-time chat between landlords and tenants (SignalR)
- Document e-signature integration

---

## Limitations

- The project is developed for educational purposes
- Not optimized for large-scale production use
- Some business flows may be simplified
- Limited testing coverage

---

## License

This project is developed for academic use in the course **PRN221**.  
All rights reserved © 2025.

---

## Author

**Group**: Group 3 - SE1815  
**Course**: PRN221 – ASP.NET Core Razor Pages  
**Institution**: FPT University

---

## Contact & Support

For questions or issues:
- Create an issue on GitHub
- Email: duyltbse182424@fpt.edu.vn

---

## Acknowledgments

- FPT University for providing the learning environment
- Course instructors for guidance and support
- Open-source community for libraries and tools used in this project
