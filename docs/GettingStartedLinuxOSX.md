#Getting Started for Linux/OS X#

##Get DNVM##

The CoreCLR-based `dnu` doesn't support all the available features
yet.  So to develop on Linux or Mac OS X we still need to install a Mono-based
DNX runtime and use `dnu` from it.

Please follow the instructions at
<https://github.com/aspnet/Home> to get DNVM and run the `upgrade`
command. (Note For Linux the doc there doesn't list `unzip` as a
prerequisites, however you need to install that as well, otherwise
DNVM commands would fail to install DNX runtimes.)

```
    dnvm upgrade -u
```

The `-u` switch uses the unstable DNX feed, which is default to
<https://www.myget.org/F/aspnetvnext/api/v2>.

Run `dnvm list` to show the installed DNX runtimes.  You should see output
similar to below

```
Active Version              Runtime Arch OperatingSystem Alias
------ -------              ------- ---- --------------- -----
       1.0.0-beta5          coreclr x64  linux
  *    1.0.0-beta5          mono         linux/darwin
       1.0.0-beta7-12335    coreclr x64  linux           default
       1.0.0-beta7-12335    mono         linux/darwin
```

A specific version of the CoreCLR runtime can be installed via

```
    dnvm install 1.0.0-beta7-12335 -u -r coreclr -arch x64
```

##Get NodeJS, Bower, and Grunt##

These tools are used in `dnu restore`.

###For Linux###

```
    sudo apt-get install npm nodejs
    sudo npm install bower -g
    sudo npm install grunt-cli -g
```

In Debian-based distros, `node` is renamed to `nodejs` to avoid
conflict with other legacy packages.  Assuming we don't care about
those legacy packages, workaround by

```
    sudo ln -s  /usr/bin/nodejs /usr/bin/node
```

###For Mac OS X###

Download then install nodejs from <https://nodejs.org/download/>, then
run the following commands

```
    sudo npm install bower -g
    sudo npm install grunt-cli -g
```

##Develop##


To install the beta5 CoreCLR runtime, we need to customize the unstable
DNX feed as the packages are hosted there.

```
    export DNX_UNSTABLE_FEED=https://www.myget.org/F/aspnetmaster/api/v2
    dnvm install 1.0.0-beta5 -r coreclr -u
```

Switch to use a Mono-based DNX runtime since the CoreCLR-based `dnu`
doesn't support restore/publish scenarios yet. 

```
    dnvm install 1.0.0-beta5 -r mono
```

Then you can use commands like `dnu build`, `dnu pack`, `dnu publish`,
etc., on your ASP.Net vNext projects.

For example on **Linux**, the following commands publish the PartsUnlimited website app to `~/site`

```
    cd src/Partsunlimited.Models
    dnu restore
    cd ../PartsUnlimitedWebsite
    dnu restore
    dnu publish --runtime ~/.dnx/runtimes/dnx-coreclr-linux-x64.1.0.0-beta5 -o ~/site
```

The specified CoreCLR runtime is bundled with the app.

For **Mac**, the only difference is in the path name.  Replace `linux`
with `darwin`

```
    cd src/Partsunlimited.Models
    dnu restore
    cd ../PartsUnlimitedWebsite
    dnu restore
    cd PartsUnlimitedWebsite
    dnu publish --runtime ~/.dnx/runtimes/dnx-coreclr-darwin-x64.1.0.0-beta5 -o ~/site
```

Run the app by

```
    ~/site/Kestrel
```

Use your favorite web browser to navigate to `http://localhost:5001`.
`lynx` and `links` are text-based web browsers, if you don't have GUI
installed for your Linux machine.

If you want to use a runtime installed on your machine, just modify
the path of `dnx` command in `~/site/Kestrel`.  For example, use the
following commands to remove the bundled CoreCLR runtime, add a shared DNX
runtime installed under `~/.dnx` to `$PATH`.

```
    rm -rf ~/site/approot/packages/dnx-coreclr-linux-x64.1.0.0-beta5/
    dnvm use 1.0.0-beta5 -r coreclr -arch x64
```

Again for **Mac** replace `linux` in the above with `darwin`.

Then change the last line of `~/site/Kestrel` so it look like this

```
    exec "dnx" --appbase "$DNX_APPBASE" Microsoft.Framework.ApplicationHost Kestrel "$@"
```

Run the `~/site/Kestrel` command again to verify that the website works.
