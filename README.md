# ?? Southzone Railway Update

A comprehensive railway update management system built with ASP.NET Core MVC (.NET 8) featuring real-time train updates, user alerts, and administrative controls.

## ?? Features

- ? **User Authentication** - Username/Email + Password login with Identity
- ? **Train Updates** - Create, view, and manage train schedule updates
- ? **Smart Scheduling** - One-time, Daily, Weekly, Custom Days, Date Range schedules
- ? **User Alerts** - Subscribe to train availability alerts
- ? **Image Upload** - Timetable images with validation
- ? **Admin Dashboard** - Complete administrative controls
- ? **Notifications** - Real-time user notifications
- ? **Mobile Responsive** - Works on all devices
- ? **Social Integration** - WhatsApp Channel & Instagram links

## ??? Technology Stack

- **Framework**: ASP.NET Core MVC 8.0
- **Database**: SQL Server / PostgreSQL (configurable)
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, Bootstrap Icons, jQuery
- **Deployment**: Docker, Render.com ready

## ?? Quick Start

### Prerequisites
- .NET 8 SDK
- SQL Server or PostgreSQL
- Visual Studio 2022 or VS Code

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/MahendranJe/RailApp.git
   cd RailApp
   ```

2. **Update connection string**
   Edit `MyAPP/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Your-SQL-Server-Connection-String"
   }
   ```

3. **Run migrations**
   ```bash
   cd MyAPP
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Open browser**
   Navigate to: `https://localhost:7098`

## ?? Default Users

### Admin Account
- **Username**: `MahiJeya@18`
- **Password**: `Mahi@Jeya@123`
- **Access**: Full admin dashboard and controls

### Normal User
- **Username**: `Rila@123`
- **Password**: `Ril@#123`
- **Access**: Standard user features

## ?? Deployment

### Deploy to Render.com

1. **Push to GitHub**
   ```bash
   git add .
   git commit -m "Ready for deployment"
   git push origin master
   ```

2. **Connect to Render**
   - Go to https://render.com/dashboard
   - Click "New +" ? "Blueprint"
   - Select repository: `MahendranJe/RailApp`
   - Blueprint path: `render.yaml`

3. **Configure Environment Variables** (in Render Dashboard)
   - `ConnectionStrings__DefaultConnection` - Your database connection string
   - Other secrets as needed

4. **Deploy!**

See [RENDER_DEPLOYMENT_GUIDE.md](RENDER_DEPLOYMENT_GUIDE.md) for detailed instructions.

## ?? Project Structure

```
MyAPP/
??? Controllers/          # MVC Controllers
??? Models/              # Data models
??? Views/               # Razor views
??? Services/            # Business logic services
??? ViewModels/          # View models
??? Data/                # Database context and seeder
??? Migrations/          # EF Core migrations
??? wwwroot/             # Static files (CSS, JS, images)
?   ??? css/
?   ??? js/
?   ??? images/
?       ??? timetables/  # Uploaded timetable images
??? appsettings.json     # Configuration
??? Program.cs           # Application entry point
```

## ?? Configuration

### App Settings
```json
{
  "AppSettings": {
    "AppName": "Southzone Railway Update",
    "WhatsAppChannel": "https://whatsapp.com/channel/...",
    "InstagramPage": "https://www.instagram.com/southzone_railwayupdate"
  }
}
```

### File Upload Limits
- Maximum file size: **5MB**
- Allowed formats: `.jpg`, `.jpeg`, `.png`, `.gif`
- Server limit: **30MB**

## ?? Social Media

- **WhatsApp Channel**: [Join Now](https://whatsapp.com/channel/0029Va6gl6EFcow4AOH5UC3i)
- **Instagram**: [@southzone_railwayupdate](https://www.instagram.com/southzone_railwayupdate)

## ?? Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## ?? License

This project is licensed under the MIT License.

## ?? Acknowledgments

- Bootstrap for UI components
- Bootstrap Icons for iconography
- ASP.NET Core team for the framework
- Entity Framework Core for ORM

## ?? Support

For support, email mahendran182000@gmail.com or join our WhatsApp channel.

---

**Blueprint Path**: `render.yaml` (located at repository root)

**Repository**: https://github.com/MahendranJe/RailApp

**Made with ?? for Southzone Railway Updates**
