using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightApplication1
{
    public class Location
    {
        public static Location currentLocation { get; set; }

        public int xcoord { get; set; }
        public int ycoord { get; set; }
        public LocationType place { get; set; }

        public Location(LocationType t)
        {
            place = t;
            xcoord = -1;
            ycoord = -1;
        }

        public Location(LocationType t, int xcoord, int ycoord)
        {
            place = t;
            this.xcoord = xcoord;
            this.ycoord = ycoord;
        }
    }

    public enum LocationType
    {
        HomeHub, Tavern, Shop, Dungeon
    }
}
