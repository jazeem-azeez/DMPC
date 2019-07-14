using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DMC.BackPlane;
using DMC.Models;

namespace DMC
{
    public interface ICommunicationChannel
    {
        Action<EventMessage, IDictionary<string, string>> OnMessage { get; set; }
        string ChannelUid { get; }

        Task<bool> PublishEventAsync(EventMessage eventMessage);
        bool Subscribe();
        bool UnSubscribe();
        
    }
    public delegate void ChannelEventConsumerHandler(EventMessage message, IDictionary<string, string> headers);

}
