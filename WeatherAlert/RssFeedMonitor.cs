using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Threading;
using System.ComponentModel.Composition;
using WinHuePluginModule;
using System.Drawing;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.ServiceModel.Syndication;
using System.Xml;

namespace RssFeedMonitor
{
    

    [Export(typeof(IWinHuePluginModule))]
    class RssFeedMonitorPlugin : IWinHuePluginModule
    {
#pragma warning disable 649
        [Import(typeof (IWinhuePluginHost))]
        private IWinhuePluginHost Host;
#pragma warning restore 649

        BackgroundWorker bgw;
        DispatcherTimer rsstimer;

        public RssFeedMonitorPlugin()
        {
            bgw = new BackgroundWorker();
            rsstimer = new DispatcherTimer();
            rsstimer.Interval = new TimeSpan(0, Properties.Settings.Default.UpdateInterval, 0);
            rsstimer.Tick += RssTimer_Tick;
            bgw.DoWork += Bgw_DoWork;
            
        }

        private void RssTimer_Tick(object sender, EventArgs e)
        {
            bgw.RunWorkerAsync();
        }

        public bool? ShowSettingsForm()
        {
            RssFeedMonitorSettingsForm sf = new RssFeedMonitorSettingsForm(Host) {Owner = Application.Current.MainWindow};
            sf.ShowDialog();
            return true;
        }

        public void Start()
        {
            rsstimer.Start(); 
        }

        public void Stop()
        {
            rsstimer.Stop();
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckAlertAndTriggers();
        }

        public void CheckAlertAndTriggers()
        {
            List<Alert> listAlert = RssFeedAlertHandler.LoadRssFeedAlerts();
            SyndicationFeed sf;

            foreach (Alert a in listAlert)
            {
                XmlReader xr = XmlReader.Create(a.Url);
                sf = SyndicationFeed.Load(xr);
                if (sf == null) return;

                List<SyndicationItem> si = sf.Items.ToList();

                if (a.Criterias.Exists(x => x.RSSElement == "Title") == true)
                {
                    Criteria cr = (Criteria)a.Criterias.Select(x => x.RSSElement == "Title");
                    if (cr.Condition == "Contains")
                        si.RemoveAll(x => !x.Title.Text.Contains(cr.UserCondition));

                    if (cr.Condition == "Equals")
                        si.RemoveAll(x => x.Title.Text != cr.UserCondition);
                }

                if (a.Criterias.Exists(x => x.RSSElement == "Description") == true)
                {
                    Criteria cr = (Criteria)a.Criterias.Select(x => x.RSSElement == "Description");
                    if (cr.Condition == "Contains")
                        si.RemoveAll(x => !x.Summary.Text.Contains(cr.UserCondition));

                    if (cr.Condition == "Equals")
                        si.RemoveAll(x => x.Summary.Text != cr.UserCondition);
                }

                if (a.Criterias.Exists(x => x.RSSElement == "Publication Date") == true)
                {
                    Criteria cr = (Criteria)a.Criterias.Select(x => x.RSSElement == "Title");
                    if (cr.Condition == "Contains")
                        si.RemoveAll(x => !x.PublishDate.ToString().Contains(cr.UserCondition));

                    if (cr.Condition == "Equals")
                        si.RemoveAll(x => x.PublishDate.ToString() != cr.UserCondition);

                    if (cr.Condition == "Greater")
                        si.RemoveAll(x => x.PublishDate > DateTimeOffset.Parse(cr.UserCondition));

                    if (cr.Condition == "Lower")
                        si.RemoveAll(x => x.PublishDate < DateTimeOffset.Parse(cr.UserCondition));
                }

                if (a.Criterias.Exists(x => x.RSSElement == "Category") == true)
                {
                    Criteria cr = (Criteria)a.Criterias.Select(x => x.RSSElement == "Title");
                    /*if (cr.Condition == "Contains")
                        si.RemoveAll(x => !x.Categories.Contains.Text.Contains(cr.UserCondition));*/

                        /*if (cr.Condition == "Equals")
                        si.RemoveAll(x => x.Title.Text != cr.UserCondition);*/
                }

                if (si.Count > 0)
                {
                    // Do ALERT ABOUT IT.

                }
            }
        }

        public Bitmap pluginIcon => Properties.Resources.pluginicon;

        public string pluginName => "Rss Feed Monitor";

        public string pluginDesc => "Gives an alert when a specific rss feed meets the requirements.";

        public string pluginAuth => "Pascal Pharand";
    }
}
