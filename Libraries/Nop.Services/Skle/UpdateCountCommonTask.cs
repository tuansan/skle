using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Skle 
{
    public partial class UpdateCountCommonTask: IScheduleTask
    {
        private readonly IMemberService _memberService;
        private readonly IGroupService _groupService;

        public UpdateCountCommonTask(IMemberService memberService, IGroupService groupService)
        {
            _memberService = memberService;
            _groupService = groupService;
        }

        public void Execute()
        {
            _memberService.UpdateCount();
            _groupService.UpdateCount();
        }
    }
}
