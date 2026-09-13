# ERP Console — Flowbite / Tailwind Admin UI

A full set of **Razor views** (Tailwind CSS + Flowbite via CDN — no npm/build step) for every entity in the ERP, plus the **MVC controllers** that serve them. Copy these files on top of your existing ASP.NET project.

Views and controllers were cross-checked property-by-property against your real `Models/` (including a script pass over every `Model.x` / `item.x` / `asp-for="x"`). This environment has no .NET SDK, so the project cannot be compiled here; that model pass is the main confidence check.

---

## Table of contents

1. [What’s included](#whats-included)
2. [Model alignment fixes](#model-alignment-fixes)
3. [Routes & controllers](#routes--controllers)
4. [Orders & line items](#orders--line-items)
5. [Additional fixes](#additional-fixes)
6. [SQLite setup](#sqlite-setup)
7. [Install steps](#install-steps)
8. [Design preview](#design-preview)
9. [Known caveats](#known-caveats)

---

## What’s included

| Area | Contents |
|------|----------|
| **Views** | Index / Create / Edit / Details (and Delete where relevant) for each entity |
| **MVC controllers** | One view controller per entity (API controllers left untouched) |
| **Dashboard** | Live counts + recent logs |
| **Styling** | Tailwind + Flowbite from CDN |
| **Preview** | `design_preview.html` — static mock UI, open in any browser |

**Entities covered:** Employees, Attendance, Leave Requests, Payroll, Orders, Products, Transactions, Suppliers, Logs, Settings, Users, Dashboard, Home/Login.

---

## Model alignment fixes

After the first build failed, controllers and views were rebuilt against your actual model classes.

### Supplier

| Incorrect (assumed) | Actual model |
|---------------------|--------------|
| `supplier_name` | `name_of_supplier` |
| `status`, `updated_at` | **Not present** |

**Fields used:** `supplier_id`, `name_of_supplier`, `contact_person`, `email`, `phone`, `address`, `created_at`.

### Enums (not raw ints)

| Property | Type | Values |
|----------|------|--------|
| `Transaction.status` | `TransactionStatus` | `pending`, `completed`, `failed` |
| `Transaction.payment_method` | `PaymentMethod` | `cash`, `card`, `bank_transfer` |
| `Order.status` | `OrderStatus` | `pending`, `completed`, `cancelled` (3 values only) |

Dropdowns and badges use `Enum.GetValues<T>()` and compare against enum members — no magic numbers.

`DashboardController` previously compared statuses to raw ints (`o.status < 3`, `t.status == 1`) and was missing `using Erp.Models;`. Both are fixed.

### Unchanged models

These already matched field names: `Employee`, `Attendance`, `LeaveRequest`, `Payroll`, `Product`, `Setting`, `Log`, `User`.

### String status casing

`Employee.status`, `Attendance.status`, `LeaveRequest.status` / `leave_type`, and `Payroll.status` are **string** columns. C# defaults are lowercase (`"active"`, `"pending"`, `"present"`), while form dropdowns submit capitalized labels (`"Active"`, `"Pending"`, …).

That is consistent within this UI package. If other code filters on the lowercase defaults, align casing in either the options or those queries.

---

## Routes & controllers

Most original controllers were JSON API only (`[ApiController] : ControllerBase`). Parallel **MVC** controllers were added so views have something to bind to. **Existing API controllers are not modified.**

| Route | Controller | Notes |
|-------|------------|--------|
| `/employees` | `EmployeeViewController` | |
| `/attendance` | `AttendanceViewController` | |
| `/leave-requests` | `LeaveRequestViewController` | |
| `/payroll` | `PayrollViewController` | Uses `Payroll.Employee` navigation |
| `/orders` | `OrderViewController` | Line items via JSON hidden field |
| `/products` | `ProductViewController` | |
| `/transactions` | `TransactionViewController` | Uses `Transaction.Order`, enums |
| `/suppliers` | `SupplierViewController` | |
| `/logs` | `LogViewController` | Read-only: Index / Details / Delete |
| `/settings` | `SettingViewController` | String PK (`setting_key`) |

`DashboardController` supplies:

- Counts: employees, pending leave, open orders, low-stock products, suppliers  
- This month’s revenue  
- Six most recent log entries  

---

## Orders & line items

`Order` has `List<OrderItem> order_items`. Variable-length rows are awkward with classic indexed form names, so:

1. Create/Edit uses a JS table (`Views/Orders/_OrderItemsEditor.cshtml`) with add/remove and a live total.
2. On submit, JS writes the rows as JSON into a hidden `order_items_json` field.
3. `OrderViewController` deserializes server-side and reconciles with the database:
   - Missing rows → deleted  
   - Matching IDs → updated  
   - `order_item_id == 0` → inserted  
4. **`total_amount` is always recomputed server-side** from `quantity * price` (client total is not trusted).

---

## Additional fixes

| Issue | Fix |
|-------|-----|
| `Views/Users/Create.cshtml` missing antiforgery | Added `@Html.AntiForgeryToken()` |
| `Views/Users/Edit.cshtml` missing `user_id` | Hidden field so `id != updatedUser.user_id` no longer always fails |
| Edit resetting `created_at` | `created_at` carried as a hidden field |
| Missing views | Added `Views/Users/Details.cshtml`, `Views/Home/Login.cshtml` |

---

## SQLite setup

`appsettings.json` in this bundle already uses:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=erp.db"
}
```

Three project-side changes (not applied here — needs your `Program.cs` / `.csproj`):

### 1. NuGet packages

```bash
dotnet remove package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### 2. Provider in `Program.cs`

```csharp
// before
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

// after
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
```

### 3. New migrations (provider-specific)

```bash
rm -r Migrations   # or move them aside on Windows
dotnet ef migrations add InitialSqlite
dotnet ef database update
```

This creates `erp.db` in the project folder on first run.

**Note:** SQLite does not enforce `decimal(12,2)` the way SQL Server does. Annotations on `Order.total_amount`, `Transaction.amount`, etc. still round-trip; there is simply no hard scale constraint in the DB.

---

## Install steps

1. Copy `Controllers/`, `Views/`, and `appsettings.json` into your project.  
   Overwrite `DashboardController.cs` and the folders:  
   `Views/Shared`, `Views/Home`, `Views/Dashboard`, `Views/Users`, `Views/Suppliers`, `Views/Orders`, `Views/Transactions` (and other entity view folders as needed).
2. Complete the [SQLite setup](#sqlite-setup) if you are switching providers.
3. No extra `Program.cs` wiring is required for these view controllers — they are normal `Controller` classes with `[Route]` attributes, discovered like existing MVC controllers.
4. Tailwind and Flowbite load from CDN (`cdn.tailwindcss.com`, Flowbite on cdnjs) — no `npm install`.

---

## Design preview

Open **`design_preview.html`** in a browser for a static mock of the admin UI (no backend required).

---

## Known caveats

- String status/leave-type values may differ in casing between DB defaults and form options — keep them aligned if you filter in code.
- Order line items depend on the hidden JSON field and server-side reconcile logic; do not bypass that with raw client totals.
- API controllers and view controllers coexist by design; keep route prefixes distinct to avoid clashes.
- Upload `Program.cs` and the `.csproj` if you want the SQLite / DI edits applied in-repo next.

---

## Quick start checklist

- [ ] Copy Controllers + Views + `appsettings.json`
- [ ] Switch EF provider to SQLite (if desired) and regenerate migrations
- [ ] Run the app and open `/` or `/dashboard`
- [ ] Sign in via Home/Login
- [ ] Smoke-test: Users, Suppliers, Orders (with line items), Transactions, Dashboard counts
