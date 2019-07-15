using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using DMC.Channels.RabbitMq;
using DMC.Models;

using Microsoft.Azure.ServiceBus;

using Newtonsoft.Json;

using RabbitMQ.Client.Events;

namespace DMC.Channels.EventMessageHandler
{
    public class MessageEncodingHelper
    {
        public static byte[] GetBytes(string str) => Encoding.UTF8.GetBytes(str);

        public static EventMessage GetMessage(BasicDeliverEventArgs message)
        {
            if (message != null && message.Body?.Length > 0)
            {
                EventMessage msg = JsonConvert.DeserializeObject<EventMessage>(GetString(message.Body));
                GetProperties(message.BasicProperties.Headers, msg.EventHeaders);
                return msg;
            }
            return null;
        }

        public static EventMessage GetMessage(Message message)
        {
            if (message != null)
            {
                EventMessage msg = JsonConvert.DeserializeObject<EventMessage>(GetString(message.Body));
                GetProperties(message.UserProperties, msg.EventHeaders);
                return msg;
            }
            return null;
        }

        public static RabbitMqPayLoad GetRabbitMqMessage(EventMessage eventMessage, RabbitMQ.Client.IBasicProperties basicProperties)
        {
            RabbitMqPayLoad msg = new RabbitMqPayLoad(GetBytes(JsonConvert.SerializeObject(eventMessage)), basicProperties);

            if (eventMessage.EventHeaders != null)
            {
                foreach (KeyValuePair<string, string> item in eventMessage.EventHeaders)
                {
                    msg.BasicProperties.Headers.Add(item.Key, item.Value);
                }
            }

            msg.BasicProperties.CorrelationId = Guid.NewGuid().ToString();

            return msg;
        }

        public static string GetString(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetString(byte[] bytes) => Encoding.UTF8.GetString(bytes);


        internal static Message GetServiceBusMessage(EventMessage eventMessage)
        {
            Message msg = new Message(GetBytes(JsonConvert.SerializeObject(eventMessage)));

            if (eventMessage.EventHeaders != null)
            {
                foreach (KeyValuePair<string, string> item in eventMessage.EventHeaders)
                {
                    msg.UserProperties.Add(item.Key, item.Value);
                }
            }

            msg.MessageId = Guid.NewGuid().ToString();
            msg.CorrelationId = Guid.NewGuid().ToString();

            return msg;
        }
         

        private static Dictionary<string, string> GetProperties(IDictionary<string, object> headers, Dictionary<string, string> eventHeaders)
        {
            if (headers != null)
            {
                foreach (KeyValuePair<string, object> header in headers)
                {
                    object value = null;

                    if (header.Value.GetType().Name == "Byte[]")
                    {
                        value = Encoding.UTF8.GetString(header.Value as byte[]);
                    }
                    else
                    {
                        value = header.Value.ToString();
                    }
                    if (eventHeaders.ContainsKey(header.Key) == false)
                    {
                        eventHeaders.Add(header.Key, value.ToString());
                    }
                }
            }

            return eventHeaders;
        }
    }
}