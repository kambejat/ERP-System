# ERP System

Enterprise Resource Planning (ERP) application for managing users, employees, HR (attendance, leave, payroll), inventory, orders, and payments.

This README documents the **SQL Server database** (`ERP_DB`) from the provided schema script and how the main modules relate.

---

## Overview

| Layer | Technology |
|-------|------------|
| Database | Microsoft SQL Server (`ERP_DB`) |
| ORM | Entity Framework Core (migrations table present) |
| Backend | ASP.NET (API + MVC admin console) |
| Admin UI | Razor views, Tailwind CSS + Flowbite (CDN) |

---

## Database

**Name:** `ERP_DB`  
**Script date:** 23 March 2025  

### Tables

| Table | Purpose | Primary key |
|-------|---------|-------------|
| `Users` | Login accounts and roles | `user_id` |
| `Employees` | Staff profiles (optional link to a user) | `employee_id` |
| `Attendances` | Daily check-in / check-out | `attendance_id` |
| `LeaveRequests` | Leave applications | `leave_id` |
| `Payrolls` | Pay runs per employee | `payroll_id` |
| `Product` | Inventory catalog | `product_id` |
| `Orders` | Sales orders | `order_id` |
| `OrderItems` | Line items on an order | `order_item_id` |
| `Transactions` | Payments against orders | `transaction_id` |
| `Settings` | Key/value app configuration | `setting_key` |
| `Logs` | Audit trail of user actions | `log_id` |
| `__EFMigrationsHistory` | EF Core migration history | `MigrationId` |

> **Note:** There is **no `Suppliers` table** in this SQL script. If the admin UI includes Suppliers, that feature needs a matching table/model or should be treated as not yet migrated.

---

## Entity reference

### Users

| Column | Type | Notes |
|--------|------|--------|
| `user_id` | int, identity | PK |
| `username` | nvarchar(50) | Required |
| `email` | nvarchar(100) | Required |
| `password_hash` | nvarchar(255) | Required |
| `role` | nvarchar(max) | Required (e.g. admin, staff) |
| `created_at` | datetime2 | Required |
| `updated_at` | datetime2 | Required |

### Employees

| Column | Type | Notes |
|--------|------|--------|
| `employee_id` | int, identity | PK |
| `user_id` | int, null | FK → `Users` (optional) |
| `first_name`, `last_name` | nvarchar(max) | Required |
| `email` | nvarchar(max) | Required |
| `phone`, `job_title`, `department` | nvarchar(max) | Optional |
| `hire_date` | datetime2 | Required |
| `salary` | decimal(18,2) | Optional |
| `status` | nvarchar(max) | Required (e.g. active) |
| `created_at`, `updated_at` | datetime2 | Required |

### Attendances

| Column | Type | Notes |
|--------|------|--------|
| `attendance_id` | int, identity | PK |
| `employee_id` | int | FK → `Employees` (cascade delete) |
| `date` | datetime2 | Required |
| `check_in`, `check_out` | datetime2 | Optional |
| `status` | nvarchar(max) | Required |
| `created_at`, `updated_at` | datetime2 | Required |

### LeaveRequests

| Column | Type | Notes |
|--------|------|--------|
| `leave_id` | int, identity | PK |
| `employee_id` | int | FK → `Employees` (cascade delete) |
| `start_date`, `end_date` | datetime2 | Required |
| `leave_type` | nvarchar(max) | Optional |
| `status` | nvarchar(max) | Required |
| `created_at`, `updated_at` | datetime2 | Required |

### Payrolls

| Column | Type | Notes |
|--------|------|--------|
| `payroll_id` | int, identity | PK |
| `employee_id` | int | FK → `Employees` (cascade delete) |
| `pay_period_start`, `pay_period_end` | datetime2 | Required |
| `gross_salary`, `tax` | decimal(18,2) | Required |
| `status` | nvarchar(max) | Required |
| `processed_at` | datetime2 | Required |

### Product

| Column | Type | Notes |
|--------|------|--------|
| `product_id` | int, identity | PK |
| `name_of_product` | nvarchar(100) | Required |
| `description` | nvarchar(max) | Optional |
| `sku` | nvarchar(50) | Optional |
| `price` | decimal(10,2) | Required |
| `quantity_in_stock` | int | Required |
| `reorder_level` | int | Required |
| `category` | nvarchar(50) | Optional |
| `created_at`, `updated_at` | datetime2 | Required |

### Orders

| Column | Type | Notes |
|--------|------|--------|
| `order_id` | int, identity | PK |
| `user_id` | int | FK → `Users` (cascade delete) |
| `order_date` | datetime2 | Required |
| `status` | **int** | Required (maps to app enum / status codes) |
| `total_amount` | decimal(12,2) | Required |
| `created_at`, `updated_at` | datetime2 | Required |

### OrderItems

| Column | Type | Notes |
|--------|------|--------|
| `order_item_id` | int, identity | PK |
| `order_id` | int | FK → `Orders` (cascade delete) |
| `product_id` | int | FK → `Product` (cascade delete) |
| `quantity` | int | Required |
| `price` | decimal(10,2) | Required |

### Transactions

