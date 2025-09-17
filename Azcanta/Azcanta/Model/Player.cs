using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azcanta.Model
{
    public partial class Player
    {
        private string _name;
        private int _x;
        private int _y;

        public Player(string name, int x, int y)
        {
            _name = name;
            _x = x;
            _y = y;
        }

    }
}
