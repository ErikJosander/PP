using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class History
    {
        public int Id { get; set; }
        public string VehicleRegNumber { get; set; }
        public string CarOrMC { get; set; }
        public DateTime? ArrivedOn { get; set; }
        public DateTime? LeavedOn { get; set; }
        public int TimeStayed { get; set; }
        public int TicketPrice { get; set; }
        public bool PayedTicket { get; set; }
    }
}
