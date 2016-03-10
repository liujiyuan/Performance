param([int] $iterations = 3000, [int] $rps = 100)

$url = "http://127.0.0.1:5000/pet"

$user = "writer@example.com"
$token_url = "http://127.0.0.1:5000/token?username=$user"

Write-Host -ForegroundColor Green Getting Bearer Token from $token_url

# First, do a GET to this page to retrieve a token, then we need to include the token
# in the post requests for them to be considered authorized.

$response = Invoke-WebRequest -Uri $token_url -Headers @{"Cache-Control"="no-cache"}
$status_code = $response.StatusCode
if ($status_code -ne 200)
{
    Write-Error Request failed: $status_code
    return;
}

Write-Host -ForegroundColor Green Got Bearer Token

$bearer_token = $response.Content

Write-Host -ForegroundColor Green Beginning workload
Write-Host `& loadtest -k -n $iterations -c 16 --rps $rps -T application/json -p post_pet.txt -H "Accept: application/json; q=0.9, application/xml; q=0.6" -H "Accept-Charset: utf-8" -H "Authorization: Bearer $bearer_token" $url
Write-Host

& loadtest -k -n $iterations -c 16 --rps $rps -T application/json -p post_pet.txt -H "Accept: application/json; q=0.9, application/xml; q=0.6" -H "Accept-Charset: utf-8" -H "Authorization: Bearer $bearer_token" $url