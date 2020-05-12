using System;
using System.Collections.Generic;
using System.Text;

namespace Configuration
{
    public class ConfigRabbit
    {
#if DEBUG

        public static string Username => Environment.GetEnvironmentVariable("RabbitMqUsername");
        public static string Password => Environment.GetEnvironmentVariable("RabbitMqPassword");
        private static string Host => Environment.GetEnvironmentVariable("RabbitMqHost");
        private static string Vhost => Environment.GetEnvironmentVariable("RabbitMqVhost");
        private static string Port
        {
            get
            {
                var port = Environment.GetEnvironmentVariable("RabbitMqPort");
                if (port == null)
                {
                    port = String.Empty;
                }
                return port;
            }
        }
        public static string RabbitMqQueueName => Environment.GetEnvironmentVariable("RabbitMqQueueName");
        //public static string HostAddress => $"rabbitmq://{Host}{Port}/{Vhost}";
        public static string HostAddress => Environment.GetEnvironmentVariable("RabbitMqHost"); //localhost;
#else
        private static string Host => Environment.GetEnvironmentVariable("RabbitMqHost");
        public static string Password => Environment.GetEnvironmentVariable("RabbitMqPassword");
        public static string Username => Environment.GetEnvironmentVariable("RabbitMqUsername");
        private static ushort Port => Convert.ToUInt16(Environment.GetEnvironmentVariable("RabbitMqPort"));
        private static string Vhost => Environment.GetEnvironmentVariable("RabbitMqVhost");
        //public static string RabbitMqQueueName =>  Environment.GetEnvironmentVariable("RabbitMqQueueName"); 
        //public static string ConnectionString => $"amqp://{Username}:{Password}@{Host}:{Port}/{Vhost}";
        public static Uri HostAddress => new Uri($"rabbitmq://{Host}:{Port}/{Vhost}"); // rabbitmq://sbdp-center-i52.bank.lan:5672/other

#endif
        #region логувння
        //public static string LogRabbitMqHost => Environment.GetEnvironmentVariable("logRabbitMqHost");
        //public static string LogRabbitMqUser => Environment.GetEnvironmentVariable("logRabbitMqUser");
        //public static string LogRabbitMqPassword => Environment.GetEnvironmentVariable("logRabbitMqPassword");
        //public static string LogRabbitMqVhost => Environment.GetEnvironmentVariable("logRabbitMqVhost");
        //public static string LogRabbitMqExchange => Environment.GetEnvironmentVariable("logRabbitMqExchange");
        #endregion
    }
}
