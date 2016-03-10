# Kestrel Large Static File Application

This application is a test application for performance and reliability for kestrel with large static file

## Setup the application
* use following command to publish the applicaiton
	- dnu restore
	- dnu publish --runtime dnx-coreclr-win-x86.1.0.0-<runtime version> --configuration release

* copy the large static file (\\funfile\Scratch\dayu\test\load.png) to published wwwroot directory under Images (replace the small file)	

## Run the application
* Run as stand alone service
	Go to published wwwroot directory and run ..\approot\web.cmd , the service run at http://localhost:5000

* Run under IIS
	- Setup/Configure IIS using default setting and install HttpPlatformHandler from http://www.iis.net/downloads/microsoft/httpplatformhandler
	- Copy published wwwroot and approot directories to IIS root (c:\inetpub by default)

The client load can follow same setup process as Hello World.