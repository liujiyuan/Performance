set SERVER_URI=%1
set NUM_CLIENTS=%2
set /A NUM_REQ_PER_ITERATION=10 * NUM_CLIENTS
set TEST_DATA_TEXT_FILE=testdatatext.data
set TEST_DATA_MULTIPART_FILE=testdata_multipartform.data
set NEW_MULTIPART_FORM_BOUNDARY=---MultiPartFormBoundary

cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/Home
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/Home/Contact


cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/Account/Login
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m POST "https://%SERVER_URI%/Account/Login?email=testemail@test.com&Password=123456&RememberMe=true"
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m POST "https://%SERVER_URI%/Account/Register?email=testemail2@test.com&Password=123456&ConfirmPassword=123456"



cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/JsonContent/GetObjects?count=1
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/JsonContent/GetObjects?count=1000


cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/TextContent/Get500B
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/TextContent/Get2KB
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/TextContent/Get500KB
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/TextContent/GetContent?size=1000000
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/TextContent/GetContent?size=3000000
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET https://%SERVER_URI%/TextContent/GetContent?size=8000000
cmd /c loadtest --insecure -c %NUM_CLIENTS% -p %TEST_DATA_TEXT_FILE% -n %NUM_REQ_PER_ITERATION% -m POST -T "application/x-www-form-urlencoded" https://%SERVER_URI%/TextContent/AddContent
cmd /c loadtest --insecure -c %NUM_CLIENTS% -p %TEST_DATA_MULTIPART_FILE% -n %NUM_REQ_PER_ITERATION% -m POST -T "multipart/form-data; boundary=%NEW_MULTIPART_FORM_BOUNDARY%" https://%SERVER_URI%/TextContent/AddContent



cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET "https://%SERVER_URI%/TextContentRelay/GetContent?size=1000000&timeoutMs=10000"
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET "https://%SERVER_URI%/TextContentRelay/GetContent?size=1000000&timeoutMs=100"
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET "https://%SERVER_URI%/TextContentRelay/GetContent?size=10000&timeoutMs=0"
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET "https://%SERVER_URI%/TextContentRelay/GetContentChained?size=1000000&timeoutMs=30000&numReqChained=5"
cmd /c loadtest --insecure -c %NUM_CLIENTS% -n %NUM_REQ_PER_ITERATION% -m GET "https://%SERVER_URI%/TextContentRelay/GetContentChained?size=1000000&timeoutMs=1000&numReqChained=5"
cmd /c loadtest --insecure -c %NUM_CLIENTS% -p %TEST_DATA_TEXT_FILE% -n %NUM_REQ_PER_ITERATION% -m POST -T "application/x-www-form-urlencoded" "https://%SERVER_URI%/TextContentRelay/AddContentChained?timeoutMs=1000&numReqChained=10"
cmd /c loadtest --insecure -c %NUM_CLIENTS% -p %TEST_DATA_MULTIPART_FILE% -n %NUM_REQ_PER_ITERATION% -m POST -T "multipart/form-data; boundary=%NEW_MULTIPART_FORM_BOUNDARY%" "https://%SERVER_URI%/TextContentRelay/AddContentChained?timeoutMs=1000&numReqChained=10"