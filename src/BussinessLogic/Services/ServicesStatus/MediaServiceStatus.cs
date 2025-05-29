using Ardalis.SmartEnum;
using Commons;
using Commons.ErrorsHandlings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Services.ServicesStatus
{
    /// <summary>
    /// Reserved 1000 => 1999
    /// </summary>
    public class MediaServiceStatus : SmartEnum<MediaServiceStatus>, IServiceResult
    {
        public static readonly MediaServiceStatus Success =
          new(nameof(Success), 0, "Opération réussie.");

        private string _message;
        public MediaServiceStatus(string name, int value,string messsage) : base(name, value)
        {
            _message = messsage;
        }

        public MessageType MessageType => this switch
        {
            var s when s == Success => MessageType.Success,
           
            _ => MessageType.Error
        };

        public string Message => this._message;

        public bool IsSuccess => MessageType == MessageType.Success;
    }
}
