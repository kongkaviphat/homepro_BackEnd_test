# homepro_BackEnd_test
# 📘ItBookShop API

ASP.NET Core 8 Web API
ระบบค้นหาหนังสือจาก ITBook API และระบบ Like / Unlike หนังสือ (Toggle System)

🚀 Tech Stack

.NET 8

ASP.NET Core Web API

Entity Framework Core 8

SQLite

External API (ITBook Store API)

Postman (Testing)

🏗 Clean Architecture Overview

Project ถูกออกแบบให้แยก Layer อย่างชัดเจน

┌──────────────────────┐
│      Presentation    │  → Controllers (API Layer)
└──────────────────────┘
            ↓
┌──────────────────────┐
│      Application     │  → DTOs / Business Logic
└──────────────────────┘
            ↓
┌──────────────────────┐
│        Domain        │  → Entities (User, Book, LikedBook)
└──────────────────────┘
            ↓
┌──────────────────────┐
│     Infrastructure   │  → DbContext, Database, External API
└──────────────────────┘

📂 Project Structure
* ItBookShop
* Controllers
  * UserController.cs
  * BooksController.cs
  * AuthController.cs
* Models
  * User.cs
  * Book.cs
  * LikedBook.cs

* Data
  * AppDbContext.cs

* Program.cs

🗄 Database Design (ERD)

![📊Entity Relationship Diagram](image/ERD.png)


## 🗂 Database Tables Overview

โปรเจกต์นี้ประกอบด้วย 3 ตารางหลัก ได้แก่:

* `Users`
* `Books`
* `LikedBooks`

---

## 🔹 Users Table

เก็บข้อมูลผู้ใช้งานระบบ

| Column   | Type   | Description |
|----------|--------|------------|
| Id       | int    | Primary Key |
| Username | string | Username |
| Password | string | Password |
| Fullname | string | Full Name |


---

## 🔹 Books Table

เก็บข้อมูลหนังสือจาก API ภายนอก 

| Column   | Type   |
|----------|--------|
| Id       | int    |
| Isbn13   | string |
| Title    | string |
| Subtitle | string |
| Price    | string |
| Image    | string |
| Url      | string |

---

## 🔹 LikedBooks Table

เก็บรายการหนังสือที่ผู้ใช้กดถูกใจ (Favorite / Like)

| Column | Type | Description |
|--------|------|------------|
| Id     | int  | Primary Key |
| UserId | int  | Foreign Key → Users |
| BookId | string | ISBN13 |
| Title  | string | Book Title |
| Image  | string | Book Image URL |


---

## 🔗 Relationships

* `Users (1) —— (Many) LikedBooks`
* `Books` ใช้ `Isbn13` เป็นตัวอ้างอิงสำหรับการกด Like

---




When user sends:

{
  "userId": 1,
  "bookId": "9781617294532"
}


System will:

Check if user exists

Check if book already liked

If liked → Remove (Unlike)

If not liked → Fetch book from external API and save

🌍 External API Integration

Used API:

Search:

GET https://api.itbook.store/1.0/search/mysql


Book Detail:

GET https://api.itbook.store/1.0/books/{isbn13}


Documentation:

https://api.itbook.store/

📡 API Endpoints
🔹 Toggle Like Book
POST /api/user/like

Request Body
{
  "userId": 1,
  "bookId": "9781617294532"
}

Response (Like)
{
  "message": "Book liked",
  "bookId": "9781617294532",
  "title": "ASP.NET Core in Action",
  "image": "https://itbook.store/img/books/9781617294532.png"
}

Response (Unlike)
{
  "message": "Book unliked",
  "bookId": "9781617294532"
}

⚙️ Setup Instructions
1️⃣ Clone Project
git clone <your-repository-url>
cd ItBookShop

2️⃣ Install EF Core Tool (Version 8)
dotnet tool install --global dotnet-ef --version 8.*

3️⃣ Create Database
dotnet ef migrations add InitialCreate
dotnet ef database update

4️⃣ Run Application
dotnet run


API จะรันที่:

https://localhost:xxxx

🧪 Testing with Postman

Method: POST

URL: https://localhost:xxxx/api/user/like

Body → raw → JSON

{
  "userId": 1,
  "bookId": "9781617294532"
}
