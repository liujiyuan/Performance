# Client Setup - Wcat

- Create A2 Standard windows VM on Azure
- Install wcat - http://www.iis.net/downloads/community/2007/05/wcat-63-x64
- cd test\Reliability\[ANYAPP] folder   
- Update scenario.ubr 
    - Change all host parameters with the server name that you have setup above 
    - Change all port parameters with the port on which your server is running
    - Update the duration (in seconds).
    
- Configure Ehpemeral port limit

        [HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters]
        "MaxUserPort"=dword:0000fffe
        "TcpTimedWaitDelay"=dword:0000001e     
        

# Test runs

- Performance runs 
    - Physical Machines - on dedicated network
        - 1 Windows Server
        - 1 Linux Server
        - 1 Windows Client
    - TestApp: HelloWorldMVC 
    - All Configurations
    - Metrics: Date, Throughput, Memory, Reliability, MedianLatency, P95Latency
    - ServerSide Metrics: ?
    - Command

            .\Scripts\StartTestRun.ps1 -scenario [YourScenarioFile] -TestPerformance $true -vc 300 -logdir %logdir%
- Reliability runs 
    - 24 hours 
    - Virutal Machines
        - 1 Windows Server, 1 Windows Client, separate VNET
        - 1 Linux Server, 1 Windows Client, separate VNET
    - Test App: Music Store: IIS + Core, Linux + Nginx + Core
            https://github.com/aspnet/musicStore
    - Test App: HelloWorldMVC: IIS + Core , Linux + Nginx + Core
    - Metrics: Date, Duration, Reliability, Memory, Throughput, MedianLatency, P95Latency
    - ServerSide Metrics: ?
    - Command
    
            .\Scripts\StartTestRun.ps1 -scenario [YourScenarioFile] -TestReliability $true -vc 300 -duration 86400 -logdir %logdir%
- Connection Density
    - Command

            .\Scripts\StartTestRun.ps1 -scenario [YourScenarioFile] -TestConnectionDensity $true -logdir  %logdir%


# JMeter Setup
- Install Java (https://www.java.com/en/ and click the download link)
- Install jMeter
    - Go to http://jmeter.apache.org/download_jmeter.cgi
    - click on apache-jmeter-2.13.zip link under the Binaries heading to download the JMeter binaries
    - Save the zip file to c:\jMeter
    - Unzip the file by right clicking the zip file and selecting "Extract All"
    
# Running JMeter
- In a command window, go to the JMeter bin folder (apache-jmeter-2.13\bin)
- Run:  jmeter.bat
- Once the JMeter UI has opened, click the File Open icon and open MusicStoreShoppingCartScenario.jmx
- Click on the Run Time Variables element at the top of the test plan and set the Host, Port, NumClients variables
- click on the Run button.
