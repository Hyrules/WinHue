/////////////////////////////////////////////////////// 
/// WinHue 3 - {release}
/////////////////////////////////////////////////////// 

NOTE : You need Microsoft Dot Net Framework 4.7.2 for this program to work properly. 
You might also need Visual C++ 2010 Redistributable library. 
Both are available at Microsoft's website. 

Fixed:
- WHC listing wrong options for the sensors.
- Transition time not working properly in scene creator.
- Some button size and window size.

Added:
- Numeric Up / Down to some of the main form sliders.
- Edit scene from the scene mapping window. Just select a row and click the edit button on the bar at the top beside the filter.
- Context menu to the rule creator value. Button event are as simple as selecting the button event you want and it will fill the value.
- Option to hide the floor plan tab in the application view settings.

Changed:
- MOVED TO .NET FRAMEWORK 4.7.2
- Scene Creator display of sliders and values.
- Replaced many Transition related up down with a practical TransitionTime Up Down. No more guessing the transition time.
- Replaced some wpf toolkit up down with my own. ( bug alert ! there might be bugs in there )
- Removed some black border on forms.
- Forked my own version of WPFToolkit 3.5.0 with the Datagrid.
- Changed to order of the steps in the Scene creator.

Updated :
- Naudio dependency.
- gong wpf drag and drop dependency.
- Sprache dependency.

WIP:
- Animation creator. (Not Available yet)
- Entertainment creator. (not Available yet)




