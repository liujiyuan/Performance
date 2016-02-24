param([int] $server ,
[int] $sites = 100,
[int] $max_rps = 10
 ) 

 for($i=1;$i -le $sites;$i++)
{
$port = 5100 + $i;

$url = "http://$server:$port/"
write-output $url
#resp = Invoke-WebRequest  $url 2>&1 
start-process -NoNewWindow loadtest "-k --rps $max_rps  $url" 
}
 
 
