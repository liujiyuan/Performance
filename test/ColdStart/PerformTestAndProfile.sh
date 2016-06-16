## linux helper script, it perform 1 iteration of test and record the profile
## This script need to be converted into a feature of Measure.ps1

#!/bin/bash

RunScenario () {
    local appLocation=$1
    local port=$2

    if [[ `netstat -tuln | grep ":${port} "` ]]; then
        echo "Port ${port} is in use"
        return
    fi

    local startTime=`date +%s%N | cut -b1-13`
    export HTTP_PLATFORM_PORT=$port
    sudo ./perfcollect collect coldStart -collectsec 15
    "${appLocation}/${targetApp}" &

    local elapsedTime=0
    while [[ $elapsedTime -lt 15000 ]]; do
        local response=`curl -s -f "http://localhost:${port}"`
        currentTime=`date +%s%N | cut -b1-13`
        elapsedTime=`expr $currentTime - $startTime`
        sleep 0.01
        if [[ $response ]]; then
            break
        fi
    done

    echo -n "$elapsedTime" >> $outputFile
}

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
    echo "Output file $outputFile does not exist"
    return
fi

coldSitePort=5000
#warmSitePort=5001

coldSiteLocation="${workspace}/publish/${targetApp}0"
#warmSiteLocation="${workspace}/publish/${targetApp}1"

if [ ! -d "${coldSiteLocation}" ]; then
    echo "Cold site does not exist in ${coldSiteLocation}"
fi

#if [ ! -d "${warmSiteLocation}" ]; then
#    echo "Warm site location does not exist, copying..."
#    cp -rf $coldSiteLocation $warmSiteLocation
#fi

RunScenario $coldSiteLocation $coldSitePort $outputFile
#echo -n "," >> $outputFile
#RunScenario $warmSiteLocation $warmSitePort $outputFile
echo "" >> $outputFile

fuser -k ${coldSitePort}/tcp
#fuser -k ${warmSitePort}/tcp

