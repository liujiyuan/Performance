#!/bin/bash

targetApp="HelloWorldMvc"
framework="netcoreapp1.0"

while getopts ":t:f:m:" opt; do
    case $opt in
        t) targetApp="$OPTARG"
        ;;
        f) framework="$OPTARG"
        ;;
        m) echo "TODO: implement the perf measurement hook (perfcollect)"
        ;;
        \?) echo "Invalid option -$OPTARG" >&2
        ;;
    esac
done

if [[ $framework -eq "net451" ]]; then
    echo "We do not currently test $framework scenario"
    return
fi

RunScenario () {
    local appLocation=$1
    local port=$2

    if [[ `netstat -tuln | grep ":${port} "` ]]; then
        echo "Port ${port} is in use"
        return
    fi

    local startTime=`date +%s%N | cut -b1-13`
    ~/.dotnet/dotnet ${appLocation}/${targetApp}.dll urls=http://+:$port/ &

    local elapsedTime=0
    while [[ $elapsedTime -lt 15000 ]]; do
        local response=`curl --write-out '%{http_code}' --silent --output /dev/null -f "http://localhost:${port}"`
        currentTime=`date +%s%N | cut -b1-13`
        elapsedTime=`expr $currentTime - $startTime`
        sleep 0.01
        case $response in
            0) ## Do nothing, service is not started yet
            ;;
            200)
            break
            ;;
            \?)
            echo "Invalid response code $response"
            return
            ;;
        esac
    done

    echo -n "$elapsedTime" >> $outputFile
}

repoRoot=`git rev-parse --show-toplevel`
scriptRoot="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
source $scriptRoot/SetEnv.sh

if [ -z "$targetApp" ]; then
    echo "Target application is not set"
    return
fi

if [ -z "$workspace" ]; then
    echo "Workspace dir is not set"
    return
fi

if [ ! -d "$workspace" ]; then
    echo "Workspace directory $workspace does not exist"
    return
fi

if [ -z "$outputFile" ]; then
    echo "Output File is not set"
    return
fi

if [ ! -f "$outputFile" ]; then
    ## File does not exist yet, write output file header
    echo "Cold,Warm,NoCache" > $outputFile
fi

coldSitePort=5001
warmSitePort=5002
warmNoCachePort=5003

publishLocation="${workspace}/publish/${targetApp}"
coldSiteLocation="${workspace}/publish/${targetApp}Cold"
warmSiteLocation="${workspace}/publish/${targetApp}WarmPkgCache"
warmNoCacheLocation="${workspace}/publish/${targetApp}WarmNoPkgCache"

if [ ! -d "${publishLocation}" ]; then
    echo "Site does not exist in ${publishLocation}, did you run SetupPerfApp.sh?"
    return
fi

if [ ! -d "${coldSiteLocation}" ]; then
    echo "Cold site location ${coldSiteLocation} does not exist, copying..."
    cp -rf $publishLocation $coldSiteLocation
fi

if [ ! -d "${warmSiteLocation}" ]; then
    echo "Warm site location ${warmSiteLocation} does not exist, copying..."
    cp -rf $publishLocation $warmSiteLocation
fi

if [ ! -d "${warmNoCacheLocation}" ]; then
    echo "Warm site location ${warmNoCacheLocation} does not exist, copying..."
    cp -rf $publishLocation $warmNoCacheLocation
fi

export DOTNET_PACKAGES_CACHE="${HOME}/.nuget/packages"
RunScenario $coldSiteLocation $coldSitePort $outputFile
echo -n "," >> $outputFile
RunScenario $warmSiteLocation $warmSitePort $outputFile
echo -n "," >> $outputFile
unset DOTNET_PACKAGES_CACHE
RunScenario $warmNoCacheLocation $warmNoCachePort $outputFile
echo "" >> $outputFile

fuser -k ${coldSitePort}/tcp
fuser -k ${warmSitePort}/tcp
fuser -k ${warmNoCachePort}/tcp

