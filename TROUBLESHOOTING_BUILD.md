# 🔧 Troubleshooting Build Errors

## Error: "Assets file doesn't have a target for 'net10.0'"

If you're seeing this error when building or publishing the project, it means your local build cache has outdated references. Follow these steps to fix it:

### Solution 1: Clean Build Artifacts (Recommended)

1. **Close Visual Studio** completely
2. **Delete build artifacts** from your project folder:
   ```
   C:\ESEMPIO\impostore\bin
   C:\ESEMPIO\impostore\obj
   ```
3. **Delete Visual Studio cache**:
   ```
   C:\ESEMPIO\impostore\.vs
   ```
4. **Open Visual Studio** again
5. **Restore NuGet packages**: Right-click on the solution → Restore NuGet Packages
6. **Rebuild** the solution

### Solution 2: Command Line Clean

Open a command prompt in the project folder and run:

```bash
# Clean the project
dotnet clean

# Delete obj and bin folders
rmdir /s /q obj
rmdir /s /q bin

# Restore dependencies
dotnet restore

# Build the project
dotnet build
```

### Solution 3: Clear NuGet Cache (If above doesn't work)

```bash
# Clear all NuGet caches
dotnet nuget locals all --clear

# Restore and build
dotnet restore
dotnet build
```

### Solution 4: Check for global.json

If a `global.json` file exists in your project or parent folders, make sure it doesn't specify .NET 10:

```json
{
  "sdk": {
    "version": "8.0.100"
  }
}
```

### Verify Project Configuration

Make sure your `ImpostoreGame.csproj` shows:
```xml
<TargetFramework>net8.0</TargetFramework>
```

NOT:
```xml
<TargetFramework>net10.0</TargetFramework>
```

### Still Having Issues?

1. Check your .NET SDK version: `dotnet --version`
   - Should show 8.x.x
2. List installed SDKs: `dotnet --list-sdks`
3. Make sure .NET 8 SDK is installed: https://dotnet.microsoft.com/download/dotnet/8.0

## Common Causes

- **Cached build artifacts** in `bin/` and `obj/` folders
- **Visual Studio cache** in `.vs/` folder
- **Outdated project.assets.json** file
- **Global.json** file specifying wrong SDK version
- **NuGet cache** with corrupted packages
