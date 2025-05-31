using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Infrastructure.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Processors
{

    public abstract class TbinProcessor<T>
    {        
        private byte[] _file { get; set; } = Array.Empty<byte>();

        public byte[] GetFile() => _file;

        public abstract byte[] ConvertToTbin(T trip);

        public abstract T ConvertTbinToTrip(byte[] tbinFile);
    }

  
}
