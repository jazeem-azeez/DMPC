using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DMC.Models;

namespace DMC.BackPlane
{
    public interface IBackPlane
    {
        void SubscribeToBackPlanEvents<T>(string filter, Action<EventMessage> onBackPlaneEvent);
        void UnSubscribeToBackPlanEvents<T>(string filter);
        void AddCommunicationChannel(ICommunicationChannel channel);
        void RemoveCommunicationChannel(ICommunicationChannel channel);

        Task DispatchEventMessageAsync(EventMessage eventMessage);
        string ListenerInstanceId { get; set; }

    } 

}