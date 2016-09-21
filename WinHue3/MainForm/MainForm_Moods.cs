using System.Windows;
using HueLib2;

namespace WinHue3
{
    public partial class MainWindow : Window
    {
        private void LoadMoods()
        {
          /*  List<Mood> moods = MoodHandler.LoadMoodsFiles();
            mnuApplyMood.Items.Clear();
       //     btnCreateMood.Items.Clear();

            foreach (Mood mood in moods)
            {
                // Fill Apply mood context menu
                MenuItem mnuMood = new MenuItem()
                {
                    Header = mood.Name
                };
                
                var actualmood = mood;

                mnuMood.Click += (sender, arg) =>
                {
                    ApplyMood((HueObject)lvMainObjects.SelectedItem, actualmood);
                };
                
                mnuApplyMood.Items.Add(mnuMood);

                // Fill CreateMood button for editing mood.
                RibbonMenuItem mnuMoodName = new RibbonMenuItem()
                {
                    Header = mood.Name
                };

                RibbonMenuItem mnuEditMood = new RibbonMenuItem()
                {
                    Header = "Edit..."
                };

                RibbonMenuItem mnuDelMood = new RibbonMenuItem()
                {
                    Header = "Delete...",
                };

                mnuMoodName.Items.Add(mnuEditMood);
                mnuMoodName.Items.Add(mnuDelMood);

               // btnCreateMood.Items.Add(mnuMoodName);
            }*/

        }

        private void ApplyMood(HueObject obj,Mood mood)
        {

    /*        dynamic state;

            if (obj is Light)
            {
                state = new State();
            }
            else
            {
                state = new Action();
            }

            if (mood.hue != null) state.hue = mood.hue;

            if (mood.sat != null) state.sat = mood.sat;

            if (mood.bri != null) state.bri = mood.bri;

            if (mood.ct != null) state.ct = mood.ct;

            if (mood.transitiontime != null) state.transitiontime = mood.transitiontime;

            if (mood.xy != null)
            {
                state.xy = new XY();

                if (mood.xy.x != null) state.xy.x = mood.xy.x;
                if (mood.xy.y != null) state.xy.y = mood.xy.y;
            }

            if (obj is Light)
            {
      //          _bridge.SetLightState(obj.Id, state);
            }
            else
            {
     //           _bridge.SetGroupAction(obj.Id, state);
            }*/
        }

        private bool DeleteMood(Mood mood)
        {
            /*      bool result = false;
                  try
                  {
                      File.Delete("moods//" + mood.Name + ".md");
                      result = true;
                  }
                  catch(Exception)
                  {
                      result = false;
                  }

                  return result;*/
            return false;
        }
    }
}