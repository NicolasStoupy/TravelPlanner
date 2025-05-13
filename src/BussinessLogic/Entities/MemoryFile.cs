using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Entities
{
    public class MemoryFile
    {
        public int FileID { get; set; }
        public byte[]? Files { get; set; }

        public string? Description { get; set; }

        public bool Checked { get; set; }

        public Guid FileGuid { get; set; }


    }
}
