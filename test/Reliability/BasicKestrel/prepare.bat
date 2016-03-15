REM run: prepare.bat servername serverport
@echo off

set TEST_DATA_SMALL_FILE=./testdatasmall.data
set TEST_DATA_LARGE_FILE=./testdatalarge.data
set TEST_JSON_SMALL_FILE=./testjsonsmall.data
set TEST_JSON_LARGE_FILE=./testjsonlarge.data


set SETTINGS_TEMPLATE=./settings_template.ubr
set SCENARIO_TEMPLATE=./scenario_template.ubr

set SETTINGS_FILE=./settings.ubr
set SCENARIO_STRESS_FILE=./scenario_stress.ubr
set SCENARIO_RELIABILITY_FILE=./scenario_reliability.ubr

set OLD_SERVER_NAME=FAKEHOSTNAME
set OLD_SERVER_PORT=FAKEPORT
set OLD_REQUEST_DELAY=FAKEDELAYTIME

set NEW_SERVER_NAME=%1
set NEW_SERVER_PORT=%2
set NEW_REQUEST_DELAY=5000

ECHO %NEW_SERVER_NAME%
ECHO %NEW_SERVER_PORT%

ECHO //Generated setting file > %SETTINGS_FILE%
ECHO //Generated stress scenario file targetting at 100 perc CPU usage on an A2 VM > %SCENARIO_STRESS_FILE%
ECHO //Generated reliability scenario file targetting at less than 80 perc CPU usage on an A2 VM > %SCENARIO_RELIABILITY_FILE%

ECHO. > %TEST_DATA_SMALL_FILE%
ECHO. > %TEST_DATA_LARGE_FILE%
ECHO. > %TEST_JSON_SMALL_FILE%
ECHO. > %TEST_JSON_LARGE_FILE%

CALL :ReplaceStringInFile %SCENARIO_TEMPLATE% %SCENARIO_RELIABILITY_FILE%
REM for stress scenario no wait after each request
set NEW_REQUEST_DELAY=0
CALL :ReplaceStringInFile %SCENARIO_TEMPLATE% %SCENARIO_STRESS_FILE%
CALL :ReplaceStringInFile %SETTINGS_TEMPLATE% %SETTINGS_FILE%



REM Prepare init test content
SET TESTDATA_CONTENT=0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
SET /A LOOPIDX=0

:PREPARE_INITDATA
SET /A LOOPIDX=LOOPIDX+1
if %LOOPIDX% GEQ 5 goto :START_GEN
SET TESTDATA_CONTENT=%TESTDATA_CONTENT%+%TESTDATA_CONTENT%
GOTO :PREPARE_INITDATA

REM generate test data file
REM About 4KB for small file, and 20MB for large file
:START_GEN
CALL :GenerateTestTextData 10 %TEST_DATA_SMALL_FILE%
CALL :GenerateTestTextData 5000 %TEST_DATA_LARGE_FILE%
CALL :GenerateTestDataJson 10 %TEST_JSON_SMALL_FILE%
CALL :GenerateTestDataJson 5000 %TEST_JSON_LARGE_FILE%

GOTO :EOF



:ReplaceStringInFile
setlocal enabledelayedexpansion
for /f "tokens=* delims=¶" %%i in ( %1 ) do (
    REM Remove all leading whitespace
    for /f "tokens=* delims= " %%a in ("%%i") do set tmpStr=%%a

    REM Check if tmpStr is defined. It's undefined if the line is all whitespace. 
    REM Need to skip it otherwise the script won't work properly
    if defined tmpStr (
        set str=%%i
        set str=!str:%OLD_SERVER_NAME%=%NEW_SERVER_NAME%!
        set str=!str:%OLD_SERVER_PORT%=%NEW_SERVER_PORT%!
        set str=!str:%OLD_REQUEST_DELAY%=%NEW_REQUEST_DELAY%!
        echo !str! >> %2
    ) else (
        echo. >> %2
    )
)
GOTO :EOF

:GenerateTestTextData REM %1=LOOPCOUNT %2=FILENAME


SET /A LOOPIDX=0
SET /A LOOPCOUNT=%1
:GENFILELOOP
ECHO %TESTDATA_CONTENT%  >> %2
SET /A LOOPIDX=LOOPIDX+1
if %LOOPIDX% GEQ %LOOPCOUNT% goto :EOF
GOTO :GENFILELOOP


:GenerateTestDataJson REM %1=LOOPCOUNT %2=FILENAME

SET /A LOOPIDX=0
SET /A LOOPCOUNT=%1
ECHO [  >> %2
:GENFILELOOP_JSON
ECHO {"Key":%LOOPIDX%,"Value":%TESTDATA_CONTENT%} >> %2
SET /A LOOPIDX=LOOPIDX+1
if %LOOPIDX% GEQ %LOOPCOUNT% goto :END_GEN_JSON
ECHO , >> %2
GOTO :GENFILELOOP_JSON

:END_GEN_JSON
ECHO ] >> %2
GOTO :EOF



:EOF
EXIT /b