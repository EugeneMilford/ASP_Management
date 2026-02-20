# ASP.NET Office Management System

A web-based office management system built with ASP.NET Core 6.0, built to assist with workplace operations and help with team productivity.
```
   ____  __  __ _            __  __                                                  _   
  / __ \/ _|/ _(_)          |  \/  |                                                | |  
 | |  | | |_| |_ _  ___ ___ | \  / | __ _ _ __   __ _  __ _  ___ _ __ ___   ___ _ __ | |_ 
 | |  | |  _|  _| |/ __/ _ \| |\/| |/ _` | '_ \ / _` |/ _` |/ _ \ '_ ` _ \ / _ \ '_ \| __|
 | |__| | | | | | | (_|  __/| |  | | (_| | | | | (_| | (_| |  __/ | | | | |  __/ | | | |_ 
  \____/|_| |_| |_|\___\___||_|  |_|\__,_|_| |_|\__,_|\__, |\___|_| |_| |_|\___|_| |_|\__|
                                                        __/ |                              
                                                       |___/                               

                     Office Management & Collaboration Platform
           ASP.NET Core 6.0 • 10+ Integrated Modules • Enterprise Ready
```

### Quick Start

**1. Prerequisites**

* .NET 6.0 SDK or later
* SQL Server (Express or higher)
* Visual Studio 2022 or VS Code
* Git

**2. Clone & Configure**
```
# Clone the repository
git clone https://github.com/EugeneMilford/ASP_Management.git
cd ASP_Management

# Update connection string in appsettings.json
# Edit the DefaultConnection to point to your SQL Server
```

**3. Initialize Database**
```
# Apply database migrations
dotnet ef database update

# This creates the database schema and seeds:
# - Administrator account (admin@example.com)
# - Demo accounts (demoAdmin@example.com, demoUser@example.com)
# - Initial system roles
```

**4. Run It**
```
# Build and run the application
dotnet run

# Or using Visual Studio: Press F5

# Access the application
# HTTPS: https://localhost:5001
# HTTP: http://localhost:5000
```

## Overview

* ✅ User & Profile Management — Comprehensive user profiles with skills, experience, and education tracking
* ✅ Internal Mail System — Send/receive internal emails with inbox/sent folders and spam filtering
* ✅ Real-Time Messaging — Direct user-to-user communication for quick collaboration
* ✅ Bug Tracking — Complete issue tracking with priorities, status, and assignment management
* ✅ Project Management — Create, track, and manage office projects with team collaboration
* ✅ Task Assignments — Assign tasks with start/end dates and progress monitoring
* ✅ Event Calendar — FullCalendar integration for scheduling meetings and events
* ✅ Staff Directory — Centralized staff management with role assignments
* ✅ Role-Based Access Control — Three-tier security (Admin, Demo Admin, User)

### Roles & Permissions

---

**👑 Administrator**
Full system access with complete control

* ✅ View, create, edit, and delete all profiles
* ✅ Manage all staff members and assignments
* ✅ Full access to bug tracking and projects
* ✅ Assign and modify user roles
* ✅ Permanently delete records
* ✅ System-wide configuration
* ✅ Access all mail and messages
* ✅ Create and manage all events

> Default Admin: admin@example.com

---

**🎭 Demo Administrator**
Full access with temporary, sandbox data

* ✅ View all user profiles (read-only for others)
* ✅ Edit accessible profiles
* ✅ Create temporary staff, roles, and bug tickets
* ✅ View permanent system records
* ✅ Access mail and messaging
* ✅ Manage own temporary data
* ❌ Cannot permanently delete production data
* ⚠️ All created data is automatically removed on logout

> Perfect for: Training, demos, and testing without affecting production
> Demo Login: demoAdmin@example.com

---

**👤 Regular User**
Standard user access for daily operations

* ✅ View and edit own profile only
* ✅ Create personal profile
* ✅ Send and receive internal mail
* ✅ Use messaging system
* ✅ Create bug tickets
* ✅ View assigned tasks
* ✅ View and create calendar events
* ✅ View staff directory (read-only)
* ❌ Cannot access other users' profiles
* ❌ Cannot manage staff or roles

> Demo Login: demoUser@example.com

## Technology Stack

### Frontend

| Technology | Version | Purpose |
|---|---|---|
| Bootstrap | 5.3.2 | Responsive UI framework |
| jQuery | 3.7.1 | DOM manipulation & AJAX |
| FullCalendar | 5.10.0 | Calendar & event management |
| Font Awesome | 6.0 | Icon library |

### Backend

| Technology | Version | Purpose |
|---|---|---|
| ASP.NET Core | 6.0 | Web framework (Razor Pages) |
| Entity Framework Core | 6.0.33 | ORM for database operations |
| ASP.NET Core Identity | 6.0.33 | Authentication & authorization |
| SQL Server | Latest | Primary database |
| xUnit | 2.4.2 | Unit testing framework |
| Moq | 4.20.72 | Mocking library for tests |

## Project Structure
```
ASP_Management/
├── Areas/
│   └── Identity/              # ASP.NET Identity pages
│       ├── Data/              # User models and database context
│       └── Pages/             # Login, register, account management
├── Data/
│   ├── OfficeContext.cs       # Main database context
│   └── RoleSeeder.cs          # Seeds initial roles and users
├── Models/
│   ├── Role.cs                # User role assignments
│   ├── Profile.cs             # User profile details
│   ├── Staff.cs               # Staff member management
│   ├── Mail.cs                # Internal mail system
│   ├── Message.cs             # Real-time messaging
│   ├── BugTracking.cs         # Bug ticket tracking
│   ├── Assignment.cs          # Task assignments
│   ├── Project.cs             # Project management
│   └── Event.cs               # Calendar events
├── Pages/
│   ├── Index.cshtml           # Dashboard home page
│   ├── About.cshtml           # System information
│   ├── FAQ.cshtml             # Frequently asked questions
│   ├── Calendar.cshtml        # FullCalendar integration
│   ├── UserProfiles/          # Profile CRUD operations
│   ├── UserMail/              # Internal mail system
│   ├── UserMessages/          # Messaging system
│   ├── UserRoles/             # Role management
│   ├── StaffMembers/          # Staff directory
│   ├── OfficeBugTracking/     # Bug tracking
│   ├── OfficeProjects/        # Project management
│   ├── OfficeEvents/          # Event management
│   └── Tasks/                 # Task assignments
├── wwwroot/
│   ├── css/                   # Stylesheets (AdminLTE, Bootstrap)
│   ├── js/                    # JavaScript files
│   └── img/                   # Images and assets
├── OfficeManagement.csproj    # Project configuration
├── Program.cs                 # Application entry point
└── appsettings.json           # Configuration settings
```
**Commands**

**Core Operations**
```
# Build the project
dotnet build

# Run the application
dotnet run

# Run tests
cd officemanagement.tests
dotnet test

# Watch for changes and auto-rebuild
dotnet watch run
```

**Database Management**
```
# Create a new migration
dotnet ef migrations add MigrationName

# Apply migrations to database
dotnet ef database update

# Revert to a previous migration
dotnet ef database update PreviousMigrationName

# Drop the database
dotnet ef database drop

# View migration history
dotnet ef migrations list
```

**Quick Actions**
```
# Clean build artifacts
dotnet clean

# Restore NuGet packages
dotnet restore

# Publish for deployment
dotnet publish -c Release -o ./publish

# Run in production mode
dotnet run --environment Production
```
                                                                                 
