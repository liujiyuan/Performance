if (! (Test-Path variable:global:workspace)) {
    Write-Error "Workspace dir is not set"
    Exit -1
}

$installExe = Join-Path $global:Workspace "dotnet-win-x64.latest.exe"
Invoke-WebRequest -Uri https://dotnetcli.blob.core.windows.net/dotnet/beta/Installers/Latest/dotnet-win-x64.latest.exe -OutFile $installExe

 &($installExe) -s

## the following is a hack to resolve a coreclr path probing issue
Start-Sleep -s 20
$sourceDir = [System.IO.Path]::Combine($env:ProgramFiles, "dotnet", "bin")
$destDir = [System.IO.Path]::Combine($env:LocalAppData, "dotnet",  "runtime", "coreclr")
Copy-Item $sourceDir -Destination $destDir -Force -Recurse
