ASP.NET Core Connection Load Test
=========================

## Test setup

- Azure VM: Standard D3
- Wcat client: refer to .ubr files 
- IIS queue limit: 20k
- IIS "appConcurrentRequestLimit” value: 20k
- MaxUserPort registry value: 65534

Here is the [link](https://technet.microsoft.com/en-us/library/dd425294(v=office.13).aspx) to change the “appConcurrentRequestLimit” setting.

Here is the [link](https://technet.microsoft.com/en-us/library/cc938196.aspx) regarding the MaxUserPort registry.

## Run test

Spin up 2 lab machines or Azure VMs. One machine will be generating the contant load and the other one generating the bursty load.

Command to kick off wcat test: `wcatrun.bat`

We should run the 2 scenarios belows at the same time:

- Constant load example (in wcatrun.bat): 
  `.\wcctl.exe -t .\scenario-contant-load.ubr -f .\settings.ubr -p 80 -c 1 -v 10000 -o .\out.xml -x`
  
- Bursty load example (in wcatrun.bat): 
  `.\wcctl.exe -t .\scenario-bursty-load.ubr -f .\settings.ubr -p 80 -c 1 -v 1000 -o .\out.xml -x`