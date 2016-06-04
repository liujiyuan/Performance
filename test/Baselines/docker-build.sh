#!/bin/bash 
set -e

pushd  $1 > /dev/null

# Get the lowercase name from the directory. 
name=${PWD##*/}
declare -l name
name=$name; echo "$name" > /dev/null
echo "--Building:$name"

#build the docker image with the dockername
docker build -t $name . | grep Success > /dev/null
popd  > /dev/null
