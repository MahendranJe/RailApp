# ?? Deployment Guide - Southzone Railway Update to Render.com

## ?? Prerequisites

- [ ] Git repository pushed to GitHub: `https://github.com/MahendranJe/RailApp`
- [ ] Render.com account (sign up at https://render.com)
- [ ] Azure SQL Database or another SQL Server provider (Render doesn't support SQL Server natively)

## ?? Files Created for Deployment

| File | Purpose | Location |
|------|---------|----------|
| `render.yaml` | Blueprint configuration | Root directory |
| `Dockerfile` | Docker container definition | Root directory |
| `.dockerignore` | Files to exclude from Docker | Root directory |

## ??? Database Setup Options

### Option 1: Azure SQL Database (Recommended for Production)

1. **Create Azure SQL Database:**
   - Go to https://portal.azure.com
   - Create a new SQL Database
   - Choose the cheapest tier (Basic or S0)
   - Note down the connection string

2. **Connection String Format:**
   ```
   Server=tcp:YOUR_SERVER.database.windows.net,1433;
   Initial Catalog=YOUR_DATABASE;
   Persist Security Info=False;
   User ID=YOUR_USERNAME;
   Password=YOUR_PASSWORD;
   MultipleActiveResultSets=True;
   Encrypt=True;
   TrustServerCertificate=False;
   Connection Timeout=30;
   ```

### Option 2: Migrate to PostgreSQL (Free on Render)

If you want to use Render's free PostgreSQL:

1. **Uncomment PostgreSQL section** in `render.yaml`
2. **Install PostgreSQL NuGet package:**
   ```bash
   dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
   ```
3. **Update Program.cs:**
   ```csharp
   // Replace UseSqlServer with UseNpgsql
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

## ?? Deployment Steps

### Step 1: Commit and Push Blueprint Files

```bash
# Make sure you're in the project root directory
cd C:\Users\TSI\Desktop\DotNetLearning\MyAPP

# Add the new files
git add render.yaml Dockerfile .dockerignore

# Commit
git commit -m "Add Render deployment configuration"

# Push to GitHub
git push origin master
```

### Step 2: Connect Render to GitHub

1. Go to https://render.com/dashboard
2. Click **"New +"** ? **"Blueprint"**
3. Connect your GitHub account
4. Select repository: **`MahendranJe/RailApp`**
5. Branch: **`master`**
6. Blueprint path: **`render.yaml`**
7. Click **"Apply"**

### Step 3: Configure Environment Variables

After the Blueprint is applied, go to your service settings and add these **secret** environment variables:

#### Required Database Connection:
```
ConnectionStrings__DefaultConnection = [Your Azure SQL or PostgreSQL connection string]
```

#### Optional SMTP Settings (for email notifications):
```
SmtpSettings__Host = smtp.gmail.com
SmtpSettings__Port = 587
SmtpSettings__Username = your-email@gmail.com
SmtpSettings__Password = your-app-password
SmtpSettings__FromEmail = your-email@gmail.com
```

#### Optional Payment Settings:
```
UpiPayment__MerchantUpiId = your-upi-id@bank
```

### Step 4: Apply Database Migrations

After deployment, you need to run migrations:

1. **Option A: From local machine** (if database is publicly accessible):
   ```bash
   # Update connection string in appsettings.json to point to production database
   dotnet ef database update
   ```

2. **Option B: Using Render Shell**:
   - Go to your service in Render Dashboard
   - Click **"Shell"**
   - Run: `dotnet ef database update`

### Step 5: Seed Initial Data

The application will automatically seed:
- Admin user: **MahiJeya@18** / **Mahi@Jeya@123**
- Normal user: **Rila@123** / **Ril@#123**
- Sample train updates

## ?? Render Configuration Details

### Service Configuration

| Setting | Value |
|---------|-------|
| **Name** | southzone-railway-update |
| **Environment** | Docker |
| **Region** | Singapore (change if needed) |
| **Plan** | Starter ($7/month) or Free |
| **Port** | 8080 |
| **Health Check** | / (homepage) |

### Scaling Options

| Plan | Price | Features |
|------|-------|----------|
| **Free** | $0 | 750 hours/month, sleeps after 15 min |
| **Starter** | $7/month | Always on, 512MB RAM |
| **Standard** | $25/month | 2GB RAM, faster |

## ?? Custom Domain Setup

1. Go to your service in Render Dashboard
2. Click **"Settings"** ? **"Custom Domain"**
3. Add your domain (e.g., `railwayupdate.com`)
4. Update DNS records as instructed by Render
5. Render provides **free SSL certificates**

## ?? Troubleshooting

### Build Fails
- Check build logs in Render Dashboard
- Ensure all NuGet packages are restored
- Verify Dockerfile syntax

### Database Connection Issues
- Verify connection string format
- Check firewall rules (Azure SQL requires whitelisting Render IPs)
- Test connection locally first

### Application Crashes
- Check application logs in Render Dashboard
- Verify environment variables are set correctly
- Ensure migrations are applied

### File Upload Issues
- Render filesystem is ephemeral (resets on redeploy)
- For production, use **Azure Blob Storage** or **Cloudinary** for image uploads

## ?? Monitoring

Render provides:
- **Metrics**: CPU, Memory, Request count
- **Logs**: Real-time application logs
- **Alerts**: Email notifications for downtime

## ?? Cost Estimation

### Minimum Cost Setup:
- **Render Web Service**: $7/month (Starter)
- **Azure SQL Database**: $5-15/month (Basic tier)
- **Total**: ~$12-22/month

### Free Tier Option:
- **Render Web Service**: Free (with limitations)
- **Render PostgreSQL**: Free (1GB, sleeps after inactivity)
- **Total**: $0/month

## ?? Security Checklist

- [ ] All secrets are in environment variables (not in code)
- [ ] Database firewall is configured
- [ ] SSL/HTTPS is enabled (automatic with Render)
- [ ] CORS is configured properly
- [ ] File upload size limits are set

## ?? Post-Deployment Tasks

1. **Test the application**: Visit your Render URL
2. **Login as admin**: MahiJeya@18
3. **Create test train update**
4. **Test image upload**
5. **Verify social media links** (WhatsApp, Instagram)
6. **Test user registration**
7. **Configure custom domain** (optional)

## ?? Support

If you encounter issues:

1. **Check Render Status**: https://status.render.com
2. **View Logs**: Render Dashboard ? Your Service ? Logs
3. **Render Documentation**: https://render.com/docs
4. **Community Forum**: https://community.render.com

## ?? Continuous Deployment

Every time you push to `master` branch:
1. Render automatically detects changes
2. Builds new Docker image
3. Runs migrations (if configured)
4. Deploys new version
5. Zero-downtime deployment

---

## Blueprint Path Answer

**Blueprint Path**: `render.yaml`

This file is located at the **root of your repository**:
```
C:\Users\TSI\Desktop\DotNetLearning\MyAPP/render.yaml
```

After pushing to GitHub, it will be at:
```
https://github.com/MahendranJe/RailApp/render.yaml
```

When connecting to Render, use: **`render.yaml`** (or leave blank to use default)

---

**?? Your Southzone Railway Update application is now ready for deployment!**
