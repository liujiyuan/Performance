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
$success = $false
ForEach ($ping in 1..$timeoutPeriod) {
    $r = &($curlPath) -s -f $url
    if (![System.String]::IsNullOrEmpty($r)) {
        $success = $true
        break
    }
    Start-Sleep -s $curlInterval | Out-Null
}

$timer.Stop()

if (!$success) {
    Write-Error "Unable to connect to service"
    Exit -1
}

if (!$success) {
    Write-Error "Unable to connect to service"
    Exit -1
}

$timer.ElapsedMilliseconds
