# CRITICAL FIXES - Application Errors Troubleshooting

## Issue Summary

Users are getting errors when clicking:
1. Login
2. Alerts
3. Notifications  
4. Admin Dashboard

## Root Cause

The **migration has not been applied** to the database. The tables exist but the schema doesn't match the models.

---

## IMMEDIATE SOLUTION - 5 Minutes

### Step 1: Delete Database & Migrations History

**Connect to Aiven MySQL and run:**

```sql
-- Drop all tables
DROP DATABASE IF EXISTS `defaultdb`;

-- Recreate database
CREATE DATABASE `defaultdb` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE `defaultdb`;
```

### Step 2: Clean Application

```bash
cd C:\Users\TSI\Desktop\DotNetLearning\MyAPP\MyAPP

# Remove build artifacts
Remove-Item -Path bin, obj -Recurse -Force -ErrorAction SilentlyContinue

# Clean project
dotnet clean MyAPP.csproj
```

### Step 3: Rebuild & Run

```bash
# Rebuild
dotnet build MyAPP.csproj -c Debug

# Run app (this will auto-migrate)
dotnet run
```

App will:
1. Auto-apply migration `20260402120000_CompleteInitialCreate`
2. Create all tables with correct schema
3. Seed admin user and roles
4. All errors should be gone!

---

## Alternative Solution - Manual SQL Script

If automatic migration fails, run this SQL script directly on your database:

**File:** `DATABASE_INITIALIZATION.sql`

Steps:
1. Open MySQL Workbench or any SQL client
2. Connect to Aiven database
3. Open `DATABASE_INITIALIZATION.sql`
4. Execute entire script
5. Verify tables created: `SHOW TABLES;`

---

## Verification Checklist

After applying migration, verify all tables exist:

```sql
-- Check all tables
SHOW TABLES;

-- Should see (13 tables):
-- AspNetRoles
-- AspNetUsers
-- AspNetRoleClaims
-- AspNetUserClaims
-- AspNetUserLogins
-- AspNetUserRoles
-- AspNetUserTokens
-- Subscriptions
-- TrainUpdates
-- TrainScheduleDays
-- Alerts
-- Notifications
-- Payments
-- __EFMigrationsHistory

-- Verify Notifications table has Title column
DESC Notifications;
-- Should show columns: Id, UserId, Title, Message, IsRead, CreatedAt
```

---

## If Still Getting Errors

### Option A: Force Fresh Migration

```bash
# Remove all migrations except the latest
cd MyAPP

# Remove migration
dotnet ef migrations remove

# Create fresh one
dotnet ef migrations add FreshInitialCreate

# Update database
dotnet ef database update
```

### Option B: Seed Database Manually

```sql
-- Insert Admin Role
INSERT IGNORE INTO `AspNetRoles` (`Id`, `Name`, `NormalizedName`) VALUES
('admin-id', 'Admin', 'ADMIN');

-- Insert User Role
INSERT IGNORE INTO `AspNetRoles` (`Id`, `Name`, `NormalizedName`) VALUES
('user-id', 'User', 'USER');

-- Verify
SELECT * FROM `AspNetRoles`;
SELECT * FROM `Subscriptions`;
SELECT * FROM `Notifications`;
```

---

## Common Error Solutions

### Error: "Unknown column 'Title' in 'field list'"

**Fix:**
```sql
-- Add missing Title column to Notifications
ALTER TABLE `Notifications` ADD COLUMN `Title` varchar(200) CHARACTER SET utf8mb4 NOT NULL DEFAULT '' AFTER `UserId`;
```

### Error: "Table doesn't exist"

**Fix:**
1. Delete database
2. Restart app to auto-migrate
3. Verify tables created with `SHOW TABLES;`

### Error: "Login still fails after migration"

**Verify user exists:**
```sql
SELECT Id, UserName, Email, FullName, IsActive FROM `AspNetUsers` LIMIT 10;
```

**If empty, seed admin user:**
```sql
-- Insert Admin User (Password: Mahi@Jeya@123 - hashed)
INSERT IGNORE INTO `AspNetUsers` VALUES (
  'admin-user-id',
  'MahiJeya@18',
  'MAHIJEYA@18',
  'mahijeya@gmail.com',
  'MAHIJEYA@GMAIL.COM',
  1,  -- EmailConfirmed
  'AHKjsoJzqpz...',  -- PasswordHash (use actual hash from successful registration)
  'security-stamp',
  'concurrency-stamp',
  '9876543210',
  0,  -- PhoneNumberConfirmed
  0,  -- TwoFactorEnabled
  NULL,  -- LockoutEnd
  0,  -- LockoutEnabled
  0,  -- AccessFailedCount
  'Mahi Jeya',
  'Male',
  'Tamil Nadu',
  'Chennai',
  '9876543210',
  1,  -- IsVerified
  NOW(),  -- CreatedAt
  1  -- IsActive
);

-- Assign Admin Role
INSERT IGNORE INTO `AspNetUserRoles` VALUES ('admin-user-id', 'admin-id');
```

---

## Testing After Fix

### Test 1: Login

```
Username: MahiJeya@18
Password: Mahi@Jeya@123
Expected: Redirect to Admin Dashboard
```

### Test 2: Create New User

```
Register page ? Fill form ? Submit
Expected: Success ? Redirect to Login ? Can login
```

### Test 3: Alerts

```
Login ? Click Alerts ? View/Create alerts
Expected: No errors, see alerts list
```

### Test 4: Notifications

```
Login ? Click Notifications ? View notifications
Expected: See notifications list
```

### Test 5: Admin Dashboard

```
Login as Admin ? Dashboard
Expected: See stats, recent updates, pending payments
```

---

## Debug Commands

```bash
# Check migration status
dotnet ef migrations list --project ./MyAPP.csproj

# Check database schema
# Run this in MySQL:
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'defaultdb';
DESC `AspNetUsers`;
DESC `Notifications`;

# View migration history in database
SELECT * FROM `__EFMigrationsHistory`;
```

---

## If Nothing Works - Nuclear Option

```bash
# Start completely fresh
cd C:\Users\TSI\Desktop\DotNetLearning\MyAPP\MyAPP

# Delete everything
Remove-Item -Path bin, obj -Recurse -Force
Remove-Item -Path Migrations\*.cs -Exclude ApplicationDbContextModelSnapshot.cs -Force

# Create new migration
dotnet ef migrations add InitialCreateFresh

# Update database
dotnet ef database update
```

---

## Support Files

- **Database Schema:** `DATABASE_SCHEMA_COMPLETE.md`
- **SQL Script:** `DATABASE_INITIALIZATION.sql`
- **Running Guide:** `RUNNING_AND_TESTING_GUIDE.md`

Apply **Step 1-3 above** first. Everything should work after that!

