REM run: wcatrun.bat servername serverport [reliability|stress]

taskkill /f /t /im wcctl
taskkill /f /t /im wcctl.exe
taskkill /f /t /im wcclient.exe
taskkill /f /t /im wcclient
reg add HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters /v MaxUserPort /t REG_DWORD /d 0x0000fffe /f
reg add HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters /v TcpTimedWaitDelay /t REG_DWORD /d 0x00000002 /f
cmd /c .\prepare.bat %1 %2
set SCENARIO_TYPE=reliability
if "%3"=="stress" set SCENARIO_TYPE=stress

start .\wcclient.exe localhost -b

if /I "%SCENARIO_TYPE%"=="stress" (
    echo running stress scenario
    .\wcctl.exe -t .\scenario_stress.ubr -f .\settings.ubr -p %2  -c 1 -v 100 -o .\out.xml -x
) else (
    echo running reliability scenario
    .\wcctl.exe -t .\scenario_reliability.ubr -f .\settings.ubr -p %2  -c 1 -v 100 -o .\out.xml -x
)
