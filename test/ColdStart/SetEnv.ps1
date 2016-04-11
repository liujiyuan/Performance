$global:targetApp = "HelloWorldMvc"
$global:toolsPath = "C:\Program Files (x86)\Git\bin"  ## CHANGE ME: point to where curl is
$global:workspace = [System.IO.Path]::Combine($env:USERPROFILE, "aspnet", "tests", "workspace")

if (! (Test-Path $global:workspace)) {
    mkdir $global:workspace
}

$outputDir = [System.IO.Path]::Combine($global:workspace, "tests", "current")

if (! (Test-Path $outputDir)) {
    mkdir $outputDir
}

$global:outputFile = (Get-ChildItem $outputDir | Select-Object FullName -Last 1).FullName

if ([System.String]::IsNullOrEmpty($global:outputFile)) {
    $runId = Get-Date -Format yyyy-MM-dd-HH-mm-ss
    $global:outputFile = (Join-Path $outputDir "${runId}.csv")
}

Write-Host "Tests will be recorded in file: $outputFile"
