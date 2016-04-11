#!/bin/bash

## usage: startloadtest.sh [Server:port] [NumConcurrentClients] [NumIterations]

SERVER_URI=$1
NUM_CLIENTS=$2
NUM_ITERATIONS=$3
#NUM_REQ_PER_ITERATION=$(($NUM_CLIENTS * 10))
echo $NUM_REQ_PER_ITERATION
TEST_DATA_TEXT_FILE="testdatatext.data"
TEST_DATA_MULTIPART_FILE="testdata_multipartform.data"
NEW_MULTIPART_FORM_BOUNDARY="---MultiPartFormBoundary"


sudo apt-get install nodejs
sudo npm install -g loadtest

## Generate test data file
LOOPIDX=0
LOOPCOUNT=50
TESTDATA_CONTENT='01234567890123456789012345678901234567890012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789&'
echo ""> $TEST_DATA_TEXT_FILE
while [ $LOOPIDX -lt $LOOPCOUNT ]; do
    if [ $LOOPIDX -eq 0 ]; then
        echo "content=">> $TEST_DATA_TEXT_FILE
    else
        echo "content$LOOPIDX=">> $TEST_DATA_TEXT_FILE
    fi

    echo $TESTDATA_CONTENT  >> $TEST_DATA_TEXT_FILE
    let LOOPIDX=$LOOPIDX+1
done


## Generate test data file for multi-part form test
LOOPIDX=0
LOOPCOUNT=50
echo -e "\r" > $TEST_DATA_MULTIPART_FILE
while [ $LOOPIDX -lt $LOOPCOUNT ]; do
    echo -e "--$NEW_MULTIPART_FORM_BOUNDARY\r">> $TEST_DATA_MULTIPART_FILE
    if [ $LOOPIDX -eq 0 ]; then
        echo -e 'Content-Disposition: form-data; name="content"\r'>> $TEST_DATA_MULTIPART_FILE
    else
        echo -e 'Content-Disposition: form-data; name="content'$LOOPIDX'"\r'>> $TEST_DATA_MULTIPART_FILE
    fi
    echo -e "\r" >> $TEST_DATA_MULTIPART_FILE
    echo -e "$TESTDATA_CONTENT\r"  >> $TEST_DATA_MULTIPART_FILE
    echo -e "\r" >> $TEST_DATA_MULTIPART_FILE
    let LOOPIDX=$LOOPIDX+1
done
echo -e "--$NEW_MULTIPART_FORM_BOUNDARY--\r">> $TEST_DATA_MULTIPART_FILE
echo -e "\r" >> $TEST_DATA_MULTIPART_FILE

## Run test in a loop

LOOPIDX=0
echo "start test"
while [ $LOOPIDX -lt $NUM_ITERATIONS ]; do
    ./runtestscenarios.sh $SERVER_URI $NUM_CLIENTS
    let LOOPIDX=$LOOPIDX+1
done
