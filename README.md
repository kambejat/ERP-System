# ERP Console — Flowbite/Tailwind admin UI

This adds a full set of Razor views (styled with Tailwind + Flowbite, loaded via CDN — no
npm/build step needed) for every entity in your ERP, plus the MVC controllers needed to serve
them. It's meant to be copied on top of your existing project.

**Update:** after the first build failed, you shared your real `Models/` — every view and
controller below has now been checked property-by-property against those actual classes (a
small script cross-referenced every `Model.x` / `item.x` / `asp-for="x"` in this project against
your real model files). I don't have a .NET SDK in my environment, so I can't literally compile
your project, but this pass gives much higher confidence than the first round did.

## What the failed build caught, and what I found on top of it

1. **`Supplier`** — I'd guessed its fields, since it wasn't in your SQL script. Your real model
   uses `name_of_supplier` (not `supplier_name`), and has no `status` or `updated_at` at all.
   `Views/Suppliers/*` and `SupplierViewController.cs` are rebuilt to match exactly: `supplier_id,
   name_of_supplier, contact_person, email, phone, address, created_at`.
2. **`Transaction.status` / `Transaction.payment_method`** — these are real C# enums
   (`TransactionStatus`: `pending, completed, failed`; `PaymentMethod`: `cash, card,
   bank_transfer`), not plain ints. The dropdowns and badges now use
   `Enum.GetValues<T>()` and compare against the enum values directly instead of magic numbers.
3. **`Order.status`** — same issue, and this one wasn't in your error list, but I found it while
   checking your `OrderModel.cs`: it's an `OrderStatus` enum with only **three** values
   (`pending, completed, cancelled` — not the five I'd assumed). Fixed in `Views/Orders/*`,
   and in `OrderViewController.cs` (a leftover `status = 0` default is gone — the model already
   defaults to `pending`).
4. **`DashboardController.cs`** compared `Order.status` and `Transaction.status` against raw
   ints (`o.status < 3`, `t.status == 1`) — same enum problem, plus it was missing
   `using Erp.Models;` for the enum types. Both fixed.

None of your other entities (`Employee`, `Attendance`, `LeaveRequest`, `Payroll`, `Product`,
`Setting`, `Log`, `User`) needed changes — their field names already matched.

One thing left as-is, worth knowing about: `Employee.status`, `Attendance.status`,
`LeaveRequest.status`/`leave_type`, and `Payroll.status` are free-text `string` columns whose
C# defaults are lowercase (`"active"`, `"pending"`, `"present"`), while the dropdowns in these
views save whatever's capitalized in the `<option>` list (`"Active"`, `"Pending"`, ...). That's
internally consistent — nothing in this project queries for the lowercase form — but if any of
your other code filters on the lowercase default, you'll want to line up the casing.

## Why controllers were added, not just views

Of your original controllers, only `UserController.cs`, `DashboardController.cs`, and
`HomeController.cs` returned Views. Everything else was a JSON API controller
(`[ApiController] : ControllerBase`) with nothing for new views to bind to, so a parallel MVC
controller was added per entity, following the same pattern `UsersController` already uses.
**Your existing API controllers are untouched** — both now coexist, same as `UsersController`
and `UsersApiController` already do.

| Route | Controller | Notes |
|---|---|---|
| `/employees` | `EmployeeViewController` | |
| `/attendance` | `AttendanceViewController` | |
| `/leave-requests` | `LeaveRequestViewController` | |
| `/payroll` | `PayrollViewController` | uses the confirmed `Payroll.Employee` nav property |
| `/orders` | `OrderViewController` | line items via a JSON hidden field, see below |
| `/products` | `ProductViewController` | |
| `/transactions` | `TransactionViewController` | uses `Transaction.Order`, `TransactionStatus`, `PaymentMethod` |
| `/suppliers` | `SupplierViewController` | |
| `/logs` | `LogViewController` | read-only: Index/Details/Delete only |
| `/settings` | `SettingViewController` | string primary key (`setting_key`) |

`DashboardController.cs` also feeds the dashboard real counts (employees, pending leave, open
orders, low-stock products, suppliers, this month's revenue) and the 6 most recent log entries.

## Orders — how the line-item editor works

`Order` has a `List<OrderItem> order_items` nav property. Binding a variable-length list of rows
from a form with add/remove buttons is fragile with standard indexed field names, so instead:

- The Create/Edit page renders a JS-driven table (`Views/Orders/_OrderItemsEditor.cshtml`) with
  add/remove rows and a live total.
- On submit, JS serializes the current rows to JSON into a hidden `order_items_json` field.
- `OrderViewController` deserializes that JSON server-side and reconciles it against the
  database (`Edit` diffs against the existing items — anything missing from the submission gets
  deleted, matching IDs get updated, `order_item_id == 0` gets inserted).
- `total_amount` is always recomputed server-side from `quantity * price`, never trusted from
  the client.

## Other fixes made along the way

- `Views/Users/Create.cshtml` posted without `@Html.AntiForgeryToken()` even though the
  controller requires it — fixed.
- `Views/Users/Edit.cshtml` never submitted `user_id`, so `id != updatedUser.user_id` in the
  controller would always fail — fixed with a hidden field. `created_at` is also now carried
  through as a hidden field so editing a user no longer silently resets their creation date.
- Added the two views that were missing for existing actions: `Views/Users/Details.cshtml` and
  `Views/Home/Login.cshtml` (mirrors `Home/Index`, which already serves as the sign-in page).

## Switching to SQLite

`appsettings.json` in this bundle is already updated:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=erp.db"
}
```

Three more changes I can't make for you directly, since you haven't shared `Program.cs` or the
`.csproj` — happy to do these too if you upload them, but here's exactly what to change:

**1. Swap the NuGet package** (run in the project folder):
```bash
dotnet remove package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

**2. In `Program.cs`, change the provider** — find where `ERPDbContext` is registered and change
`UseSqlServer` to `UseSqlite`:
```csharp
// before
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
// after
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
```

**3. Regenerate migrations.** EF Core migrations are provider-specific, so your existing ones
(written for SQL Server) can't be reused against SQLite:
```bash
rm -r Migrations
dotnet ef migrations add InitialSqlite
dotnet ef database update
```
This creates `erp.db` in your project folder on first run. One thing to know: SQLite doesn't
enforce `decimal` precision the way SQL Server does, so the `decimal(12,2)`-style annotations in
your models (`Order.total_amount`, `Transaction.amount`, etc.) are accepted but not strictly
enforced at the database level — values still round-trip correctly, there's just no hard column
constraint on scale.

## Setup

1. Copy `Controllers/`, `Views/`, and `appsettings.json` into your project, overwriting
   `DashboardController.cs` and the `Views/Shared`, `Views/Home`, `Views/Dashboard`,
   `Views/Users`, `Views/Suppliers`, `Views/Orders`, `Views/Transactions` folders.
2. Beyond the SQLite steps above, nothing else changes — no `Program.cs` edits are needed for
   the views/controllers themselves. They're plain `Controller` classes with `[Route]`
   attributes, discovered the same way `UsersController` already is.
3. Tailwind and Flowbite load from CDN (`cdn.tailwindcss.com`,
   `cdnjs.cloudflare.com/ajax/libs/flowbite`), so nothing to `npm install`.
4. `design_preview.html` (shared alongside this project) is a static, standalone preview with
   mock data — open it directly in a browser to see the look before wiring anything up.
