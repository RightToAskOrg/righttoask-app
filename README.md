# Right to Ask app

This is a very basic start for the RightToAsk app, intended to be used for testing the UI to gather user feedback.

## Setup Instructions
On Mac or Windows, you'll need something to compile Xamarin: Visual Studio, VS Code, or JetBrains Rider. Both Android and iOS should work (the latter if you're compiling on a Mac), but UWP is not currently expected to work. You'll need the Xamarin.Essentials and Portable.Bouncycastle Nuget packages.

On Linux, follow @eleanor-em's [Xamarin-on-Linux setup instructions](XamarinOnLinux.md).

## Setting up the Right to Ask Server Files
The app expects a local instance of [the Right To Ask Server](https://github.com/RightToAskOrg/right_to_ask_server). Remote instances should probably also work but have not been tested. If you don't use the server, the app will still load but will not have access to the data it needs about Parliamentary structures etc.

Check that the server is running by visiting
(http://localhost:8099)[
http://localhost:8099] via your web browser. You should see a web page headed "Right To Ask API" and some html links.

We'll need 2 files from the Right To Ask server:
- MPs.json
- PublicServerKey

With the RightToAsk server running, you can get the MPs.json file from
(http://localhost:8099/MPs.json)[http://localhost:8099/MPs.json].  Add it to the righttoask-app project's "Resources" folder (righttoask-app/RightToAskClient/RightToAskClient/RightToAskClient/Resources). In your IDE, under properties for that file, set the build action to be "Embedded Resource". 

Next you'll need to create a "PublicServerKey" file. 
After running the Right To Ask server, you should be able to go to localhost:8099 and under the "Info" section click on the 3rd link: "Get the server public key, base64 encoded 32 bytes ED25519". 
Copy the string key without the quotation marks into the empty "PublicServerKey" file. 
Place that file into the app's Resources folder, next to "MPs.json", and set the build action to be "Embedded Resource".

## Setting up Geoscape access
We'll need one file for Geoscape:
- GeoscapeAPIKey

The app uses Geoscape's electorate-identifying service, which tells the user which electorates they live in when they enter their address. You may need a Geoscape API key - the [Free account](https://geoscape.com.au/developer/) should suffice. (Geoscape's terms of service can change, and it is possible that this will work for free without an API key, but if it's not working for you, try getting one.)

Then, exactly as for the PublicServerKey, make a file in the Resources directory called GeoscapeAPIKey, copy your Geoscape API Key into the GeoscapeAPIKey file, without the quotation marks, and set the build action to "Embedded Resource".

