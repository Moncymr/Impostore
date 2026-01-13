@echo off
REM Fix Build Script for Impostore Project
REM This script fixes the "net10.0" target framework error

echo ============================================
echo Impostore Build Fix Script
echo ============================================
echo.

echo Step 1: Checking .NET SDK version...
dotnet --version
echo.

echo Step 2: Listing installed SDKs...
dotnet --list-sdks
echo.

echo Step 3: Verifying project file...
findstr /C:"TargetFramework" ImpostoreGame.csproj
echo.

echo Step 4: Closing any Visual Studio instances...
echo Please close Visual Studio manually if it's open, then press any key...
pause > nul

echo Step 5: Cleaning build artifacts...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj
if exist .vs rmdir /s /q .vs
echo Build artifacts deleted.
echo.

echo Step 6: Clearing NuGet cache...
dotnet nuget locals all --clear
echo NuGet cache cleared.
echo.

echo Step 7: Restoring packages...
dotnet restore
echo.

echo Step 8: Building project...
dotnet build
echo.

echo ============================================
echo Build fix complete!
echo ============================================
echo.
echo If you still see the "net10.0" error:
echo 1. Open ImpostoreGame.csproj in Notepad
echo 2. Find the line: ^<TargetFramework^>...^</TargetFramework^>
echo 3. Ensure it says: ^<TargetFramework^>net8.0^</TargetFramework^>
echo 4. If it says net10.0, change it to net8.0 and save
echo 5. Run this script again
echo.
pause
