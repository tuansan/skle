using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Skle
{
    public partial class SendNotiTask : IScheduleTask
    {
        private readonly INotificationService _notificationService;

        public SendNotiTask(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Execute()
        {
            _notificationService.ExecuteTaskNoti();
        }
    }
}
