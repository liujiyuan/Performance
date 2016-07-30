# HelloWorldMvc

## Environment
- ScriptSource="https://github.com/aspnet/Performance.git" 
- BasePath="test\reliability\scripts"
- BuildPath="$(buildPath)"
- ShareUser=""
- ShareKey =""
- WindowsLogPath="/mnt/logshare/testruns/$(testId)"
- LinuxLogPath="x:\testruns\$(testid)"
- EnvFile="HelloWorldMvc.md"

## Test

| Command     | Host      |Description|
|-------------|-----------|-----------|
| `.\Server-InstallPrereqs.ps1 ` | $(server) | Install Prereqs | 
| `.\Server-SetupAndRun.ps1 -TestId ($testid) -BuildPath $(buildPath) -AppName ($appName) -ServerGC $(serverGC)  > Server-SetupAndRun.log 2>&1` | $(server) | Server Setup and Run |
| `.\Client-InstallPrereqs.ps1 -TestId $(testid)` | $(client) | Install Prereqs on Client |
| `.\Client-SetupAndRun.ps1 -TestId $(testid)` | $(client) | Client Setup And Run |

