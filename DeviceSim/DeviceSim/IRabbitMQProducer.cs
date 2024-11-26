using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSim
{
    public interface IRabbitMQProducer
    {
        Task SendMessageAsync(DeviceMessage message);
    }
}
