using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicles
{
    public class Car : Vehicle, IDrivable
    {
        public Car(bool iscarmc, string regnumber) : base(iscarmc, regnumber)
        {

        }       
    }
}
