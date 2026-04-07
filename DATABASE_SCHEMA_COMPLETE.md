# Complete Database Schema - Southzone Railway Update App

## Overview
This document details the complete database schema for the .NET 8 Razor Pages application with MySQL 8.0+.

---

## Tables

### 1. AspNetRoles
**Purpose:** Identity Framework - Role Management

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| Id | VARCHAR(255) | PK, NOT NULL | Primary Key |
| Name | VARCHAR(256) | UNIQUE | Role name (e.g., "Admin", "User") |
| NormalizedName | VARCHAR(256) | UNIQUE | Normalized role name |
| ConcurrencyStamp | LONGTEXT | | Concurrency control |

**Indexes:**
- RoleNameIndex: UNIQUE on NormalizedName

---

### 2. AspNetUsers
**Purpose:** Identity Framework - User Accounts

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| Id | VARCHAR(255) | PK, NOT NULL | Primary Key - User ID |
| UserName | VARCHAR(256) | UNIQUE | Login username |
| NormalizedUserName | VARCHAR(256) | UNIQUE | Normalized username |
| Email | VARCHAR(256) | UNIQUE | Email address |
| NormalizedEmail | VARCHAR(256) | UNIQUE | Normalized email |
| EmailConfirmed | TINYINT(1) | | Boolean flag |
| PasswordHash | LONGTEXT | | Hashed password |
| SecurityStamp | LONGTEXT | | Security token |
| ConcurrencyStamp | LONGTEXT | | Concurrency control |
| PhoneNumber | LONGTEXT | | Phone number |
| PhoneNumberConfirmed | TINYINT(1) | | Boolean flag |
| TwoFactorEnabled | TINYINT(1) | | Boolean flag |
| LockoutEnd | DATETIME(6) | | Nullable lockout date |
| LockoutEnabled | TINYINT(1) | | Allow lockout flag |
| AccessFailedCount | INT | | Failed login attempts |
| **FullName** | VARCHAR(100) | NOT NULL | **Custom: User's full name** |
| **Gender** | VARCHAR(10) | | **Custom: Gender (M/F/Other)** |
| **State** | VARCHAR(50) | | **Custom: State/Province** |
| **City** | VARCHAR(50) | | **Custom: City** |
| **MobileNumber** | VARCHAR(15) | UNIQUE | **Custom: Mobile number** |
| **IsVerified** | TINYINT(1) | | **Custom: Email verified flag** |
| **CreatedAt** | DATETIME(6) | | **Custom: Account creation date** |
| **IsActive** | TINYINT(1) | | **Custom: Active status flag** |

**Indexes:**
- UserNameIndex: UNIQUE on NormalizedUserName
- EmailIndex: UNIQUE on NormalizedEmail
- FK on MobileNumber (UNIQUE, nullable filter)
- FK on Email (UNIQUE)

**Relationships:**
- 1:N AspNetUserClaims
- 1:N AspNetUserLogins
- 1:N AspNetUserRoles
- 1:N AspNetUserTokens
- 1:N Alerts (as User)
- 1:N Alerts (as CreatedBy)
- 1:N TrainUpdates (as CreatedBy)
- 1:N Subscriptions
- 1:N Notifications
- 1:N Payments

---

### 3. AspNetRoleClaims
**Purpose:** Identity Framework - Role Claims

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| Id | INT | PK, AUTO_INCREMENT | Primary Key |
| RoleId | VARCHAR(255) | FK, NOT NULL | Foreign Key to AspNetRoles |
| ClaimType | LONGTEXT | | Claim type (e.g., "permission") |
| ClaimValue | LONGTEXT | | Claim value |

**Indexes:**
- FK_AspNetRoleClaims_RoleId

---

### 4. AspNetUserClaims
**Purpose:** Identity Framework - User Claims

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| Id | INT | PK, AUTO_INCREMENT | Primary Key |
| UserId | VARCHAR(255) | FK, NOT NULL | Foreign Key to AspNetUsers |
| ClaimType | LONGTEXT | | Claim type |
| ClaimValue | LONGTEXT | | Claim value |

**Indexes:**
- FK_AspNetUserClaims_UserId

---

### 5. AspNetUserLogins
**Purpose:** Identity Framework - External Login Providers

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| LoginProvider | VARCHAR(128) | PK, NOT NULL | Login provider (e.g., "Google") |
| ProviderKey | VARCHAR(128) | PK, NOT NULL | Provider-specific key |
| ProviderDisplayName | LONGTEXT | | Display name |
| UserId | VARCHAR(255) | FK, NOT NULL | Foreign Key to AspNetUsers |

