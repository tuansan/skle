using Nop.Core;
using Nop.Core.Domain.Skle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Skle
{
    public partial interface IMemberService
    {
        IList<Member> GetAllMember(string KeySearch = null, int StatusId = (int)ENStatusMember.Active, string fireBaseId = null);

        IPagedList<Member> GetAllMemberPagedList(string KeySearch = null, int StatusId = (int)ENStatusMember.Active, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        IPagedList<Member> GetAllFriends(int memberId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        IPagedList<Member> GetAllFriendRequestSend(int memberId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<Member> GetAllFriendRequestReceive(int memberId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<Member> GetAllMemberBacklists(int memberId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<Member> GetAllMemberGroup(int groupId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<Member> GetAllFriendNotInGroup(int groupId, int memberId, string KeySearch = null, int pageIndex = 0, int pageSize = int.MaxValue);

        IList<Member> GetAllMemberNotInGroup(int groupId);

        Member GetMemberById(int id);

        Member GetMemberByCustomerId(int id);

        bool Insert(Member item, string content = "Insert");

        bool Update(Member item, string content = "Update");

        bool DeleteMember(int id, string content = "Delete");

        IList<MemberLog> GetAllMemberLog(int MemberId, string KeySearch = null);

        MemberLog GetMemberLogById(int id);

        bool Insert(MemberLog item);

        bool Update(MemberLog item);

        bool DeleteMemberLog(int id);

        IList<Field> GetAllField(string KeySearch = null);

        IList<Field> GetAllFieldByMemberId(string KeySearch = null, int MemberId = 0, int StatusId = 0);

        Field GetFieldById(int id);

        bool Insert(Field item);

        bool Update(Field item);

        bool DeleteField(int id);

        IList<MemberField> GetAllMemberField(int MemberId = 0, int FieldId = 0);

        MemberField GetMemberFieldById(int id);

        bool Insert(MemberField item);

        bool Update(MemberField item);

        bool Delete(MemberField item);

        IList<Friend> GetAllFriend(int id);

        Friend GetFriendById(int id);

        Friend GetFriendByFromIdAndToId(int FromId, int ToId);

        bool CheckFriend(int FromId, int ToId);

        bool Insert(Friend item);

        bool Update(Friend item);

        bool DeleteFriend(int id);

        IList<FriendRequest> GetAllFriendRequest(int FromId = 0, int ToId = 0);

        FriendRequest GetFriendRequestById(int id);

        FriendRequest GetFriendRequestByFromIdAndToId(int FromId, int ToId);


        bool Insert(FriendRequest item);

        bool Update(FriendRequest item);

        bool DeleteFriendRequest(int id);

        IList<MemberBackList> GetAllMemberBackList(int FromId = 0, int ToId = 0);

        MemberBackList GetMemberBackListById(int id);

        bool CheckMemberBackList(int FromId, int ToId);
        bool CheckMemberBackListFrom(int FromId, int ToId);

        MemberBackList GetMemberBackListByFromIdAndToId(int FromId, int ToId);

        bool Insert(MemberBackList item);

        bool Update(MemberBackList item);

        bool DeleteMemberBackList(int id);

        void UpdateCount();
    }
}