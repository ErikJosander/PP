using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class TempVehicle
    {
        public int Id { get; set; }
        public bool CarOrMC { get; set; }
        public string RegNumber { get; set; }
        public DateTime? ArrivedOn { get; set; }
    }
}
