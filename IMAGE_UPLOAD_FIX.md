# Image Upload Error Fix for Train Updates

## Problem
When saving a train update with an image, an error occurred.

## Root Causes
1. **File size limit too small** - Default ASP.NET Core limit is ~28MB
2. **Missing error handling** - No specific error messages for file upload issues
3. **No file validation** - No checks for file size, type, or format
4. **Directory permissions** - Timetables folder might not exist

## Solutions Applied

### 1. **Program.cs** - Increased File Upload Limits
```csharp
// Configure file upload size limits (30MB)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 31457280; // 30MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 31457280; // 30MB
});
```

### 2. **web.config** - IIS Configuration for Large Files
```xml
<security>
  <requestFiltering>
    <requestLimits maxAllowedContentLength="31457280" />
  </requestFiltering>
</security>
```

### 3. **AdminController.cs** - File Validation
Added validation for:
- **File size**: Maximum 5MB per image
- **File type**: Only .jpg, .jpeg, .png, .gif allowed
- **Error handling**: Try-catch blocks with descriptive error messages

**Before:**
```csharp
if (model.ImageFile != null && model.ImageFile.Length > 0)
{
    imagePath = await SaveImageAsync(model.ImageFile);
}
```

**After:**
```csharp
if (model.ImageFile != null && model.ImageFile.Length > 0)
{
    // Validate file size (max 5MB)
    if (model.ImageFile.Length > 5 * 1024 * 1024)
    {
        ModelState.AddModelError("ImageFile", "Image size cannot exceed 5MB");
        ViewBag.DayNames = _scheduleService.GetDayNames();
        return View(model);
    }

    // Validate file type
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    var extension = Path.GetExtension(model.ImageFile.FileName).ToLowerInvariant();
    if (!allowedExtensions.Contains(extension))
    {
        ModelState.AddModelError("ImageFile", "Only image files are allowed");
        ViewBag.DayNames = _scheduleService.GetDayNames();
        return View(model);
    }

    imagePath = await SaveImageAsync(model.ImageFile);
}
```

### 4. **SaveImageAsync** - Better Error Handling
```csharp
private async Task<string> SaveImageAsync(IFormFile imageFile)
{
    try
    {
        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "timetables");
        
        // Ensure directory exists
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Create unique filename
        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Save file
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }

        // Return relative path for database
        return $"/images/timetables/{uniqueFileName}";
    }
    catch (Exception ex)
    {
        throw new Exception($"Failed to save image: {ex.Message}", ex);
    }
}
```

## File Upload Limits

| Configuration | Limit |
|--------------|-------|
| **Maximum file size** | 5MB (recommended for images) |
| **Server maximum** | 30MB (for larger files if needed) |
| **Allowed formats** | .jpg, .jpeg, .png, .gif |

## Testing the Fix

1. **Restart your application** to apply the new configuration
2. Try uploading a small image (< 1MB) first
3. Try uploading a larger image (2-4MB)
4. Try uploading a file larger than 5MB - should show validation error
5. Try uploading a non-image file (.txt, .pdf) - should show validation error

## Error Messages You'll See Now

### Success:
? "Train update created successfully!"

### Validation Errors:
? "Image size cannot exceed 5MB"
? "Only image files (.jpg, .jpeg, .png, .gif) are allowed"

### System Errors:
? "Error creating train update: [specific error message]"
? "Failed to save image: [specific error message]"

## Troubleshooting

If you still get errors:

1. **Check folder permissions**:
   - Navigate to `MyAPP\wwwroot\images\`
   - Right-click > Properties > Security
   - Ensure your user has write permissions

2. **Check disk space**:
   - Ensure you have enough space on the drive

3. **Check the error message**:
   - The new error handling will show you exactly what went wrong
   - Look for the error in the red alert box at the top of the form

4. **Check IIS Application Pool** (if using IIS):
   - Restart the Application Pool
   - Ensure the identity has write permissions

## Files Modified

? `Controllers/AdminController.cs` - Added validation and error handling
? `Program.cs` - Increased file upload limits
? `web.config` - Added IIS configuration for large files

---

**Note:** After these changes, restart your application for all configurations to take effect!
