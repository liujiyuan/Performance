# HelloWorldMvc

## Environment
<env value='
{
"scriptSource": "https://github.com/aspnet/Performance.git" 
"appDll": "HelloWorldMvc.dll"
"basepath" : "c:\$(testid)"
}
'/>

## Test

| Command     | Host      |Description|
|-------------|-----------|-----------|
| `git clone $scriptSource` | $(server) | Clone the repo | 
| `build.ps1` <config cwd="$(basepath)\Performance"/>| $(server) | Install CLI |
| `%LOCALAPPDATA%\microsoft\dotnet\dotnet.exe restore --infer-runtimes` <config cwd="$(basepath)\Performance\testapp\HelloWorldMvc"/> | $(client) | Restore packages |
| `%LOCALAPPDATA%\microsoft\dotnet\dotnet .\$(appdll)` <config cwd="$(basepath)\Performance\testapp\HelloWorldMvc\bin\release\netcoreapp1.0\publish"/> | $(client) | Run Server |

