#!/bin/bash

SERVER_URI=$1
NUM_CLIENTS=$2
NUM_REQ_PER_ITERATION=$(($NUM_CLIENTS * 10))
TEST_DATA_TEXT_FILE='testdatatext.data'
TEST_DATA_MULTIPART_FILE='testdata_multipartform.data'
NEW_MULTIPART_FORM_BOUNDARY='---MultiPartFormBoundary'

loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/Home
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/Home/Contact


loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/Account/Login
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m POST "https://$SERVER_URI/Account/Login?email=testemail@test.com&Password=123456&RememberMe=true"
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m POST "https://$SERVER_URI/Account/Register?email=testemail2@test.com&Password=123456&ConfirmPassword=123456"



loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/JsonContent/GetObjects?count=1
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/JsonContent/GetObjects?count=1000


loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/TextContent/Get500B
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/TextContent/Get2KB
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/TextContent/Get500KB
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/TextContent/GetContent?size=1000000
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/TextContent/GetContent?size=3000000
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET https://$SERVER_URI/TextContent/GetContent?size=8000000
loadtest --insecure -c $NUM_CLIENTS -p $TEST_DATA_TEXT_FILE -n $NUM_REQ_PER_ITERATION -m POST -T "application/x-www-form-urlencoded" https://$SERVER_URI/TextContent/AddContent
loadtest --insecure -c $NUM_CLIENTS -p $TEST_DATA_MULTIPART_FILE -n $NUM_REQ_PER_ITERATION -m POST -T "multipart/form-data; boundary=$NEW_MULTIPART_FORM_BOUNDARY" https://$SERVER_URI/TextContent/AddContent



loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET "https://$SERVER_URI/TextContentRelay/GetContent?size=1000000&timeoutMs=10000"
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET "https://$SERVER_URI/TextContentRelay/GetContent?size=1000000&timeoutMs=100"
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET "https://$SERVER_URI/TextContentRelay/GetContent?size=10000&timeoutMs=0"
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET "https://$SERVER_URI/TextContentRelay/GetContentChained?size=1000000&timeoutMs=30000&numReqChained=5"
loadtest --insecure -c $NUM_CLIENTS -n $NUM_REQ_PER_ITERATION -m GET "https://$SERVER_URI/TextContentRelay/GetContentChained?size=1000000&timeoutMs=1000&numReqChained=5"
loadtest --insecure -c $NUM_CLIENTS -p $TEST_DATA_TEXT_FILE -n $NUM_REQ_PER_ITERATION -m POST -T "application/x-www-form-urlencoded" "https://$SERVER_URI/TextContentRelay/AddContentChained?timeoutMs=1000&numReqChained=10"
loadtest --insecure -c $NUM_CLIENTS -p $TEST_DATA_MULTIPART_FILE -n $NUM_REQ_PER_ITERATION -m POST -T "multipart/form-data; boundary=$NEW_MULTIPART_FORM_BOUNDARY" "https://$SERVER_URI/TextContentRelay/AddContentChained?timeoutMs=1000&numReqChained=10"
