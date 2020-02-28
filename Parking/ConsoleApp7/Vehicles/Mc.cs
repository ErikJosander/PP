using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicles
{
    public class Mc : Vehicle, IDrivable
    {
        public Mc(bool iscarmc, string regnumber) : base(iscarmc, regnumber)
        {

        }
    }
}