| Column | Type | Notes |
|--------|------|--------|
| `transaction_id` | int, identity | PK |
| `order_id` | int | FK → `Orders` (cascade delete) |
| `transaction_date` | datetime2 | Required |
| `amount` | decimal(12,2) | Required |
| `payment_method` | **int** | Required (enum in app layer) |
| `status` | **int** | Required (enum in app layer) |

### Settings

| Column | Type | Notes |
|--------|------|--------|
| `setting_key` | nvarchar(450) | PK |
| `setting_value` | nvarchar(max) | Required |

### Logs

| Column | Type | Notes |
|--------|------|--------|
| `log_id` | int, identity | PK |
| `user_id` | int | FK → `Users` (cascade delete) |
| `action` | nvarchar(max) | Required |
| `timestamp` | datetime2 | Required |

---

## Relationships

```
Users 1───* Orders
Users 1───* Logs
Users 1───0..1 Employees   (optional user_id on Employee)

Employees 1───* Attendances
Employees 1───* LeaveRequests
Employees 1───* Payrolls

Orders 1───* OrderItems  *───1 Product
Orders 1───* Transactions
```

**Cascade delete** is enabled on:

- Attendance / Leave / Payroll → Employee  
- OrderItems / Transactions → Order  
- OrderItems → Product  
- Orders / Logs → User  

Deleting a user or order can remove a large amount of related data — use with care in production.

---

## Status & enum columns

In the **database**, these are stored as integers or free text:

| Column | DB type | Typical app mapping |
|--------|---------|---------------------|
| `Orders.status` | int | e.g. pending / completed / cancelled |
| `Transactions.status` | int | e.g. pending / completed / failed |
| `Transactions.payment_method` | int | e.g. cash / card / bank_transfer |
| `Employees.status`, `Attendances.status`, `LeaveRequests.status`, `Payrolls.status` | nvarchar | e.g. active, present, pending |

Keep C# enums (or string constants) aligned with whatever values the UI and APIs write.

---

## Indexes

Nonclustered indexes exist on foreign keys:

- `Attendances.employee_id`
- `Employees.user_id`
- `LeaveRequests.employee_id`
- `Logs.user_id`
- `OrderItems.order_id`, `OrderItems.product_id`
- `Orders.user_id`
- `Payrolls.employee_id`
- `Transactions.order_id`

---

## Modules (application)

| Module | Tables | Description |
|--------|--------|-------------|
| **Auth / Users** | `Users` | Accounts, roles, password hashes |
| **HR — Employees** | `Employees` | Staff directory |
| **HR — Attendance** | `Attendances` | Time tracking |
| **HR — Leave** | `LeaveRequests` | Leave workflow |
| **HR — Payroll** | `Payrolls` | Salary processing |
| **Inventory** | `Product` | Stock and pricing |
| **Sales** | `Orders`, `OrderItems` | Order capture and lines |
| **Payments** | `Transactions` | Payment records |
| **Config** | `Settings` | App key/value settings |
| **Audit** | `Logs` | User action log |

---

## Admin console (optional UI package)

If you use the Flowbite/Tailwind ERP Console views:

| Route (example) | Area |
|-----------------|------|
| `/dashboard` | KPIs and recent logs |
| `/users` | User management |
| `/employees` | Employees |
| `/attendance` | Attendance |
| `/leave-requests` | Leave |
| `/payroll` | Payroll |
| `/products` | Products |
| `/orders` | Orders + line-item editor |
| `/transactions` | Transactions |
| `/settings` | Settings |
| `/logs` | Logs (read-oriented) |

API controllers and MVC view controllers can coexist; keep route prefixes distinct.

---

## Setup (SQL Server)

1. Run the provided script against SQL Server (creates `ERP_DB` and all objects).  
2. Adjust file paths in the script if your instance is not under the default `MSSQL16.MSSQLSERVER` data folder.  
3. Point the app connection string at `ERP_DB`, for example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ERP_DB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

4. Ensure EF migrations match this schema, or use the database as-is with `EnsureCreated` / scaffold only if that matches your workflow.

### Optional: SQLite for local dev

You can retarget EF Core to SQLite (`Data Source=erp.db`) for lighter local work. Regenerate migrations after changing provider; SQLite will not enforce decimal precision the same way as SQL Server.

---

## Security notes

- Store only **password hashes** in `Users.password_hash` (never plain text).
- Restrict who can delete `Users` or `Orders` because of cascade deletes.
- Treat `Logs` as append-oriented; limit who can delete audit rows.
- Prefer parameterized queries / EF — do not concatenate SQL from user input.

---

## Quick reference — create order flow

1. User authenticated (`Users`).  
2. Create `Orders` row (`user_id`, `status`, dates).  
3. Insert `OrderItems` (`product_id`, `quantity`, `price`).  
4. Recompute and save `Orders.total_amount` from line items.  
5. Optionally create `Transactions` when payment is taken.  
6. Write a `Logs` entry for the action.

---

## File / script

- Database script: SQL Server create script for `ERP_DB` (tables, FKs, indexes).  
- Application: ASP.NET project with EF Core models mapping to the tables above.

For UI-specific install steps (views, controllers, CDN assets), see the separate **ERP Console** README if you shipped that package alongside the backend.
