ASP.NET Performance Tests
=========================

AppVeyor: [![AppVeyor](https://ci.appveyor.com/api/projects/status/nnon47ayo1luvi3f/branch/dev?svg=true)](https://ci.appveyor.com/project/aspnetci/Performance/branch/dev)

Travis:   [![Travis](https://travis-ci.org/aspnet/Performance.svg?branch=dev)](https://travis-ci.org/aspnet/Performance)

Performance tests and infrastructure for ASP.NET.

This project is part of ASP.NET Core. You can find samples, documentation and getting started instructions for ASP.NET Core at the [Home](https://github.com/aspnet/home) repo.

## Dependencies
Your system must have `gulp`, `bower` and `loadtest` readily available from the command line. Feel free to install them in any way you want. If you don't know how to go about it, here are some suggestions.

### Windows
  * [npm](https://nodejs.org/en/)
  * gulp: `npm install -g gulp`
  * bower: `npm install -g bower`
  * loadtest: `npm install -g loadtest`

To build, execute `.\build.cmd`.

### Ubuntu
First, update the package index by running `sudo apt-get update`. Follow by installing basic libraries required by all ASP.NET projects on Linux as described [here](https://docs.asp.net/en/latest/getting-started/installing-on-linux.html).

Then proceed to install `gulp`, `bower` and `mono`:
  * unzip: `sudo apt-get install unzip`
  * nodejs-legacy: `sudo apt-get install nodejs-legacy`
  * npm: `sudo apt-get install npm`
  * gulp: `sudo npm install -g gulp`
  * bower: `sudo npm install -g bower`
  * loadtest: `sudo npm install -g loadtest`
  * [Mono](http://www.mono-project.com/docs/getting-started/install/linux/#debian-ubuntu-and-derivatives) (use package mono-complete)

To build, execute `./build.sh`.

If you run into the "Cannot handle address family xxxxx" error, please refer to [mono bug 30018](https://bugzilla.xamarin.com/show_bug.cgi?id=30018).

### OSX
  * [brew](http://brew.sh/), and run `brew update` after install to update the package index.
  * npm: `brew install npm`
  * gulp: `npm install -g gulp`
  * bower: `npm install -g bower`
  * loadtest: `npm install -g loadtest`
  * [Mono](http://www.mono-project.com/docs/getting-started/install/mac/)

To build, execute `./build.sh`.

## [Microbenchmark tests](testapp)
These are targeted tests covering specific feature areas. The tests are quite self explanatory. You can use the `loadtests.ps1` script to load test the specific scenario. We use the [loadtest npm module](https://www.npmjs.com/package/loadtest) in these scripts to keep them as simple as possible.

For e.g. here is how to run the Hello World MVC microbenchmark.

```powershell
cd testapp\HelloWorldMvc
dotnet restore
dotnet run
```

Run the load test client
```powershell
.\loadtest.ps1
```

A `loadtest.sh` script will also be added to cover cross platform testing.
