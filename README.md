# TrackmaniaExchangeRandomDownloader
 TMX - RandomMapDownloader

# Function
This Programm downloads random maps with the Trackmania Exchange API.

# Disclaimer
This Software is work in progress. I did it pretty fast, so its not the best way to do things.
I will work on it in the future but atm it does what i need it to do.

# Why dont you use the TMX Randomiser instead?
Good Question! I wanted a programm for Multiplayer. The TMX Randomiser is just for soloplay.
This Programm is used to download the .Gpx files directly to an specified folder so they can be used on an Trackmania server.

# Use
to use the Software you need to checkout the Repo (I used VS-Code to develop it). In "Porgramm.cs" you need to change the "mapPath to the folder where you want the map folders to be created.
"\Documents\Trackmania\Maps\My Maps" in my case

The Programm will create a folder with the current date and time.

After that you need to choose the Tags for your maps. If you just type "0" all of the shown tags will be selected.
(FYI: There are some tags that are filtered on default (stuff like LOL, RPG etc.). these are definded in the "etags" Variable. If you want to change that, you need to delete or add tags there. "%2C" is the spacer between the Tags.)

After the Tag selection you need set the number of maps you want.

The Programm then will download random maps and save them in the specified folder.
After that the programm closes.






