using System;
using System.Collections.Generic;
using Vehicles;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class Space
    {
        public Space()
        {           
            Empty = true;
            McSlotOneEmpty = true;
            McSlotTwoEmpty = true;
        }
        public int Id { get; set; }
        public bool Empty { get; set; }
        public bool McSlotOneEmpty { get; set; }
        public bool McSlotTwoEmpty { get; set; }
        public string CarRegNumber { get; set; }
        public string McOneRegNumber { get; set; }
        public string McTwoRegNumber { get; set; }
        public DateTime? CarArrivedOn { get; set; }
        public DateTime? McOneArrivedOn { get; set; }
        public DateTime? McTwoArrivedOn { get; set; }
    }
}
