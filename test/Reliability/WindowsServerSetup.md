# Description
This setup is intended for reliability & performance tests for Kestrel and ASP.NET core 

# Setup CoreCLR and ASP.NET Core 
- Install git - http://git-scm.com/download for example
- Install dotnet cli - http://dotnet.github.io
- Clone the Performance repo

        git clone https://github.com/aspnet/Performance.git
- Run Build.cmd
- Make sure of ASPNetVNext Feed in Nuget.Config    
    
        <add key="AspNetVNext" value="https://www.myget.org/F/aspnetcidev/api/v3/index.json" />
- Publish the binaries

    - Go to Permformance repo
    - cd testapp\[ANYAPP]
    
            Dotnet --version > %logfolder%\version.log 2>&1
            Dotnet restore > %logfolder%\restore.log 2>&1
            Dotnet publish -c release > %logfolder%\publish.log 2>&1       
        
# Configuration: Windows + Core 

        set COREHOST_SERVER_GC=1
        cd bin\release\netstandardapp1.5\win7-x64\publish
        .\HelloWorldMVC.exe --sever.urls=http://*:5000 > %logdir%\kestrel.log 2>&1

        
# Configuration: IIS + Core
- One Time Setup - Install and Configure IIS

    - Install IIS from server manager -> Manage -> Add Roles and features
    - Install HttpPlatformHandler from http://www.iis.net/downloads/microsoft/httpplatformhandler
    - In IIS manager, Open Configuration editor and make sure system.webserver/handlers section is unlocked (i.e. write access is enabled).
    - Create new site (not application or vdir) in IIS; 
        - set its Physical Path to testapp\HelloWorldMvc\bin\bin\release\netstandardapp1.5\win7-x64\publish 
        - Set the Host Name to the machine name.
    - Make sure user for site AppPool has full permissions to this folder. 
    - In IIS manager, edit the site Application Pool to set '.NET CLR version' to  'No Managed Code'.
    - Open the Site's port in the fire wall
    - Test the new  site with a browser specifying the machine name in the URL both locally and remotely
    - Configure IIS Queue Length to 10000 - see [this](https://technet.microsoft.com/en-us/library/dd441171.aspx)
    - Configure ASP.NET Queue Length 
    
            appcmd.exe set config /section:serverRuntime /appConcurrentRequestLimit:25000 
- Turn on Server GC in the web.config file

        <environmentVariable name="COREHOST_SERVER_GC" value="1" />
- Enable log output

        processPath=".\<YourProcess>.exe" stdoutLogEnabled="true" stdoutLogFile="<LogFolder>\kestrel.log"         
# Configuration: IIS + DesktopCLR + Core
- Same as Configuration : IIS + Core 
- except: Set its Physical Path to testapp\HelloWorldMvc\bin\bin\release\dnx451\win7-x64\publish
- Turn on Server GC in the web.config file  
    
            <runtime>
            <gcServer enabled="true"/>
      

# Monitor Server machines

        In another Window - Monitor the processes.
        .\Monitor.ps1 -ProcessList "HelloWorldMVC" -logdir %logdir%
    
    
# Check for crash and memory leaks

        Get-Content .\PerfCounters.csv | Select-String "id process" > processid.txt
            The file should have only one process id for the entire test duration
        Get-Content .\PerfCounters.csv | Select-String "working set - private" > memory.txt
            The file should not have monotonously increasing memory usage.            
             
# Cleanup for daily runs

        Goto Programs and Features and Uninstall Dotnet CLI  
        Delete folders
            rmdir -Recurse -Path testapp\HelloWorldMvc\bin\release
              rmdir -Recurse -Path $env:LOCALAPPDATA\NUGET\v3-cache\
              rmdir -Recurse -Path $env:USERPROFILE\.dnx
              rmdir -Recurse -Path $env:USERPROFILE\.dnu
              rmdir -Recurse -Path $env:USERPROFILE\.nuget
              rmdir -Recurse -Path $env:LOCALAPPDATA\Microsoft\dotnet
                rmdir -Recurse -Path $env:ProgramW6432\dotnet
        cd <RepoFolder>
        git clean -xdf
