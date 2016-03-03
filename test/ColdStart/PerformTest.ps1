param (
    ## If -PerfView option is on, PerfView.exe needs to be in the tools folder specified by $global:toolsPath
    [switch]
    $PerfView
)
## interval of calling curl in seconds
$curlInterval = 0.01
## number of retries
$timeoutPeriod = 1500

## functions

function RunScenario {
    param($appLocation, $port, $clearCache)

    $portQuery = netstat -an | findstr ":$port "
    if (![System.String]::IsNullOrEmpty($portQuery)) {
        Write-Error "$port is in use."
        Exit -1
    }

    $timer = [System.Diagnostics.Stopwatch]::StartNew()

    $curlPath = Join-Path $global:toolsPath curl.exe

    $env:HTTP_PLATFORM_PORT = $port

    if ($PerfView) {
        $PerfViewTimestamp = Get-Date -Format yyyy-MM-dd-HH-mm-ss
        $perfviewStartLog = Join-Path $outputFolder "${PerfViewTimestamp}.start.log"
        $perfviewTraceFile = Join-Path $outputFolder "${PerfViewTimestamp}.etl"
        & $PerfViewExe -AcceptEULA -NoGui -BufferSizeMB:1024 -clrEvents=default+GCSampledObjectAllocationHigh -KernelEvents=Default+FileIO+FileIOInit+ThreadTime -LogFile:$perfviewStartLog start $perfviewTraceFile
    }

    try {
        $process = Start-Process -FilePath (Join-Path $appLocation "${global:targetApp}.exe") -PassThru

        Write-Host Process started with PID $process.Id

        $timer.Start()
        $success = $false
        ForEach ($ping in 1..$timeoutPeriod) {
            $r = &($curlPath) -s -f "http://localhost:$port"
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

        [System.IO.File]::AppendAllText("$global:outputFile", $timer.ElapsedMilliseconds, [System.Text.Encoding]::Unicode)
        return $process.Id
    }
    finally {
        if ($PerfView) {
            $perfviewStopLog = Join-Path $outputFolder "${PerfViewTimestamp}.stop.log"
            & $PerfViewExe -AcceptEULA -LogFile:$perfviewStopLog -Zip:true -NoView -NoNGenRundown stop
        }
    }
}

if ($PerfView) {
    $PerfViewExe = (Join-Path $global:toolsPath "PerfView.exe")
    if (! (Test-Path $PerfViewExe)) {
        Write-Error "For -PerfView option, please edit `$global:toolsPath in SetEnv.ps1 to a directory containing PerfView.exe"
        Exit -1
    }
}

## Main script
if (! (Test-Path variable:global:targetApp)) {
    Write-Error "Target application is not set"
    Exit -1
}

if (! (Test-Path variable:global:workspace)) {
    Write-Error "Workspace dir is not set"
    Exit -1
}

if (! (Test-Path $global:workspace)) {
    Write-Error "Workspace ${global:workspace} does not exist"
    Exit -1
}

if (! (Test-Path variable:global:outputFile)) {
    Write-Error "Output file is not set"
    Exit -1
}

if (! (Test-Path $global:outputFile)) {
    ## File does not exist yet, write output file header
    "Cold,Warm" | Out-File $global:outputFile
}

## TODO: This is not a good logic, we should name the folder with timestamp and have a $global:outputFolder variable instead
$outputFolder = Split-Path $global:outputFile

$coldSitePort = 5000
$warmSitePort = 5001

$coldSiteLocation = [System.IO.Path]::Combine($global:workspace, "publish", $global:targetApp + "0")
$warmSiteLocation = [System.IO.Path]::Combine($global:workspace, "publish", $global:targetApp + "1")

if (! (Test-Path $coldSiteLocation)) {
    Write-Error "Cold site does not exist in ${coldSiteLocation}, did you run SetupPerfApp.ps1?"
    Exit -1
}

if (! (Test-Path $warmSiteLocation)) {
    Write-Host "Warm site location ${warmSiteLocation} does not exist, copying..."
    Copy-Item $coldSiteLocation $warmSiteLocation -Force -Recurse
}

try {
    $warmPID = RunScenario -appLocation $coldSiteLocation -port $coldSitePort
    [System.IO.File]::AppendAllText("$global:outputFile", ",", [System.Text.Encoding]::Unicode)
    if ($PerfView) {
        Start-Sleep 120    # It takes a while to process the profile even after command finishs
    }
    $coldPID = RunScenario -appLocation $warmSiteLocation -port $warmSitePort
    [System.IO.File]::AppendAllText("$global:outputFile", "`r`n", [System.Text.Encoding]::Unicode)
}
finally {
    try {
        Stop-Process $warmPID
    }
    finally {
        Stop-Process $coldPID
    }
}

