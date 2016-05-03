Param(
[string]$TestScenario,
[string]$ConnectionString,
[string]$FileShareUser,
[string]$FileSharePassword
)

$DotnetInstaller = "dotnet-win-x64.1.0.0.001598.exe";
$GitPath = "$env:ProgramFiles\Git";
$RepoPath= "$env:SystemDrive\Repo";
$SivaBranchRepoPath= "$env:SystemDrive\Repo\sivag";
$DotnetPath="$env:LOCALAPPDATA\Microsoft\dotnet\cli\bin";
$date=$(((get-date).ToUniversalTime()).ToString("yyyy-MM-dd"));

if($TestScenario -eq "HelloWorldMvc")
{
    $SitePhysicalPath = "$RepoPath\Performance\testapp\HelloWorldMvc\bin\release\netstandardapp1.5\win7-x64\publish";
    $WorkDir =  "$RepoPath\Performance\testapp\HelloWorldMvc";
    $ResultSharePath="X:\Reliability\$date\WinHelloWorldMvc";
    $GitUrl = "https://github.com/aspnet/Performance.git";
    $RepoRoot = "$RepoPath\Performance";
    $ProcessName= "HelloWorldMvc";
}
else
{
    $SitePhysicalPath = "$RepoPath\musicStore\src\MusicStore\bin\release\netstandardapp1.5\win7-x64\publish\wwwroot";
    $WorkDir = "$RepoPath\musicStore\src\MusicStore";
    $ResultSharePath="X:\Reliability\$date\WinMusicStoreBasic";
    $GitUrl = "https://github.com/aspnet/MusicStore.git";
    $RepoRoot = "$RepoPath\MusicStore";
    $ProcessName= "MusicStore" ;
}


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
    KillProcess $ProcessName;
    KillProcess "w3wp" ;

    $dotnetCli = Get-WmiObject -Class Win32_Product | where-object{
        $_.Name -match "Dotnet CLI";
    }
    if($dotnetCli){
        Start-Process -FilePath .\$DotnetInstaller -ArgumentList "/uninstall /quiet" -NoNewWindow -Wait;
    }

    if(Test-Path "$RepoPath\Performance\")
    {
        Start-Process -FilePath "$GitPath\cmd\git.exe" -ArgumentList "clean -xdf" -WorkingDirectory "$RepoPath\Performance\" -NoNewWindow -Wait;
        #rmdir -Recurse -Path $RepoPath\Performance\;
        Remove-Item -Recurse -Force $RepoPath\Performance\
    }
    if(Test-Path "$RepoPath\musicStore\")
    {
        Start-Process -FilePath "$GitPath\cmd\git.exe" -ArgumentList "clean -xdf" -WorkingDirectory "$RepoPath\musicStore\" -NoNewWindow -Wait;
        #rmdir -Recurse -Path $RepoPath\musicStore\;
        Remove-Item -Recurse -Force $RepoPath\musicStore\
    }
    if(Test-Path "$SivaBranchRepoPath\Performance\")
    {
        #rmdir -Confirm:$false -Path $SivaBranchRepoPath\Performance\;
        Remove-Item -Recurse -Force $SivaBranchRepoPath\Performance
    }
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

function CreateAppPoolAndSite{

    param(
    [string]$RepoRoot
    )

    echo "Create Application Pool and site";

    $ServerName = (Get-WmiObject win32_computersystem).DNSHostName;
    cmd /c $env:windir\System32\inetsrv\appcmd.exe add apppool /name:aspnetcore /managedRuntimeVersion:"" /queueLength:10000 /enableConfigurationOverride:true /processModel.identitytype:SpecificUser /processModel.userName:$FileShareUser /processModel.password:$FileSharePassword
    cmd /c $env:windir\System32\inetsrv\appcmd.exe add site /name:aspnetcore /bindings:http/*:8080:$ServerName /physicalPath:$SitePhysicalPath /applicationDefaults.applicationPool:"aspnetcore"
}



function CreateUser
{
    echo "Create IIS User";
    #Add User 
    cmd /c net user $FileShareUser /delete
    cmd /c net user $FileShareUser $FileSharePassword /add /y
    cmd /c net localgroup IIS_IUSRS $FileShareUser /add
}

function CreateSharePath{
    echo "Create Result Share Path";

    #cmd /c net use x: /delete
    cmd /c net use x: \\fastfs.file.core.windows.net\testruns /u:$FileShareUser $FileSharePassword

    if(!(Test-Path $ResultSharePath)){
        mkdir $ResultSharePath;
    }
}

function InstallPreReqs{
    # Install dotnet   
    Start-Process -FilePath .\$DotnetInstaller -ArgumentList "/install /quiet" -NoNewWindow -Wait;

    # Install Git
    if(!(Test-Path $GitPath))
    {
         Start-Process -FilePath .\Git-2.7.3-64-bit.exe -ArgumentList "/silent /norestart" -NoNewWindow -wait;
    }

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

    CreateSharePath;

    CreateAppPoolAndSite $SitePhysicalPath;


}

function ChangeConfiguration{

    if($TestScenario -eq "MusicStore")
    {
        (Get-Content "$SitePhysicalPath\..\config.json").Replace(
        'Server=(localdb)\\MSSQLLocalDB;Database=MusicStore;Trusted_Connection=True;MultipleActiveResultSets=true;Connect Timeout=30;', 
        $ConnectionString) | Set-Content "$SitePhysicalPath\..\config.json";                    
    }
    
    (Get-Content "$SitePhysicalPath\web.config").Replace(
    "..\logs\stdout.log", "$WorkDir\kestrel.log"   ) | Set-Content "$SitePhysicalPath\web.config";  
}

function CloneAndBuild{

    #echo $GitPath
    if(!(Test-Path $RepoPath)){
        mkdir $RepoPath;
    }
    if(!(Test-Path $SivaBranchRepoPath)){
        mkdir $SivaBranchRepoPath;
    }

    Start-Process -FilePath "$GitPath\cmd\git.exe" -ArgumentList "clone $GitUrl" -WorkingDirectory $RepoPath -NoNewWindow -Wait;
    Start-Process -FilePath "$GitPath\cmd\git.exe" -ArgumentList "clone -b sivag/reliability https://github.com/aspnet/Performance.git" -WorkingDirectory $SivaBranchRepoPath -NoNewWindow -Wait;
    cd $RepoRoot;
    cmd /c .\build.cmd;
    
    ChangeConfiguration;

    Start-Process -FilePath "$DotnetPath\dotnet.exe" -ArgumentList "--version" -WorkingDirectory $WorkDir -NoNewWindow -Wait -RedirectStandardOutput "$ResultSharePath\version.log";
    Start-Process -FilePath "$DotnetPath\dotnet.exe" -ArgumentList "restore" -WorkingDirectory $WorkDir -NoNewWindow -Wait -RedirectStandardOutput "$ResultSharePath\restore.log";
    Start-Process -FilePath "$DotnetPath\dotnet.exe" -ArgumentList "publish -c release" -WorkingDirectory $WorkDir -NoNewWindow -Wait -RedirectStandardOutput "$ResultSharePath\publish.log";
}


Cleanup;
InstallPreReqs;
CloneAndBuild;
