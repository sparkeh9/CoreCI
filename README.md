# Running on docker toolbox in Windows
In order to communicate with docker running inside docker toolbox, we must install the CA into the trusted root CA certificate store in the form of a pfx.
By default, docker toolbox provides pem files, which can be converted into pfx easily.

Go to `%UserProfile%\.docker\machine\certs`

Once there, convert the CA and cert files to pfx as below. 
- `openssl pkcs12 -export -inkey ca-key.pem -in ca.pem -out ca.pfx` 
- `openssl pkcs12 -export -inkey key.pem -in cert.pem -out key.pfx`

Double click on the ca.pfx file, and install it into the trusted root CA store.