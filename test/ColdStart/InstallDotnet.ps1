[CmdletBinding()]
Param(
   [Parameter(Mandatory=$false)]
   [string]$outputDirectory = "."
)

if (! (Test-Path $outputDirectory)) {
    Write-Error ("Output dir '" + $outputDirectory + "' is incorrect.")
    Exit -1
}

$installExe = Join-Path $outputDirectory "dotnet-win-x64.latest.exe"
if(Test-path $installExe) {
    Write-Warning ($installExe + " already exists and will be replaced.")
}

$url = "https://dotnetcli.blob.core.windows.net/dotnet/beta/Installers/Latest/dotnet-win-x64.latest.exe"

# Bits is fast but if its failing or not avaialble then switch to Invoke-WebRequest if we can't enable it on the box. 
# Invoke-WebRequest -Uri $url  -OutFile $installExe
Import-Module BitsTransfer
Start-BitsTransfer -Source $url -Destination $installExe

Unblock-File $installExe
start-process -Wait $installExe