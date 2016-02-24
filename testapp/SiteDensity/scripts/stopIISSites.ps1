Param(
[string] $path,
[int32] $count

)

For($i=1;$i -le $count;$i++)
{
  $port = 5100 + $i;
  $site = "HelloworldMVC$i";
$app = "$site/"
  $bind = "http://*:$port";
Write-Output $bind
 
 & .\appcmd.exe stop site $site
}