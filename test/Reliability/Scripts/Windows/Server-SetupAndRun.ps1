Param(
    [string]$TestId,
    [string]$BuildPath,
    [string]$AppName,
    [string]$ServerGC
)

$BasePath = "$env:SystemDrive\$TestId";
if(!(Test-Path $BasePath))
{
    mkdir $BasePath
}

$LocalBuildPath = "$BasePath\Build";

$DotnetPath="$env:LOCALAPPDATA\Microsoft\dotnet\";
$SitePhysicalPath="$LocalBuildPath\$TestappName\bin\release\netcoreapp1.0\publish\wwwroot"
$ServerName = (Get-WmiObject win32_computersystem).DNSHostName;
$ResultSharePath = "";

function KillProcess
{
    param(
    [string]$Name
    )

    $ret = Get-Process $Name -ErrorAction SilentlyContinue;
    if($ret)
    {
        Stop-Process -processname $Name ;
    }
}

function Cleanup{
    KillProcess $AppName;
    KillProcess "w3wp" ;

    if(Test-Path "$($env:LOCALAPPDATA)\NUGET\v3-cache\")
    {
        rmdir -Recurse -Path $env:LOCALAPPDATA\NUGET\v3-cache\;
    }
    if(Test-Path "$($env:USERPROFILE)\.dnx\packages\")
    {
        rmdir -Recurse -Path $env:USERPROFILE\.dnx\packages\;
    }
    if(Test-Path "$($env:USERPROFILE)\.nuget\packages\")
    {
        rmdir -Recurse -Path $env:USERPROFILE\.nuget\packages\;
    }
    if(Test-Path "$($env:LOCALAPPDATA)\Microsoft\dotnet")
    {
        rmdir -Recurse -Path $env:LOCALAPPDATA\Microsoft\dotnet\;
    }
    if(Test-Path "$($env:ProgramW6432)\dotnet\")
    {
        rmdir -Recurse -Path $env:ProgramW6432\dotnet\;
    }
}

function CopyBinaries{

    cmd /c xcopy /s /i "$BuildPath\$AppName" "$LocalBuildPath\$AppName";
}

function CreateSharePath{
    echo "Create Result Share Path";

    $SharePath="x:";
    $date=$(((get-date).ToUniversalTime()).ToString("yyyy-MM-dd"));
    $ResultSharePath="$SharePath\Reliability\$date\ServerWin$AppName";
    if(!($ServerGC)){
    $ResultSharePath="$SharePath\Reliability\$date\WorkstationWin$AppName";
    }

    if(!(Test-Path $ResultSharePath)){
        mkdir $ResultSharePath;
    }
}

function SetPhysicalPath{
    CreateSharePath;
    cmd /c $env:windir\System32\inetsrv\appcmd.exe set vdir "aspnetcore/" -physicalPath:$SitePhysicalPath
    cmd /c $env:windir\System32\inetsrv\appcmd.exe stop site /site.name:
}

function AssignUserPermission
{
    $icaclspath = [System.Environment]::GetFolderPath('System')+"\icacls.exe";
    $nugetpath="$env:USERPROFILE\.nuget";
    Start-Process -FilePath $icaclspath -ArgumentList "$DotnetPath /grant IIS_IUSRS:(OI)(CI)F" -Wait;
    Start-Process -FilePath $icaclspath -ArgumentList "$nugetpath /grant IIS_IUSRS:(OI)(CI)F" -Wait;
    Start-Process -FilePath $icaclspath -ArgumentList "$SitePhysicalPath /grant IIS_IUSRS:(OI)(CI)F" -Wait;
}

function ChangeConfiguration{

    if($TestScenario -eq "MusicStore")
    {
        (Get-Content "$SitePhysicalPath\..\config.json").Replace(
        'Server=(localdb)\\MSSQLLocalDB;Database=MusicStore;Trusted_Connection=True;MultipleActiveResultSets=true;Connect Timeout=30;', 
        $ConnectionString) | Set-Content "$SitePhysicalPath\..\config.json";                    
    }
    
    (Get-Content "$SitePhysicalPath\web.config").Replace(
    "..\logs\stdout.log", "$ResultSharePath\kestrel.log"   ) | Set-Content "$SitePhysicalPath\web.config";  
}


Cleanup;
CopyBinaries;
SetPhysicalPath;
ChangeConfiguration;