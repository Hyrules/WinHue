<h1>WinHue</h1>

<b>Requirements :</b> <br/>
- [Microsoft Dot Net Framework 4.5.2](https://dotnet.microsoft.com/download/dotnet-framework/net452) <br/>
- [Visual C++ 2010 Redistributable](https://www.microsoft.com/en-ca/download/details.aspx?id=5555)<br/>
- [A complete Philips Hue Light system including some philips hue lights and bridge (Api version 1.16 or greater)](https://www2.meethue.com/en-us/products/starter-kits#filters=STARTER_KITS_SU&sliders=&support=&price=&priceBoxes=&page=&layout=12.subcategory.p-grid-icon)

***PHILIPS HUE HAS RELEASED BLUETOOTH ENABLES HUE BULBS. WINHUE DOES NOT SUPPORT THAT NEW AND SEVERELY LIMITED MODE OF COMMUNICATION FOUND IN THOSE NEW HUE LIGHTS BULBS. HOWEVER, THOSE LIGHT BULBS ARE STILL HUE LIGHT BULBS AND WILL WORK JUST FINE WITH WINHUE, EXACTLY LIKE THE PREVIOUS NONE BLUETOOTH ENABLE HUE LIGHT BULBS. THE USE OF A BRIDGE IS MANDATORY AS IT IS THE COMPUTER THAT CONTROLS THE ENTIRE SYSTEM PERMITTING AUTOMATION. IT'S ALSO THE INTERFACE TO YOUR HOME ETHERNET AND Wi-Fi USED BY WINHUE, AND IT WILL PERMIT A FAR GREATER RANGE OF COVERAGE COMPARED TO BLUETOOTH. THE BRIDGE RANGE WILL USUALLY COVER AN ENTIRE REGULAR SIZE HOUSE IF NOT WAY MORE AS EACH ADDED LIGHT ACT LIKE A SIGNAL REPEATER EXTENDING THE RANGE EVEN FURTHER. AS FOR BLUETOOTH, IT IS QUITE SEVERELY RESTRICTED IN RANGE, TO ABOUT 15 FEET (ONE ROOM), AND IT WILL LIMIT YOUR CONTROL TO 10 LIGHT OR LESS. FINALLY, BLUETOOTH DOES NOT SUPPORT THE USE OF TIMERS.
***

<b>Description :</b>

WinHue is a windows desktop application to control the philips hue lighting system. It is was created for desktop pc using the microsoft .net framework. It is not intended to run on Windows phones and tablet and those plateforms will not be supported. WinHue also include a console version that comes with the installer. 

<b>Features : </b>

- Control the Philips Hue Lighting system via Microsoft Windows.
- Create / Modify and delete : Groups, Schedules, Scenes and more.
- Full control over lights / groups color, brightness, temperature, saturation, name.
- Recurring Schedules and Timers !
- Control your light from console.
- Assign global keyboard shortcuts to your favorite actions and trigger them anywhere within windows.
- Add rules or sensors.
- IT'S FREE !

<b>Install Procedure :</b>

Download for git hub release page :

1. Download the [latest release](https://github.com/Hyrules/WinHue/releases/latest)
2. Install the release.
3. Pair the bridge with WinHue.
4. Have fun !

Download using chocolatey :

1. Install [chocolatey](https://chocolatey.org/)
2. Run choco install winhue 
3. Pair the bridge with WinHue.
4. Have fun !

*Thanks to [bcurran3](https://chocolatey.org/packages/winhue) for the chocolatey package.

<b>Usage : </b>
- See the [Usage wiki](../../wiki/Basic-Usage) page for WinHue usage.<br/>
- See the [Console wiki](../../wiki/Console-usage) page for the Console usage.<br/>

<b> FAQ : </b>
- Does WinHue work with x system ?
    Answer : No WinHue is made to be used with the philips hue system and api and won't work with other systems.
- Does WinHue work with alexa ?
    Answer : If you have alexa and philips hue bulbs without any bridge WinHue won't work. WinHue will only work with a bridge and     philips hue bulbs.
- Can you make WinHue work with x system :
    Answer : My goal was and still is to have a Windows application be able to control the philips hue lighting system. In other words no but feel free to fork my project and try to adapt it yourself. The Hue API is too deeply embedded inside WinHue that it will most likely need a complete rework of the code to be able to make it work with other system.
       
<b>Licence : </b>

[Creative Commons Attribution Non-Commercial License V2.0](https://creativecommons.org/licenses/by-nc/2.0/)

The creator of this projet cannot be held responsible for any problem that might arise for the use of the software.

You are free to:

    Share — copy and redistribute the material in any medium or format
    Adapt — remix, transform, and build upon the material 

http://creativecommons.org/licenses/by-nc/2.0/ca/
