/////////////////////////////////////////////////////// 
/// WinHue 3 - {release}
/////////////////////////////////////////////////////// 

NOTE : You need Microsoft Dot Net Framework 4.5.1 for this program to work properly. 
You might also need Visual C++ 2010 Redistributable library. 
Both are available at Microsoft's website. 

Fixed:
- Editing the Daylight sensor cause CTD.
- Deserialization error with a ClipGenericStatusSensor status of null.
- Advanced creator sending an empty url or text cause a crash.
- Update system not working properly.
- Rolled Back to AsyncRelayCommand.

Added:
- Mouse wheel will now work on the sliders of the mainform.
- Fix/bypass for OSRAM having unreachable issue. (You can activate it in the application settings)
- Power failure settings. Hue now officialy retain the power mode of the light in case of a failure.
- Right click on property in propertygrids (those used for settings objects) reset value.

Changed:
- 

WIP:
- Animation creator. (Not Available yet)
- Entertainment creator. (not Available yet)




