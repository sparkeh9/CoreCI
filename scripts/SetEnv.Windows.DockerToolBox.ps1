#Requires -RunAsAdministrator
Write-Output "Setting up Docker Toolbox environment.  Press enter for any password requests"
Write-Output "-----"

$certificatePath = [Environment]::GetFolderPath("UserProfile") + "\.docker\machine\certs"
Push-Location $certificatePath

openssl pkcs12 -export -inkey ca-key.pem -in ca.pem -out ca.pfx
openssl pkcs12 -export -inkey key.pem -in cert.pem -out key.pfx

certutil -f -user -importpfx ca.pfx

[Environment]::SetEnvironmentVariable("DOCKER_HOST", "http://192.168.99.100:2376", [System.EnvironmentVariableTarget]::User)
[Environment]::SetEnvironmentVariable("DOCKER_CERT_PATH", $certificatePath, [System.EnvironmentVariableTarget]::User)
[Environment]::SetEnvironmentVariable("DOCKER_TLS_VERIFY ", "0", [System.EnvironmentVariableTarget]::User)

Pop-Location