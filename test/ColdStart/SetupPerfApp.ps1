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

## clone performance branch
$performanceReproPath = [System.IO.Path]::Combine($global:workspace, "Performance")

if (Test-Path $performanceReproPath)
{
    Remove-Item $performanceReproPath -Recurse -Force
}

git clone "https://github.com/aspnet/Performance.git" "$performanceReproPath"

$appLocation = [System.IO.Path]::Combine($performanceReproPath, "testapp", $global:targetApp)

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
    Remove-Item $publishLocation -Force -Recurse
}

dotnet publish -o (Join-Path $publishLocation ($global:targetApp + "0")) --configuration Release --framework "netstandardapp1.5"
popd
