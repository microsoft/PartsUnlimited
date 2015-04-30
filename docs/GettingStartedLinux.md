#Getting Started for Linux#

##Get DNVM##

The CoreCLR-based `dnu` doesn't support all the available features
yet.  So to develop on Linux we still need to install a Mono-based
DNX runtime and use `dnu` from it.

Please follow the instructions for Linux at
<https://github.com/aspnet/Home> to get DNVM and run the `upgrade`
command. (Note that the doc there doesn't list `unzip` as a
prerequisites, however you need to install that as well, otherwise
DNVM commands would fail.)

```
    dnvm upgrade -u
```

The `-u` switch uses the unstable DNX feed, which is default to
<https://www.myget.org/F/aspnetvnext/api/v2>.

We can install a CoreCLR runtime by running the following command

```
    dnvm install 1.0.0-beta5-11649 -u -r coreclr -arch x64
```

Run `dnvm list` to show the installed DNX runtimes.  You should see output
like

```
    Active Version              Runtime Arch Location             Alias
    ------ -------              ------- ---- --------             -----
      *    1.0.0-beta5-11649    coreclr x64  ~/.dnx/runtimes      default
           1.0.0-beta5-11649    mono         ~/.dnx/runtimes
```

##Get NodeJS, Bower, and Grunt##

These tools are needed in `dnu restore`.

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

##Develop##

Switch to use a Mono-based DNX runtime since the CoreCLR-based `dnu`
doesn't support restore/publish scenarios yet. The version can be picked
from the `dnvm list` output.  In our example the version is
`1.0.0-beta5-11649`.  Due to changes in how DNX layouts files,
`1.0.0-beta5-11624` or a later version is required.

```
    dnvm use 1.0.0-beta5-11649 -r mono
```

Then you can use commands like `dnu build`, `dnu pack`, `dnu publish`,
etc., on your ASP.Net vNext projects.

For example, to publish the PartsUnlimited website app to `~/site`

```
    cd src/PartsUnlimitedWebsite
    dnu restore
    dnu publish --runtime ~/.dnx/runtimes/dnx-coreclr-linux-x64.1.0.0-beta5-11649 -o ~/site
```

The above command bundles the specified CoreCLR runtime with the app.

Run the app by

```
    ~/site/Kestrel
```

Use your favorite web browser to navigate to `http://localhost:5004`.
`lynx` and `links` are text-based web browsers, if you don't have GUI
installed for your Linux machine.

If you want to use a runtime installed on your machine, just modify
the path of `dnx` command in `~/site/Kestrel`.  For example, use the
following command to remove the bundled CoreCLR runtime, add a shared DNX
runtime installed under `~/.dnx` to `$PATH`.

```
    rm -rf ~/site/approot/packages/dnx-coreclr-linux-x64.1.0.0-beta5-11649/
    dnvm use 1.0.0-beta5-11649 -r coreclr -arch x64
```

Then change the last line of `~/site/Kestrel` so it look like this

```
    exec "dnx" --appbase "$DNX_APPBASE" Microsoft.Framework.ApplicationHost Kestrel "$@"
```

Run the `~/site/Kestrel` command again to verify that the website works.
