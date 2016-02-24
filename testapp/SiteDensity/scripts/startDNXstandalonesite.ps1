Param(
[string] $apppath,
[string] $clr,
[int32] $port
)

cd $apppath
$env:DNX_FEED="https://www.myget.org/F/aspnetcidev/api/v2"
$aspenv = "ASPNET_SERVER`.URLS"
$aspportvalue="http://*:"+$port
write-host "Server listening at $aspportvalue"
[Environment]::SetEnvironmentVariable($aspenv, $aspportvalue, "Process")
#dnvm upgrade -r $clr -a x86
#dnu restore
dnx run


