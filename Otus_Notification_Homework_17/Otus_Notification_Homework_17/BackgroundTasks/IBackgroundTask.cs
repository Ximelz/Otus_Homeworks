using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    public interface IBackgroundTask
    {
        Task Start(CancellationToken ct);
    }
}
