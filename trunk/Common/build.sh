#!/bin/sh
#  Copyright (C) 2011 Gerhard Olsson
# 
# This library is free software; you can redistribute it and/or
# modify it under the terms of the GNU Lesser General Public
#  License as published by the Free Software Foundation; either
#  version 2 of the License, or (at your option) any later version.
# 
# This library is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
#  Lesser General Public License for more details.
# 
#  You should have received a copy of the GNU Lesser General Public
# License along with this library. If not, see <http://www.gnu.org/licenses/>.

# $(PluginId)
guid=$1
# $(StPluginVersion)
StPluginVersion=$2
# $(ProjectName)
ProjectName=$3
# $(ProjectDir)
ProjectDir=$4
# $(StPluginPath)
StPluginPath=$5
# $(TargetDir)
TargetDir=$6
# $(ConfigurationType)
ConfigurationType=$7

# ST version, for plugin.xml file
#if [ "$StPluginVersion" == "2" ]; then
#  StVersion=2.1.3478
#  StPluginVersionExt=st2plugin
#else
  StVersion=3.0.4205
  StPluginVersionExt=st3plugin
#fi

# Plugin version
PluginVersion=0.1
#perl=C:\Cygwin\bin\perl.exe
#IF NOT EXIST "%perl%" GOTO endversion 
#set cygwin=nodosfilewarning
#tempfile=$TEMP\$ProjectName-stpluginversion.tmp
#perl -ne "if(/^^\[assembly: AssemblyVersion\(.([\.\d]*)(\.\*)*.\)\]/){print $1;}" $ProjectDir\Properties\AssemblyInfo.cs > $tempfile
#set /p PluginVersion= < %tempfile%
#rem DEL %tempfile%
#:endversion

stPlgFile=%ProjectDir%%ProjectName%-%PluginVersion%.st%StPluginVersion%plugin


#REM To move a .stplugin to common area, create environment variable (or set it below)
#REM set stPlgoutdir=g:\Users\go\dev\web



# This is the folder which will appear as the subfolder under the Plugins/Installed/<Plugin GUID> folder
PLUGIN_FOLDER=GlobalsatImportPlugin

# This is the name of the plugin install package which users will download. It must have an extension of .st3plugin to play nicely with the built-in plugin manager
#INSTALL_PACKAGE_NAME=globalsat-export-plugin-v3.0.NNNN.st3plugin
INSTALL_PACKAGE_NAME=globalsat-import-plugin-$PluginVersion.$StPluginVersionExt


#Delete the expanded zip-file folder and recreate an empty one
rm -fr $PLUGIN_FOLDER
mkdir $PLUGIN_FOLDER

# Delete the old install package
rm $INSTALL_PACKAGE_NAME

# Copy the contents into the zip-file folder
echo Creating contents of build folder $PLUGIN_FOLDER
#cp bin/x86/release/plugin.xml $PLUGIN_FOLDER
#cp bin/x86/release/GlobalsatDevicePlugin.dll $PLUGIN_FOLDER
#cp ../../dist/plugin.xml $PLUGIN_FOLDER
echo "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" >  $PLUGIN_FOLDER/plugin.xml
echo "<plugin id=\"$guid\" minimumCommonVersion=\"$StVersion\" />" >> $PLUGIN_FOLDER/plugin.xml


cp GlobalsatDevicePlugin.dll $PLUGIN_FOLDER
# Zip it up recursively with a SportTracks 3.0 Plugin Install Package extension. Make sure the util zip.exe is on your path. A free zip util can be found here: http://www.info-zip.org/Zip.html
echo Building plugin install package $INSTALL_PACKAGE_NAME
zip -r $INSTALL_PACKAGE_NAME $PLUGIN_FOLDER

echo Done!
