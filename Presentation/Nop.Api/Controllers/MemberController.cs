using Microsoft.AspNetCore.Mvc;
using Nop.Api.Factories;
using Nop.Api.Infrastructure.Mapper.Extensions;
using Nop.Api.Models;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Skle;
using Nop.Services.Customers;
using Nop.Services.Media;
using Nop.Services.Skle;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Api.Controllers
{
    public class MemberController : BaseRouteController
    {
        private readonly IMemberService _memberService;
        private readonly IPictureService _pictureService;
        private readonly IProvinceService _provinceService;
        private readonly ICustomerService _customerService;
        private readonly IGroupService _groupService;
        private readonly ICommonFactory _commonFactory;
        private readonly IPostService _postService;

        public MemberController(IMemberService memberService, IPictureService pictureService, IProvinceService provinceService, ICustomerService customerService, IGroupService groupService, ICommonFactory commonFactory, IPostService postService)
        {
            _memberService = memberService;
            _pictureService = pictureService;
            _provinceService = provinceService;
            _customerService = customerService;
            _groupService = groupService;
            _commonFactory = commonFactory;
            _postService = postService;
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

        private MemberTinyModel ReturnMemberTinyModel(Member entity)
        {
            var model = new MemberTinyModel();
            model.Id = entity.Id;
            model.Name = entity.Name;
            model.AvatarUrl = _pictureService.GetPictureUrl(pictureId: entity.AvatarId, defaultPictureType: PictureType.Avatar);
            model.CoveUrl = _pictureService.GetPictureUrl(pictureId: entity.CoveId, showDefaultPicture: false);
            if (entity.ProvinceId > 0)
            {
                var province = _provinceService.GetProvinceById(entity.ProvinceId);
                model.Province = province.Name;
            }
            model.Fields = _memberService.GetAllFieldByMemberId(MemberId: entity.Id, StatusId: (int)ENStatusField.show).Select(s => s.Name).ToList();
            return model;
        }

        [HttpGet]
        public IActionResult Gett(string KeySearch = null, int BranchId = (int)ENGetMemberBranch.ALL, int GroupId = 0, int PageIndex = 0, int PageSize = int.MaxValue)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            switch (BranchId)
            {
                case (int)ENGetMemberBranch.FRIEND:
                    var list1 = _memberService.GetAllFriends(currentMemberId, KeySearch, PageIndex, PageSize);
                    return Ok(MessageReturn.Success("Ok", list1.Select(s => ReturnMemberTinyModel(s))));
                case (int)ENGetMemberBranch.FRIEND_REQUEST_SEND:
                    var list2 = _memberService.GetAllFriendRequestSend(currentMemberId, KeySearch, PageIndex, PageSize);
                    return Ok(MessageReturn.Success("Ok", list2.Select(s => ReturnMemberTinyModel(s))));
                case (int)ENGetMemberBranch.FRIEND_REQUEST_RECEIVE:
                    var list3 = _memberService.GetAllFriendRequestReceive(currentMemberId, KeySearch, PageIndex, PageSize);
                    return Ok(MessageReturn.Success("Ok", list3.Select(s => ReturnMemberTinyModel(s))));
                case (int)ENGetMemberBranch.MEMBER_BACK_LIST:
                    var list4 = _memberService.GetAllMemberBacklists(currentMemberId, KeySearch, PageIndex, PageSize);
                    return Ok(MessageReturn.Success("Ok", list4.Select(s => ReturnMemberTinyModel(s))));
                case (int)ENGetMemberBranch.MEMBER_GROUP:
                    var list5 = _memberService.GetAllMemberGroup(GroupId, KeySearch, PageIndex, PageSize);
                    return Ok(MessageReturn.Success("Ok", list5.Select(s => ReturnMemberTinyModel(s))));
                case (int)ENGetMemberBranch.FRIEND_NOTIN_GROUP:
                    var list6 = _memberService.GetAllFriendNotInGroup(GroupId, currentMemberId, KeySearch, PageIndex, PageSize);
                    return Ok(MessageReturn.Success("Ok", list6.Select(s => ReturnMemberTinyModel(s))));
                default:
                    var list = _memberService.GetAllMemberPagedList(KeySearch, (int)ENStatusMember.Active, PageIndex, PageSize);
                    return Ok(MessageReturn.Success("Ok", list.Select(s => ReturnMemberTinyModel(s))));
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var res = _memberService.GetMemberById(id);
            if (res == null)
                return Ok(MessageReturn.Error("Không có dữ liệu"));
            var model = ReturnMemberModel(res);

            return Ok(MessageReturn.Success("Ok", model));
        }

        [HttpPost]
        public IActionResult Post(Member item)
        {
            return BadRequest();
        }

        [HttpPut]
        public IActionResult Put(Member item)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();
            if (item.Id != currentMemberId)
                return BadRequest();

            var res = _memberService.Update(item);
            if (res)
                return Ok(MessageReturn.Success("Ok"));
            return Ok(MessageReturn.Error("Lỗi"));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var res = _memberService.DeleteMember(id);
            if (res)
                return Ok(MessageReturn.Success("Ok"));
            return Ok(MessageReturn.Error("Lỗi"));
        }
    }
}