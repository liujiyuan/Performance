#!/bin/bash

for i in `seq 1 5`;
do
  loadtest -c 10 --rps 100 -k http://localhost:5000 &
done

while true
do
  curl http://localhost:5000?exit
  sleep 10
done
