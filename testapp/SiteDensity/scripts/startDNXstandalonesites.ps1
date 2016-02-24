Param(
[string] $startDNXstandalonesiteScriptPath,
[string] $apppath,
[string] $clr,
[int32] $count

)



For($i=1;$i -le $count;$i++)
{
  $port = 5100 + $i;
$commandLine = "-NoExit & $startDNXstandalonesiteScriptPath,$apppath $clr $port"
  Start-Process powershell.exe -ArgumentList $commandLine 
}
