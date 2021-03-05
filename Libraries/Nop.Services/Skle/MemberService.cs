using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Skle;
using Nop.Data;
using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Skle
{
    public partial class MemberService : IMemberService
    {
        #region Fields

        private readonly IRepository<Member> _MemberRepository;
        private readonly IRepository<MemberLog> _MemberLogRepository;
        private readonly IRepository<Field> _FieldRepository;
        private readonly IRepository<MemberField> _MemberFieldRepository;
        private readonly IRepository<Friend> _FriendRepository;
        private readonly IRepository<FriendRequest> _FriendRequestRepository;
        private readonly IRepository<MemberBackList> _MemberBackListRepository;
        private readonly IRepository<GroupMember> _GroupMemberRepository;
        private readonly ICustomerService _customerService;
        private readonly IGroupService _groupService;
        private readonly IPostService _postService;

        #endregion Fields

        #region Ctor

        public MemberService(IRepository<Member> memberRepository, IRepository<MemberLog> memberLogRepository, IRepository<Field> fieldRepository, IRepository<MemberField> memberFieldRepository, IRepository<Friend> friendRepository, IRepository<FriendRequest> friendRequestRepository, IRepository<MemberBackList> memberBackListRepository, IRepository<GroupMember> groupMemberRepository, ICustomerService customerService, IGroupService groupService, IPostService postService)
        {
            _MemberRepository = memberRepository;
            _MemberLogRepository = memberLogRepository;
            _FieldRepository = fieldRepository;
            _MemberFieldRepository = memberFieldRepository;
            _FriendRepository = friendRepository;
            _FriendRequestRepository = friendRequestRepository;
            _MemberBackListRepository = memberBackListRepository;
            _GroupMemberRepository = groupMemberRepository;
            _customerService = customerService;
            _groupService = groupService;
            _postService = postService;
        }

        #endregion Ctor

        #region Methods

        #region Member

        public IPagedList<Member> GetAllFriends(int memberId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var friends = _FriendRepository.Table.Where(s => s.ToId == memberId || s.FromId == memberId).Select(s => s.FromId == memberId ? s.ToId : s.FromId).ToList();
            var query = _MemberRepository.Table.Where(s => !s.Deleted && s.StatusId == (int)ENStatusMember.Active);

            query = query.Where(s => friends.Contains(s.Id));

            if (!string.IsNullOrEmpty(KeySearch))
                query = query.Where(s => s.Name.Contains(KeySearch));
            return new PagedList<Member>(query, pageIndex, pageSize, getOnlyTotalCount);
        }

        public IPagedList<Member> GetAllFriendRequestSend(int memberId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from t1 in _MemberRepository.Table
                        join t2 in _FriendRequestRepository.Table on t1.Id equals t2.ToId
                        where !t1.Deleted && t1.StatusId == (int)ENStatusMember.Active && !t2.Deleted && t2.FromId == memberId
                        select t1;

            if (!string.IsNullOrEmpty(KeySearch))
                query = query.Where(s => s.Name.Contains(KeySearch));
            return new PagedList<Member>(query, pageIndex, pageSize);
        }

        public IPagedList<Member> GetAllFriendRequestReceive(int memberId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from t1 in _MemberRepository.Table
                        join t2 in _FriendRequestRepository.Table on t1.Id equals t2.FromId
                        where !t1.Deleted && t1.StatusId == (int)ENStatusMember.Active && !t2.Deleted && t2.ToId == memberId
                        select t1;

            if (!string.IsNullOrEmpty(KeySearch))
                query = query.Where(s => s.Name.Contains(KeySearch));
            return new PagedList<Member>(query, pageIndex, pageSize);
        }

        public IPagedList<Member> GetAllMemberBacklists(int memberId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var backlists = _MemberBackListRepository.Table.Where(s => s.ToId == memberId || s.FromId == memberId).Select(s => s.FromId == memberId ? s.ToId : s.FromId).ToList();
            var query = _MemberRepository.Table.Where(s => !s.Deleted && s.StatusId == (int)ENStatusMember.Active);
            query = query.Where(s => backlists.Contains(s.Id));
            if (!string.IsNullOrEmpty(KeySearch))
                query = query.Where(s => s.Name.Contains(KeySearch));
            return new PagedList<Member>(query, pageIndex, pageSize);
        }

        public IPagedList<Member> GetAllMemberGroup(int groupId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from t1 in _MemberRepository.Table
                        join t2 in _GroupMemberRepository.Table on t1.Id equals t2.MemberId
                        where !t1.Deleted && t1.StatusId == (int)ENStatusMember.Active && t2.StatusId == (int)ENStatusGroupRequest.confirm && groupId == t2.GroupId
                        select t1;

            if (!string.IsNullOrEmpty(KeySearch))
                query = query.Where(s => s.Name.Contains(KeySearch));
            return new PagedList<Member>(query, pageIndex, pageSize);
        }

        public IPagedList<Member> GetAllFriendNotInGroup(int groupId, int memberId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var list = (from t1 in _MemberRepository.Table
                        join t2 in _GroupMemberRepository.Table on t1.Id equals t2.MemberId
                        where !t1.Deleted && t1.StatusId == (int)ENStatusMember.Active && t2.StatusId == (int)ENStatusGroupRequest.confirm && groupId == t2.GroupId
                        select t1.Id).ToList();

            var friends = _FriendRepository.Table.Where(s => s.ToId == memberId || s.FromId == memberId).Select(s => s.FromId == memberId ? s.ToId : s.FromId).ToList();
            var query = _MemberRepository.Table.Where(s => !s.Deleted && s.StatusId == (int)ENStatusMember.Active);

            query = query.Where(s => friends.Contains(s.Id) && !list.Contains(s.Id));

            if (!string.IsNullOrEmpty(KeySearch))
                query = query.Where(s => s.Name.Contains(KeySearch));

            return new PagedList<Member>(query, pageIndex, pageSize);
        }

        public IPagedList<Member> GetAllMemberPagedList(string KeySearch = null, int StatusId = (int)ENStatusMember.Active, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _MemberRepository.Table.Where(s => !s.Deleted);
            if(StatusId > 0)
            {
                query = query.Where(s => s.StatusId == StatusId);
            }
            if (!string.IsNullOrEmpty(KeySearch))
                query = query.Where(s => s.Name.Contains(KeySearch));

            return new PagedList<Member>(query, pageIndex, pageSize, getOnlyTotalCount);
        }

        public IList<Member> GetAllMember(string KeySearch = null, int StatusId = (int)ENStatusMember.Active, string fireBaseId = null)
        {
            var query = _MemberRepository.Table.Where(s => !s.Deleted);
            if(StatusId > 0)
            {
                query = query.Where(s => s.StatusId == StatusId);
            }
            if (!string.IsNullOrEmpty(KeySearch))
            {
                query = query.Where(s => s.Name.Contains(KeySearch));
            }
            if (!string.IsNullOrEmpty(fireBaseId))
            {
                query = query.Where(s => s.FirebaseId == fireBaseId);
            }

            return query.ToList();
        }
        

        public IList<Member> GetAllMemberNotInGroup(int groupId)
        {
            var listmember = (from t1 in _MemberRepository.Table
                        join t2 in _GroupMemberRepository.Table on t1.Id equals t2.MemberId
                        where (t2.StatusId == (int)ENStatusGroupRequest.confirm && t2.GroupId == groupId)
                        select t1.Id).ToList();
            var query = _MemberRepository.Table.Where(s => !s.Deleted && s.StatusId == (int)ENStatusMember.Active && !listmember.Contains(s.Id));
            
            return query.ToList();
        }


        public Member GetMemberById(int id)
        {
            if (id == 0)
                return null;
            return _MemberRepository.GetById(id);
        }

        public Member GetMemberByCustomerId(int id)
        {
            if (id == 0)
                return null;
            return _MemberRepository.Table.FirstOrDefault(s => s.CustomerId == id);
        }

        public bool Insert(Member item, string content = "Insert")
        {
            if (item == null)
                return false;
            _MemberRepository.Insert(item);

            Insert(new MemberLog()
            {
                MemberId = item.Id,
                Content = content,
                After = JsonConvert.SerializeObject(item),
                CreatedAt = DateTime.Now,
                StatusId = (int)ENStatusLog.Insert
            });
            return true;
        }

        public bool Update(Member item, string content = "Update")
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Member));
            _MemberRepository.Update(item);

            Insert(new MemberLog()
            {
                MemberId = item.Id,
                Content = content,
                After = JsonConvert.SerializeObject(item),
                CreatedAt = DateTime.Now,
                StatusId = (int)ENStatusLog.Insert
            });
            return true;
        }

        public bool DeleteMember(int id, string content = "Delete")
        {
            var item = _MemberRepository.GetById(id);

            if (item == null)
                return false;
            item.Deleted = true;
            _MemberRepository.Update(item);

            Insert(new MemberLog()
            {
                MemberId = item.Id,
                Content = content,
                After = JsonConvert.SerializeObject(item),
                CreatedAt = DateTime.Now,
                StatusId = (int)ENStatusLog.Delete
            });
            return true;
        }

        #endregion Member

        #region MemberLog

        public IList<MemberLog> GetAllMemberLog(int MemberId, string KeySearch = null)
        {
            var query = _MemberLogRepository.Table.Where(s => s.MemberId == MemberId);
            if (!string.IsNullOrEmpty(KeySearch))
            {
                query = query.Where(s => s.Content.Contains(KeySearch));
            }
            return query.ToList();
        }

        public MemberLog GetMemberLogById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(MemberLog));
            return _MemberLogRepository.GetById(id);
        }

        public bool Insert(MemberLog item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MemberLog));
            _MemberLogRepository.Insert(item);

            return true;
        }

        public bool Update(MemberLog item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MemberLog));
            _MemberLogRepository.Update(item);

            return true;
        }

        public bool DeleteMemberLog(int id)
        {
            var item = _MemberLogRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(MemberLog));

            _MemberLogRepository.Delete(item);

            return true;
        }

        #endregion MemberLog

        #region Field

        public IList<Field> GetAllField(string KeySearch = null)
        {
            var query = _FieldRepository.Table;
            if (!string.IsNullOrEmpty(KeySearch))
            {
                query = query.Where(s => s.Name.Contains(KeySearch));
            }
            return query.ToList();
        }

        public IList<Field> GetAllFieldByMemberId(string KeySearch = null, int MemberId = 0, int StatusId = 0)
        {
            var query = from t1 in _FieldRepository.Table
                        join t2 in _MemberFieldRepository.Table on t1.Id equals t2.FieldId
                        where t2.MemberId == MemberId && (string.IsNullOrEmpty(KeySearch) || t1.Name.Contains(KeySearch)) && (StatusId == 0 || t1.StatusId == StatusId)
                        select t1;
            
            return query.ToList();
        }

        public Field GetFieldById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(Field));
            return _FieldRepository.GetById(id);
        }

        public bool Insert(Field item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Field));
            _FieldRepository.Insert(item);

            return true;
        }

        public bool Update(Field item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Field));
            _FieldRepository.Update(item);

            return true;
        }

        public bool DeleteField(int id)
        {
            var item = _FieldRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(Field));

            _FieldRepository.Delete(item);

            return true;
        }

        #endregion Field

        #region MemberField

        public IList<MemberField> GetAllMemberField(int MemberId = 0, int FieldId = 0)
        {
            var query = _MemberFieldRepository.Table;
            if (MemberId > 0)
            {
                query = query.Where(s => s.MemberId == MemberId);
            }
            if (FieldId > 0)
            {
                query = query.Where(s => s.FieldId == FieldId);
            }
            return query.ToList();
        }

        public MemberField GetMemberFieldById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(MemberField));
            return _MemberFieldRepository.GetById(id);
        }

        public bool Insert(MemberField item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MemberField));
            _MemberFieldRepository.Insert(item);

            return true;
        }

        public bool Update(MemberField item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MemberField));
            _MemberFieldRepository.Update(item);

            return true;
        }

        public bool Delete(MemberField item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MemberField));

            _MemberFieldRepository.Delete(item);

            return true;
        }

        #endregion MemberField

        #region Friend

        public IList<Friend> GetAllFriend(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(Friend));


            var query = _FriendRepository.Table.Where(s => s.FromId == id || s.ToId == id);
            query = query.Select(s => new Friend()
            {
                Id = s.Id,
                FromId = id == s.FromId ? s.FromId : s.ToId,
                ToId = id == s.FromId ? s.ToId : s.FromId
            });
            return query.ToList();
        }

        public Friend GetFriendById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(Friend));
            return _FriendRepository.GetById(id);
        }

        public bool CheckFriend(int FromId, int ToId)
        {
            if (FromId == 0 || ToId == 0)
                throw new ArgumentNullException(nameof(FriendRequest));
            bool bl = GetAllFriend(FromId).Any(s => s.ToId == ToId);
            return bl;
        }

        public Friend GetFriendByFromIdAndToId(int FromId, int ToId)
        {
            if (FromId == 0 || ToId == 0)
                throw new ArgumentNullException(nameof(Friend));

            return GetAllFriend(FromId).FirstOrDefault(s => s.ToId == ToId);
        }

        public bool Insert(Friend item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Friend));
            _FriendRepository.Insert(item);

            return true;
        }

        public bool Update(Friend item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Friend));
            _FriendRepository.Update(item);

            return true;
        }

        public bool DeleteFriend(int id)
        {
            var item = _FriendRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(Friend));

            _FriendRepository.Delete(item);

            return true;
        }

        #endregion Friend

        #region FriendRequest

        public IList<FriendRequest> GetAllFriendRequest(int FromId = 0, int ToId = 0)
        {
            var query = _FriendRequestRepository.Table;
            if (FromId > 0)
            {
                query = query.Where(s => s.FromId == FromId);
            }
            if (ToId > 0)
            {
                query = query.Where(s => s.ToId == ToId);
            }
            return query.ToList();
        }

        public FriendRequest GetFriendRequestById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(FriendRequest));
            return _FriendRequestRepository.GetById(id);
        }

        public FriendRequest GetFriendRequestByFromIdAndToId(int FromId, int ToId)
        {
            if (FromId == 0 || ToId == 0)
                throw new ArgumentNullException(nameof(FriendRequest));
            return _FriendRequestRepository.Table.FirstOrDefault(s => s.FromId == FromId && s.ToId == s.ToId);
        }

        public bool Insert(FriendRequest item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(FriendRequest));
            _FriendRequestRepository.Insert(item);

            return true;
        }

        public bool Update(FriendRequest item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(FriendRequest));
            _FriendRequestRepository.Update(item);

            return true;
        }

        public bool DeleteFriendRequest(int id)
        {
            var item = _FriendRequestRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(FriendRequest));

            _FriendRequestRepository.Delete(item);

            return true;
        }

        #endregion FriendRequest

        #region MemberBackList

        public IList<MemberBackList> GetAllMemberBackList(int FromId = 0, int ToId = 0)
        {
            var query = _MemberBackListRepository.Table;
            if (FromId > 0)
            {
                query = query.Where(s => s.FromId == FromId);
            }
            if (ToId > 0)
            {
                query = query.Where(s => s.ToId == ToId);
            }
            return query.ToList();
        }

        public bool CheckMemberBackList(int FromId, int ToId)
        {
            if (FromId == 0 || ToId == 0)
                throw new ArgumentNullException(nameof(FriendRequest));

            return _MemberBackListRepository.Table.Any(s => (s.FromId == FromId && s.ToId == ToId) || (s.FromId == ToId && s.ToId == FromId));
        }

        public bool CheckMemberBackListFrom(int FromId, int ToId)
        {
            if (FromId == 0 || ToId == 0)
                throw new ArgumentNullException(nameof(FriendRequest));

            return _MemberBackListRepository.Table.Any(s => (s.FromId == FromId && s.ToId == ToId));
        }

        public MemberBackList GetMemberBackListByFromIdAndToId(int FromId, int ToId)
        {
            if (FromId == 0 || ToId == 0)
                throw new ArgumentNullException(nameof(MemberBackList));
            return _MemberBackListRepository.Table.FirstOrDefault(s => (s.FromId == FromId && s.ToId == ToId) || (s.FromId == ToId && s.ToId == FromId));
        }

        public MemberBackList GetMemberBackListById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(MemberBackList));
            return _MemberBackListRepository.GetById(id);
        }

        public bool Insert(MemberBackList item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MemberBackList));
            _MemberBackListRepository.Insert(item);

            return true;
        }

        public bool Update(MemberBackList item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(MemberBackList));
            _MemberBackListRepository.Update(item);

            return true;
        }

        public bool DeleteMemberBackList(int id)
        {
            var item = _MemberBackListRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(MemberBackList));

            _MemberBackListRepository.Delete(item);

            return true;
        }

        public void UpdateCount()
        {
            var members = _MemberRepository.Table.Where(s => !s.Deleted && s.StatusId == (int)ENStatusMember.Active).ToList();
            foreach (var item in members)
            {
                item.CountFriend = GetAllFriends(item.Id, getOnlyTotalCount: true).TotalCount;
                item.CountGroup = _groupService.GetAllGroupByMemberId(item.Id, getOnlyTotalCount: true).TotalCount;
                item.CountPost = _postService.GetAllPostPagedList(MemberId: item.Id, getOnlyTotalCount: true).TotalCount;
                _MemberRepository.Update(item);
            }
        }

        #endregion MemberBackList

        #endregion Methods
    }
}