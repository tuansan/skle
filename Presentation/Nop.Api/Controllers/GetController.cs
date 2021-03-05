using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Api.Factories;
using Nop.Api.Infrastructure.Mapper.Extensions;
using Nop.Api.Models;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Skle;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Skle;
using System;
using System.Linq;

namespace Nop.Api.Controllers
{
    [Route("api/Get/{action}")]
    public class GetController : BaseAuthenController
    {
        #region Fields

        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly IGroupService _groupService;
        private readonly IMemberService _memberService;
        private readonly IProvinceService _provinceService;
        private readonly IPostService _postService;
        private readonly ICommonFactory _commonFactory;
        private readonly INotificationService _notificationService;
        private readonly ICustomerService _customerService;
        private readonly StoreInformationSettings _storeInformationSettings;

        #endregion Fields

        #region Ctor

        public GetController(IDownloadService downloadService, ILocalizationService localizationService, IPictureService pictureService, IWebHelper webHelper, IGroupService groupService, IMemberService memberService, IProvinceService provinceService, IPostService postService, ICommonFactory commonFactory, INotificationService notificationService, ICustomerService customerService, StoreInformationSettings storeInformationSettings)
        {
            _downloadService = downloadService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _webHelper = webHelper;
            _groupService = groupService;
            _memberService = memberService;
            _provinceService = provinceService;
            _postService = postService;
            _commonFactory = commonFactory;
            _notificationService = notificationService;
            _customerService = customerService;
            _storeInformationSettings = storeInformationSettings;
        }

        #endregion Ctor

        #region Methos

        #region Common

        public IActionResult Index()
        {
            return Ok("test");
        }

        [HttpGet]
        public virtual IActionResult DownloadFile(Guid downloadGuid)
        {
            var download = _downloadService.GetDownloadByGuid(downloadGuid);
            if (download == null)
                return BadRequest("No download record found with the specified id");

            return Ok(download);
        }

        [HttpGet]
        public virtual IActionResult TestGet(Guid downloadGuid)
        {
            var currentMember = currentMemberId;
            return Ok("Ok");
        }

        [HttpGet]
        [AllowAnonymous]
        public virtual IActionResult TestGetPuclic(Guid downloadGuid)
        {
            return Ok("Ok");
        }

        [HttpGet]
        public virtual IActionResult RestartAppApi()
        {
            //restart application
            _webHelper.RestartAppDomain();
            return Ok();
        }

        private PostModel EntityPostToModel(Post entity, bool isFull = false)
        {
            var model = entity.ToModel<PostModel>();
            var listFile = _postService.GetAllPostFile(entity.Id);
            foreach (var item in listFile)
            {
                var file = new GetPostFileModel()
                {
                    MimeType = item.MimeType,
                    Id = item.Id
                };
                switch (item.MimeType.Split("/").First())
                {
                    case "image":
                        if (isFull)
                            file.Url = _pictureService.GetPictureUrl(item.PictureId);
                        else
                            file.Url = _pictureService.GetPictureUrl(item.PictureId, 640);
                        break;

                    case "video":
                        if (isFull)
                            file.Url = item.VideoUrl;
                        else
                            file.Url = item.VideoUrl.Replace("/video/upload/", "/video/upload/thumbs/").Replace(item.VideoUrl.Split(".").LastOrDefault(), "jpg");
                        break;

                    case "audio":
                        file.Url = item.VideoUrl;
                        break;

                    default:
                        var download = _downloadService.GetDownloadById(item.DownloadId);
                        file.FileName = download.Filename + download.Extension;
                        file.Url = Url.ActionLink("GetFileUpload", "Home", new { downloadId = download.DownloadGuid });
                        break;
                }
                model.ListFile.Add(file);
            }
            var mem = _memberService.GetMemberById(entity.MemberId);
            model.AvatarUrl = _pictureService.GetPictureUrl(mem.AvatarId, defaultPictureType: Core.Domain.Media.PictureType.Avatar);
            model.Name = mem.Name;
            model.isLike = _postService.GetAllPostLike(entity.Id).Any(s => s.MemberId == currentMemberId);
            model.isSpam = _postService.GetAllPostSpam(entity.Id).Any(s => s.MemberId == currentMemberId);
            return model;
        }

