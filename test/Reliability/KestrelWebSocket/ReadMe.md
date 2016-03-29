# loadtest script to run stress test using websocket against StressMVC server
These are loadtest scripts on client side to run stress against [StressMVC](../../../testapp/StressMvc) server

## Goals

*  Basic web socket scenario
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
