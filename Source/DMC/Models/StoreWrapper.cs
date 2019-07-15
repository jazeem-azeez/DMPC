using System;

namespace DMC.Models
{
    public class StoreWrapper<T>
    {
        public DateTime CreateTimeStamp { get; set; }
        public T Data { get; set; }
        public DateTime LastAccessTime { get; set; }
        public TimeSpan? TimeToLive { get; set; }
        public double Weight { get; set; }
        public static StoreWrapper<T> GetStoreWrapperEntry(T value, TimeSpan? expiry)
        {
            DateTime utcNow = DateTime.UtcNow;
            return new StoreWrapper<T>
            {
                CreateTimeStamp = utcNow,
                Data = value,
                LastAccessTime = utcNow,
                TimeToLive = expiry,
                Weight = 1.0
            };
        }
    }
}