using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Skle;
using Nop.Web.Areas.Admin.Models.Skle;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class CloudFirestoreController : BaseAdminController
    {
        protected readonly INopFileProvider _fileProvider;
        private readonly IPermissionService _permissionService;
        private readonly IMemberService _memberService;
        private readonly IPictureService _pictureService;

        public CloudFirestoreController(INopFileProvider fileProvider, IPermissionService permissionService, IMemberService memberService, IPictureService pictureService)
        {
            _fileProvider = fileProvider;
            _permissionService = permissionService;
            _memberService = memberService;
            _pictureService = pictureService;
        }

        private FirestoreDb firestoreDb => FirestoreDb.Create("skle-6e296", new FirestoreClientBuilder { JsonCredentials = System.IO.File.ReadAllText(_fileProvider.MapPath("firebase-adminsdk.json")) }.Build());

        public async Task<IActionResult> IndexAsync()
        {
            var usersRef = firestoreDb.Collection("chatRooms/8,5/room");
            var snapshot = await usersRef.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> documentDictionary = document.ToDictionary();
            }
            return Content("Ok");
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        #region ListRoomChat

        public virtual IActionResult ListRoomChat(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new RoomChatSearchModel();

            model.SetGridPageSize();
            model.MemberId = id;
            model.Members = _memberService.GetAllMember(StatusId: (int)ENStatusMember.Active).Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(s.Name + " #" + s.Id, s.Id.ToString())).ToList();
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteRoomChatSelectedAsync(ICollection<string> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    var mem = item.Split(",");
                    DocumentReference mem1 = firestoreDb.Collection("users/" + mem.FirstOrDefault() + "/chatlist").Document(item);
                    DocumentReference mem2 = firestoreDb.Collection("users/" + mem.LastOrDefault() + "/chatlist").Document(item);
                    DocumentReference room = firestoreDb.Collection("chatRooms").Document(item);
                    await mem1.DeleteAsync();
                    await mem2.DeleteAsync();
                    await room.DeleteAsync();
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ListRoomChatAsync(RoomChatSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var usersRef = firestoreDb.Collection("users/" + searchModel.MemberId + "/chatlist");
            var query = usersRef.OrderByDescending("timestamp");
            var snapshot = await query.GetSnapshotAsync();
            var list = new List<RoomChat>();
            int totalCount = snapshot.Documents.Count;
            int start = (searchModel.Page - 1) * searchModel.PageSize;
            int end = searchModel.Page * searchModel.PageSize;
            if (end > totalCount) end = totalCount;

            for (int i = start; i < end; i++)
            {
                Dictionary<string, object> documentDictionary = snapshot.Documents[i].ToDictionary();
                var serializeDictionary = JsonConvert.SerializeObject(documentDictionary);
                var doc = JsonConvert.DeserializeObject<RoomChat>(serializeDictionary);
                list.Add(doc);
            }

            var entitys = new PagedList<RoomChat>(list, searchModel.Page - 1, searchModel.PageSize, totalCount);

            var model = new RoomChatListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var member = _memberService.GetMemberById(int.Parse(entity.chatWith));
                    return new RoomChatModel()
                    {
                        avatarUrl = _pictureService.GetPictureUrl(member.AvatarId, 100, defaultPictureType: Core.Domain.Media.PictureType.Avatar),
                        chatId = entity.chatId,
                        lastChat = entity.lastChat,
                        name = member.Name,
                        times = UnixTimeStampToDateTime(entity.timestamp),
                        timeCreate = UnixTimeStampToDateTime(entity.timeCreate)
                    };
                });
            });

            return Json(model);
        }

        #endregion ListRoomChat
        #region Chat
        public virtual IActionResult Chat(string id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new ChatSearchModel();
            model.SetGridPageSize();
            model.chatId = id;
            if (int.TryParse(id.Split(",").FirstOrDefault(), out int id1))
            {
                model.mem1 = _memberService.GetMemberById(id1).Name;
            }
            if (int.TryParse(id.Split(",").LastOrDefault(), out int id2))
            {
                model.mem2 = _memberService.GetMemberById(id2).Name;
            }
            return View(model);
        }


        [HttpPost]
        public virtual async Task<IActionResult> ListChatAsync(ChatSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var usersRef = firestoreDb.Collection("chatRooms/" + searchModel.chatId + "/room");
            var query = usersRef.OrderByDescending("timestamp");
            var snapshot = await query.GetSnapshotAsync();
            var list = new List<Chat>();
            int totalCount = snapshot.Documents.Count;
            int start = (searchModel.Page - 1) * searchModel.PageSize;
            int end = searchModel.Page * searchModel.PageSize;
            if (end > totalCount) end = totalCount;
            for (int i = start; i < end; i++)
            {
                Dictionary<string, object> documentDictionary = snapshot.Documents[i].ToDictionary();
                var serializeDictionary = JsonConvert.SerializeObject(documentDictionary);
                var doc = JsonConvert.DeserializeObject<Chat>(serializeDictionary);
                doc.Id = snapshot.Documents[i].Id;
                list.Add(doc);
            }

            var entitys = new PagedList<Chat>(list, searchModel.Page - 1, searchModel.PageSize, totalCount: totalCount);

            var model = new ChatListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var member1 = _memberService.GetMemberById(int.Parse(entity.idFrom));
                    string name1 = string.Empty;
                    string name2 = string.Empty;
                    if (searchModel.chatId.Equals(entity.idFrom + "," + entity.idTo))
                    {
                        name1 = member1.Name;
                    }
                    else
                    {
                        name2 = member1.Name;
                    }
                    string content = string.Empty;
                    if (entity.type == "image")
                    {
                        content = String.Join(",", entity.content);
                    }
                    else content = entity.content;
                    return new ChatModel()
                    {
                        times = UnixTimeStampToDateTime(entity.timestamp),
                        Name1 = name1,
                        Name2 = name2,
                        content = content,
                        type = entity.type,
                        Id = entity.Id
                    };
                });
            });

            return Json(model);
        }


        [HttpPost]
        public virtual async Task<IActionResult> DeleteChatSelectedAsync(ICollection<string> selectedIds, string chatId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    DocumentReference document = firestoreDb.Collection("chatRooms/" + chatId + "/room").Document(item);
                    await document.DeleteAsync();
                }
            }

            return Json(new { Result = true });
        }
        [HttpPost]
        public virtual async Task<IActionResult> DeleteChatRequest(string id, string chatId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            DocumentReference document = firestoreDb.Collection("chatRooms/" + chatId + "/room").Document(id);
            await document.DeleteAsync();

            return Json(new { Result = true });
        }
        #endregion
    }
}