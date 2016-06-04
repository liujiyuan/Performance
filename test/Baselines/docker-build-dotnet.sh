#!/bin/bash 
set -e

pushd $1 > /dev/null
dotnet restore
dotnet build
dotnet publish --configuration release -o .testpublish/ --framework netcoreapp1.0


#Get the lowercase name from the directory. 
name=${PWD##*/}
declare -l name
name=$name; echo "$name" > /dev/null
echo "--Building:$name"

docker build -t $name .
popd