**Composite Primary Key:** (LoginProvider, ProviderKey)

**Indexes:**
- FK_AspNetUserLogins_UserId

---

### 6. AspNetUserRoles
**Purpose:** Identity Framework - User-Role Mapping

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| UserId | VARCHAR(255) | PK, FK, NOT NULL | Foreign Key to AspNetUsers |
| RoleId | VARCHAR(255) | PK, FK, NOT NULL | Foreign Key to AspNetRoles |

**Composite Primary Key:** (UserId, RoleId)

**Indexes:**
- FK_AspNetUserRoles_RoleId
- FK_AspNetUserRoles_UserId

---

### 7. AspNetUserTokens
**Purpose:** Identity Framework - User Tokens

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| UserId | VARCHAR(255) | PK, FK, NOT NULL | Foreign Key to AspNetUsers |
| LoginProvider | VARCHAR(128) | PK, NOT NULL | Provider name |
| Name | VARCHAR(128) | PK, NOT NULL | Token name |
| Value | LONGTEXT | | Token value |

**Composite Primary Key:** (UserId, LoginProvider, Name)

**Indexes:**
- FK_AspNetUserTokens_UserId

---

### 8. Subscriptions
**Purpose:** User Subscription Plans

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| Id | INT | PK, AUTO_INCREMENT | Primary Key |
| UserId | VARCHAR(255) | FK, NOT NULL | Foreign Key to AspNetUsers |
| PlanName | VARCHAR(50) | NOT NULL | Plan type: "Free", "Premium", "Monthly" |
| Price | DECIMAL(18,2) | | Subscription cost |
| StartDate | DATETIME(6) | NOT NULL | Subscription start |
| ExpiryDate | DATETIME(6) | | Nullable - expiry date |
| IsActive | TINYINT(1) | NOT NULL | Active flag |
| CreatedAt | DATETIME(6) | NOT NULL | Record creation date |

**Indexes:**
- FK_Subscriptions_UserId

**Relationships:**
- 1:N Payments

---

### 9. TrainUpdates
**Purpose:** Train Update Announcements

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| Id | INT | PK, AUTO_INCREMENT | Primary Key |
| Title | VARCHAR(200) | NOT NULL | Update title |
| Description | VARCHAR(2000) | NOT NULL | Detailed description |
| TrainNumber | VARCHAR(20) | NOT NULL | Train number |
| FromStation | VARCHAR(100) | NOT NULL | Departure station |
| ToStation | VARCHAR(100) | NOT NULL | Destination station |
| TravelDate | DATETIME(6) | NOT NULL | Travel date (legacy field) |
| ScheduleType | VARCHAR(20) | NOT NULL | "OneTime", "Daily", "Weekly" |
| StartDate | DATETIME(6) | | Schedule start date |
| EndDate | DATETIME(6) | | Schedule end date |
| ImagePath | VARCHAR(500) | | Image file path |
| IsPremium | TINYINT(1) | | Premium content flag |
| CreatedAt | DATETIME(6) | NOT NULL | Creation timestamp |
| UpdatedAt | DATETIME(6) | | Last update timestamp |
| IsActive | TINYINT(1) | NOT NULL | Active flag |
| CreatedByUserId | VARCHAR(255) | FK | Foreign Key to AspNetUsers (Admin creator) |

**Indexes:**
- FK_TrainUpdates_CreatedByUserId

**Relationships:**
- 1:N TrainScheduleDays

---

### 10. TrainScheduleDays
**Purpose:** Weekly Schedule for TrainUpdates

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| Id | INT | PK, AUTO_INCREMENT | Primary Key |
| TrainUpdateId | INT | FK, NOT NULL | Foreign Key to TrainUpdates |
| DayOfWeek | VARCHAR(20) | NOT NULL | "Monday", "Tuesday", etc. |

**Indexes:**
- FK_TrainScheduleDays_TrainUpdateId

---

