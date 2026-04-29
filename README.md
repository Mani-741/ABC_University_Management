# 🎓 ABC University Management System



[![Framework](https://img.shields.io/badge/.NET-10.0-blueviolet)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/SQL--Server-EF--Core-blue)](https://learn.microsoft.com/en-us/ef/core/)
[![Architecture](https://img.shields.io/badge/Pattern-Repository--Service-green)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/repository-pattern)

The **University Academic Management System (UAMS)** is a digital platform built to handle all the daily activities of a university in one place. Instead of using paper forms or different disconnected systems, this application brings everything together so that students, teachers, and staff can work together easily.

In simple terms, it helps the university run smoothly by:

**Helping Students**: They can register for classes, view their grades, and check their academic history online.

**Helping Teachers**: It gives them a simple way to enter grades and manage the courses they are teaching.

**Helping Staff**: It automates the "paperwork" of managing thousands of student records, making sure the information is always accurate and up-to-date.

**Keeping Data Secure**: It ensures that everyone (like a Student or an Admin) only sees the information they are allowed to see.


---

## 🏗 Architecture & Data Flow

This application uses a **Multi-Layered Architecture** to ensure strict separation of concerns, making the system highly maintainable and testable.
```mermaid
%%{init: {'theme': 'base', 'themeVariables': { 'primaryColor': '#161b22', 'edgeLabelBackground':'#0d1117', 'tertiaryColor': '#0d1117', 'lineColor': '#58a6ff', 'mainBkg': '#0d1117'}}}%%

flowchart TD
    %% Custom Styling
    classDef userNode fill:#238636,stroke:#2ea44f,stroke-width:2px,color:#ffffff,font-weight:bold;
    classDef layerBox fill:#0d1117,stroke:#30363d,stroke-width:2px,color:#58a6ff,font-style:italic;
    classDef actionNode fill:#1f293a,stroke:#58a6ff,stroke-width:1px,color:#c9d1d9;
    classDef pulsingNode fill:#1f293a,stroke:#58a6ff,stroke-width:3px,color:#ffffff,font-weight:bold;
    classDef dbNode fill:#bd2c00,stroke:#f97316,stroke-width:2px,color:#ffffff;
    classDef persistenceNode fill:#238636,stroke:#2ea44f,stroke-width:2px,color:#ffffff;

    %% Workflow start
    User([👤 User: Admin/Faculty/Student]):::userNode -->|HTTP Request| Nginx{{Web Server / Kestrel}}:::actionNode
    
    subgraph Presentation_Layer [Presentation Layer]
        direction TB
        Nginx -->|Route Request| Ctrl[C# Controllers]:::actionNode
        Ctrl -->|Bind Data| Razor(Razor Views .cshtml):::actionNode
    end
    
    Ctrl -->|Invoke Action| DI_Cont(DI Container <br> Program.cs):::pulsingNode
    
    subgraph App_Layer [Application Layer: Business Logic]
        direction TB
        DI_Cont -->|Instantiate| SI[Service Interfaces]:::actionNode
        SI -->|Implement| SL[Service Implementations]:::pulsingNode
    end
    
    %% Business Logic talking directly to DbContext
    subgraph Data_Layer [Data Access Layer]
        SL -->|Direct EF Core Queries| DBC[[UniversityDbContext]]:::persistenceNode
        DBC -.->|Map to| MD[Domain Models / Entities]:::actionNode
    end
    
    DBC -->|Generate T-SQL| EF[EF Core ORM Layer]:::pulsingNode
    EF -->|Execute Commands| SQLServer[(🗄 SQL Server Database)]:::dbNode

    %% Styling the Subgraph Headers
    class Presentation_Layer,App_Layer,Data_Layer layerBox;
```
---

## 📊 Database Tables & Relationships
The system is built on a relational database design with strong referential integrity. I utilized Fluent API and Data Annotations to enforce business rules at the database level.

```mermaid

erDiagram
    %% Core Identity
    USER ||--|| STUDENT : "is a"
    
    %% Academic Links
    STUDENT ||--o{ ENROLLMENT : "registers"
    STUDENT ||--o{ GRADE : "earns"
    STUDENT ||--o{ ACADEMIC_RECORD : "finalizes"
    
    COURSE ||--o{ ENROLLMENT : "contains"
    COURSE ||--o{ GRADE : "assigned to"
    COURSE ||--o{ ACADEMIC_RECORD : "history of"

    USER {
        int UserId PK
        string Email
        string Role
    }

    STUDENT {
        int StudentId PK
        int UserId FK "Links to User"
        string Name
        string Department
    }

    COURSE {
        int CourseId PK
        string CourseName
        int Credits
    }

    ENROLLMENT {
        int EnrollmentId PK
        int StudentId FK
        int CourseId FK
        string Status "Enrolled/Dropped"
    }

    GRADE {
        int GradeId PK
        int StudentId FK
        int CourseId FK
        string GradeValue "A-F"
    }

    ACADEMIC_RECORD {
        int RecordId PK
        int StudentId FK
        int CourseId FK
        string Semester
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