        private MemberModel ReturnMemberModel(Member entity)
        {
            var model = entity.ToModel<MemberModel>();
            model.AvatarUrl = _pictureService.GetPictureUrl(pictureId: entity.AvatarId, defaultPictureType: PictureType.Avatar);
            model.CoveUrl = _pictureService.GetPictureUrl(pictureId: entity.CoveId, showDefaultPicture: false);
            model.Province = entity.ProvinceId > 0 ? _provinceService.GetProvinceById(entity.ProvinceId).Name : "";
            model.PhoneNumber = _customerService.GetCustomerById(entity.CustomerId).PhoneNumber;
            if (entity.Id == currentMemberId)
                model.StatusFrienRequestId = (int)ENStatusFrienRequest.current;
            else
            {
                if (_memberService.CheckFriend(currentMemberId, entity.Id))
                    model.StatusFrienRequestId = (int)ENStatusFrienRequest.confirm;
                else
                {
                    var frq = _memberService.GetFriendRequestByFromIdAndToId(currentMemberId, entity.Id);
                    if (frq == null)
                        model.StatusFrienRequestId = (int)ENStatusFrienRequest.none;
                    else
                        model.StatusFrienRequestId = frq.Deleted ? (int)ENStatusFrienRequest.cancel : (int)ENStatusFrienRequest.wait_confirm;

                    frq = _memberService.GetFriendRequestByFromIdAndToId(entity.Id, currentMemberId);

                    if (frq != null && !frq.Deleted)
                        model.StatusFrienRequestId = (int)ENStatusFrienRequest.confirm_friend;
                }
            }
            model.Fields = _memberService.GetAllFieldByMemberId(MemberId: entity.Id, StatusId: (int)ENStatusField.show).Select(s => s.Name).ToList();
            model.isHiddenPost = _postService.CheckPostHidden(currentMemberId, entity.Id);
            return model;
        }

        #endregion Common

        #region Group

        [HttpGet]
        public virtual IActionResult GetAllGroup(string KeySearch, int PageIndex = 0, int PageSize = int.MaxValue)
        {
            return Ok(MessageReturn.Success("Ok", _groupService.GetAllGroupPagedList(KeySearch, PageIndex, PageSize).Select(s => EntityToGroupModel(s, true))));
        }

        private GroupModel EntityToGroupModel(Group entity, bool isList = false)
        {
            var model = entity.ToModel<GroupModel>();
            var groupMemberRequest = _groupService.GetGroupMemberByGroupIdAndMemberId(currentMemberId, entity.Id);

            if (groupMemberRequest == null)
                model.StatusGroupRequestId = (int)ENStatusGroupRequest.none;
            else
            {
                model.StatusGroupRequestId = groupMemberRequest.StatusId;
                if (model.StatusGroupRequestId == (int)ENStatusGroupRequest.cancel && groupMemberRequest.CreatedAt < DateTime.Now.AddMonths(-1))
                    model.StatusGroupRequestId = (int)ENStatusGroupRequest.none;
            }
            if (isList)
            {
                if (model.StatusGroupRequestId == (int)ENStatusGroupRequest.confirm)
                {
                    model.CountPostNew = _groupService.GetCountNewPostGroup(currentMemberId, entity.Id, entity.CountPost);
                    model.CountPostNew = model.CountPostNew < 0 ? 0 : model.CountPostNew;
                }
                model.AvatarUrl = _pictureService.GetPictureUrl(entity.AvatarId, defaultPictureType: Core.Domain.Media.PictureType.Avatar);
                model.CoverUrl = _pictureService.GetPictureUrl(entity.CoveId, showDefaultPicture: false);
            }
            return model;
        }

        private GroupModel EntityToGroupModelByPostId(Post post)
        {
            var entity = _groupService.GetGroupById(post.GroupId);
            var model = entity.ToModel<GroupModel>();
            var groupMemberRequest = _groupService.GetGroupMemberByGroupIdAndMemberId(currentMemberId, entity.Id);

            if (groupMemberRequest == null)
                model.StatusGroupRequestId = (int)ENStatusGroupRequest.none;
            else
            {
                model.StatusGroupRequestId = groupMemberRequest.StatusId;
                if (model.StatusGroupRequestId == (int)ENStatusGroupRequest.cancel && groupMemberRequest.CreatedAt < DateTime.Now.AddMonths(-1))
                    model.StatusGroupRequestId = (int)ENStatusGroupRequest.none;
            }
            model.PostId = post.Id;
            return model;
        }

        [HttpGet]
        public virtual IActionResult GetGroupByGroupId(int GroupId)
        {
            var group = _groupService.GetGroupById(GroupId);
            var model = EntityToGroupModel(group);

            return Ok(MessageReturn.Success("Ok", model));
        }

