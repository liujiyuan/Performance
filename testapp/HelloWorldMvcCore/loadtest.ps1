param(
    [int] $iterations = 3000,
    [int] $rps = 250,
    [string][ValidateSet("plaintext")] $variation = "plaintext")

if ($variation -eq "plaintext")
{
    $url = "http://127.0.0.1:5000/Plaintext"
}

Write-Host -ForegroundColor Green Beginning workload
Write-Host "`& loadtest -k -n $iterations -c 32 --rps $rps $url"
Write-Host

& loadtest -k -n $iterations -c 64 --rps $rps $url