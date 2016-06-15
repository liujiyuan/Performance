Write-Host "Starting server in a separate process"
pushd MultipartPOST
if (-Not (Test-Path ".\project.lock.json"))
{
    dotnet restore --infer-runtimes
}
dotnet build
Start cmd "/C title Server && dotnet run && pause"
popd

Write-Host "Starting multiple clients"
pushd MultipartPOSTClient
if (-Not (Test-Path ".\project.lock.json"))
{
    dotnet restore --infer-runtimes
}
dotnet build
for ($i = 0; $i -lt 3; $i++)
{
    Write-Host "Iteration $i"
    Start cmd "/C title Client $i && dotnet run -i 100 -l && pause"
    Start-Sleep -s 3
}
popd
