#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./artifacts");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./NewFrontEnd_Project.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      DotNetCoreBuild("./NewFrontEnd_Project.sln", new DotNetCoreBuildSettings
      {
        Framework = "netcoreapp3.1",
        Configuration = "Release",
        OutputDirectory = "./artifacts"
      });
});

Task("Npm CI")
    .Description("Performs a clean install of node modules")
    .Does(() => {
         if (StartProcess("powershell", "npm --prefix ./ClientApp/ ci") != 0){
            throw new Exception("Failed to build Client App");
        }
    
    });

Task("Build Client App")
    .IsDependentOn("Npm CI")
    .Does(() => {
        if (StartProcess("powershell", "npm --prefix ./ClientApp/ run build") != 0){
            throw new Exception("Failed to build Client App");
        }
        CopyDirectory("./ClientApp/build", "./artifacts/ClientApp/build");
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Build Client App");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);