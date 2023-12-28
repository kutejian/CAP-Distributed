using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Persistence;
using DotNetCore.CAP.Processor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zhaoxi.DistributedTransaction.Utility
{
    //自定义调度器
    public class CustomDispatcher : Dispatcher
    {
        public CustomDispatcher(ILogger<Dispatcher> logger,
            IMessageSender sender,
            IOptions<CapOptions> options,
            ISubscribeDispatcher executor) : base(logger, sender, options, executor)
        {
            Console.WriteLine("这是自定义调度程序调用");
        }


        public new void EnqueueToPublish(MediumMessage message)
        {
            Console.WriteLine("这是自定义调度程序调用队列发布");
            base.EnqueueToPublish(message);
        }

        public new void EnqueueToExecute(MediumMessage message, ConsumerExecutorDescriptor descriptor)
        {
            Console.WriteLine("这是自定义调度程序调用队列执行");
            base.EnqueueToExecute(message,descriptor);
        }
    }
}
