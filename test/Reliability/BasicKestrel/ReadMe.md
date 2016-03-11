# WCAT script to run stress test against StressMVC server
These are wcat scripts on client side to run stress against [StressMVC](../../../testapp/StressMvc) server

## Goals

*  Exercise all scenarios on the server
*  Automated script using WCAT. 

## How to run
* Install wcat on a Windows machine, or simply copy wcat binaries over
* Copy the content of this folder to the same folder of wcat
* Run the following command:
```
	wcatrun.bat [ServerName] [ServerPort]
```	
The above command will update the script with correct configuration, and generate data file needed to run test.
