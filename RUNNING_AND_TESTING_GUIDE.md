# Running & Testing the Southzone Railway Update Application

## Quick Start Guide

### Prerequisites
- .NET 8 SDK installed
- MySQL 8.0+ running (Aiven Cloud or local)
- Visual Studio 2022 or VS Code
- Git

---

## Step 1: Clone & Setup

```bash
# Clone repository
git clone https://github.com/MahendranJe/RailApp.git
cd RailApp

# Restore NuGet packages
dotnet restore

# Build solution
dotnet build
```

---

## Step 2: Database Configuration

### Option A: Use Aiven Cloud MySQL (Recommended for Production)

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=mysql-XXXX.a.aivencloud.com;Port=13853;Database=defaultdb;User=avnadmin;Password=YOUR_PASSWORD;SslMode=Required;"
  }
}
```

### Option B: Use Local SQL Server (Development)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TrainUpdatesDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
}
```

### Option C: Use Local MySQL (Docker)

```bash
# Start MySQL in Docker
docker run --name mysql-railapp -e MYSQL_ROOT_PASSWORD=root123 -p 3306:3306 -d mysql:8.0

# Connect string
"Server=localhost;Port=3306;Database=defaultdb;User=root;Password=root123;"
```

---

## Step 3: Run Database Migrations

### Using Entity Framework CLI

```bash
# Ensure tools are installed
dotnet tool restore

# Apply migrations
dotnet ef database update --project ./MyAPP/MyAPP.csproj

# Check migration status
dotnet ef migrations list --project ./MyAPP/MyAPP.csproj
```

### Using Manual SQL Script

1. Connect to your MySQL database
2. Run: `DATABASE_INITIALIZATION.sql`
3. Verify tables were created:
   ```sql
   SHOW TABLES;
   ```

---

## Step 4: Run Application

### From Visual Studio

1. Open `MyAPP.sln`
2. Set `MyAPP` as startup project
3. Press **F5** (Debug) or **Ctrl+F5** (Run without Debug)
4. App launches at: `https://localhost:7001`

### From Command Line

```bash
cd MyAPP
dotnet run
```

---

## Step 5: Test Application

### 1. **Home Page**
   - Navigate to: `https://localhost:7001`
   - View train updates
   - Check features

### 2. **User Registration**

   **Test URL:** `https://localhost:7001/Account/Register`

   **Test User 1:**
   ```
   Username: testuser1
   Email: testuser1@gmail.com
   Password: Test@123
   Full Name: Test User One
   Phone: 9876543210
   ```

   **Test User 2:**
   ```
   Username: testuser2
   Email: testuser2@gmail.com
   Password: Test@456
   Full Name: Test User Two
   Phone: 9876543211
   ```

### 3. **User Login**

   **Test URL:** `https://localhost:7001/Account/Login`

   ```
   Username: testuser1
   Password: Test@123
   ```

   **OR use seeded admin:**
   ```
   Username: MahiJeya@18
   Password: Mahi@Jeya@123
   ```

### 4. **Admin Dashboard**

   **Test URL:** `https://localhost:7001/Admin/Dashboard` (if logged in as Admin)

   **Features to test:**
   - Create Train Update
   - Create Alert
   - View Payments
   - Manage Users
   - Manage Subscriptions

### 5. **Train Updates**

   **Test URL:** `https://localhost:7001/Home/Index`

   ```
   - Search by train number
   - Filter by date
   - Filter by stations
   - View details
   - Check premium updates
   ```

### 6. **Alerts**

   **Test URL:** `https://localhost:7001/Alerts/Index`

   ```
   - Create alert for train
   - Receive notifications
   - Mark as read
   ```

### 7. **Subscriptions**

   **Test URL:** `https://localhost:7001/Subscription/Index`

   ```
   - View subscription plans
   - Purchase subscription
   - Check payment status
   - View payment history
   ```

### 8. **Notifications**

   **Test URL:** `https://localhost:7001/Notifications/Index`

   ```
   - View all notifications
   - Mark as read
   - Automatic alerts
   ```

---

## Database Troubleshooting

### Issue: "Unknown column 'ColumnName'" Error

**Solution:**
```bash
# Delete and recreate database
dotnet ef database drop --project ./MyAPP/MyAPP.csproj
dotnet ef database update --project ./MyAPP/MyAPP.csproj
```

### Issue: Migration Conflicts

**Solution:**
```bash
# Remove last migration
dotnet ef migrations remove --project ./MyAPP/MyAPP.csproj

# Create fresh migration
dotnet ef migrations add FreshMigration --project ./MyAPP/MyAPP.csproj

# Update database
dotnet ef database update --project ./MyAPP/MyAPP.csproj
```

### Issue: Connection String Issues

