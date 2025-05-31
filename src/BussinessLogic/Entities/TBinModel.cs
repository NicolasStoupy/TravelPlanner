using Infrastructure.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Entities
{
    public class TBinModel
    {
        public Trip trip { get; set; }

        public Dictionary<Guid, byte[]> medias { get; set; }
    }
}
