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
- BUg where adding a rule condition with DX was causing the program to serialize the empty value.

Added:
- Change the order of the actions in the rule creator.
- New icon for refresh and bridge settings
- New tab in menu for Help, maybe to replace Help tab in ribbon to make the program more in line with how Office and other programs place the help menu. (banksio)
- Moved the delete button, rename button, edit button from context menu to right panel.

Changed:
- Reverted to CommandCombobox to solve issues.
- Adjusted the hue slider so it is close to real hue color.
- Minimize to tray option / button moved to application settings. (banksio)
- Ribbonbuttons to fluent buttons (banksio)
- Moved the property grid to it's own window and added a property grid button in the view tab.

Removed:
- Mahapp theme styling and dependencies. Will implement a styling system later.

WIP:
- Window Styling : Not everything matches perfectly.
- Animation creator. (Not Available yet)
- Rss feed monitor plugin. (Not Available yet)
- Clapper monitor. (Not Available yet)