### 11. Alerts
**Purpose:** Train Availability Alerts

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| Id | INT | PK, AUTO_INCREMENT | Primary Key |
| UserId | VARCHAR(255) | FK, NULLABLE | FK to AspNetUsers (NULL = broadcast) |
| CreatedByUserId | VARCHAR(255) | FK, NULLABLE | Admin creator ID |
| TrainNumber | VARCHAR(20) | NOT NULL | Train number |
| TravelDate | DATETIME(6) | NOT NULL | Travel date |
| FromStation | VARCHAR(100) | | Departure station |
| ToStation | VARCHAR(100) | | Destination station |
| Message | VARCHAR(500) | | Alert message |
| IsNotified | TINYINT(1) | | Notification sent flag |
| IsAvailable | TINYINT(1) | | Availability flag |
| CreatedAt | DATETIME(6) | NOT NULL | Alert creation date |
| NotifiedAt | DATETIME(6) | | Notification sent time |
| IsActive | TINYINT(1) | NOT NULL | Active flag |
| IsBroadcast | TINYINT(1) | | Broadcast alert flag |

**Indexes:**
- FK_Alerts_UserId
- FK_Alerts_CreatedByUserId

---

### 12. Notifications
**Purpose:** User Notifications

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| Id | INT | PK, AUTO_INCREMENT | Primary Key |
| UserId | VARCHAR(255) | FK, NOT NULL | Foreign Key to AspNetUsers |
| Title | VARCHAR(200) | NOT NULL | Notification title |
| Message | VARCHAR(500) | NOT NULL | Notification message |
| IsRead | TINYINT(1) | NOT NULL | Read flag |
| CreatedAt | DATETIME(6) | NOT NULL | Creation timestamp |

**Indexes:**
- FK_Notifications_UserId

---

### 13. Payments
**Purpose:** Payment Records for Subscriptions

| Column | Type | Constraints | Notes |
|--------|------|-----------|-------|
| Id | INT | PK, AUTO_INCREMENT | Primary Key |
| UserId | VARCHAR(255) | FK, NOT NULL | Foreign Key to AspNetUsers |
| SubscriptionId | INT | FK, NULLABLE | Foreign Key to Subscriptions |
| Amount | DECIMAL(18,2) | NOT NULL | Payment amount |
| PlanName | VARCHAR(50) | NOT NULL | Plan name |
| PaymentDate | DATETIME(6) | NOT NULL | Payment timestamp |
| Status | VARCHAR(20) | NOT NULL | "Pending", "Completed", "Failed" |
| TransactionId | VARCHAR(100) | | Payment gateway transaction ID |
| UtrNumber | VARCHAR(50) | | UPI transfer reference number |

**Indexes:**
- FK_Payments_UserId
- FK_Payments_SubscriptionId

---

## Summary Statistics

| Entity | Count | Type |
|--------|-------|------|
| Identity Tables | 6 | Built-in (AspNetRoles, AspNetUsers, AspNetRoleClaims, AspNetUserClaims, AspNetUserLogins, AspNetUserRoles, AspNetUserTokens) |
| Custom Tables | 7 | Application-specific (TrainUpdates, TrainScheduleDays, Alerts, Subscriptions, Notifications, Payments) |
| **Total Tables** | **13** | - |

---

## Character Set & Collation

- **Default Charset:** utf8mb4
- **Collation:** utf8mb4_unicode_ci
- **Engine:** InnoDB (with cascading deletes)

---

## Constraints & Relationships

### Cascade Delete ON:
- AspNetRoleClaims ? AspNetRoles
- AspNetUserClaims ? AspNetUsers
- AspNetUserRoles ? AspNetRoles, AspNetUsers
- AspNetUserTokens ? AspNetUsers
- AspNetUserLogins ? AspNetUsers
- Alerts ? AspNetUsers (UserId only)
- Subscriptions ? AspNetUsers
- Notifications ? AspNetUsers
- Payments ? AspNetUsers
- TrainScheduleDays ? TrainUpdates
- Payments ? Subscriptions (NO ACTION)

### Restrict DELETE ON:
- TrainUpdates ? AspNetUsers (CreatedByUserId)

---

## Important Notes for MySQL 8.0+

1. **String Fields:** All STRING/VARCHAR fields use `utf8mb4` charset for full Unicode support
2. **Boolean Fields:** Use TINYINT(1) for boolean values (MySQL doesn't have native BOOLEAN)
3. **Timestamps:** Use DATETIME(6) for microsecond precision
4. **Decimal Fields:** Use DECIMAL(18,2) for currency (Amount, Price)
5. **Unique Filters:** Email and MobileNumber indexes handle NULL values correctly

---

## Migration Command

```bash
dotnet ef database update --project ./MyAPP/MyAPP.csproj
```

---

## Deployment Steps

1. **Backup existing database** (if upgrading)
2. **Run migration:** `dotnet ef database update`
3. **Seed initial data** (Roles and Admin user)
4. **Verify all tables created:** `SHOW TABLES;`
5. **Test application** with user registration and login

