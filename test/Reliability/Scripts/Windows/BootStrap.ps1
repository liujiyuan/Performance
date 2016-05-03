Param(
    [string]$RootPath = "$env:SystemDrive\Repo"
)

$RepoUrl = "https://github.com/aspnet/Performance.git";
$RepoName = "Performance";

if(!(Test-Path $RootPath)){
    mkdir $RootPath;
}
cd $RootPath;

$GitPath = "$env:ProgramFiles\Git";
if(!(Test-Path $GitPath))
{
    $GitUrl = "https://github.com/git-for-windows/git/releases/download/v2.8.1.windows.1/Git-2.8.1-64-bit.exe";
    (New-Object System.Net.WebClient).DownloadFile($GitUrl,"$RootPath\git.exe");

    # Install Git
    Start-Process -FilePath .\git.exe -ArgumentList "/silent /norestart" -NoNewWindow -wait;
}

if(Test-Path "$RootPath\$RepoName")
{
    Start-Process -FilePath "$GitPath\cmd\git.exe" -ArgumentList "clean -xdf" -WorkingDirectory "$RootPath\$RepoName\" -NoNewWindow -Wait;
    #Remove-Item -Recurse -Force "$RootPath\$RepoName\"
}


Start-Process -FilePath "$GitPath\cmd\git.exe" -ArgumentList "clone -b release $RepoUrl" -WorkingDirectory $RootPath -NoNewWindow -Wait;



