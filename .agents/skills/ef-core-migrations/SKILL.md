---
name: ef-core-migrations
description: How to create, manage, and troubleshoot EF Core database migrations in the AddictedProxy project. Use this skill whenever asked to generate, apply, remove, or debug Entity Framework Core migrations.
---

## Project layout

| Concern                                        | Project                                                           | Notes                                                   |
| ---------------------------------------------- | ----------------------------------------------------------------- | ------------------------------------------------------- |
| DbContext (`EntityContext`)                    | `AddictedProxy.Database`                                          | Contains all entities, DbSets, and `OnModelCreating`    |
| Design-time factory                            | `AddictedProxy.Database/Context/Factory/EntityContextFactory.cs`  | Implements `IDesignTimeDbContextFactory<EntityContext>` |
| `Microsoft.EntityFrameworkCore.Design` package | `AddictedProxy.Database.csproj`                                   | Required by `dotnet-ef` tooling                         |
| Generated migrations                           | `AddictedProxy.Database/Migrations/`                              | Timestamped `.cs` + `.Designer.cs` files                |
| Model snapshot                                 | `AddictedProxy.Database/Migrations/EntityContextModelSnapshot.cs` | Auto-updated by `dotnet-ef`                             |

> **Key point:** The main web project (`AddictedProxy/`) does **not** reference
> `Microsoft.EntityFrameworkCore.Design`. The Database project owns both the
> design-time factory and the Design package, so it must be used as the
> `--startup-project`.

---

## 1 — Creating a new migration

Always run from the **repository root**:

```bash
dotnet ef migrations add <MigrationName> \
  --project AddictedProxy.Database \
  --startup-project AddictedProxy.Database
```

Both `--project` and `--startup-project` point to `AddictedProxy.Database`
because it contains:

- The `EntityContext` DbContext
- The `IDesignTimeDbContextFactory<EntityContext>` implementation
- The `Microsoft.EntityFrameworkCore.Design` NuGet package

### Naming conventions

Use PascalCase descriptive names that reflect the change:

- `AddSeasonPackEntries` — adding a new table
- `AddSeasonPackDownloadCountAndSeasonFk` — adding columns and a FK
- `AddMultiProviderTables` — adding multiple related tables

### Provenance convention for new models

When adding a new persisted model, default to inheriting from `BaseEntity`
unless there is a clear reason not to. That gives the table `CreatedAt` and
`UpdatedAt` provenance columns automatically.

If a new table should participate in timestamp provenance, make it inherit from
`BaseEntity`, ensure it is mapped in `EntityContext`, and add trigger SQL to the
EF migration that creates the table:

```sql
CREATE TRIGGER updated_at_trigger_<table>
BEFORE UPDATE ON "<Table>"
FOR EACH ROW EXECUTE FUNCTION updated_set_now();
```

The migration `Down()` should drop that trigger explicitly. The one-time
migration `EnsureUpdatedAtTriggersMigration` repairs older databases, but it is
not a replacement for adding the trigger in the schema migration itself.

---

## 2 — Removing the last migration

If the migration hasn't been applied to any database yet:

```bash
dotnet ef migrations remove \
  --project AddictedProxy.Database \
  --startup-project AddictedProxy.Database
```

This deletes the most recent migration files and reverts the model snapshot.

---

## 3 — Generating a SQL script

To preview the SQL without applying it:

```bash
dotnet ef migrations script \
  --project AddictedProxy.Database \
  --startup-project AddictedProxy.Database \
  --idempotent
```

---

## 4 — How migrations are applied at runtime

Migrations are **auto-applied on application startup** in `AddictedProxy/Program.cs`.
There is no need to run
`dotnet ef database update` manually against production — the app handles it.

For local development with Docker Compose (`compose.yaml`), the PostgreSQL
container starts alongside the app, and migrations run automatically.

---

## 5 — Typical workflow for schema changes

1. **Modify entities** in `AddictedProxy.Database/Model/` (add/change properties,
   attributes, indexes, navigation properties).
2. **Update `EntityContext`** if needed (new `DbSet<T>`, fluent configuration in
   `OnModelCreating`).
3. **Generate the migration** using the command in §1.
4. **Review** the generated `Up()` / `Down()` methods for correctness.
5. **Build the solution** to verify no compile errors:
   ```bash
   dotnet build AddictedProxy/AddictedProxy.csproj
   ```
6. **Commit** the migration files together with the entity changes.

---

## 6 — Troubleshooting

### "doesn't reference Microsoft.EntityFrameworkCore.Design"

You used `--startup-project AddictedProxy` (the web project). Switch to
`--startup-project AddictedProxy.Database`.

### Build fails during migration generation

The `dotnet-ef` tool builds the startup project before generating the migration.
If the startup project doesn't compile, the migration tool will fail with
"Build failed." Fix compile errors first, then retry.

When using `AddictedProxy.Database` as the startup project, only that project
and its dependencies need to compile — not the full web application.

### "No DbContext was found"

Ensure `EntityContextFactory` in
`AddictedProxy.Database/Context/Factory/EntityContextFactory.cs` implements
`IDesignTimeDbContextFactory<EntityContext>` and is public.

### Migration has unexpected changes

EF Core diffs the current model snapshot against your entities. If the
migration includes unrelated changes, someone may have modified entities without
generating a migration. Review carefully and split if needed.
