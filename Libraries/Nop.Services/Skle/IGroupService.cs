using Nop.Core;
using Nop.Core.Domain.Skle;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Skle
{
    public partial interface IGroupService
    {
        IPagedList<Group> GetAllGroupPagedList(string KeySearch = null, int pageIndex = 0, int pagesize = int.MaxValue, bool getOnlyTotalCount = false);

        IList<Group> GetAllGroup(string KeySearch = null);

        IPagedList<Group> GetAllGroupByMemberId(int MemberId, int pageIndex = 0, int pagesize = int.MaxValue, bool getOnlyTotalCount = false);

        Group GetGroupById(int id);

        bool Insert(Group item, string content = "Insert");

        bool Update(Group item, string content = "Update");

        bool DeleteGroup(int id, string content = "Delete");

        IList<GroupLog> GetAllGroupLog(string KeySearch = null);

        GroupLog GetGroupLogById(int id);

        bool Insert(GroupLog item);

        bool Update(GroupLog item);

        bool DeleteGroupLog(int id);

        IList<GroupMember> GetAllGroupMember(int MemberId = 0, int GroupId = 0);

        GroupMember GetGroupMemberById(int id);

        GroupMember GetGroupMemberByGroupIdAndMemberId(int MemberId, int GroupId);

        bool Insert(GroupMember item);

        bool Update(GroupMember item);

        bool DeleteGroupMember(int id);
        void UpdateCountNewPostGroup(int memberId, int groupId);
        int GetCountNewPostGroup(int memberId, int groupId, int count);

        void UpdateCount();
    }
}