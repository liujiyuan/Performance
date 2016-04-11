\\clrmain\tools\setPath

WPR –start GeneralProfile -start FileIO
set startTime1=%time%
curl http://localhost:7001
set endTime1=%time%
WPR -Stop COLD.ETL

WPR –start GeneralProfile -start FileIO
set startTime2=%time%
curl http://localhost:7002
set endTime2=%time%
WPR -Stop WARM.ETL


\\clrmain\tools\setPath

WPR –start GeneralProfile -start FileIO
set startTime1=%time%
curl http://localhost:6001
set endTime1=%time%
WPR -Stop COLD.DOTNET.ETL

WPR –start GeneralProfile -start FileIO
set startTime2=%time%
curl http://localhost:6002
set endTime2=%time%
WPR -Stop WARM.DOTNET.ETL



set DOTNET_PACKAGES_CACHE=C:\Users\shhsu\.nuget\packages\
set HTTP_PLATFORM_PORT=5001
cd C:\apps\HelloMvc1
set startTime1=%time%
START /B dotnet HelloMvc.dll
curl http://localhost:5001
set endTime1=%time%


set HTTP_PLATFORM_PORT=5002
cd C:\apps\HelloMvc2
set startTime2=%time%
START /B dotnet HelloMvc.dll
curl http://localhost:5002
set endTime2=%time%

set DOTNET_PACKAGES_CACHE=
set HTTP_PLATFORM_PORT=5003
cd C:\apps\HelloMvc3
set startTime3=%time%
START /B dotnet HelloMvc.dll
curl http://localhost:5003
set endTime3=%time%

echo %startTime1%
echo %endTime1%
echo %startTime2%
echo %endTime2%
echo %startTime3%
echo %endTime3%

