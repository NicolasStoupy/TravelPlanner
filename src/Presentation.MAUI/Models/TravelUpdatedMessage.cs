using BussinessLogic.Entities;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Presentation.MAUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Models
{
    public class TravelUpdatedMessage : ValueChangedMessage<string>
    {
        public TravelUpdatedMessage(Travel value) : base(value.Id.ToString()) { }
    }
}
