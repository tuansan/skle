using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Skle;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Skle
{
    public partial class GroupService : IGroupService
    {
        #region Fields

        private readonly IRepository<Group> _GroupRepository;
        private readonly IRepository<GroupLog> _GroupLogRepository;
        private readonly IRepository<GroupMember> _GroupMemberRepository;
        private readonly IRepository<CountNewPostGroup> _CountNewPostGroupRepository;
        private readonly IPostService _PostService;

        #endregion Fields

        #region Ctor

        public GroupService(IRepository<Group> groupRepository, IRepository<GroupLog> groupLogRepository, IRepository<GroupMember> groupMemberRepository, IRepository<CountNewPostGroup> countNewPostGroupRepository, IPostService postService)
        {
            _GroupRepository = groupRepository;
            _GroupLogRepository = groupLogRepository;
            _GroupMemberRepository = groupMemberRepository;
            _CountNewPostGroupRepository = countNewPostGroupRepository;
            _PostService = postService;
        }

        #endregion Ctor

        #region Methods

        #region Group

        public virtual IPagedList<Group> GetAllGroupPagedList(string KeySearch = null, int pageIndex = 0, int pagesize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _GroupRepository.Table.Where(s => !s.Deleted);
            if (!string.IsNullOrEmpty(KeySearch))
            {
                query = query.Where(s => s.Name.Contains(KeySearch));
            }

            return new PagedList<Group>(query, pageIndex, pagesize, getOnlyTotalCount);
        }
        
        public IPagedList<Group> GetAllGroupByMemberId(int MemberId, int pageIndex = 0, int pagesize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = from t1 in _GroupRepository.Table
                        join t2 in _GroupMemberRepository.Table on t1.Id equals t2.GroupId
                        where t2.MemberId == MemberId && !t1.Deleted && t2.StatusId == (int)ENStatusGroupRequest.confirm
                        select t1;
            
            return new PagedList<Group>(query, pageIndex, pagesize, getOnlyTotalCount);
        }
        
        public IList<Group> GetAllGroup(string KeySearch = null)
        {
            var query = _GroupRepository.Table.Where(s => !s.Deleted);
            if (!string.IsNullOrEmpty(KeySearch))
            {
                query = query.Where(s => s.Name.Contains(KeySearch));
            }
            return query.ToList();
        }

        public Group GetGroupById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(Group));
            return _GroupRepository.GetById(id);
        }

        public bool Insert(Group item, string content = "Insert")
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Group));
            _GroupRepository.Insert(item);

            Insert(new GroupLog()
            {
                GroupId = item.Id,
                Content = content,
                After = JsonConvert.SerializeObject(item),
                CreatedAt = DateTime.Now,
                StatusId = (int)ENStatusLog.Insert
            });

            return true;
        }

        public bool Update(Group item, string content = "Update")
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Group));
            _GroupRepository.Update(item);

            Insert(new GroupLog()
            {
                GroupId = item.Id,
                Content = content,
                After = JsonConvert.SerializeObject(item),
                CreatedAt = DateTime.Now,
                StatusId = (int)ENStatusLog.Update
            });
            return true;
        }

        public bool DeleteGroup(int id, string content = "Delete")
        {
            var item = _GroupRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(Group));
            item.Deleted = true;
            _GroupRepository.Update(item);

            Insert(new GroupLog()
            {
                GroupId = item.Id,
                Content = content,
                After = JsonConvert.SerializeObject(item),
                CreatedAt = DateTime.Now,
                StatusId = (int)ENStatusLog.Delete
            });
            return true;
        }

        #endregion Group

        #region GroupLog

        public IList<GroupLog> GetAllGroupLog(string KeySearch = null)
        {
            var query = _GroupLogRepository.Table;
            if (!string.IsNullOrEmpty(KeySearch))
            {
                query = query.Where(s => s.Content.Contains(KeySearch));
            }
            return query.ToList();
        }

        public GroupLog GetGroupLogById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(GroupLog));
            return _GroupLogRepository.GetById(id);
        }

        public bool Insert(GroupLog item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(GroupLog));
            _GroupLogRepository.Insert(item);

            return true;
        }

        public bool Update(GroupLog item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(GroupLog));
            _GroupLogRepository.Update(item);

            return true;
        }

        public bool DeleteGroupLog(int id)
        {
            var item = _GroupLogRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(GroupLog));

            _GroupLogRepository.Delete(item);

            return true;
        }

        #endregion GroupLog

        #region GroupMember

        public IList<GroupMember> GetAllGroupMember(int MemberId = 0, int GroupId = 0)
        {
            var query = _GroupMemberRepository.Table;
            if (MemberId > 0)
            {
                query = query.Where(s => s.MemberId == MemberId);
            }
            if (GroupId > 0)
            {
                query = query.Where(s => s.GroupId == GroupId);
            }
            return query.ToList();
        }

        public GroupMember GetGroupMemberByGroupIdAndMemberId(int MemberId, int GroupId)
        {
            if (MemberId == 0 || GroupId ==0)
                throw new ArgumentNullException(nameof(GroupMember));

            return _GroupMemberRepository.Table.FirstOrDefault(s => s.MemberId == MemberId && s.GroupId == GroupId);
        }
        

        public GroupMember GetGroupMemberById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(GroupMember));
            return _GroupMemberRepository.GetById(id);
        }

        public bool Insert(GroupMember item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(GroupMember));
            _GroupMemberRepository.Insert(item);

            return true;
        }

        public bool Update(GroupMember item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(GroupMember));
            _GroupMemberRepository.Update(item);

            return true;
        }

        public bool DeleteGroupMember(int id)
        {
            var item = _GroupMemberRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(GroupMember));

            _GroupMemberRepository.Delete(item);

            return true;
        }

        #endregion GroupMember


        #region CountNewPostGroup

        public void UpdateCountNewPostGroup(int memberId, int groupId)
        {
            var item = _CountNewPostGroupRepository.Table.Where(s => s.MemberId == memberId && s.GroupId == groupId).FirstOrDefault();
            var gr = GetGroupById(groupId);
            if (item == null)
            {
                item = new CountNewPostGroup() { GroupId = groupId, MemberId = memberId, CountPost = gr.CountPost };
                _CountNewPostGroupRepository.Insert(item);
            }
            else
            {
                item.CountPost = gr.CountPost;
                _CountNewPostGroupRepository.Update(item);
            }
        }

        public int GetCountNewPostGroup(int memberId, int groupId, int count)
        {
            var item = _CountNewPostGroupRepository.Table.Where(s => s.MemberId == memberId && s.GroupId == groupId).FirstOrDefault();
            if (item == null) {
                item = new CountNewPostGroup() { MemberId = memberId, GroupId = groupId, CountPost = count };
                _CountNewPostGroupRepository.Insert(item);
            }
            return count - item.CountPost;
        }

        public void UpdateCount()
        {
            var groups = _GroupRepository.Table.Where(s => !s.Deleted).ToList();
            foreach (var item in groups)
            {
                item.CountMember = _GroupMemberRepository.Table.Count(s => s.GroupId == item.Id && s.StatusId == (int)ENStatusGroupRequest.confirm);
                item.CountPost = _PostService.GetAllPostPagedList(item.Id, getOnlyTotalCount: true).TotalCount;
                _GroupRepository.Update(groups);
            }

        }

        #endregion

        #endregion Methods
    }
}