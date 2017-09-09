#Requires -RunAsAdministrator
Write-Output "Setting up named pipe."
Write-Output "-----"

[Environment]::SetEnvironmentVariable("CORECI_DOCKER_HOST", "npipe://./pipe/docker_engine",[System.EnvironmentVariableTarget]::User)