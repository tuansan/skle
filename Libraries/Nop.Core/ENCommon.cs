namespace Nop.Core
{
    public enum ENStatusLog
    {
        Delete, Update, Insert
    }

    public enum ENStatusDeleted
    {
        All, Active, Deleted
    }

    public enum ENStatusMember
    {
        All, Active, Not_authentic, Blocked
    }

    public enum ENSortByPost
    {
        time, like, spam
    }

    public enum ENGetMemberBranch
    {
        ALL, FRIEND, FRIEND_REQUEST_SEND, FRIEND_REQUEST_RECEIVE, MEMBER_BACK_LIST, MEMBER_GROUP, FRIEND_NOTIN_GROUP
    }

    public enum ENStatusGroupRequest
    {
        none, wait_confirm, confirm, cancel
    }

    public enum ENStatusFrienRequest
    {
        none, wait_confirm, confirm, cancel, current, confirm_friend
    }

    public enum ENStatusField
    {
        all, show, hide
    }

    public enum ENTypeNotification
    {
        all, member, group
    }

    public enum ENTypeNotiMobi
    {
        he_thong, tin_nhan, bai_dang, loimoi_ketban, xacnhan_ketban, xacnhan_vaonhom, invite_group, like
    }
}