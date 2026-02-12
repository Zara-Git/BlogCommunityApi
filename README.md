BlogCommunityApi

```md
# BlogCommunityApi — ASP.NET Core Web API (JWT + EF Core + SQL Server)

A production-style REST API for a small blog community platform.  
Built with **ASP.NET Core**, **Entity Framework Core**, **SQL Server**, and **JWT Authentication**, with real-world authorization rules (ownership checks) and clean endpoint design.

---

## 🚀 Highlights

- ✅ **JWT Authentication** (Register/Login + Swagger Authorize)
- ✅ **Ownership-based Authorization**
  - Only the **author** can **update/delete** their posts
- ✅ **Posts** CRUD + Search + Filter by Category
- ✅ **Comments** (Create + Get by Post)
- ✅ **Categories** (Create + List)
- ✅ **Users/me** endpoints (Update/Delete current logged-in user)
- ✅ Swagger/OpenAPI ready for testing

---

## 🧱 Tech Stack

- **ASP.NET Core Web API**
- **Entity Framework Core** (Code First + Migrations)
- **SQL Server / LocalDB**
- **JWT Bearer Authentication**
- **Swagger (OpenAPI 3.0)**

---

## 📁 Project Structure

```

BlogCommunityApi/
├─ Controllers/
│  ├─ AuthController.cs
│  ├─ CategoriesController.cs
│  ├─ CommentsController.cs
│  ├─ PostsController.cs
│  ├─ TestController.cs
│  └─ UsersController.cs
├─ Data/
│  └─ AppDbContext.cs
├─ DTOs/
│  ├─ AuthDtos.cs
│  ├─ CategoryDtos.cs
│  ├─ CommentDtos.cs
│  ├─ PostDtos.cs
│  └─ UserDtos.cs
├─ Models/
│  ├─ User.cs
│  ├─ Post.cs
│  ├─ Comment.cs
│  ├─ Category.cs
│  └─ RefreshToken.cs
├─ Services/
│  ├─ JwtService.cs
│  └─ RefreshTokenService.cs
├─ Settings/
│  └─ JwtSettings.cs
├─ Migrations/
└─ appsettings.json

````

---

## ✅ Available Endpoints (Swagger)

### Auth
- `POST /api/Auth/register`
- `POST /api/Auth/login`

### Categories
- `GET /api/Categories`
- `POST /api/Categories` 🔒

### Comments
- `POST /api/Comments` 🔒
- `GET /api/Comments/by-post/{postId}` 🔒

### Posts
- `GET /api/Posts` 🔒
- `POST /api/Posts` 🔒
- `GET /api/Posts/search` 🔒
- `GET /api/Posts/by-category/{categoryId}` 🔒
- `PUT /api/Posts/{id}` 🔒 (only author)
- `DELETE /api/Posts/{id}` 🔒 (only author)

### Users (Current user)
- `PUT /api/Users/me` 🔒
- `DELETE /api/Users/me` 🔒

### Test
- `GET /api/Test` 🔒

> 🔒 = Requires Bearer Token

---

## ⚙️ Getting Started

### Prerequisites
- .NET SDK (7/8)
- SQL Server / LocalDB
- (Optional) SSMS

### 1) Clone & Restore
```bash
git clone <repo-url>
cd BlogCommunityApi
dotnet restore
````

### 2) Configure Database

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BlogCommunityDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Issuer": "BlogCommunityApi",
    "Audience": "BlogCommunityApiUsers",
    "ExpireMinutes": 120
  }
}
```

### 3) Apply Migrations

```bash
dotnet ef database update
```

### 4) Run the API

```bash
dotnet run
```

Swagger:

* `https://localhost:<port>/swagger`

---

## 🧪 How to Test in Swagger (Recommended)

### Step 1 — Register

`POST /api/Auth/register`

Example:

```json
{
  "username": "raya",
  "email": "raya@example.com",
  "password": "12345678"
}
```

### Step 2 — Login + Copy Token

`POST /api/Auth/login`

Example:

```json
{
  "username": "raya",
  "password": "12345678"
}
```

Copy the returned JWT token.

### Step 3 — Authorize (🔒)

Click **Authorize** in Swagger and paste:

```
Bearer <YOUR_TOKEN>
```

---

## 🔐 Authorization Rules (Real-world behavior)

### ✅ 401 Unauthorized

* Token missing/expired/invalid
  Example: `invalid_token` or expired token.

### ✅ 403 Forbidden

* You are authenticated, but **not allowed** (not the author).
  Example response:
* `Only the author can delete this post.`

### ✅ 404 Not Found

* Resource not found (e.g., deleting/updating a post id that doesn’t exist)

### ✅ 204 No Content

* Successful delete (Post/User)

---

## 🧾 Example Workflows

### 1) Create a Category

`POST /api/Categories`

```json
{
  "name": "Training"
}
```

### 2) Create a Post

`POST /api/Posts`

```json
{
  "title": "My first post",
  "text": "Hello community!",
  "categoryId": 1
}
```

### 3) Search Posts by Title

`GET /api/Posts/search?title=My%20first%20post`

### 4) Filter by Category

`GET /api/Posts/by-category/1`

### 5) Update Your Post

`PUT /api/Posts/{id}`

```json
{
  "title": "Updated title",
  "text": "Updated text",
  "categoryId": 1
}
```

### 6) Delete Your Post

`DELETE /api/Posts/{id}`

* Expected: `204 No Content`

### 7) Update Current User

`PUT /api/Users/me`

```json
{
  "username": "Raya",
  "email": "raya@example.com",
  "newPassword": "12345678"
}
```

### 8) Delete Current User

`DELETE /api/Users/me`

* Expected: `204 No Content`
* Login after deletion should fail with `401 Invalid username or password`

---

## 🧠 Professional Notes / Design Choices

* **/me endpoints** avoid exposing user IDs in clients and match modern APIs.
* Ownership checks prevent users from editing/deleting other users’ content.
* Uses correct HTTP semantics:

  * `201 Created` for successful POST
  * `204 No Content` for successful DELETE
  * `401/403/404` for auth/permission/resource issues

---

## 🛣️ Future Improvements

* Refresh token rotation + endpoint exposure (if needed)
* Pagination/sorting for Posts
* Global error handling middleware (ProblemDetails)
* Validation polish + consistent error responses
* Rate limiting + logging enhancements
* Docker + CI/CD pipeline

---

## 👩‍💻 Author

**Zara Rangkhoni**
Fullstack Developer (ASP.NET Core / EF Core / SQL Server / React)


