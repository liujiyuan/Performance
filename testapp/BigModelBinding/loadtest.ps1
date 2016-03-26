param(
    [int] $iterations = 3000,
    [int] $rps = 150)

$url = "http://127.0.0.1:5000"

& loadtest -k -n $iterations -c 16 --rps $rps -T application/x-www-form-urlencoded -p postdata.txt -H "Accept: application/json; q=0.9, application/xml; q=0.6" -H "Accept-Charset: utf-8" $url