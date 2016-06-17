
for %%i in (1 2 3 4 5) do (
    start cmd "/c loadtest -c 10 --rps 100 -k http://localhost:5000"
)

:whiletrue
curl http://localhost:5000?exit
powershell Start-Sleep -s 10
goto whiletrue
