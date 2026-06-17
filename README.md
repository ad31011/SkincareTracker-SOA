# 🌸 Skincare Tracker

A full-stack skincare management web application built with **ASP.NET Core 8 Web API** and **Vanilla HTML/CSS/JS**. Developed as a final project for the *Service Oriented Architecture* course at South East European University.

---

## 📋 Features

- **User Authentication** — JWT-based login/register with role-based authorization (Admin / User)
- **Product Library** — Browse, add, and manage skincare products with ingredient tagging
- **Routine Builder** — Create AM/PM routines with drag-and-drop step ordering
- **⚠️ Conflict Checker** — Automatically detects incompatible ingredient combinations (e.g. Retinol + AHA)
- **Daily Skin Logs** — Track skin condition, hydration, oiliness, and sensitivity daily
- **Progress Analytics** — Visual charts showing skin health trends over time
- **Streak Tracking** — Gamified consecutive logging streaks
- **Ingredient Guide** — Full database of active ingredients with conflict information
- **Admin Panel** — User management and content administration

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL |
| Auth | JWT Bearer Tokens |
| Frontend | HTML5 + CSS3 + Vanilla JS |
| Testing | xUnit + Moq + FluentAssertions |
| CI/CD | GitHub Actions |
| Cloud | Microsoft Azure (App Service + Azure Database for PostgreSQL) |

---

## 📁 Project Structure

```
SkincareTracker/
├── api/                          # ASP.NET Core Web API
│   ├── Controllers/              # API endpoint controllers
│   ├── Services/                 # Business logic layer
│   ├── Repositories/             # Data access layer
│   ├── Interfaces/               # Service & repository contracts
│   ├── Models/                   # Domain entities
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Data/                     # AppDbContext + migrations
│   ├── Helpers/                  # JWT helper
│   ├── Migrations/               # EF Core migrations
│   ├── appsettings.json          # Configuration
│   └── Program.cs                # App entry point & DI setup
├── tests/                        # xUnit unit tests
│   └── ServiceTests.cs           # Tests for services
├── frontend/                     # Static HTML/CSS/JS frontend
│   ├── index.html                # Login / Register
│   ├── css/style.css             # Global styles
│   ├── js/
│   │   ├── app.js               # API client, auth helpers, toasts
│   │   └── sidebar.js           # Shared sidebar renderer
│   └── pages/
│       ├── dashboard.html       # Main dashboard
│       ├── skinlogs.html        # Skin log CRUD
│       ├── routines.html        # Routine builder + conflict checker
│       ├── products.html        # Product library
│       ├── ingredients.html     # Ingredient guide
│       ├── progress.html        # Progress charts
│       └── users.html           # Admin user management
├── .github/workflows/deploy.yml # CI/CD pipeline
└── SkincareTracker.sln          # Visual Studio solution
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- Any static file server (e.g. VS Code Live Server, `npx serve`)

### 1. Clone the repository

```bash
git clone https://github.com/your-username/skincare-tracker.git
cd skincare-tracker
```

### 2. Configure the database

Edit `api/appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=skincare_tracker;Username=postgres;Password=yourpassword"
  }
}
```

Create the database in PostgreSQL:

```sql
CREATE DATABASE skincare_tracker;
```

### 3. Run the API

```bash
cd api
dotnet restore
dotnet run
```

The API starts at `http://localhost:5000`. Swagger UI is available at `http://localhost:5000/swagger`.

> **Note:** EF Core migrations run automatically on startup and seed the ingredient data.

### 4. Run the frontend

Open `frontend/index.html` in a browser, or use a live server:

```bash
cd frontend
npx serve .
```

### 5. Run unit tests

```bash
cd tests
dotnet test --verbosity normal
```

---

## 🔑 API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/auth/register` | ❌ | Register new user |
| POST | `/api/auth/login` | ❌ | Login & get JWT |
| GET | `/api/auth/me` | ✅ | Get current user |
| GET | `/api/products` | ✅ | List all products |
| POST | `/api/products` | Admin | Create product |
| PUT | `/api/products/{id}` | Admin | Update product |
| DELETE | `/api/products/{id}` | Admin | Delete product |
| GET | `/api/routines` | ✅ | Get my routines |
| POST | `/api/routines` | ✅ | Create routine |
| GET | `/api/routines/{id}/conflicts` | ✅ | Check ingredient conflicts |
| GET | `/api/skinlogs` | ✅ | Get my skin logs |
| POST | `/api/skinlogs` | ✅ | Create skin log |
| GET | `/api/skinlogs/streak` | ✅ | Get streak data |
| GET | `/api/skinlogs/progress` | ✅ | Get progress data (date range) |
| GET | `/api/ingredients` | ✅ | List all ingredients |
| GET | `/api/users` | Admin | List all users |

---

## ☁️ Azure Deployment

### Deploy API to Azure App Service

1. Create an Azure App Service (Linux, .NET 8)
2. Create an Azure Database for PostgreSQL
3. Set the connection string in App Service → Configuration → Application Settings
4. Add your `AZURE_WEBAPP_PUBLISH_PROFILE` secret to GitHub repository secrets
5. Push to `main` — GitHub Actions handles the rest automatically

---

## 🧪 Testing

Tests cover:
- `SkinLogService` — streak calculation, progress queries, CRUD authorization
- `AuthService` — registration, duplicate email, login validation
- `ProductService` — retrieval, deletion

```bash
dotnet test tests/ --verbosity normal
```

---

## 👥 Team

| Member | Responsibilities |
|--------|----------------|
| Student 1 | Backend API, Auth, Business Logic, Testing |
| Student 2 | Frontend, Routines, Progress Charts, Deployment |

---

## 📄 License

This project was created for academic purposes at South East European University.
