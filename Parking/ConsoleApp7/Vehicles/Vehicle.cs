using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicles
{
    public class Vehicle
    {
        public Vehicle(bool iscarmc, string regnumber)
        {
            IsCarMc = iscarmc;
            RegNumber = regnumber;
        }
        public bool IsCarMc { get; set; }
        public string RegNumber { get; set; }
    }
}
