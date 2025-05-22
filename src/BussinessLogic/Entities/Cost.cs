using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Entities
{
    public class Cost
    {

        public int CostID { get; set; }
        public string Name { get; set; }

        public double Price { get; set; }

        public string Currency { get; set; }


        public List<Guid> TicketsList { get; set; } = new List<Guid>();
    }
}
