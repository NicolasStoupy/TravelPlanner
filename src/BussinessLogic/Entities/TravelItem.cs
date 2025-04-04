using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Entities
{
    public class TravelItem
    {

        public string name { get; set; }

        public byte[] image { get; set; }

        public string description { get; set; }

        public DateOnly travelDate { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }
    }
}
