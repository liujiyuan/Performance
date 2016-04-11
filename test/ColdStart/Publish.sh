#!/bin/bash

if [ -z "$targetApp" ]; then
    echo "Target application is not set"
    return
fi

if [ -z "$workspace" ]; then
    echo "Workspace dir is not set"
    return
fi

if [ -z "$outputFile" ]; then
    echo "Output file is not set"
    return
fi

if [ ! -d "$workspace" ]; then
    echo "Workspace $workspace does not exist"
    return
fi

## clone performance branch
performanceReproPath="${workspace}/Performance"

rm -rf $performanceReproPath

git clone "https://github.com/aspnet/Performance.git" $performanceReproPath

appLocation="$performanceReproPath/testapp/$targetApp"

if [ ! -d "$appLocation" ]; then
    echo "$appLocation is not a valid performance app"
    return
fi

## publish targeted application
pushd $appLocation
dotnet restore
publishLocation="../../../publish"
rm -rf $publishLocation
publishAppLocation=${publishLocation}/${targetApp}0
dotnet publish -o $publishAppLocation --framework "netstandardapp1.5" --configuration release
popd

