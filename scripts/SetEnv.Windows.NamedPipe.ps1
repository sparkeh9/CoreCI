#Requires -RunAsAdministrator
Write-Output "Setting up named pipe."
Write-Output "-----"

[Environment]::SetEnvironmentVariable("DOCKER_HOST", "npipe://./pipe/docker_engine",[System.EnvironmentVariableTarget]::User)
[Environment]::SetEnvironmentVariable("DOCKER_CERT_PATH", $null, [System.EnvironmentVariableTarget]::User)
[Environment]::SetEnvironmentVariable("DOCKER_TLS_VERIFY ", $null, [System.EnvironmentVariableTarget]::User)