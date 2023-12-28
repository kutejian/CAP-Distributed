using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.DistributedTransaction.Utility
{
    //取得名称
    public class ConstPublish
    {
        public const string UserPublishName = "RabbitMQ.SQLServer.DistributedDemo.User-Order";

        public const string OrderPublishName = "RabbitMQ.SQLServer.DistributedDemo.Order-Storage";

        public const string StoragePublishName = "RabbitMQ.SQLServer.DistributedDemo.Storage-Logistics";

        public const string LogisticsPublishName = "RabbitMQ.SQLServer.DistributedDemo.Logistics-Payment";

        public const string PaymentPublishName = "RabbitMQ.SQLServer.DistributedDemo.Payment-Other";

    }
}
