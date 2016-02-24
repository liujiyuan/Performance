This folder has all the necessary files that were used to get baseline comparison of HelloWorld MVC for DesktopCLR and CoreCLR.

For numbers/results please refer to https://github.com/aspnet/Reliability/issues/150 

Core CLR
--------
Setup on Server machine 
**********************

1. Copy the https://github.com/aspnet/Performance/tree/dev/testapp/HelloWorldMvc app on to a folder in the server machine
2. Also copy the scripts folder https://github.com/aspnet/Performance/tree/dev/testapp/SiteDensity/scripts to the server machine
2. Let it be d:\HelloWorldMVC and cd to that path
3. Install all necessary core CLR tools like dnu,dnx etc.
4. Open a powershell command prompt.
5. Set the environment variable $env:DNX_FEED="https://www.myget.org/F/aspnetcidev/api/v2"
6. dnvm upgrade -r coreclr -a x86
7. dnu restore
8. do a trial run dnx run and see if a request http://localhost:5000 works.
9. Now to create 100 stand alone dnx for HelloWorld MVC app, navigate to the scripts folder
10. Execute the powershell script as below
   .\startDNXstandalonesites.ps1 d:\HelloWorldMVC coreclr 100
11. This will start 100 DNX sites on the localhost listening at the ports ranging from 5101-5200
12. To stop the DNX sites, use
taskkill /im powershell.exe

Running test on client machine
****************************
1. Make sure you have installed node.js and installed loadtest module
 npm -g install loadtest
2. Clone/ copy the scripts folder https://github.com/aspnet/Performance/tree/dev/testapp/SiteDensity/scripts to the client machine
3. Run the powershell script as follows
.\loadtestonclient.ps1 <server machine name or ip> 100 <max RPS>
4. This script will start loadtest against all the sites on server on ports ranging from 5101-5200


Desktop CLR (MVC 4.0)
-----------
Setup on Server machine 
***********************
1. Open HelloWorldMVC solution  at 
https://github.com/aspnet/Performance/tree/dev/testapp/SiteDensity/src/Baseline_DesktopCLR_MVC4_HellowWorldApp
2. Build for Release and publish it to a file system 
3. Open the publish output dir and copy all the artifacts to a folder in the server machine.
4. Let it be d:\DesktopCLRHelloWorldMVC
5. Also copy the scripts folder https://github.com/aspnet/Performance/tree/dev/testapp/SiteDensity/scripts to the server machine
6. We will use IIS AppCMD.exe to create sites from command prompt.
6. If you are running 64-bit Windows, use Appcmd.exe from the %windir%\system32\inetsrv directory. Also run the powershell as administrator.
6. We will host 100 IIS sites on server using the below script
 .\createIISDesktopCLRSites.ps1 d:\DesktopCLRHelloWorldMVC 100
7. This should have started 100  sites on ports 5101 to 5200
8. To stop the sites you can run 
.\stopIISSites.ps1 

Running test on client machine
****************************
1. Make sure you have installed node.js and installed loadtest module
 npm -g install loadtest
2. Clone/ copy the scripts folder https://github.com/aspnet/Performance/tree/dev/testapp/SiteDensity/scripts to the client machine
3. Run the powershell script as follows
.\loadtestonclient.ps1 <server machine name or ip> 100 <max RPS>
4. This script will start loadtest against all the sites on server on ports ranging from 5101-5200