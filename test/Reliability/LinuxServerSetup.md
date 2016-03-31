# Description
This setup is intended for reliability & performance tests for Kestrel and ASP.NET core 

# Setup CoreCLR and ASP.NET Core 
- Install git - http://git-scm.com/download for example
- Install dotnet cli - http://dotnet.github.io
   
        curl https://dotnetcli.blob.core.windows.net/dotnet/beta/Installers/Latest/dotnet-ubuntu-x64.latest.deb -o dotnet-ubuntu-x64.latest.deb
        sudo dpkg -i ./dotnet-ubuntu-x64.latest.deb
- Clone the Performance repo

        git clone https://github.com/aspnet/Performance.git
- Run Build.sh
- Make sure of ASPNetVNext Feed in Nuget.Config    
    
      <add key="AspNetVNext" value="https://www.myget.org/F/aspnetcidev/api/v3/index.json" />
- Publish the binaries

    - Go to Permformance repo
    - cd testapp\[ANYAPP]
    - dotnet --version &> $logdir/version.log 
    - dotnet restore &> $logdir/restore.log
    - dotnet publish -c release &> $logdir/publish.log
        
# Configuration: Linux + Core
        
        ulimit -c unlimited
        ulimit -n 100000
        export COREHOST_SERVER_GC=1
        cd bin\release\netstandardapp1.5\ubuntu.14.04-x64\publish
        .\HelloWorldMVC.exe --sever.urls=http://*:5000 &> $logdir/kestrel.log
        
# Configuration: Linux + Nginx + Core
- One Time Setup  - Install anc Configure Nginx 
    - Install libuv

            sudo apt-get install make automake libtool curl
            curl -sSL https://github.com/libuv/libuv/archive/v1.8.0.tar.gz | sudo tar zxfv - -C /usr/local/src
            cd /usr/local/src/libuv-1.8.0
            sudo sh autogen.sh
            sudo ./configure
            sudo make
            sudo make install
            sudo rm -rf /usr/local/src/libuv-1.8.0 && cd ~/
            sudo ldconfig
    - sudo apt-get install nginx
    - cp test/reliability/scripts/nginx-perfserver /etc/nginx/sites-available/
        - Update error logfile location to central file share 
    - cp test/reliability/scripts/nginx.conf /etc/nginx/nginx.conf
    - sudo ln -s /etc/nginx/sites-available/nginx-perfserver /etc/nginx/sites-enabled/nginx-perfserver
    - sudo /etc/init.d/apache2 stop 
    - sudo service nginx restart
    - Verify: On another machine, test the site with a browser (http://serverName:8080)
      

# Monitor Server machines

        In another Window - Monitor the processes    	
        top | grep 'nginx\|<YourProcess>' &> $logdir/cpumemlog.log
    
    
# Check for crash and memory leaks
        ./corezip.py  -c ./core -z ./coreFiles.zip -f <yourExecutablePath>    
             
# Cleanup for daily runs

        Uninstall dotnet 
            
            sudo dpkg -r dotnet
        Delete folders
            rm -rf testapp\HelloWorldMvc\bin\release
            rm -rf ~/.nuget
            rm -rf ~/.dotnet
            rm -rf ~/.dnx
            rm -rf ~/.local
            rm -rf /usr/share/dotnet
            rm /usr/share/nginx/*.log
            rm -rf /tmp/NugetScratch
        cd <RepoFolder>
        git clean -xdf
