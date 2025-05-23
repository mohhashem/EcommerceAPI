# Ecommerce API (.NET 8, EF Core, Redis, Mailtrap, JWT)

This is a backend RESTful API built with **ASP.NET Core 8**, featuring:

- Authentication via JWT
- Entity Framework Core with SQL Server
- Redis-based caching
- Email confirmation with SMTP (Mailtrap)
- Full Swagger Documentation

---


📂 Project Structure
Controllers/ → Web API endpoints

Services/ → Business logic

Infrastructure/ → EF Core, Repositories, DB context

Domain/ → Entities, DTOs

Database/ → SQL schema script

## 🧾 Features

- User registration with email confirmation
- JWT authentication
- Soft deletion and recovery for products
- CRUD operations for Users, Stores, Brands, and Products
- Caching with Redis for product listings and user data

---

## 🔧 Tech Stack

- ASP.NET Core 8
- Entity Framework Core 9
- SQL Server
- Redis (StackExchange)
- Swagger (Swashbuckle)
- Mailtrap (SMTP Email Testing)
- JWT for secure auth

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- SQL Server
- Redis (running locally or via Docker)

---

## 🧱 Setting up the Database

1. Open SQL Server Management Studio.
2. Run the SQL script located at:

```bash
Database/EcommerceDB_Schema.sql

📦 Running the Application

dotnet restore
dotnet build
dotnet run --project Ecommerce.API

🔐 Authentication
Register using your email, then confirm the email using swagger (An email will be sent to my mailtrap account and will automatically mark the email as confirmed in the db to be able to login, you can see the email sent through https://mailtrap.io/ email: mohamad.accuv@gmail.com password: sFrcM!VmWcMEh9N )
to be able to use the Login endpoint to get a JWT token.
Add the token to Swagger under the Authorize button to access protected endpoints.

🧊 Redis Configuration
You can run Redis locally using Docker:

docker run -d --name redis-cache -p 6379:6379 redis

📘 Swagger Docs
https://localhost:{port}/swagger
