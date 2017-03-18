#!/bin/bash
CONFIGURATION="Debug"
STAGINGDIR="bin"

while [[ $# -gt 1 ]]
do
key="$1"

case $key in
    -c|--configuration)
    CONFIGURATION="$2"
    shift # past argument
    ;;
    -s|--stagingdirectory)
    STAGINGDIR="$2"
    shift # past argument
    ;;
    *)
    echo "Valid arguments are -c (--configuartion) and -s (--stagingdirectory)"
    exit 1          
    ;;
esac
shift # past argument or value 
done

# Restore and build projects
dotnet restore
dotnet build ./src/PartsUnlimitedWebsite --configuration ${CONFIGURATION}
dotnet build ./test/PartsUnlimited.UnitTests --configuration ${CONFIGURATION}

# Run tests
dotnet test ./test/PartsUnlimited.UnitTests -xml testresults.xml

# Publish
dotnet publish ./src/PartsUnlimitedWebsite --framework netcoreapp1.0 --output ${STAGINGDIR} --configuration ${CONFIGURATION} --no-build
