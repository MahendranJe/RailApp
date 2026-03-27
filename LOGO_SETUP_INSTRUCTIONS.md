# Southzone Railway Update - Logo Setup Instructions

## ?? IMPORTANT: Logo Image is Not Added Yet!

Currently, the application is using a **train icon placeholder** instead of your actual logo.

## How to Add Your Logo

### Step 1: Save Your Logo Image

1. **Download/Save** your circular train logo image (the one you shared)
2. **Rename** the file to: `logo.png`
3. **Navigate** to this folder in Windows Explorer:
   ```
   C:\Users\TSI\Desktop\DotNetLearning\MyAPP\MyAPP\wwwroot\images\
   ```
4. **Copy** the `logo.png` file into this folder

### Step 2: Update the Code to Use the Logo

Once you've added the logo file, update these files:

#### 1. Update `Views\Shared\_Layout.cshtml` (Line ~196)
**Replace:**
```html
<i class="bi bi-train-freight-front-fill" style="font-size: 1.8rem; margin-right: 10px;"></i>
```
**With:**
```html
<img src="~/images/logo.png" alt="Southzone Railway Update Logo" style="height: 40px; width: 40px; object-fit: contain; margin-right: 10px;" />
```

#### 2. Update `Views\Shared\_Layout.cshtml` (Line ~260 - User Avatar)
**Replace:**
```html
<div class="user-avatar me-2 d-flex align-items-center justify-content-center bg-primary text-white" style="font-weight: bold;">
    @currentUser?.FullName.Substring(0, 1).ToUpper()
</div>
```
**With:**
```html
<img src="~/images/logo.png" alt="Profile" class="user-avatar me-2" />
```

#### 3. Update `Views\Account\Login.cshtml` (Line ~9)
**Replace:**
```html
<i class="bi bi-train-freight-front-fill fs-1 text-primary mb-3" style="font-size: 4rem;"></i>
```
**With:**
```html
<img src="~/images/logo.png" alt="Southzone Railway Update" style="height: 80px; width: 80px; object-fit: contain; margin-bottom: 15px;" />
```

#### 4. Update `Views\Account\Register.cshtml` (Line ~12)
**Replace:**
```html
<i class="bi bi-train-freight-front-fill fs-1 text-primary mb-3" style="font-size: 3.5rem;"></i>
```
**With:**
```html
<img src="~/images/logo.png" alt="Southzone Railway Update" style="height: 70px; width: 70px; object-fit: contain; margin-bottom: 10px;" />
```

## Current Status

? **App Name:** Southzone Railway Update
? **Social Media Links:** WhatsApp + Instagram (Working)
? **Footer:** Branded with social links (Working)
? **Home Page:** Follow Us section (Working)
?? **Logo:** Using train icon placeholder (NEEDS YOUR LOGO FILE)
?? **Favicon:** Will work once logo.png is added

## Image Requirements

- **Format:** PNG (recommended for transparency)
- **Size:** 500x500 pixels (square format works best)
- **Background:** Transparent preferred
- **File Name:** Must be exactly `logo.png`

## Quick Test

After adding the logo file:
1. **Restart** your application
2. Check the **browser tab** (should show your logo as favicon)
3. Check the **navbar** (should show logo + brand name)
4. **Login** and check user dropdown (should show logo as avatar)

---

## Social Media Links (Already Working)

- **WhatsApp Channel:** https://whatsapp.com/channel/0029Va6gl6EFcow4AOH5UC3i
- **Instagram:** https://www.instagram.com/southzone_railwayupdate

## Admin Credentials

- **Username:** MahiJeya@18
- **Password:** Mahi@Jeya@123

## Normal User Credentials

- **Username:** Rila@123
- **Password:** Ril@#123

---

**Note:** The logo file must be physically present in the `wwwroot\images\` folder for it to display!
