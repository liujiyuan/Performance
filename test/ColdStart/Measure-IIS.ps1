## This script measures 1 iteration of cold start data for cold/warm startup scenario
## and write a line of result to the file

#parameters not used. It's here for to adapt for the framework
param (
    [Alias("t")]$targetApp = "HelloWorldMvc",
    [Alias("f")]$framework = "netcoreapp1.0"
)

& (Join-Path $PSScriptRoot SetEnv.ps1)

$hostname = & hostname

$coldSiteDelay = & (Join-Path $PSScriptRoot MeasureRequestLatency.ps1) http://${hostname}:5001
$warmSiteDelay = & (Join-Path $PSScriptRoot MeasureRequestLatency.ps1) http://${hostname}:5002

if (!$coldSiteDelay -or ($coldSiteDelay -le 0) -or !$warmSiteDelay -or ($warmSiteDelay -le 0)) {
    Write-Host "Unexpected delays $coldSiteDelay $warmSiteDelay"
    Exit -1
}

if (!(Test-Path $global:outputFile)) {
    ## File does not exist yet, write output file header
    "Cold,Warm" | Out-File $global:outputFile
}

"${coldSiteDelay},${warmSiteDelay}" | Out-File $global:outputFile -Append
