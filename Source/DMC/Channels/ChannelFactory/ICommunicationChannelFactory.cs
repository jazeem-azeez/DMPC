using System.Collections.Generic;

namespace DMC.Channels.ChannelFactory
{
    public interface ICommunicationChannelFactory
    {
        List<ICommunicationChannel> GetActiveChannels();
    }
}