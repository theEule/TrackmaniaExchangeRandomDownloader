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
To use the software just download the "Bin\Release\net6.0\publish" folder, save it somewhere and run the "TrackmaniaExchangeRandomDownloader.exe"

The programm will ask you to set the Path of the Map folder (or the path where you want your random maps to be stored). this will be saved in the config.json file. If you want to change the path, just delete that file.

The Programm will create a folder with the current date and time.

After that you need to choose the Tags for your maps. If you just type "0" all of the shown tags will be selected.
(FYI: There are some tags that are filtered on default (stuff like LOL, RPG etc.). these are definded in the "etags" Variable. If you want to change that, you need to delete or add tags there. "%2C" is the spacer between the Tags.)

After the Tag selection you need set the number of maps you want.

The Programm then will download random maps and save them in the specified folder.
After that the programm closes.






