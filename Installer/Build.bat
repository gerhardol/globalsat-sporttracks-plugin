@ECHO OFF

REM This is the folder which will appear as the subfolder under the Plugins/Installed/<Plugin GUID> folder
SET PLUGIN_FOLDER=GlobalsatDevicePlugin

REM This is the name of the plugin install package which users will download. It must have an extension of .st3plugin to play nicely with the built-in plugin manager
SET INSTALL_PACKAGE_NAME=globalsat-device-plugin-v3.0.NNNN.st3plugin

REM Delete the expanded zip-file folder and recreate an empty one
RMDIR /S /Q %PLUGIN_FOLDER%
MD %PLUGIN_FOLDER%

REM Delete the old install package
DEL %INSTALL_PACKAGE_NAME% 

REM Copy the contents into the zip-file folder
ECHO Creating contents of build folder %PLUGIN_FOLDER%
COPY ..\bin\x86\release\plugin.xml %PLUGIN_FOLDER%
COPY ..\bin\x86\release\GlobalsatDevicePlugin.dll %PLUGIN_FOLDER%

REM Zip it up recursively with a SportTracks 3.0 Plugin Install Package extension. Make sure the util zip.exe is on your path. A free zip util can be found here: http://www.info-zip.org/Zip.html
ECHO Building plugin install package %INSTALL_PACKAGE_NAME%
zip.exe -r %INSTALL_PACKAGE_NAME% %PLUGIN_FOLDER%

ECHO Done!