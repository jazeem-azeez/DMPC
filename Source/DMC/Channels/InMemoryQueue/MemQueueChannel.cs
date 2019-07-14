using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMC.BackPlane;
using DMC.Models;

namespace DMC.Channels.InMemoryQueue
{
    public class MemQueueChannel : ICommunicationChannel
    { 

        private ConcurrentQueue<EventMessage> _eventMessages;
        private CancellationToken _cancellationToken;
        private Task _eventPumpTask;

        public Action<EventMessage, IDictionary<string, string>> OnMessage { get; set; }


        private CancellationTokenSource CancellationTokenSource { get; set; }
        public string ChannelUid { get=>nameof(MemQueueChannel);  }

        public MemQueueChannel()
        {
            _eventMessages = new ConcurrentQueue<EventMessage>();
            CancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = CancellationTokenSource.Token;
        }

        public Task<bool> PublishEventAsync(EventMessage eventMessage)
        {
            _eventMessages.Enqueue(eventMessage);
            return Task.FromResult(true);
        }

        public bool Subscribe()
        {
            _eventPumpTask = Task.Run(async () =>
            {
                while (_cancellationToken.IsCancellationRequested == false)
                {
                    if (_eventMessages.IsEmpty == false)
                    {
                        if (_eventMessages.TryDequeue(out EventMessage eventMessage))
                        {
                            OnMessage.Invoke(eventMessage, new Dictionary<string, string>() { });
                        }
                    }
                    await Task.Delay(10);
                }
            });
            return true;
        }



        public bool UnSubscribe()
        {
            CancellationTokenSource.Cancel();
            return true;
        }
    }
}
