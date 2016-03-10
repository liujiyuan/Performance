# StressSimpleMvc
A simple test app built based StartMvc app but with the cut of dependency on SQL, security, etc. so that it can be used for stress easily.

## Goals

### Test app to be used to stress Kestrel/Mvc functionality easily
### Work cross-platform
### Low overhead: with very minimum fake business logic
### Exercise all basic functionalities
* Kestrel server
* Basic Mvc models and routings
* Different type of contents (json, plain text, etc.)
* Different type of HTTP methods
* Different size of contents
* Different status code

## How to run
* install the runtime for coreclr
* dotnet restore
* dotnet run --configuration release


## Other configuration
* By default it litens on localhost:5000. To listen on other url, run as:
dotnet run server.urls=http://server:port

* If using other http server as reverse proxy, you may need to update configuration to allow large content to be sent by client


