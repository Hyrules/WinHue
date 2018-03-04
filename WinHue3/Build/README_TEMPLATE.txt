/////////////////////////////////////////////////////// 
/// WinHue 3 - {release}
/////////////////////////////////////////////////////// 

NOTE : You need Microsoft Dot Net Framework 4.5.1 for this program to work properly. 
You might also need Visual C++ 2010 Redistributable library. 
Both are available at Microsoft's website. 

Fixed:
- Bug in the rule creator that caused object not to be selected when selecting a property.
- Bug when app would crash when editing an existing rule.
- Bug where replacing condition would crash the app.
- Bug where a crash would occur if you select a rule condition with a config proprerty.
- Bug where user could not apply bridge settings.
- Bug where adding a rule condition with DX was causing the program to serialize the empty value.
- Bug where user could not edit rule with action on schedule

Added:
- Change the order of the actions in the rule creator.
- New icon for refresh and bridge settings
- New tab in menu for Help, maybe to replace Help tab in ribbon to make the program more in line with how Office and other programs place the help menu. (banksio)
- Moved the delete button, rename button, edit button from context menu to right panel.
- Missing rule creator action on schedule.
- New schedule creator. 
- List bridge capabilities. if firmware version is greater than 1.15.
- Settings to change slider behaviors. Disabled when light is off and toggle on only or Enabled when light is off / apply when turned on.

Changed:
- Reverted to CommandCombobox to solve issues.
- Adjusted the hue slider so it is close to real hue color.
- Minimize to tray option / button moved to application settings. (banksio)
- Ribbonbuttons to fluent buttons (banksio)
- Moved the property grid to it's own window and added a property grid button in the view tab.
- Updated the update system for the bridge. Updates are now done from the Bridge Settings Window and allow updates from bridge version 1.20 and greater.

Removed:
- Mahapp theme styling and dependencies. Will implement a styling system later.

WIP:
- Animation creator. (Not Available yet)
- Rss feed monitor plugin. (Not Available yet)
- Clapper monitor. (Not Available yet)




