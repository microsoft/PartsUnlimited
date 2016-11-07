@if "%SCM_TRACE_LEVEL%" NEQ "4" @echo off

:: ----------------------
:: KUDU Deployment Script
:: Version: 1.0.9
:: ----------------------

:: Prerequisites
:: -------------

:: Verify node.js installed
where node 2>nul >nul
IF %ERRORLEVEL% NEQ 0 (
  echo Missing node.js executable, please install node.js, if already installed make sure it can be reached from current environment.
  goto error
)

:: Setup
:: -----

setlocal enabledelayedexpansion

SET ARTIFACTS=%~dp0%..\artifacts

IF NOT DEFINED DEPLOYMENT_SOURCE (
  SET DEPLOYMENT_SOURCE=%~dp0%.
)

IF NOT DEFINED DEPLOYMENT_TARGET (
  SET DEPLOYMENT_TARGET=%ARTIFACTS%\wwwroot
)

IF NOT DEFINED NEXT_MANIFEST_PATH (
  SET NEXT_MANIFEST_PATH=%ARTIFACTS%\manifest

  IF NOT DEFINED PREVIOUS_MANIFEST_PATH (
    SET PREVIOUS_MANIFEST_PATH=%ARTIFACTS%\manifest
  )
)

IF NOT DEFINED KUDU_SYNC_CMD (
  :: Install kudu sync
  echo Installing Kudu Sync
  call npm install kudusync -g --silent
  IF !ERRORLEVEL! NEQ 0 goto error

  :: Locally just running "kuduSync" would also work
  SET KUDU_SYNC_CMD=%appdata%\npm\kuduSync.cmd
)
IF NOT DEFINED DEPLOYMENT_TEMP (
  SET DEPLOYMENT_TEMP=%temp%\___deployTemp%random%
  SET CLEAN_LOCAL_DEPLOYMENT_TEMP=true
)

IF DEFINED CLEAN_LOCAL_DEPLOYMENT_TEMP (
  IF EXIST "%DEPLOYMENT_TEMP%" rd /s /q "%DEPLOYMENT_TEMP%"
  mkdir "%DEPLOYMENT_TEMP%"
)

IF DEFINED MSBUILD_PATH goto MsbuildPathDefined
SET MSBUILD_PATH=%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe
:MsbuildPathDefined
::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:: Deployment
:: ----------

echo Handling ASP.NET Core Web Application deployment with MSBuild.

:: 1. Restore nuget packages
call :ExecuteCmd nuget.exe restore "%DEPLOYMENT_SOURCE%\PartsUnlimited.sln" -packagesavemode nuspec
IF !ERRORLEVEL! NEQ 0 goto error

pushd "%DEPLOYMENT_SOURCE%\src\PartsUnlimitedWebsite"

  echo .
  echo .
  echo Clearing node modules folder
  echo .
  echo .


call :ExecuteCmd rmdir /S /Q "node_modules"
IF !ERRORLEVEL! NEQ 0 goto error

  echo .
  echo .
  echo Installing node packages
  echo .
  echo .

call :ExecuteCmd npm install 
IF !ERRORLEVEL! NEQ 0 goto error

  echo .
  echo .
  echo Installing Bower
  echo .
  echo .

call :ExecuteCmd npm install bower -g
IF !ERRORLEVEL! NEQ 0 goto error

  echo .
  echo .
  echo Installing Grunt
  echo .
  echo .

call :ExecuteCmd npm install grunt-cli
IF !ERRORLEVEL! NEQ 0 goto error

pushd "./node_modules/grunt"
call :ExecuteCmd npm install glob@^6.0.4 --save
popd

  echo .
  echo .
  echo Running Grunt tasks
  echo .
  echo .

call :ExecuteCmd ".\node_modules\.bin\grunt"
IF !ERRORLEVEL! NEQ 0 goto error

:: 2. Build
call :ExecuteCmd "%MSBUILD_PATH%" "%DEPLOYMENT_SOURCE%\PartsUnlimited.sln" /nologo /verbosity:m /p:AutoParameterizationWebConfigConnectionStrings=false;Configuration=Release;UseSharedCompilation=false %SCM_BUILD_ARGS%
IF !ERRORLEVEL! NEQ 0 goto error

:: 3. Publish
call :ExecuteCmd "%MSBUILD_PATH%" "%DEPLOYMENT_SOURCE%\src\PartsUnlimitedWebsite\PartsUnlimitedWebsite.xproj" /nologo /verbosity:m /t:GatherAllFilesToPublish /p:AutoParameterizationWebConfigConnectionStrings=false;Configuration=Release;UseSharedCompilation=false;PublishOutputPathNoTrailingSlash="%DEPLOYMENT_TEMP%" %SCM_BUILD_ARGS%
IF !ERRORLEVEL! NEQ 0 goto error

:: 4. KuduSync
call :ExecuteCmd "%KUDU_SYNC_CMD%" -v 50 -f "%DEPLOYMENT_TEMP%" -t "%DEPLOYMENT_TARGET%" -n "%NEXT_MANIFEST_PATH%" -p "%PREVIOUS_MANIFEST_PATH%" -i ".git;.hg;.deployment;deploy.cmd"
IF !ERRORLEVEL! NEQ 0 goto error

::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
goto end

:: Execute command routine that will echo out when error
:ExecuteCmd
setlocal
set _CMD_=%*
call %_CMD_%
if "%ERRORLEVEL%" NEQ "0" echo Failed exitCode=%ERRORLEVEL%, command=%_CMD_%
exit /b %ERRORLEVEL%

:error
endlocal
echo An error has occurred during web site deployment.
call :exitSetErrorLevel
call :exitFromFunction 2>nul

:exitSetErrorLevel
exit /b 1

:exitFromFunction
()

:end
endlocal
echo Finished successfully.
