# 🎓 ABC University Management System



[![Framework](https://img.shields.io/badge/.NET-10.0-blueviolet)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/SQL--Server-EF--Core-blue)](https://learn.microsoft.com/en-us/ef/core/)
[![Architecture](https://img.shields.io/badge/Pattern-Repository--Service-green)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/repository-pattern)

A comprehensive Academic Management solution built with **ASP.NET Core MVC**. This project demonstrates a high-scale architecture transition from core business logic to a full-stack web application, utilizing **Entity Framework Core (Code-First)** and the **Repository Pattern**.

---

## 🏗 Architecture & Data Flow

This application uses a **Multi-Layered Architecture** to ensure strict separation of concerns, making the system highly maintainable and testable.

```mermaid
flowchart TD
    User([Administrator / Faculty / Student]) -->|Interacts via| UI[ASP.NET Core MVC / Razor Views]
    
    subgraph Presentation_Layer
        UI -->|Calls| Controllers[Controllers]
    end

    subgraph Service_Layer
        Controllers -->|Business Logic| Services[Services / Interfaces]
    end

    subgraph Data_Access_Layer
        Services -->|Abstract Queries| Repos[Repositories]
        Repos -->|EF Core| DbContext[[UniversityDbContext]]
    end

    subgraph Database
        DbContext -->|T-SQL| SQLServer[(SQL Server)]
    end

    %% Models are shared across layers
    Models[Domain Models] -.-> Presentation_Layer
    Models -.-> Service_Layer
    Models -.-> Data_Access_Layer

```

---

## 📊 Database Tables & Relationships
The system is built on a relational database design with strong referential integrity. I utilized Fluent API and Data Annotations to enforce business rules at the database level.

```mermaid
erDiagram
    User ||--|| Student : "is assigned"
    Student ||--o{ Enrollment : "registers"
    Student ||--o{ Grade : "receives"
    Student ||--o{ AcademicRecord : "maintains"
    
    Course ||--o{ Enrollment : "has"
    Course ||--o{ Grade : "awarded in"
    Course ||--o{ AcademicRecord : "is recorded in"

    User {
        int UserId PK
        string Email
        string Password
        Role Role
    }

    Student {
        int StudentId PK
        int UserId FK
        string Name
        string Email
        string Department
        int EnrollmentYear
    }

    Course {
        int CourseId PK
        string CourseName
        int Credits
        string SemesterOffered
    }

    Enrollment {
        int EnrollmentId PK
        int StudentId FK
        int CourseId FK
        Enum Status "Enrolled/Dropped"
    }

    Grade {
        int GradeId PK
        int StudentId FK
        int CourseId FK
        string GradeValue "A-F"
        string Remarks
    }

```
---

## ✨ Core & Advanced Features

🔐 **Role-Based Access Control (RBAC)**: Distinct dashboards and permissions for Admins, Registrars, Faculty, and Students.

🛡️ **Robust Data Validation**: Implemented strict Regex and Data Annotations for GPA entry (A-F), Student contact details, and secure password handling.

📊 **Dynamic Academic Tracking**: Real-time calculation of student enrollments and automatic history logging in the AcademicRecord table.

🔄 **Decoupled Business Logic**: All calculations (like credits/grades) are handled in the Service Layer, keeping Controllers thin and efficient.

🚀 **Code-First Migrations**: Fully version-controlled database schema management using EF Core.

## 📂 Project Structure

```
UniversityAcademicManagementSystem/
│
├── 📂 Controllers/          # Orchestrates HTTP requests and View routing
├── 📂 Services/             # Business Logic Layer (The "Brain")
│   ├── 📂 Interfaces/       # Abstractions for Dependency Injection (e.g., IStudentService)
│   └── 📂 Implementations/  # Concrete Logic and Validations
├── 📂 Repositories/         # Data Access Logic (Encapsulating EF Core queries)
├── 📂 Models/               # Domain Entities (Student, Course, User, etc.)
├── 📂 Data/                 # DbContext and Model configuration
├── 📂 Views/                # Razor UI templates organized by User Role
├── 📂 Migrations/           # EF Core Database Schema History
├── 📂 wwwroot/              # Static assets (Bootstrap, CSS, JS)
└── Program.cs               # App entry point & DI Container configuration

```

## 🚀 Getting Started

```
Prerequisites
.NET SDK (v9.0 or v10.0)

SQL Server & SSMS

Visual Studio 2022

Installation
Clone the repository:
git clone [https://github.com/Mani-741/ABC_University_Management.git](https://github.com/Mani-741/ABC_University_Management.git)
cd ABC_University_Management

Configure Connection String: Update appsettings.json with your local SQL Server instance details.

Apply Database Migrations:

PowerShell
dotnet ef database update
Run Application:

dotnet run

```
## 🔒 Security & Best Practices:
**Repository Pattern**: Ensures the DbContext is not exposed directly to the UI, allowing for easier unit testing and database swapping.

**Dependency Injection**: All services and repositories are injected via the Program.cs container to promote loose coupling.

**DTOs & ViewModels**: Used to prevent over-posting attacks and to keep domain models secure.
