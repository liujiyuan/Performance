Param(
    [int]$Duration=86500,
    [int]$SampleInterval=3,
    [string]$ProcessList="w3wp",
    [string]$logdir
)

if(!($logdir))
{
    $tstamp=$(((get-date).ToUniversalTime()).ToString("yyyy-MM-dd"));
    $logdir = "c:\$tstamp-$scenario";
}
mkdir $logdir

$OutFile=$("$logdir\\PerfCounters.csv");
$ProcessList = $ProcessList.Split(",");

$GlobalCounters = @"  
\Processor(_total)\% processor time  
\system\processor queue length 
\Memory\Available MBytes  
\Memory\% committed bytes in use 
\PhysicalDisk(*)\Current Disk Queue Length  
\PhysicalDisk(*)\Avg. Disk sec/Transfer  
\PhysicalDisk(*)\% Disk Time  
\PhysicalDisk(*)\Disk Read Bytes/sec  
\PhysicalDisk(*)\Disk Write Bytes/sec  
\Network Interface(*)\Bytes Total/sec
\TCPv4\*  
\TCPv6\*
"@  ;

[Collections.Generic.List[String]]$Counters = $GlobalCounters -split "`n" | ForEach-Object { $_.trim(); };

foreach($process in $ProcessList)
{
    $Counters.Add($("\Process($process)\*"));
}

$Time = [System.Diagnostics.Stopwatch]::StartNew();
Write-Host "Logging PerfCounters to $OutFile";
While($Time.Elapsed.Seconds -lt $Duration)
{
    (Get-Counter -Counter $Counters).counterSamples | ForEach-Object { 
        New-Object PSObject -Property @{ 
            TimeStamp = $_.TimeStamp;
            Counter = $_.Path;
            Value = [Math]::Round($_.CookedValue,2);
        }
    } | Select-Object TimeStamp, Counter, Value | Export-Csv $OutFile  -Append -NoTypeInformation;

    Start-Sleep -s $SampleInterval;
}
