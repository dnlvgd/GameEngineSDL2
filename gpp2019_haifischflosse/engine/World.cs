using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpp2019_haifischflosse
{
    public class World
    {
        public static int WORLD_WIDTH;
        public static int WORLD_HEIGHT;

        public World(int width, int height)
        {
            WORLD_WIDTH = (int)(width * Window.SCALEFACTOR_X);
            WORLD_HEIGHT = (int)(height * Window.SCALEFACTOR_Y);
        }
    }
}


