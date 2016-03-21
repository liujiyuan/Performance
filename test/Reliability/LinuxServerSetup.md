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
    - Install Nginx
    ```sh
    $ sudo apt-get install nginx
    ```
    - Create and enable the site/application in ngnix.  Ngnix is a popular proxy for exposing endpoints on a linux server.
        - Open a new SSH session
        - Create a configuration file for a new site to enable the reverse proxy in the nginx sites-available dir.  You can call the file anything.  We'll call it Performance here.  We'll edit it usng the nano editor.
        
                cd /
                cd etc/nginx/sites-available/
                sudo touch Performance
                sudo nano Peformance
                
        Paste the following into nano and save.      
                
                server {
                    listen 8080;		 
                    location / {
                            proxy_set_header    Host $host;
                            proxy_set_header    X-Real-IP   $remote_addr;
                            proxy_set_header    X-Forwarded-For $proxy_add_x_forwarded_for;
                            proxy_pass  http://localhost:5000;
                            access_log off;
                            error_log $logdir/nginx_error.log error;
                    }
                }
        - Update nginx configuration - /etc/nginx/nginx.conf 
        
                worker_processes  4;  # 2 * Number of CPUs
        
                events {
                worker_connections  2048;  # It's the key to high performance - have a lot of connections available
            }
        
                worker_rlimit_nofile    100000;  # Each connection needs a filehandle (or 2 if you are proxying)
                
                http {
                    keepalive_requests 100000;
                }
        
        Total amount of users you can serve = worker_processes * worker_connections

        - Activate the host in by creating a symbolic link between the nginx sites-available directory and the sites-enabled directory:
                
                sudo ln -s /etc/nginx/sites-available/Performance /etc/nginx/sites-enabled/Performance
        - Restart nginx
                
                sudo service nginx restart
            Now all requests to 8080 will be proxied to 5000 where the Kestrel server listens
            On another machine, test the site with a browser (http://serverName:8080)
            
            Note that if the nginx restart fails, a likely reason is that some other service, such as apache is using port 80.  
            Nginx logs are at:  /var/log/nginx
            To stop Apache,  run the command:  
            
                sudo /etc/init.d/apache2 stop

- Increase max file handles 

        sudo vi /etc/sysctl.conf  
        fs.file-max = 100000
        
        sudo vi /etc/security/limits.conf  and add below the menstioed
        *          soft     nofile         100000
        *          hard     nofile         100000
        
        Reboot
      

# Monitor Server machines

        In another Window - Monitor the processes    	
        top | grep 'nginx\|<YourProcess>' &> $logdir/cpumemlog.log
    
    
# Check for crash and memory leaks
            
             
# Cleanup for daily runs

        Uninstall dotnet 
            
            sudo dpkg -r dotnet
        Delete folders
            rm -rf testapp\HelloWorldMvc\bin\release
            rm -rf ~/.nuget
            rm -rf ~/.dotnet
            rm -rf ~/.dnx
            rm -rf /usr/share/dotnet
            rm /usr/share/nginx/*.log
            rm -rf /tmp/NugetScratch
        cd <RepoFolder>
        git clean -xdf
