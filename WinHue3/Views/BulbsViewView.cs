using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib;
using HueLib_base;

namespace WinHue3
{
    public class BulbsViewView : View
    {
        private Bridge _bridge;
        private DataTable _dt;

        public BulbsViewView(Bridge br)
        {
            _bridge = br;
            BuildBulbsView();
        }

        public DataView BulbsDetails => _dt.DefaultView;

        private void BuildBulbsView()
        {
            Dictionary<string, Light> llights = _bridge.GetLightList();


        }

    }
}
