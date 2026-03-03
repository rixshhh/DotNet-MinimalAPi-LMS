# 📚 Library Management System (LMS) – Minimal API (Backend Only)

A **Library Management System (LMS)** built using **.NET Minimal API**.
This project provides RESTful endpoints to manage:

* 📖 Books
* 🗂 Categories
* 👤 Users
* 👥 User Types
* 📦 Book Issuing & Returning

> ⚡ Backend-only project (No frontend included)

---

## 🚀 Tech Stack

* **.NET 6/7/8 Minimal API**
* **Entity Framework Core**
* **SQL Server**
* RESTful API Architecture

---

# 📂 Database Schema

## 1️⃣ Categories

```sql
CREATE TABLE Categories(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName VARCHAR(50) NOT NULL
);
```

Represents book categories (e.g., Fiction / Non-Fiction).

---

## 2️⃣ Books

```sql
CREATE TABLE Books(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    BookName VARCHAR(100) NOT NULL,
    Author VARCHAR(100) NOT NULL,
    Publisher VARCHAR(100) NOT NULL,
    Price decimal(6,2) NOT NULL,
    CategoryID INT,
    FOREIGN KEY (CategoryID) REFERENCES Categories(ID)
);
```

Each book belongs to a category.

---

## 3️⃣ UserTypes

```sql
CREATE TABLE UserTypes(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    TypeName VARCHAR(20) NOT NULL,
    MaxBooks INT NOT NULL
);
```

Defines membership types:

* Standard
* Premium

Each type defines maximum books allowed.

---

## 4️⃣ Users

```sql
CREATE TABLE Users(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    UserTypeID INT,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (UserTypeID) REFERENCES UserTypes(ID)
);
```

Users can be activated/deactivated.

---

## 5️⃣ BookIssued

```sql
CREATE TABLE BookIssued(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    BookID INT NOT NULL,
    UserID INT NOT NULL,
    IssueDate DATE DEFAULT GETDATE(),
    RenewDate DATE NOT NULL,
    RenewCount BIT DEFAULT 0,
    ReturnDate DATE,
    BookPrice DECIMAL(6,2) NOT NULL,
    FOREIGN KEY (BookID) REFERENCES Books(ID),
    FOREIGN KEY (UserID) REFERENCES Users(ID)
);
```

Tracks issued books, renewals, and returns.

---

# 📡 API Endpoints

Base URL:

```
https://localhost:{port}/
```

---

# 📚 Books Module

| Method | Endpoint               | Description         |
| ------ | ---------------------- | ------------------- |
| GET    | `/books`               | Get all books       |
| GET    | `/books/{id}`          | Get book by ID      |
| GET    | `/books/search/{name}` | Search book by name |
| POST   | `/books`               | Add new book        |
| PUT    | `/books/{id}`          | Update book         |
| DELETE | `/books/{id}`          | Delete book         |

### ➕ Sample Request – Add Book

```json
POST /books
{
  "bookName": "Atomic Habits",
  "author": "James Clear",
  "publisher": "Penguin",
  "price": 499.99,
  "categoryId": 1
}
```

---

# 👤 Users Module

| Method | Endpoint               | Description              |
| ------ | ---------------------- | ------------------------ |
| GET    | `/users`               | Get all users            |
| GET    | `/users/{id}`          | Get user by ID           |
| GET    | `/users/bytype/{typeId}` | Get users by type      |
| POST   | `/users`               | Create user              |
| PUT    | `/users/{id}`          | Update user              |
| PATCH  | `/users/{id}`          | Activate/Deactivate user |

### ➕ Sample Request – Create User

```json
POST /users
{
  "name": "John Doe",
  "userTypeId": 1
}
```

---

# 🗂 Category Module

| Method | Endpoint           | Description        |
| ------ | ------------------ | ------------------ |
| GET    | `/categories`      | Get all categories |
| GET    | `/categories/{id}` | Get category by ID |

---

# 📦 Issued Books Module

| Method | Endpoint                     | Description              |
| ------ | ---------------------------- | ------------------------ |
| GET    | `/bookIssued`               | Get all issued books     |
| GET    | `/bookIssued/{id}`          | Get issued record by ID  |
| GET    | `/bookIssued/search/{userName}` | Get books issued by userName |
| GET    | `/user/{userID}/bookIssued` | Get books issued by userID |
| POST   | `/bookIssued`               | Issue book               |
| PUT    | `/bookIssued/renew/{id}`    | Renew book               |

---

## 📌 Issue Book – Sample Request

```json
POST /issuedbooks/issue
{
  "bookId": 1,
  "userId": 2,
  "renewDate": "2026-04-01"
}
```

---

# 🧠 Business Logic Rules

* ✅ User must be **Active**
* ✅ User cannot exceed `MaxBooks` limit
* ✅ Book must exist
* ✅ Renew allowed only once (`RenewCount`)
* ✅ Return updates `ReturnDate`
* ✅ Cannot issue already issued book (without return)

---

# ⚙️ How to Run the Project

1. Clone repository

```bash
git clone https://github.com/rixshhh/LMS-MinimalAPI.git
```

2. Update connection string in `appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=Your_Server;Database=Your_Databse;Trusted_Connection=True;"
}
```

3. Apply migrations

```bash
dotnet ef database update
```

4. Run the project

```bash
dotnet run
```


---

# 📌 Features

* Clean Minimal API architecture
* RESTful endpoints
* Proper Foreign Key relationships
* Book issuing & renewal logic
* Active/Inactive user handling
* Membership-based borrowing limits
