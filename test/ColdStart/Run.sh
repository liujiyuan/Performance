#!/bin/bash

## Prerequisites:
##     * Dotnet cli needs to be installed (see installDotnet.sh)
##     * Install git command line client
##
##     Note: The script would create and publish application on the workspace folder specified and will erase that folder prior to start.

source ./SetEnv.sh
source ./SetupPerfApp.sh
source ./PerformTest.sh