        [HttpGet]
        public virtual IActionResult GetAllGroupByMemberId()
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var groups = _groupService.GetAllGroupByMemberId(currentMemberId);
            return Ok(MessageReturn.Success("Ok", groups.Select(s => EntityToGroupModel(s))));
        }

        #endregion Group

        #region Province

        [HttpGet]
        [AllowAnonymous]
        public virtual IActionResult GetAllProvince(string KeySearch)
        {
            return Ok(MessageReturn.Success("Ok", _provinceService.GetAllProvince(KeySearch)));
        }

        #endregion Province

        #region Field

        [HttpGet]
        [AllowAnonymous]
        public virtual IActionResult GetAllField(string KeySearch)
        {
            return Ok(MessageReturn.Success("Ok", _memberService.GetAllField(KeySearch)));
        }

        #endregion Field

        #region Get picture

        [HttpGet]
        public virtual IActionResult GetAllPicture(int MemberId, int PageIndex = 0, int PageSize = 20)
        {
            if (MemberId == 0)
                return BadRequest();
            var model = _postService.GetAllPicturePostByMemberId(MemberId, PageIndex, PageSize);

            return Ok(MessageReturn.Success("Ok", model.Select(s => new PicturePostModel() { CreateAt = s.CreatedAt, PictureUrl = _pictureService.GetPictureUrl(s.PictureId) })));
        }

        #endregion Get picture

        #region Get avatar member

        [HttpGet]
        public virtual IActionResult AvartarMember(int MemberId)
        {
            if (MemberId == 0)
                return BadRequest();
            var Member = _memberService.GetMemberById(MemberId);

            return Ok(MessageReturn.Success("Ok", new
            {
                Name = Member.Name,
                AvatarUrl = _pictureService.GetPictureUrl(Member.AvatarId, 100, defaultPictureType: Core.Domain.Media.PictureType.Avatar)
            }));
        }

        #endregion Get avatar member

        #region Message

        [HttpGet]
        public virtual IActionResult MessageByGroup(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var mms = _notificationService.GetAllMemberMessage(currentMemberId, pageIndex, pageSize);
            var res = mms.Select(s =>
            {
                var mtg = _notificationService.GetMessageToGroupById(s.MessageId);
                var mem = _memberService.GetMemberById(mtg.MemberId);
                var grname = _groupService.GetGroupById(mtg.GroupId).Name;
                return new
                {
                    Id = s.Id,
                    MemberId = mem.Id,
                    AvatarUrl = _pictureService.GetPictureUrl(mem.AvatarId, 100, defaultPictureType: Core.Domain.Media.PictureType.Avatar),
                    MemberName = mem.Name,
                    GroupName = grname,
                    Content = mtg.Content,
                    CreatedAt = mtg.CreatedAt
                };
            });
            return Ok(MessageReturn.Success("Ok", res));
        }

        [HttpDelete]
        public virtual IActionResult MessageByGroup(int id)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var mess = _notificationService.GetMemberMessageById(id);
            if(mess == null)
                return Ok(MessageReturn.Error(_localizationService.GetResource("Xoa.MessageByGroup.Fail")));

            _notificationService.Delete(mess);

