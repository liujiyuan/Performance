taskkill /f /t /im wcctl
taskkill /f /t /im wcctl.exe
taskkill /f /t /im wcclient.exe
taskkill /f /t /im wcclient
start .\wcclient.exe localhost -b
.\wcctl.exe -t .\scenario.ubr -f .\settings.ubr -p 80 -c 1 -v 1000 -o .\out.xml -x
