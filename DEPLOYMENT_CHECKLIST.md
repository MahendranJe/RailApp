# ? Deployment Checklist for Southzone Railway Update

## ?? Pre-Deployment Checklist

### Git Repository
- [ ] All code is committed
- [ ] Blueprint files are added (`render.yaml`, `Dockerfile`, `.dockerignore`)
- [ ] Pushed to GitHub: `https://github.com/MahendranJe/RailApp`

### Database
- [ ] Database provider chosen (Azure SQL or PostgreSQL)
- [ ] Connection string prepared
- [ ] Migrations are up to date (`dotnet ef database update`)

### Configuration
- [ ] `appsettings.json` has production settings
- [ ] Secrets removed from code (stored as environment variables)
- [ ] SMTP settings prepared (if using email)
- [ ] UPI payment settings configured (if using payments)

### Testing
- [ ] Application builds successfully (`dotnet build`)
- [ ] All features tested locally
- [ ] Image upload works
- [ ] User registration/login works
- [ ] Admin dashboard accessible

## ?? Deployment Steps

### Step 1: Push to GitHub
```bash
cd C:\Users\TSI\Desktop\DotNetLearning\MyAPP
git add render.yaml Dockerfile .dockerignore README.md RENDER_DEPLOYMENT_GUIDE.md
git commit -m "Add Render deployment configuration"
git push origin master
```

- [ ] Files committed and pushed

### Step 2: Setup Render Account
- [ ] Created account at https://render.com
- [ ] Connected GitHub account
- [ ] Verified email

### Step 3: Create Blueprint
1. [ ] Click "New +" ? "Blueprint"
2. [ ] Select repository: `MahendranJe/RailApp`
3. [ ] Branch: `master`
4. [ ] Blueprint path: `render.yaml` (or leave blank)
5. [ ] Click "Apply"

### Step 4: Configure Environment Variables
Go to Service Settings ? Environment and add:

**Required:**
- [ ] `ConnectionStrings__DefaultConnection` = [Your database connection string]

**Optional (SMTP):**
- [ ] `SmtpSettings__Host`
- [ ] `SmtpSettings__Port`
- [ ] `SmtpSettings__Username`
- [ ] `SmtpSettings__Password`
- [ ] `SmtpSettings__FromEmail`

**Optional (Payment):**
- [ ] `UpiPayment__MerchantUpiId`

### Step 5: Database Migration
Choose one option:

**Option A: Local Migration**
- [ ] Updated local `appsettings.json` with production connection string
- [ ] Run: `dotnet ef database update`

**Option B: Render Shell**
- [ ] Open Render service ? Shell
- [ ] Run: `dotnet ef database update`

### Step 6: Initial Testing
- [ ] Service deployed successfully
- [ ] Homepage loads
- [ ] Login with admin: `MahiJeya@18` / `Mahi@Jeya@123`
- [ ] Login with user: `Rila@123` / `Ril@#123`
- [ ] Create test train update
- [ ] Upload test image
- [ ] Test user registration
- [ ] Check social media links (WhatsApp, Instagram)

## ?? Post-Deployment Tasks

### Verification
- [ ] Application is accessible at Render URL
- [ ] SSL certificate is active (automatic)
- [ ] Database connection works
- [ ] File uploads work
- [ ] Email notifications work (if configured)

### Monitoring
- [ ] Check Render Dashboard metrics
- [ ] Monitor application logs
- [ ] Set up health check alerts

### Optimization
- [ ] Configure custom domain (optional)
- [ ] Set up CDN for static files (optional)
- [ ] Configure backup strategy
- [ ] Set up monitoring/analytics

### Documentation
- [ ] Update team with deployment details
- [ ] Document environment variables
- [ ] Note down admin credentials securely
- [ ] Create incident response plan

## ?? Common Issues & Solutions

### Build Fails
- [ ] Check build logs in Render Dashboard
- [ ] Verify Dockerfile syntax
- [ ] Ensure all dependencies are in `.csproj`

### Database Connection Error
- [ ] Verify connection string format
- [ ] Check database firewall settings
- [ ] Whitelist Render IPs (for Azure SQL)

### Application Crashes
- [ ] Check application logs
- [ ] Verify environment variables
- [ ] Ensure migrations are applied

### Image Upload Fails
- [ ] Check file size (max 5MB)
- [ ] Verify file format (.jpg, .jpeg, .png, .gif)
- [ ] Consider using cloud storage (Azure Blob, Cloudinary)

## ?? Production Settings Checklist

### Security
- [ ] HTTPS enabled (automatic with Render)
- [ ] Environment variables secure
- [ ] Database firewall configured
- [ ] CORS configured properly
- [ ] File upload validation in place

### Performance
- [ ] Render plan selected (Free/Starter/Standard)
- [ ] Database size appropriate
- [ ] Image optimization enabled
- [ ] Caching strategy implemented

### Monitoring
- [ ] Error logging configured
- [ ] Performance metrics tracked
- [ ] Uptime monitoring enabled
- [ ] Alerts configured

## ?? Cost Estimate

Current setup cost:
- [ ] Render Web Service: $7/month (Starter)
- [ ] Database: $5-15/month (Azure SQL Basic)
- [ ] **Total**: ~$12-22/month

Alternative (Free):
- [ ] Render Web Service: Free (with sleep)
- [ ] Render PostgreSQL: Free (1GB)
- [ ] **Total**: $0/month

## ?? Support Contacts

- **Render Support**: https://render.com/docs
- **Community**: https://community.render.com
- **GitHub Issues**: https://github.com/MahendranJe/RailApp/issues

---

## ? Blueprint Path Answer

**Blueprint Path**: `render.yaml`

**Location in Repository**: 
- Local: `C:\Users\TSI\Desktop\DotNetLearning\MyAPP/render.yaml`
- GitHub: `https://github.com/MahendranJe/RailApp/render.yaml`

**When connecting to Render**, use: `render.yaml` or leave blank (it will auto-detect)

---

**?? Checklist Complete? Deploy with confidence!**

**Last Updated**: January 2025
**App Version**: 1.0.0
**Deployment Platform**: Render.com
