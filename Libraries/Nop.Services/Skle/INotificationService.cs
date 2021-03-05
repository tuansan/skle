using FirebaseAdmin.Messaging;
using Nop.Core;
using Nop.Core.Domain.Skle;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Skle
{
    public partial interface INotificationService
    {
        IEnumerable<MyNotification> GetAllNotification(string KeySearch = null);

        MyNotification GetNotificationById(int id);

        MessageToGroup GetMessageToGroupById(int id);

        MemberMessage GetMemberMessageById(int id);

        MemberMessage GetMemberMessageByMemberIdAndMtgId(int MemberId, int MtgId);

        bool Insert(MyNotification item);

        bool Insert(MessageToGroup item);

        bool Insert(MemberMessage item);

        bool Insert(NotiMobiClick item);

        bool CheckClick(int memberId, int notiId);

        bool Update(MyNotification item);

        bool DeleteNotification(int id);

        bool Delete(MemberMessage item);

        IPagedList<MemberMessage> GetAllMemberMessage(int MemberId, int pageIndex = 0, int pageSize = int.MaxValue);

        void SendMessagingAsync(int typeId, int targetId, string token, Notification notification = null);

        void SendMessagingMulticastAsync(int typeId, int targetId, List<string> tokens, Notification notification = null);

        IPagedList<NotiMobi> GetAllNotiMobi(int MemberId, int pageIndex = 0, int pageSize = int.MaxValue, bool isNoMessGroup = false, string KeySearch = null);

        NotiMobi GetNotiMobiById(int id);

        NotiNew GetNotiNewById(int id);

        bool Insert(NotiMobi item);

        bool Insert(NotiNew item);

        bool CheckNewNoti(int memberId, DateTime? time = null);

        void ExecuteTaskNoti();
    }
}