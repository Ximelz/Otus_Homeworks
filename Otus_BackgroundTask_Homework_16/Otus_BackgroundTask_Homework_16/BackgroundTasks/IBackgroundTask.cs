using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_BackgroundTask_Homework_16
{
    public interface IBackgroundTask
    {
        Task Start(CancellationToken ct);
    }
}
