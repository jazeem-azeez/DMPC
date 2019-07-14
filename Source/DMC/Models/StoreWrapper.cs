using System;

namespace DMC.Models
{
    public class StoreWrapper<T>
    {
        DateTime CreateTimeStamp { get; set; }
        T Data { get; set; }
        DateTime LastAccessTime { get; set; }
        TimeSpan TimeToLive { get; set; }
        double Weight { get; set; }
    }
}