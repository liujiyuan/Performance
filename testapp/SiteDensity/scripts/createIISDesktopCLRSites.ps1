Param(
[string] $path,
[int32] $count

)




For($i=2;$i -le $count;$i++)
{
  $port = 5100 + $i;
  $site = "HelloworldMVC$i";
$app = "$site/"
  $bind = "http://*:$port";
Write-Output $bind
  & .\appcmd.exe add apppool /name:$site
  & .\appcmd.exe add site /name:$site /physicalPath:$path  /bindings:$bind

 & .\appcmd.exe set app $app /applicationPool:$site
 & .\appcmd.exe start site $site
}