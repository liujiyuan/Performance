SET scenario=Throughput\HelloWorldMVC
SET dtStamp24=%date:~-4%%date:~4,2%%date:~7,2%_%time:~0,2%%time:~3,2%%time:~6,2%
SET dtStamp24=%DTSTAMP24: =0%
set dir=C:\%scenario%\%dtStamp24%
mkdir %dir%

for /l %%i in (1,1,3) do (
taskkill /f /t /im wcctl
taskkill /f /t /im wcctl.exe
taskkill /f /t /im wcclient.exe
taskkill /f /t /im wcclient
start .\wcclient.exe localhost -b
.\wcctl.exe -t .\scenario.ubr -f .\settings.ubr -p 5000 -c 1 -v 500 -o %dir%\result%%i.xml -x -k "%scenario%"
