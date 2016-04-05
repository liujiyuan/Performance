param(
    [int] $iterations = 3000,
    [int] $rps = 100,
    [string][ValidateSet("taghelpers", "htmlhelpers")] $variation = "taghelpers")

if ($variation -eq "taghelpers")
{
    $url = "http://127.0.0.1:5000/"
}
elseif ($variation -eq "htmlhelpers")
{
    $url = "http://127.0.0.1:5000/Home/HtmlHelpers"
}

Write-Host -ForegroundColor Green Getting antiforgery token from $url

# First, do a GET to this page to load the form, then we need to include the cookie + form token
# in the post requests for them to be considered valid.

$response = Invoke-WebRequest -Uri $url -Headers @{"Cache-Control"="no-cache"}
$status_code = $response.StatusCode
if ($status_code -ne 200)
{
    Write-Error Request failed: $status_code
    return;
}

$user_agent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36"

Write-Host -ForegroundColor Green Got token

$set_cookie = $response.Headers["Set-Cookie"]
$cookie_name = $set_cookie.Substring(0, $set_cookie.IndexOf('='))
$cookie_value = $set_cookie.Substring($cookie_name.Length + 1, $set_cookie.IndexOf(';') - $cookie_name.Length - 1)

$form_name = "__RequestVerificationToken"
$form_value = $response.Forms[0].Fields[$form_name]

$body = "name=Joey^^^&age=15^^^&birthdate=9-9-1985^^^&$form_name=$form_value"
$content_type = "application/x-www-form-urlencoded"

Write-Host -ForegroundColor Green Beginning workload
Write-Host "`& loadtest -k -n $iterations -c 16 --rps $rps -P $body -T $content_type -C $cookie_name=$cookie_value -H User-Agent: $user_agent $url"
Write-Host

& loadtest -k -n $iterations -c 16 --rps $rps -P $body -T $content_type -C $cookie_name=$cookie_value -H "User-Agent: $user_agent" $url