            return Ok(MessageReturn.Success("Ok"));
        }

        #endregion Message

        #region Filebase

        [HttpGet]
        public virtual IActionResult FirebaseId(int id)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var member = _memberService.GetMemberById(id);

            return Ok(MessageReturn.Success("Ok", member.FirebaseId));
        }

        #endregion Filebase

        #region notification

        [HttpGet]
        public virtual IActionResult GetNotification(int pageIndex = 0, int pageSize = int.MaxValue, string KeySearch = null)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();
            var notis = _notificationService.GetAllNotiMobi(currentMemberId, pageIndex, pageSize, true, KeySearch);

            _notificationService.Insert(_notificationService.GetNotiNewById(currentMemberId));
            return Ok(MessageReturn.Success("Ok", notis.Select(s =>
            {
                string title = string.Empty;
                switch (s.TypeId)
                {
                    case (int)ENTypeNotiMobi.he_thong:
                        title = _localizationService.GetResource("NotiMobi.hethong");
                        s.Name = null;
                        break;

                    //case (int)ENTypeNotiMobi.tin_nhan:
                    //    title = _localizationService.GetResource("NotiMobi.tinnhan");
                    //    break;

                    case (int)ENTypeNotiMobi.bai_dang:
                        title = _localizationService.GetResource("NotiMobi.baidang");
                        break;

                    case (int)ENTypeNotiMobi.loimoi_ketban:
                        title = _localizationService.GetResource("NotiMobi.loimoi.ketban");
                        break;

                    case (int)ENTypeNotiMobi.xacnhan_ketban:
                        title = _localizationService.GetResource("NotiMobi.xacnhan.ketban");
                        break;

                    case (int)ENTypeNotiMobi.xacnhan_vaonhom:
                        title = _localizationService.GetResource("NotiMobi.xacnhan.vaonhom");
                        break;

                    case (int)ENTypeNotiMobi.invite_group:
                        title = _localizationService.GetResource("NotiMobi.invite.group");
                        break;

                    case (int)ENTypeNotiMobi.like:
                        title = _localizationService.GetResource("NotiMobi.member.like");
                        break;
                }

                if (!string.IsNullOrEmpty(s.Title))
                {
                    title += ": " + s.Title;
                }
                return new
                {
                    Id = s.Id,
                    TypeId = s.TypeId,
                    //PictureUrl = s.PictureUrl,
                    Name = s.Name,
                    Content = s.Content,
                    Title = title,
                    CreatedAt = s.CreatedAt,
                    isRead = _notificationService.CheckClick(currentMemberId, s.Id)
                };
            })));
        }

        [HttpGet]
        public virtual IActionResult GetValueNotification(int id)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var noti = _notificationService.GetNotiMobiById(id);
            if (noti == null)
                return BadRequest();
            _notificationService.Insert(new NotiMobiClick()
            {
                NotificationId = id,
                MemberId = currentMemberId
            });
            switch (noti.TypeId)
            {
                case (int)ENTypeNotiMobi.he_thong:
                    return Ok(MessageReturn.Success("Ok"));

                case (int)ENTypeNotiMobi.tin_nhan:
                    var mtg = _notificationService.GetMessageToGroupById(noti.MessageId);
                    var mem = _memberService.GetMemberById(mtg.MemberId);
                    var grname = _groupService.GetGroupById(mtg.GroupId).Name;
                    var mm = _notificationService.GetMemberMessageByMemberIdAndMtgId(currentMemberId, noti.MessageId);
                    return Ok(MessageReturn.Success("Ok", new
                    {
                        Id = mm.Id,
                        MemberId = mem.Id,
                        AvatarUrl = _pictureService.GetPictureUrl(mem.AvatarId, 100, defaultPictureType: Core.Domain.Media.PictureType.Avatar),
                        MemberName = mem.Name,
                        GroupName = grname,
                        Content = mtg.Content,
                        CreatedAt = mtg.CreatedAt
                    }));

                case (int)ENTypeNotiMobi.loimoi_ketban:
                case (int)ENTypeNotiMobi.xacnhan_ketban:
                    var member2 = _memberService.GetMemberById(noti.currentMemberId);
                    return Ok(MessageReturn.Success("Ok", ReturnMemberModel(member2)));

                case (int)ENTypeNotiMobi.bai_dang:
                case (int)ENTypeNotiMobi.like:
                    var post = _postService.GetPostById(noti.PostId);
                    return Ok(MessageReturn.Success("Ok", EntityToGroupModelByPostId(post)));

                case (int)ENTypeNotiMobi.xacnhan_vaonhom:
                case (int)ENTypeNotiMobi.invite_group:
                    var group = _groupService.GetGroupById(noti.GroupId);
                    return Ok(MessageReturn.Success("Ok", EntityToGroupModel(group)));
            }

            return BadRequest();
        }

        [HttpGet]
        public virtual IActionResult GetNewNoti()
        {
            var notinew = _notificationService.GetNotiNewById(currentMemberId);
            if (notinew.Id == 0)
                return Ok(MessageReturn.Success("Ok", _notificationService.CheckNewNoti(currentMemberId)));
            return Ok(MessageReturn.Success("Ok", _notificationService.CheckNewNoti(currentMemberId, notinew.ReadTime)));
        }

        #endregion notification

        #region Banner

        [HttpGet]
        [AllowAnonymous]
        public virtual IActionResult GetBanner(string KeySearch)
        {
            return Ok(MessageReturn.Success("Ok", _pictureService.GetPictureUrl(_storeInformationSettings.BannerPictureId, 1000)));
        }
        #endregion Banner

        #endregion Methos
    }
}