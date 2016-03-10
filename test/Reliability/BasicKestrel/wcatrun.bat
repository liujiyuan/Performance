REM run: wcatrun.bat servername serverport

taskkill /f /t /im wcctl
taskkill /f /t /im wcctl.exe
taskkill /f /t /im wcclient.exe
taskkill /f /t /im wcclient
cmd /c .\prepare.bat %1 %2
start .\wcclient.exe localhost -b
.\wcctl.exe -t .\scenario.ubr -f .\settings.ubr -p %2  -c 1 -v 500 -o .\out.xml -x
