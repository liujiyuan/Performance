#Prerequisites:
    * Install git command line client
    * Install curl
        - For Windows, edit "SetEnv.ps1" and set $global:toolsPath to the folder where curl.exe is
        - For Linux, curl should be in PATH
    * Install libuv

********* Windows *****************

To build and run 1 iteration of test
.\InstallDotnet.ps1                 ## admin mode required
## refresh PATH if this is fresh installation, i.e.
## $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine")
.\Run.ps1

To build and prepare application only without test run:
.\SetEnv.ps1
.\SetupPerfApp.ps1

To run 1 iteration of test when application is already built
.\SetEnv.ps1
.\PerformTest.ps1


********* Linux *********************

To build and run 1 iteration of test
sudo ./InstallDotnet.sh
source ./Run.ps1

To build and prepare application only without test run:
export ./SetEnv.sh
source ./SetupPerfApp.sh

To run 1 iteration of test when application is already built
source ./SetEnv.sh
source ./PerformTest.sh
