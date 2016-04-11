# loadtest script to run stress HTTPS test against StressMVC server
These are loadtest scripts on client side to run stress against [StressMVC](../../../testapp/StressMvc) server

## Goals

*  Basic scenarios on HTTPS
*  Automated script using loadtest on both Windows/Linux

## How to run
On Windows:
* Install node.js first
* Run the following command:
```
    startloadtest.cmd [Server:port] [NumConcurrentClients] [NumIterations]
```

On Linux:
* Run the following command:
```
	startloadtest.sh [Server:port] [NumConcurrentClients] [NumIterations]
```


Also, server needs to listen on the HTTPS port as well. You can start StressMvc as below:
```
dotnet StressMvc.dll server.urls="http://*:5000;https://*:5001"
```
