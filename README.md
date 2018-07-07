# DokanRainbow

Browse the Sitecore content tree as if it is a file system.

> Warning: although this works to some extent, it's still highly experimental. It hooks into your Windows system at a very low level and may cause some programs to crash. Please use this at your own risk.

## Get started

  - Install [Dokany](https://github.com/dokan-dev/dokany/releases) (choose `DokanSetup.exe` if you're not sure)
  - Go to the installation folder (e.g.: `c:\Program Files\Dokan\Dokan Library-1.1.0`) and run the following: `dokanctl /i d`. This will install the driver/service that allows disks to be added.
  - Clone this repository
  - Update the file `drives.json` with connection info to Sitecore instances/databases that you want to be available as drives.
  - Build and run the project.

You should then be able to go to use any tools that can deal with files to navigate the content tree, including Windows Explorer, command prompt, PowerShell, VSCode... Any tool you want, really.

## Technical details

  - Uses [Dokany](https://github.com/dokan-dev/dokany) together with [Dokan.NET](https://github.com/dokan-dev/dokan-dotnet). Dokany acts as a device driver and allows us to easily simulate disk drives.
  - The format of the files that are exposed is called [Rainbow](https://github.com/SitecoreUnicorn/Rainbow), which is used by [Unicorn](https://github.com/SitecoreUnicorn/Unicorn). Be sure to check these out if you're developing with Sitecore.
  - To get the items from Sitecore, the [ItemService](https://doc.sitecore.net/sitecore_experience_platform/developing/developing_with_sitecore/sitecoreservicesclient/the_restful_api_for_the_itemservice) is used. Because of this, you are required to use a HTTPS connection.
  - Since many tools expect a very fast implementation, they may be quite lazy/inefficient in the way the filesystem is queried. This is why the calls to Sitecore are cached (for 5 seconds by default). You can change this setting in the `drives.json` file by specifying `cacheTimeSeconds`.
  - There is currently no write support. It should be possible, but would probably take a lot of time to implement. Deserializing the yaml files and file structure and converting this to REST calls for changing the items in Sitecore seems like a lot of work.

## Screenshots

![WinMerge_screenshot.png](https://raw.githubusercontent.com/hermanussen/DokanRainbow/master/img/Command_Prompt_screenshot.png)


![Command prompt screenshot](https://raw.githubusercontent.com/hermanussen/DokanRainbow/master/img/Command_Prompt_screenshot.png)


![Visual Studio Code screenshot](https://raw.githubusercontent.com/hermanussen/DokanRainbow/master/img/VSCode_screenshot.png)