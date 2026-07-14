# 📊 UMT Backend & Dashboard System

A **full-stack enterprise dashboard** for monitoring engineering application usage (CATIA, NX, etc.), with **admin management, domain mapping, VDI users, and analytics**.

---

# 🚀 Overview

Modernized migration from:

.NET Framework 4.6.2 → .NET 10 (ASP.NET Core)

Includes:

- ✅ ASP.NET Core backend APIs
- ✅ React + TypeScript + Vite frontend
- ✅ MySQL database
- ✅ IIS hosting with Windows Authentication
- ✅ JSON-based high-performance data layer

---

# 🏗️ Architecture

Frontend (React)
↓
ASP.NET Core API (.NET 10)
↓
MySQL Database + JSON Generator
↓
Static JSON Files → Used by Dashboard

---

# 🧱 Tech Stack

## Backend
- ASP.NET Core (.NET 10)
- MySqlConnector
- IIS (Windows Authentication)

## Frontend
- React + TypeScript
- Vite
- TailwindCSS + ShadCN

## Database
- MySQL

---

# 🔐 Authentication

- Uses **Windows Authentication (IIS)**
- Extracts user from: DOMAIN\user → user

---

# 📂 Core Modules

## AuthController
GET /api/auth/me

## AdminController
GET /api/admin
POST /api/admin
DELETE /api/admin/{userId}

## DomainController
GET /api/domain
POST /api/domain
PUT /api/domain/{userId}
DELETE /api/domain/{userId}

## VdiController
GET /api/vdi
POST /api/vdi
PUT /api/vdi/{userId}
DELETE /api/vdi/{userId}

## ExportController
GET /api/export/mst-usage-tool
GET /api/export/application-data/{key}
GET /api/export/raw-sessions-json
GET /api/export/raw-sessions-compact

---

# 📊 RawSessionsService

Generates:
- raw-sessions.json
- raw-sessions-compact.json
- domains.json

---

# 🧠 Frontend State Management

useAdminData()

Supports:
- upsertVdiUser / removeVdiUser
- upsertDomainRecord / removeDomainRecord
- upsertAdmin / removeAdmin

---

# 📈 Dashboard Display Rules

Two independent kinds of rules shape the dashboard. **Row filters** decide which rows are
counted (graphs/KPIs only). The **product-line collapse** decides how the `ProductLine`
value is labelled and applies **everywhere**, including the CSV export and Sessions table.

## Home page charts & KPIs — row filters (display only)
The Home-page **graphs, charts, and KPI cards** count only a subset of rows. These row
filters do **NOT** affect the **Download CSV** export or the raw **Sessions** table:
- **Production only** — only rows where `IsProd = 1` are counted (non-production runs excluded)
- **Successful runs only** — only rows where `Status = 'Success'` are counted (`Failed` excluded)
- **Exclude validation** — rows where `Functionality = 'VALIDATION'` (literal value, case-insensitive) are dropped

## Product line — collapsed to two buckets (graphs, filter, Sessions table AND CSV)
The DB `ProductLine` column has six raw values (`SEALING`, `FBD`, `FTS`, `FLUIDS`,
`GENERAL`, `Not Available`). Everywhere the dashboard **shows or exports** it, they are
collapsed to two:
  - `FLUIDS` → **FLUIDS**
  - `SEALING`, `FBD`, `FTS`, `GENERAL`, `Not Available` (and any other value) → **SEALING**
  - The product-line filter dropdown lists only `FLUIDS` and `SEALING`; selecting one
    matches its whole bucket (e.g. `SEALING` includes `FBD`, `FTS`, `GENERAL`, and
    `Not Available` rows)
  - Unlike the row filters above, this **also** applies to the **Download CSV**
    `ProductLine` column and the **Sessions** table — not just the graphs.
  - The database is never modified; this is display/export labelling only.

## Row-level exceptions (Download CSV & Sessions table)
- **Download CSV** (main raw export) — keeps every **row** (all `IsProd`, all `Status`,
  all `Functionality`); only the `ProductLine` value is collapsed as described above.
- **Sessions table** (raw session browser) — same: every row, with `ProductLine` collapsed.
- Per-chart downloads mirror the chart they belong to.

> Implemented in the frontend. Row filters: `src/lib/filtering.ts` (`filterRawSessions`,
> via the `includeNonProd` / `includeNonSuccess` / `includeValidation` options).
> Product-line collapse: `src/lib/mock-data.ts` (`canonicalProductLine`), applied
> unconditionally inside `filterRawSessions` and driving the two-value `PRODUCT_LINES`
> dropdown.

---

# ⚡ Performance

- JSON caching
- Indexed queries
- Async DB operations

---

# 📦 Deployment

1. Install .NET 10 Hosting Bundle
2. Enable Windows Auth in IIS
3. Publish:
   dotnet publish -c Release

---

# ⚠️ Notes

- Always use fullName (NOT userId) in frontend
- DB uses UserID (case-sensitive)
- Avoid window.location.reload()

---

# 👨‍💻 Author
Haardik Guha Thakurta
Vinayakan V S