## This script measures the latency of a request to given url,
## If the return code was not 200, the test will fail and exit with eror code -1

param (
    [Parameter(Mandatory=$true)]
    $url
)

## interval of calling curl in seconds
$curlInterval = 0.01
## number of retries
$timeoutPeriod = 1500

$timer = [System.Diagnostics.Stopwatch]::StartNew();

function EnsureTool {
    param($toolExe, $errorMsg)
    $toolPath = where.exe $toolExe

    if (! $toolPath ) {
        Write-Error $errorMsg
        Exit -1
    }

    if ($toolPath -is [object[]]) {
        $toolPath = $toolPath[0]
    }

    return $toolPath
}

$curlPath = EnsureTool "curl.exe" "Ensure you have curl on the path"

$timer.Start()
$status = &($curlPath) --write-out '%{http_code}' --silent --output /dev/null -f $url
$status = [System.Int32]::Parse($status)
if ($status -ne 200) {
    Write-Host "Unexpected HTTP Status: ${status}"
    Exit -1
}
$timer.Stop()

$timer.ElapsedMilliseconds
