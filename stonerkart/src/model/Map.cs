using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace stonerkart
{
    public class Map
    {
        public int width { get; }
        public int height { get; }

        public List<Color[]> xds;

        public Map(int[] ws)
        {
            ws = new int[] {4, 5, 2};
            width = ws.Max();
            height = ws.Length;
            xds = new List<Color[]>();
            xds.Add(new[] {Color.ForestGreen, Color.Fuchsia, Color.ForestGreen, Color.Fuchsia, });
            xds.Add(new[] {Color.Firebrick, Color.DeepSkyBlue, Color.Gainsboro, Color.Gold, Color.Green, });
            xds.Add(new[] { Color.ForestGreen, Color.Fuchsia, });
        }
    }
}
