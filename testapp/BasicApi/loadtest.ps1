param([int] $iterations = 3000)

$url = "http://127.0.0.1:5000/pet"

& loadtest -k -n $iterations -c 16 --rps 250 -T application/json -p post_pet.txt -H "Accept: application/json; q=0.9, application/xml; q=0.6" -H "Accept-Charset: utf-8" $url