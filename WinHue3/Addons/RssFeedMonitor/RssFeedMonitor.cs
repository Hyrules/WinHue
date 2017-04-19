using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Threading;
using System.ComponentModel.Composition;

using System.Drawing;
using System.Windows;
using System.ComponentModel;
using System.ServiceModel.Syndication;
using System.Windows.Threading;

using System.Xml;

namespace WinHue3
{

    public class RssFeedMonitor : View
    {
        private readonly BackgroundWorker _bgw;
        private readonly DispatcherTimer _rsstimer;
        private bool _isrunning;

        public RssFeedMonitor()
        {
            _bgw = new BackgroundWorker();
            _rsstimer = new DispatcherTimer();
            _rsstimer.Interval = new TimeSpan(0, (int)Properties.Settings.Default.UpdateInterval, 0);
            _rsstimer.Tick += RssTimer_Tick;
            _bgw.DoWork += Bgw_DoWork;
            
        }

        private void RssTimer_Tick(object sender, EventArgs e)
        {
            _bgw.RunWorkerAsync();
        }

        public bool? ShowSettingsForm()
        {
            RssFeedMonitorSettingsForm sf = new RssFeedMonitorSettingsForm() {Owner = Application.Current.MainWindow};
            sf.ShowDialog();
            return true;
        }

        public void Start()
        {
            _rsstimer.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            _rsstimer.Stop();
            IsRunning = false;
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckAlertAndTriggers();
        }

        public bool IsRunning
        {
            get { return _isrunning; }
            private set { _isrunning = true; RaisePropertyChanged(); }
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

                if (a.Criterias.Any(x => x.RSSElement == "Title") == true)
                {
                    Criteria cr = (Criteria)a.Criterias.Select(x => x.RSSElement == "Title");
                    if (cr.Condition == "Contains")
                        si.RemoveAll(x => !x.Title.Text.Contains(cr.UserCondition));

                    if (cr.Condition == "Equals")
                        si.RemoveAll(x => x.Title.Text != cr.UserCondition);
                }

                if (a.Criterias.Any(x => x.RSSElement == "Description") == true)
                {
                    Criteria cr = (Criteria)a.Criterias.Select(x => x.RSSElement == "Description");
                    if (cr.Condition == "Contains")
                        si.RemoveAll(x => !x.Summary.Text.Contains(cr.UserCondition));

                    if (cr.Condition == "Equals")
                        si.RemoveAll(x => x.Summary.Text != cr.UserCondition);
                }

                if (a.Criterias.Any(x => x.RSSElement == "Publication Date") == true)
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

                if (a.Criterias.Any(x => x.RSSElement == "Category") == true)
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

        public Bitmap pluginIcon => Properties.Resources.rss; 

        public string pluginName => "Rss Feed Monitor";

        public string pluginDesc => "Gives an alert when a specific rss feed meets the requirements.";

        public string pluginAuth => "Pascal Pharand";
    }
}
