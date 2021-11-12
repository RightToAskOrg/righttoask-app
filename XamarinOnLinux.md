## How to set up a Xamarin development environment on Linux

Unfortunately Xamarin does not really properly support Linux. The following steps allowed me to set up a Xamarin development environment and test the app on Android (not iOS).

1. Download and extract [JetBrains Rider](https://www.jetbrains.com/rider/), and move it to a useful location (e.g. `$HOME/opt/`). (Note: JetBrains offers free licenses for those with university email addresses.)
2. Install the [.NET Core SDK](https://docs.microsoft.com/en-us/dotnet/core/install/linux).
3. Install the [Mono SDK](https://www.mono-project.com/download/stable/#download-lin-ubuntu).
4. Install the [JDK](https://openjdk.java.net/projects/jdk/15/).
5. Download and install [Android Studio](https://developer.android.com/studio), and run through the first-time use setup to download and install the Android Emulator and SDKs.
6. In the Android SDK Manager, ensure you have installed Android 9.0, 10.0, and 11.0, as well as the NDK (visible under "SDK Tools").
7. In the [Xamarin DevOps pipelines](https://dev.azure.com/xamarin/public/_build?definitionId=48), find the most recent successful Linux build (two ticks) and click it.
8. Under "Linux", click "3 artifacts".
9. Hover over "Installers - Linux", click the three dots on the right, and click "Download artifacts".
10. Extract the downloaded package, navigate to the directory, and extract the `tar.bz2` archive. (Note: the version I tested is `xamarin.android-oss-v11.2.99.0_Linux-x86_64_5634_2488bf7d-Release`.)
11. In the archive, run the following commands:
```shell
sudo mkdir /usr/lib/xamarin.android
sudo mkdir /usr/lib/mono/xbuild/Xamarin/
sudo cp -a "bin/Release/lib/xamarin.android/." "/usr/lib/xamarin.android/"
rm -rf "/usr/lib/mono/xbuild/Xamarin/Android"
rm -rf "/usr/lib/mono/xbuild-frameworks/MonoAndroid"
sudo ln -s "/usr/lib/xamarin.android/xbuild/Xamarin/Android/" "/usr/lib/mono/xbuild/Xamarin/Android"
sudo ln -s "/usr/lib/xamarin.android/xbuild-frameworks/MonoAndroid/" "/usr/lib/mono/xbuild-frameworks/MonoAndroid"
```
12. Create some symlinks to support JetBrains Rider:
```shell
cd $HOME
mkdir Library
cd Library
ln -s ../Android .
cd Android
ln -s Sdk sdk
```
13. Open JetBrains Rider, and click Configure -> Settings (in the bottom-left).
14. Click "Android" in the menu, and set the paths accordingly. For example, I have:
* Android SDK Location: `/home/eleanor/Android/Sdk`
* Android NDK Location: `/home/eleanor/Android/Sdk/ndk`
* Java Development Kit Location: `/lib/jvm/default`
15. Creating a new Xamarin Application solution (called `App1` in this example), as follows:
* "New -> Application -> Xamarin Application"
* Select an up-to-date Android API (29 or 30) 
* run the following commands in the JetBrains Rider terminal:
```shell
cd App1/App1.Android/Resources/ 
ln -s Resource.designer.cs Resource.Designer.cs
```
16. You should now be able to build and run the app on the Android Emulator!

### Troubleshooting

Make sure the target is set to App1.Android - iOS won't build on Linux. (This is a small drop-down menu near the green play button for building.)

If you get an error "  Xamarin.Android.Tooling.targets(64, 5): [XA5300] The Android SDK directory could not be found. Check that the Android SDK Manager in Visual Studio shows a valid installation. To use a custom SDK path for a command line build, set the 'AndroidSdkDirectory' MSBuild property to the custom path.", go to "Build, Execution, Deployment" -> Android -> "SDK Components" and check the Android SDK location.  Even if it seems correct, click "edit" and, when prompted, agree to update new components.