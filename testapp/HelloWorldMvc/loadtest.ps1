param(
    [int] $iterations = 3000,
    [int] $rps = 250,
    [string][ValidateSet("plaintext", "html")] $variation = "plaintext")

if ($variation -eq "plaintext")
{
    $url = "http://127.0.0.1:5000/Plaintext"
}
elseif ($variation -eq "html")
{
    $url = "http://127.0.0.1:5000/Home"
}

Write-Host -ForegroundColor Green Beginning workload
Write-Host "`& loadtest -k -n $iterations -c 32 --rps $rps $url"
Write-Host

& loadtest -k -n $iterations -c 32 --rps $rps $url