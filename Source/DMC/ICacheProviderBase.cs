using System.Collections.Generic;
using DMC.Models;

namespace DMC
{
    public interface ICacheProviderBase
    {
        string FilterName { get; }

        bool Compact();

        void ConnectToBackPlane();

        void DisconnectFromBackPlane();

        bool Invalidate(string key);

        bool Invalidate(List<string> keys);

        bool Invalidate(List<string> keys, List<string> filters);

        void OnBackPlaneEvent(EventMessage eventMessage);
    }
}