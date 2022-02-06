# Right to Ask app

This is a very basic start for the RightToAsk app, intended to be used for testing the UI to gather user feedback.

## Setup Instructions
On Mac or Windows, you'll need something to compile Xamarin: Visual Studio, VS Code, or JetBrains Rider. Both Android and iOS should work (the latter if you're compiling on a Mac), but UWP is not currently expected to work. You'll need the Xamarin.Essentials and Portable.Bouncycastle Nuget packages.

On Linux, follow @eleanor-em's [Xamarin-on-Linux setup instructions](XamarinOnLinux.md).

The app expects a local instance of [the Right To Ask Server](https://github.com/RightToAskOrg/right_to_ask_server). Remote instances should probably also work but have not been tested. If you don't use the server, the app will still load but will not have access to the data it needs about Parliamentary structures etc.

In order to access Geoscape's electorate-identifying service, which tells the user which electorates they live in when they enter their address, you may need a Geoscape API key - the [Free account](https://geoscape.com.au/developer/) should suffice. (Geoscape's terms of service can change, and it is possible that this will work for free without an API key, but if it's not working for you, try getting one.)

