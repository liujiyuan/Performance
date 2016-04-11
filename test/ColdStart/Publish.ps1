
# install dotnet using build.ps1 at the root.
$repoRoot = $(git rev-parse --show-toplevel)
& (Join-Path $repoRoot build.ps1) pre-clean

# Set targetApp name and workspace
& (Join-Path $PSScriptRoot SetEnv.ps1)

if (! (Test-Path variable:global:targetApp)) {
    Write-Error "Target application is not set"
    Exit -1
}

if (! (Test-Path variable:global:workspace)) {
    Write-Error "Workspace dir is not set"
    Exit -1
}

if (! (Test-Path variable:global:outputFile)) {
    Write-Error "Output file is not set"
    Exit -1
}

if (! (Test-Path $global:workspace)) {
    Write-Error "Workspace ${global:workspace} does not exist"
    Exit -1
}

$appLocation = [System.IO.Path]::Combine($repoRoot, "testapp" , $global:targetApp)

if (! (Test-Path $appLocation) )
{
    Write-Error "$appLocation is not a valid performance app"
    Exit -1
}

## publish targeted application
pushd $appLocation
dotnet restore
$publishLocation = [System.IO.Path]::Combine($global:workspace, "publish")

if (Test-Path $publishLocation) {
    Write-Host "Clearing publish directory ${publishLocation}..."
    Remove-Item $publishLocation -Force -Recurse
}

dotnet publish -o (Join-Path $publishLocation ($global:targetApp)) --configuration Release
popd
