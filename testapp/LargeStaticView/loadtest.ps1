param(
    [int] $iterations = 3000,
    [int] $rps = 50,
    [string][ValidateSet("taghelpers", "notaghelpers")] $variation = "taghelpers")

if ($variation -eq "notaghelpers")
{
    $url = "http://127.0.0.1:5000/"
}
elseif ($variation -eq "taghelpers")
{
    $url = "http://127.0.0.1:5000/TagHelpers"
}

Write-Host -ForegroundColor Green loadtest -k -n $iterations -c 16 --rps $rps $url
Write-Host

& loadtest -k -n $iterations -c 16 --rps $rps $url