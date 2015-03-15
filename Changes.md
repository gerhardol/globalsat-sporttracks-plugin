This page describes the changes between plugin releases. See the SVN change log for details.

### Changes ###

3.2.255 2014-06-10
  * Support for GB-580P Firmware F-GGB-2O-1402241, recording a temperature track. The firmware breaks importing lap information with previous releases.
> Note: When transferring an activity without importing first, the format is unknown and the user gets a popup.
> Note 2: You cannot import settings from a previous firmware to a newer firmware, the update is silently ignored: [Configurations](Features#other-functions).

3.2.248 2014-05-10 (Plugin Catalog)
  * Separate option to insert pauses when speed-GPS differs. Previously the global option "Split Activity if GPS gap" was used (this option has no other use in Globalsat).
  * Support for a-rival SpoQ in separate plugin from same code base

3.2.241 2014-03-12
  * Support for GB-1000 bike computer

3.2.236 2013-11-10 (Plugin Catalog)
  * Transfer error messages to Waypoints plugin when waypoints and routepoints exceed device capacity.
  * Allow sending the waypoints in a route for GB-580, even if routes are not supported.

3.2.229 2013-06-17 (Plugin Catalog)
  * GB-580 barometric elevation could have a bad point last (similar to GPS), avoid when importing.
  * If starting recording before GPS fix, time is incorrect (for instance 2009-02-15 for the GB-580). If this has occurred more than once, it can be hard to find the activity when importing (or be skipped by ST import). To improve this, the _latest_ activity is set to the time at import so the activities can easily be found (and manually corrected) after import.
  * Add distance information for laps. This is useful if there is no GPS track, for instance GB-580P with only speed data.
  * Set ElevationChanged for laps if elevation but no GPS exists. This is a workaround as ST prefers the lap data to the separate elevation.
  * When sending routes to the GH-625XT (using the Waypoints plugin) do not include separate waypoints, not needed.
  * Support for importing from myCiclo DCY-580P.

3.2.219 2013-02-04 (Plugin Catalog)
  * GH-561 support, for waypoints only (using Waypoints plugin) [Supported Devices](Features#supported-devices)
  * No longer required to close import dialog if importing from GsSport device and device was not connected at first import attempt.
  * Error message when exporting route to 505 updated: Not supported by device
  * Reworked the internal error message passing, also to Waypoints plugin (must be updated too)
  * Better estimation of remaining recording time in device
  * Changed minimum remaining time to warn at import from 3 to 5h

3.2.211 2012-12-10 (Plugin Catalog)
  * It was possible to get an exception in device configuration if no device was connected.

3.2.210 2012-12-02
  * Major internal redesign to show Device Configuration and Screen Capture for Globalsat devices in Import Device Configuration instead of in Settings subpages.
  * Show approximate remaining recording time in device configuration dialog
  * Possibility to the delete activities in the device from the device configuration dialog
  * If the estimated recording time is less than 3h, show a popup.
  * If no Globalsat device connected after installing 3.2.190, ST Preferences were not stored.

3.2.191 2012-10-22
  * If HoursOffset was set for GH-625XT, GB-580P, GH-505, the activity offset was not used. (HourOffset can be used if the device time zone is different from the PC.)
  * GH-625M and GH-615 tracks were always imported, regardless if "Import new data only" was checked

3.2.190 2012-10-21
  * Cache last used COM port used, to speed up device detection
  * Increase communication timeouts (from 1s to 4s by default), to avoid occasional communication failures seen on 625M (580P had 2s before). The first import on a certain COM port(and waypoint handling) will be slower, subsequent imports will be slightly faster due to caching.

> 3.2.186 2012-09-26 (Plugin Catalog)
  * Some refined messages at import errors

3.2.185 2012-07-09
  * GB-580 recording fix

3.2.181 2012-07-09
  * Update for import Distance track

3.2.178 2012-06-05
  * Handle non-GPS activities better. This especially applies to GB-580, that could have Elevation and Speed data also if no GPS.
  * Import Distance track (as before 3.2.173).
  * Auto-rotate screenshot for GB-580, based on current setting
  * Screenshot save image format is selectable

3.2.173 2012-05-13
  * Distance track (calculated from Speed) is no longer imported. This data was not used (by default) before ST 3.1.4515, but the data is since then used by default. the Globalsat Distance track has lower accuracy than the GPS Distance track.
  * Rotate screenshot for GB-580
  * Increase height when importing screenshots for GB-580

3.2.167 2012-04-30 (Plugin Catalog)
  * Prepare for plugin catalog

3.2.166 2012-04-06
  * Resend packets at no response from device, to be able to send packets to GB-580
  * GH-561 has no support. Speed up detection of device by only query for 561 if 561 is configured: Add GS\_Sport import device, stop ST, add GH-561 to AllowedId (comma separated string of all allowed ids)
  * Minor layout updates in Settings, use status line when export/import configuration to device.

3.2.157 2012-03-21
  * Set errortext when failure to upload activity (in Waypoints plugin)
  * Progressbar when sending activities to Globalsat (in Waypoints plugin)

3.2.149 2012-03-08
  * Update images for GH-625XT and GS-Sport
  * Increase detection timeout for GB-580P

3.2.144 2012-03-04
  * Increase read timeouts for GB-580P, the device hardly worked previously
  * Remove Chinese translation as it seem to create problems for some users. Only one string translated anyway.
  * Device settings were incorrectly saved. This increase the time to detect when no device were detected as all serial speeds were tried, as well as slower start of import for 625M.
> > Remove all existing Globalsat devices, add them again to update the settings.
  * Popup when ExportActivityAsRoute to GH-625M, GB-580P. (GUI for action is in Waypoints plugin, the popup is initiated in this plugin.)

3.2.138 2012-02-19
  * When a device is connected and turned on but not prepared to communicate, try to inform the user. Requires updated Waypoints plugin for waypoints/activities/routes.
  * Minor translation update - some added to online spreadsheet
  * Max 99 (not 100) waypoints could be deleted

3.2.136 2012-02-17
  * Get screenshot for GH-625M
  * Read/write waypoints for 561
  * Elevation always 0 for waypoints

3.2.125 2012-02-01
  * Incorrect offsets when writing activities to 505, 580

3.2.123 2012-01-29
  * Export activity for GH625XT

3.2.113 2012-01-27
  * Routes to GH625XT cannot have "-" in the name, name filtered.
  * Estimated pauses in complete seconds only, to avoid changing activity time.
  * GH625XT export

3.2.108 2011-12-17
  * Pause handling updated. See [Pause Handling](Features#special-features).

3.2.105 2011-12-07
  * Plugin GUID was changed after 3.1.3. Uninstall previous 3.2.**versions prior to installing this version.
  * Pauses were inserted in some situation, but ST did not save the pauses if they contained milliseconds. Workaround done, pauses are now always estimated to full seconds.
  * Lap info: Cadence and Power were added as 0. This was only visible in .fitlog exports.
  * Lap Distance markers set.
  * Possibility to cancel import for 625XT.
  * Import problems for GH-625M in 3.2.94**

3.2.94 2011-12-01
See [Features](Features) for major changes.
  * Many changes to device handling. For instance can Globalsat devices be autodetected, using GS-Sport import device. This handling is always used when the plugin is accessed from other plugins.
  * Implemented known Globalsat protocols (several moved from Waypoints). Some functionality is accessible from this plugin, some from other plugins. See [Features](Features).
  * Device check: The plugin checks that the identification matches for the device you try to import from. If you have more than one device connected, the plugin will now import from the correct device.
  * Some tweaking of device detection, should be a little faster.
  * SendRoutes did probably not work at all before (except for KeyMaze?)

3.2.12 2011-10-12
  * GH-625XT support

3.1.3 2011-07-01
  * Latest update from ZoneFiveSoftware


> ### Feedback ###
For patches, bugreports or feature suggestions, use the Google Code issue list.
For other feedback please use the SportTracks forum or this wiki.