#
Server & Client Setup
- See [this] (../README.md)
  
Clone and build
- Log on to Azure VM
- git clone https://github.com/aspnet/MusicStore.git
- Linux: 
        Apt-get install unzip gettext
        ./build.sh
- Windows: run build.cmd
- Remove %Programfiles%\dotnet path from system path variable
- Add C:\Users\<USER>\AppData\Local\Microsoft\dotnet\cli\bin to User Path
- cd MusicStore\src\MusicStore

Configuration and Additional steps
- Create SQL Azure database
- Open config.json
- Update Connectionstring with the newly created SQL Azure database connection string.
- In Ibiza Portal, add firewall rule for SQL Azure server for this machine.

- dotnet restore
- dotnet publish --configuration Release
- Host it in IIS - See [this](../README.md)
Tests
- MusicStoreBasic.ubr