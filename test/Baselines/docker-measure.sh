#!/bin/bash

RunScenario () 
{
    local imagename=$1
    local port=$2
    if [[ `netstat -tuln | grep ":${port} "` ]]; then
        echo "Port ${port} is in use"
        return
    fi

    local startTime=`date +%s%N | cut -b1-13`
#    ~/.dotnet/dotnet ${appLocation}/${targetApp}.dll urls=http://+:$port/ &
    docker run -d --name $imagename -p $port:$port/tcp  $imagename  > /dev/null 
    local failed=1
    local elapsedTime=0
    while [[ $elapsedTime -lt 15000 ]]; do
        local response=`curl -s -f "http://localhost:${port}"`
        currentTime=`date +%s%N | cut -b1-13`
        elapsedTime=`expr $currentTime - $startTime`
        sleep 0.01
        if [[ $response ]]; then
            failed=0;
            break
        fi        
    done
    if [[ "$failed" == 1 ]] ; then
       echo "Timed out"
       exit 1
    fi 
    echo  "$elapsedTime" 
    docker rm -f $imagename  > /dev/null
}


#Jump into the directory
pushd  $1 > /dev/null

# Get the lowercase name from the directory. 
name=${PWD##*/}
declare -l name
name=$name; echo "$name" > /dev/null
  
echo "--Scenario:$name"
for i in `seq 1 5`;
do
    RunScenario $name 5000   
done 

popd  > /dev/null