**Check connection:**
```bash
# Test with MySQL CLI
mysql -h mysql-XXXX.a.aivencloud.com -P 13853 -u avnadmin -p
```

**Verify Aiven credentials in appsettings.json**

---

## Deployment to Render

### 1. Update `render.yaml`:

```yaml
services:
  - type: web
    name: southzone-railway-update
    env: docker
    region: singapore
    branch: master
    buildCommand: dotnet restore && dotnet publish -c Release -o out
    # NO startCommand for Docker
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://+:8080
      - key: ConnectionStrings__DefaultConnection
        fromDatabase:
          name: railway-db
          property: connectionString

databases:
  - name: railway-db
    databaseName: railway_updates_db
    user: railway_user
    plan: starter
    region: singapore
```

### 2. Push to GitHub

```bash
git add .
git commit -m "Deployment configuration"
git push origin master
```

### 3. Deploy via Render Dashboard

1. Go to https://render.com
2. Connect GitHub repository
3. Create web service
4. Set environment: `Singapore`
5. Deploy

---

## Performance Optimization

### Add Caching

```csharp
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Configuration.GetConnectionString("Redis");
});
```

### Database Indexes (Already in Schema)

```sql
CREATE INDEX IX_TrainUpdates_IsActive ON TrainUpdates(IsActive);
CREATE INDEX IX_Alerts_IsBroadcast ON Alerts(IsBroadcast);
CREATE INDEX IX_Subscriptions_IsActive ON Subscriptions(IsActive);
```

### Connection Pooling

Already configured in Entity Framework:
```csharp
UseMySql(connectionString, ServerVersion.Parse("8.0.23-mysql"))
```

---

## Logging & Debugging

### Enable Detailed Logging

```csharp
// In Program.cs
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);
```

### View Migration History

```sql
SELECT * FROM `__EFMigrationsHistory`;
```

### Check Table Structure

```sql
DESC `AspNetUsers`;
DESC `TrainUpdates`;
DESC `Alerts`;
```

---

## Common Test Scenarios

### Scenario 1: New User Registration + Premium Subscription

```
1. Register new user
2. Navigate to Subscription page
3. Select "Premium" plan
4. Make payment (test with UPI or test payment gateway)
5. Verify subscription activated
6. Check Notifications
```

### Scenario 2: Admin Creates Train Update

```
1. Login as Admin (MahiJeya@18 / Mahi@Jeya@123)
2. Go to Admin > Create Train Update
3. Fill in details:
   - Train Number: 12345
   - Title: Express Update
   - Route: Delhi to Mumbai
4. Set schedule (Daily, Weekly, OneTime)
5. Publish
6. Verify appears on Home page
```

### Scenario 3: User Creates Alert

```
1. Login as regular user
2. Go to Alerts > Create Alert
3. Enter train number & travel date
4. Wait for notification
5. Mark notification as read
```

---

## Database Backup & Restore

### Backup (MySQL Command)

```bash
mysqldump -h mysql-XXXX.a.aivencloud.com -P 13853 -u avnadmin -p defaultdb > backup.sql
```

### Restore

```bash
mysql -h mysql-XXXX.a.aivencloud.com -P 13853 -u avnadmin -p defaultdb < backup.sql
```

---

## Monitoring & Maintenance

### Check Database Size

```sql
SELECT 
    table_name, 
    ROUND(((data_length + index_length) / 1024 / 1024), 2) AS `Size in MB`
FROM information_schema.tables
WHERE table_schema = 'defaultdb';
```

### Clean Old Data

```sql
-- Delete old alerts (older than 90 days)
DELETE FROM Alerts 
WHERE CreatedAt < DATE_SUB(NOW(), INTERVAL 90 DAY);

-- Delete expired subscriptions
DELETE FROM Subscriptions 
WHERE ExpiryDate < NOW() AND IsActive = 0;
```

---

## Support & Documentation

- **GitHub Repo:** https://github.com/MahendranJe/RailApp
- **Issues:** Report bugs on GitHub Issues
- **Database Schema:** See `DATABASE_SCHEMA_COMPLETE.md`
- **SQL Script:** See `DATABASE_INITIALIZATION.sql`

---

## Quick Command Reference

| Task | Command |
|------|---------|
| Build | `dotnet build` |
| Run | `dotnet run --project ./MyAPP/MyAPP.csproj` |
| Migrate | `dotnet ef database update` |
| Create Migration | `dotnet ef migrations add MigrationName` |
| Drop Database | `dotnet ef database drop` |
| Update Database | `dotnet ef database update` |
| Check Migrations | `dotnet ef migrations list` |
| Docker Build | `docker build -t railapp:latest .` |
| Docker Run | `docker run -p 8080:8080 railapp:latest` |

