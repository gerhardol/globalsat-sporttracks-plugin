REM The plugin guid
SET PLUGIN_GUID=fb88d87e-5bea-4b70-892a-b97530108cfb

REM The minimum SportTracks version
SET MIN_ST_VER=3.1

REM This is the folder which will appear as the subfolder under the Plugins/Installed/<Plugin GUID> folder
SET PLUGIN_FOLDER=globalsat-device-plugin

REM Extract the assembly version. Get filever from http://www.zonefivesoftware.com/tools/util/filever.exe
for /F "delims=" %%F in ('filever.exe ..\bin\x86\release\GlobalsatDevicePlugin.dll 3') do @set VERSION=%%F

REM This is the name of the plugin install package which users will download.
SET INSTALL_PACKAGE_NAME=globalsat-device-plugin

REM Delete the expanded zip-file folder and recreate an empty one
RMDIR /S /Q %PLUGIN_FOLDER%
MD %PLUGIN_FOLDER%

REM Delete the old install package
DEL %INSTALL_PACKAGE_NAME%*
DEL plugin.xml

SET INSTALL_PACKAGE_NAME=%INSTALL_PACKAGE_NAME%-v%VERSION%.st3plugin

REM Copy the contents into the zip-file folder
ECHO Creating contents of build folder %PLUGIN_FOLDER%
COPY ..\bin\x86\release\GlobalsatDevicePlugin.dll %PLUGIN_FOLDER%

REM Create plugin xml file
echo ^<?xml version='1.0' encoding='UTF-8' ?^>^<plugin id='%PLUGIN_GUID%' minimumCommonVersion='%MIN_ST_VER%' version='%VERSION%' /^> > plugin.xml

REM Zip it up recursively with a SportTracks 3.0 Plugin Install Package extension. Make sure the util zip.exe is on your path. A free zip util can be found here: http://www.info-zip.org/Zip.html
ECHO Building plugin install package %INSTALL_PACKAGE_NAME%
zip.exe -r %INSTALL_PACKAGE_NAME% %PLUGIN_FOLDER% plugin.xml

ECHO Done!