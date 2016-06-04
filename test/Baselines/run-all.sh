#!/bin/sh
set -e

./docker-build.sh BasicNodejs > /dev/null 
./docker-measure.sh BasicNodejs
./docker-rmi.sh BasicNodejs

./docker-build-dotnet.sh ../../testapp/BasicKestrel > /dev/null 
./docker-measure.sh ../../testapp/BasicKestrel
./docker-rmi.sh ../../testapp/BasicKestrel

./docker-build-dotnet.sh ../../testapp/HelloWorldMvc> /dev/null
./docker-measure.sh  ../../testapp/HelloWorldMvc
./docker-rmi.sh ../../testapp/HelloWorldMvc
