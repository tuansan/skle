using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Configuration;
using Nop.Core;
using Nop.Core.Domain.Skle;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Skle
{
    public partial class NotificationService : INotificationService
    {
        #region Fields

        private readonly IRepository<MyNotification> _NotificationRepository;
        private readonly IRepository<MessageToGroup> _MessageToGroupRepository;
        private readonly IRepository<MemberMessage> _MemberMessageRepository;
        private readonly IRepository<Member> _MemberRepository;
        private readonly IRepository<Group> _GroupRepository;
        private readonly IRepository<GroupMember> _GroupMemberRepository;
        private readonly IRepository<MemberBackList> _MemberBackListRepository;
        private readonly IRepository<PostHidden> _PostHiddenRepository;
        private readonly IRepository<NotiMobi> _NotiMobiRepository;
        private readonly IRepository<NotiNew> _NotiNewRepository;
        private readonly IRepository<NotiMobiClick> _NotiMobiClickRepository;
        private readonly IConfiguration _config;
        private readonly ILocalizationService _localizationService;

        #endregion Fields
        #region Ctor

        public NotificationService(IRepository<MyNotification> notificationRepository, IRepository<MessageToGroup> messageToGroupRepository, IRepository<MemberMessage> memberMessageRepository, IRepository<Member> memberRepository, IRepository<Group> groupRepository, IRepository<GroupMember> groupMemberRepository, IRepository<MemberBackList> memberBackListRepository, IRepository<PostHidden> postHiddenRepository, IRepository<NotiMobi> notiMobiRepository, IRepository<NotiNew> notiNewRepository, IRepository<NotiMobiClick> notiMobiClickRepository, IConfiguration config, ILocalizationService localizationService)
        {
            _NotificationRepository = notificationRepository;
            _MessageToGroupRepository = messageToGroupRepository;
            _MemberMessageRepository = memberMessageRepository;
            _MemberRepository = memberRepository;
            _GroupRepository = groupRepository;
            _GroupMemberRepository = groupMemberRepository;
            _MemberBackListRepository = memberBackListRepository;
            _PostHiddenRepository = postHiddenRepository;
            _NotiMobiRepository = notiMobiRepository;
            _NotiNewRepository = notiNewRepository;
            _NotiMobiClickRepository = notiMobiClickRepository;
            _config = config;
            _localizationService = localizationService;
        }
        #endregion


        #region Methods

        #region Service

        #region Notification

        public IEnumerable<MyNotification> GetAllNotification(string KeySearch = null)
        {
            var query = _NotificationRepository.Table;
            if (!string.IsNullOrEmpty(KeySearch))
            {
                query = query.Where(s => s.Title.Contains(KeySearch));
            }
            return query.OrderByDescending(s => s.CreatedAt).ToList();
        }

        public MyNotification GetNotificationById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(MyNotification));
            return _NotificationRepository.GetById(id);
        }

        public bool Insert(MyNotification item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MyNotification));
            item.CreatedAt = DateTime.Now;
            _NotificationRepository.Insert(item);

            return true;
        }

        public bool Update(MyNotification item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MyNotification));
            _NotificationRepository.Update(item);

            return true;
        }

        public bool DeleteNotification(int id)
        {
            var item = _NotificationRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(MyNotification));

            _NotificationRepository.Delete(item);

            return true;
        }

        #endregion Notification

        #region MessageToGroup

        public MessageToGroup GetMessageToGroupById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(MessageToGroup));
            return _MessageToGroupRepository.GetById(id);
        }

        public bool Insert(MessageToGroup item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MessageToGroup));
            item.CreatedAt = DateTime.Now;
            _MessageToGroupRepository.Insert(item);

            return true;
        }

        #endregion MessageToGroup

        #region MemberMessage

        public MemberMessage GetMemberMessageById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(MemberMessage));
            return _MemberMessageRepository.GetById(id);
        }


        public MemberMessage GetMemberMessageByMemberIdAndMtgId(int MemberId, int MtgId)
        {
            if (MemberId == 0 || MtgId == 0)
                return null;
            return _MemberMessageRepository.Table.FirstOrDefault(s => s.MemberId == MemberId && s.MessageId == MtgId);
        }
        public bool Insert(MemberMessage item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MemberMessage));

            _MemberMessageRepository.Insert(item);

            return true;
        }

        public bool Delete(MemberMessage item)
        {
            if (item == null)
                return false;

            _MemberMessageRepository.Delete(item);

            return true;
        }

        public virtual IPagedList<MemberMessage> GetAllMemberMessage(int MemberId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _MemberMessageRepository.Table.Where(s => s.MemberId == MemberId);
            return new PagedList<MemberMessage>(query.OrderByDescending(s => s.Id), pageIndex, pageSize);
        }

        #endregion MemberMessage

        #region NotiMobi

        public virtual IPagedList<NotiMobi> GetAllNotiMobi(int MemberId, int pageIndex = 0, int pageSize = int.MaxValue, bool isNoMessGroup = false, string KeySearch = null)
        {
            var query = from s in _NotiMobiRepository.Table
                        join t2 in _MemberRepository.Table on s.MemberId equals t2.Id
                      where 
                        t2.CreatedAt > s.CreatedAt 
                        && s.BranchId == (int)ENTypeNotification.all
                        || (s.BranchId == (int)ENTypeNotification.@group 
                            && s.currentMemberId != MemberId 
                            && _GroupMemberRepository.Table.Any(a => 
                                a.MemberId == MemberId 
                                && a.StatusId == (int)ENStatusGroupRequest.confirm 
                                && a.CreatedAt > s.CreatedAt && a.GroupId == s.GroupId))
                        || (s.BranchId == (int)ENTypeNotification.member && s.MemberId == MemberId)
                      select s;
            if (isNoMessGroup)
                query = query.Where(w => w.TypeId != (int)ENTypeNotiMobi.tin_nhan);
            if (!string.IsNullOrEmpty(KeySearch))
                query = query.Where(w => w.Title.Contains(KeySearch) || w.Content.Contains(KeySearch));
            return new PagedList<NotiMobi>(query.OrderByDescending(s => s.CreatedAt), pageIndex, pageSize);
        }


        public NotiMobi GetNotiMobiById(int id)
        {
            if (id == 0)
                return null;
            return _NotiMobiRepository.GetById(id);
        }
        public bool Insert(NotiMobi item)
        {
            try
            {
                item.SentTries = 0;
                item.CreatedAt = DateTime.Now;
                _NotiMobiRepository.Insert(item);
            }
            catch
            {
                return false;
            }

            ExecuteTaskNoti();
            return true;
        }

        #endregion NotiMobi

        #endregion Service

        #region firebase

        public virtual void SendMessagingAsync(int typeId, int id, string token, Notification notification = null)
        {
            var message = new Message
            {
                Token = token,
                Data = new Dictionary<string, string>()
                {
                    { "TypeId" , typeId.ToString() },
                    { "TargetId" , id.ToString() },
                    { "click_action" , "FLUTTER_NOTIFICATION_CLICK" }
                },
                Notification = notification
            };
            FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        public virtual void SendMessagingMulticastAsync(int typeId, int id, List<string> tokens, Notification notification = null)
        {
            var message = new MulticastMessage
            {
                Tokens = tokens.ToList(),
                Data = new Dictionary<string, string>()
                {
                    { "TypeId" , typeId.ToString() },
                    { "TargetId" , id.ToString() },
                    { "click_action" , "FLUTTER_NOTIFICATION_CLICK" }
                },
                Notification = notification
            };
            FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
        }

        #endregion firebase

        #region Task

        public void ExecuteTaskNoti()
        {
            var query = _NotiMobiRepository.Table.Where(s => !s.Sent && s.SentTries < 3).ToList();
            foreach (var item in query)
            {
                string title = string.Empty;
                string body = string.Empty;
                switch (item.TypeId)
                {
                    case (int)ENTypeNotiMobi.he_thong:
                        title = _localizationService.GetResource("NotiMobi.hethong");
                        body = item.Content;
                        break;

                    case (int)ENTypeNotiMobi.tin_nhan:
                        title = _localizationService.GetResource("NotiMobi.tinnhan");
                        body = item.Name + ": " + item.Content;
                        break;

                    case (int)ENTypeNotiMobi.bai_dang:
                        title = _localizationService.GetResource("NotiMobi.baidang");
                        body = item.Name + " " + item.Content;
                        break;

                    case (int)ENTypeNotiMobi.loimoi_ketban:
                        title = _localizationService.GetResource("NotiMobi.loimoi.ketban");
                        body = item.Name + " " + item.Content;
                        break;

                    case (int)ENTypeNotiMobi.xacnhan_ketban:
                        title = _localizationService.GetResource("NotiMobi.xacnhan.ketban");
                        body = item.Name + " " + item.Content;
                        break;

                    case (int)ENTypeNotiMobi.xacnhan_vaonhom:
                        title = _localizationService.GetResource("NotiMobi.xacnhan.vaonhom");
                        body = item.Name + " " + item.Content;
                        break;

                    case (int)ENTypeNotiMobi.invite_group:
                        title = _localizationService.GetResource("NotiMobi.invite.group");
                        body = item.Name + " " + item.Content;
                        break;

                    case (int)ENTypeNotiMobi.like:
                        title = _localizationService.GetResource("NotiMobi.member.like");
                        body = item.Name + " " + item.Content;
                        break;
                }


                if (!string.IsNullOrEmpty(item.Title))
                {
                    title += ": " + item.Title;
                }

                try
                {
                    switch (item.BranchId)
                    {
                        case (int)ENTypeNotification.all:
                            var tokens = _MemberRepository.Table.Where(s => !s.Deleted && s.StatusId == (int)ENStatusMember.Active).Select(s => s.FirebaseId).Distinct().ToList();
                            SendMessagingMulticastAsync(item.TypeId, item.Id, tokens, new Notification()
                            {
                                Title = title,
                                Body = body
                            });
                            break;

                        case (int)ENTypeNotification.group:
                            var mb = _GroupMemberRepository.Table
                                .Where(s => s.GroupId == item.GroupId && s.MemberId != item.currentMemberId && s.StatusId == (int)ENStatusGroupRequest.confirm)
                                .Select(s => s.MemberId)
                                .ToList();

                            if(item.TypeId == (int)ENTypeNotiMobi.tin_nhan)
                                mb = mb.Where(s => !_MemberBackListRepository.Table.Any(a => (a.FromId == s && a.ToId == item.currentMemberId) || (a.FromId == item.currentMemberId && a.ToId == s))).ToList();
                            if (item.TypeId == (int)ENTypeNotiMobi.bai_dang)
                                mb = mb.Where(s => !_PostHiddenRepository.Table.Any(z => z.FormId == s && z.ToId == item.currentMemberId && (z.isAll || z.TargetId == item.PostId))).ToList();
                            var tokens2 = _MemberRepository.Table.Where(s => !s.Deleted && s.StatusId == (int)ENStatusMember.Active && mb.Contains(s.Id)).Select(s => s.FirebaseId).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
                            SendMessagingMulticastAsync(item.TypeId, item.Id, tokens2, new Notification()
                            {
                                Title = title,
                                Body = body
                            });
                            break;

                        case (int)ENTypeNotification.member:
                            var token = _MemberRepository.GetById(item.MemberId).FirebaseId;
                            SendMessagingAsync(item.TypeId, item.Id, token, new Notification()
                            {
                                Title = title,
                                Body = body
                            });
                            break;
                    }
                    item.Sent = true;
                }
                catch
                {
                    item.SentTries++;
                }
                _NotiMobiRepository.Update(item);
            }
        }

        public bool Insert(NotiMobiClick item)
        {
            if (item == null)
                return false;
            _NotiMobiClickRepository.Insert(item);
            return true;
        }

        public bool CheckClick(int memberId, int notiId)
        {
            return _NotiMobiClickRepository.Table.Any(s => s.MemberId == memberId && s.NotificationId == notiId);
        }

        public NotiNew GetNotiNewById(int id)
        {
            var noti =  _NotiNewRepository.Table.FirstOrDefault(s => s.MemberId == id);
            if (noti == null)
                noti = new NotiNew() { MemberId = id, ReadTime = DateTime.Now };
            return noti;
        }

        public bool Insert(NotiNew item)
        {
            if (item.Id == 0)
            {
                _NotiNewRepository.Insert(item);
            }
            else
            {
                item.ReadTime = DateTime.Now;
                _NotiNewRepository.Update(item);
            }
            return true;
        }

        public bool CheckNewNoti(int memberId, DateTime? time = null)
        {
            var gr = _GroupMemberRepository.Table.Where(s => s.MemberId == memberId && s.StatusId == (int)ENStatusGroupRequest.confirm).Select(s => s.GroupId).ToList();
            var query = _NotiMobiRepository.Table.Where(s => s.BranchId == (int)ENTypeNotification.all || (s.BranchId == (int)ENTypeNotification.group && gr.Contains(s.GroupId) && s.currentMemberId != memberId) || (s.BranchId == (int)ENTypeNotification.member && s.MemberId == memberId));

            if (time.HasValue)
            {
                query = query.Where(s => s.CreatedAt > time);
            }

            return query.OrderByDescending(s => s.CreatedAt).Take(10).ToList().Any(s => !CheckClick(memberId, s.Id));
        }

        #endregion Task

        #endregion Methods
    }
}