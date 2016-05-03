Param(
    [string]$TestId=$env:TestId,
    [string]$ShareLocation=$env:ShareLocation,
    [string]$ShareUser=$env:ShareUser,
    [string]$ShareKey=$env:ShareKey
)

$BasePath = "$env:SystemDrive\$TestId";
if(!(Test-Path $BasePath))
{
    mkdir $BasePath
}

$GitPath = "$env:ProgramFiles\Git"
$DotnetPath="$env:LOCALAPPDATA\Microsoft\dotnet\";
$ServerName = (Get-WmiObject win32_computersystem).DNSHostName;

cmd \c net use $ShareLocation /u:$ShareUser $ShareKey;

function CreateAppPoolAndSite{

    echo "Create Application Pool and site";
    cmd /c $env:windir\System32\inetsrv\appcmd.exe add apppool /name:aspnetcore /managedRuntimeVersion:"" /queueLength:10000 /enableConfigurationOverride:true /processModel.identitytype:SpecificUser /processModel.userName:$ShareUser /processModel.password:$ShareKey
    cmd /c $env:windir\System32\inetsrv\appcmd.exe add site /name:aspnetcore /bindings:http/*:8080:$ServerName /applicationDefaults.applicationPool:"aspnetcore"
    cmd /c $env:windir\System32\inetsrv\appcmd.exe set config /section:system.webserver/serverRuntime /appConcurrentRequestLimit:25000;
}

function CreateUser
{
    echo "Create IIS User";
    cmd /c net user $ShareUser $ShareKey /add /y
    cmd /c net localgroup IIS_IUSRS $ShareUser /add
}


function InstallPreReqs{

    # Install Core Module
    $AspnetCoreMoudle = Get-WmiObject -Class Win32_Product | where-object{
            $_.Name -match "Core Module";
        }
    if(!($AspnetCoreMoudle))
    {
        cmd /c .\aspnetcoremodule_x64_en.msi /quiet;
    }

    #install IIS
    if(!(Get-Service -Name W3SVC -ErrorAction SilentlyContinue))
    {
        $pkgmgrpath = [System.Environment]::GetFolderPath('System')+"\pkgmgr.exe";
        Start-Process -FilePath $pkgmgrpath -ArgumentList "/l:log.etw /iu:IIS-WebServerRole;WAS-WindowsActivationService;WAS-ProcessModel;WAS-NetFxEnvironment;WAS-ConfigurationAPI;IIS-ApplicationDevelopment;IIS-ASPNET;IIS-DefaultDocument;IIS-NetFxExtensibility;IIS-ISAPIExtensions;IIS-ISAPIFilter;IIS-RequestFiltering;IIS-Metabase;IIS-WMICompatibility;IIS-LegacyScripts;IIS-IIS6ManagementCompatibility;IIS-WebServerManagementTools;IIS-HttpTracing" -Wait
    }

    CreateUser;

    CreateAppPoolAndSite;
}

InstallPreReqs;