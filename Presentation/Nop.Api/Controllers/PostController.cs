using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Api.Factories;
using Nop.Api.Infrastructure.Api;
using Nop.Api.Models;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Skle;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Skle;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nop.Api.Controllers
{
    [Route("api/Post/{action}")]
    public class PostController : BaseAuthenController
    {
        #region const

        private const int TIME_TOKEN = -5;

        #endregion const

        #region Feilds

        private readonly Random _random = new Random();

        private readonly ICustomerService _customerService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly CustomerSettings _customerSettings;
        private readonly IStoreContext _storeContext;
        private readonly ApiSettings _settingContext;
        private readonly ICommonFactory _commonFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IMemberService _memberService;
        private readonly IGroupService _groupService;
        private readonly IPictureService _pictureService;
        private readonly IPostService _postService;
        private readonly INopFileProvider _fileProvider;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDownloadService _downloadService;
        private readonly IOtpService _otpService;

        #endregion Feilds

        #region Ctor

        public PostController(ICustomerService customerService, ICustomerRegistrationService customerRegistrationService, CustomerSettings customerSettings, IStoreContext storeContext, ApiSettings settingContext, ICommonFactory commonFactory, ILocalizationService localizationService, IGenericAttributeService genericAttributeService, IMemberService memberService, IGroupService groupService, IPictureService pictureService, IPostService postService, INopFileProvider fileProvider, INotificationService notificationService, IHttpContextAccessor httpContextAccessor, IDownloadService downloadService, IOtpService otpService)
        {
            _customerService = customerService;
            _customerRegistrationService = customerRegistrationService;
            _customerSettings = customerSettings;
            _storeContext = storeContext;
            _settingContext = settingContext;
            _commonFactory = commonFactory;
            _localizationService = localizationService;
            _genericAttributeService = genericAttributeService;
            _memberService = memberService;
            _groupService = groupService;
            _pictureService = pictureService;
            _postService = postService;
            _fileProvider = fileProvider;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _downloadService = downloadService;
            _otpService = otpService;
        }

        #endregion Ctor

        #region Common

        [HttpPost]
        public virtual IActionResult DeleteFileByPath(FileUrlModel model)
        {
            var path = _fileProvider.GetAbsolutePath(model.Fileurl);
            System.IO.File.Delete(path);
            return Ok(true);
        }

        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        private MessageReturn SendOTP(string phone, string token)
        {
            var Otp = _otpService.GetOtpByPhoneNumber(phone);
            if (Otp != null && Otp.ResponseSuccess && Otp.CreatedAt > DateTime.Now.AddSeconds(-60))
                return MessageReturn.Error(_localizationService.GetResource("SendOTP.time"));

            int OptCode = RandomNumber(100000, 999999);
            string message = "SKLE gửi mã xác thực OTP: " + OptCode + ". Mã có hiệu lực "+ Math.Abs(TIME_TOKEN) + " phút.";
            var res = _otpService.SendSMS(phone, message);

            dynamic err = JsonConvert.DeserializeObject(res);
            try
            {
                var errcode = ((JValue)err["error"])?.Value?.ToString();
                _otpService.Insret(new Otp
                {
                    CreatedAt = DateTime.Now,
                    OptCode = OptCode + "",
                    PhoneNumber = phone,
                    Response = res,
                    Token = token,
                    ResponseSuccess = string.IsNullOrEmpty(errcode)
                });
                if (!string.IsNullOrEmpty(errcode))
                {
                    switch (errcode)
                    {
                        case "1001":
                            return MessageReturn.Error("Request không hợp lệ.");

                        case "1002":
                            return MessageReturn.Error("Client không được cấp phép truy cập.");

                        case "1003":
                            return MessageReturn.Error("Truy cập bị từ chối.");

                        case "1004":
                            return MessageReturn.Error("Loại response yêu cầu không được hỗ trợ.");

                        case "1005":
                            return MessageReturn.Error("Lỗi server.");

                        case "1006":
                            return MessageReturn.Error("Server tạm thời không thể xử lý request từ client.");

                        case "1007":
                            return MessageReturn.Error("Thông tin client không đúng.");

                        case "1008":
                            return MessageReturn.Error("Loại hình cấp quyền không hợp lệ.");

                        case "1009":
                            return MessageReturn.Error("Các scope không hợp lệ");

                        case "1010":
                            return MessageReturn.Error("Scope không đủ để truy cập API");

                        case "1011":
                            return MessageReturn.Error("Access token không hợp lệ");

                        case "1012":
                            return MessageReturn.Error("Access token đã bị thay đổi");

                        case "1013":
                            return MessageReturn.Error("Access token hết hạn");

                        case "1014":
                            return MessageReturn.Error("Các tham số truyền vào bị lỗi");

                        default:
                            return MessageReturn.Error("Có gì đó sai sai");
                    }
                }
            }
            catch
            {
                return MessageReturn.Error("Có gì đó sai sai");
            }
            

            return MessageReturn.Success(_localizationService.GetResource("Account.PasswordRecovery.SmsTokenHasBeenSent"), token);
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual IActionResult testsms(SendSMS model)
        {
            var res = _otpService.SendSMS(model.Phone, model.Message);
            return Ok(res);
        }

        #endregion Common

        #region login / Register / Recovery / Change Password

        [HttpPost]
        public virtual async System.Threading.Tasks.Task<IActionResult> LoginAsync(LoginModel model)
        {
            var loginResult = _customerRegistrationService.ValidateCustomerByPhoneNumber(model.PhoneNumber, model.Password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var customer = _customerService.GetCustomerByPhoneNumber(model.PhoneNumber);
                        var member = _memberService.GetMemberByCustomerId(customer.Id);
                        if (member == null || member.Deleted)
                        {
                            return Ok(MessageReturn.Error(_localizationService.GetResource("Account.Login.WrongCredentials.Member.Deleted")));
                        }
                        var members = _memberService.GetAllMember(fireBaseId: model.FirebaseId);
                        foreach (var item in members)
                        {
                            item.FirebaseId = null;
                            _memberService.Update(item, "Remove FirebaseId");
                        }
                        member.FirebaseId = model.FirebaseId;
                        _memberService.Update(member, "Login, Insert FirebaseId");
                        var docRef = firestoreDb.Collection("users").Document(member.Id + "");
                        Dictionary<string, object> update = new Dictionary<string, object>
                        {
                            { "userName", member.Name },
                        };
                        await docRef.SetAsync(update, SetOptions.MergeAll);
                        return Ok(MessageReturn.Success("Ok", new
                        {
                            MemberId = member.Id,
                            Token = _commonFactory.GenerateJWTToken(member.Id, DateTime.Now.AddMonths(4)),
                            Name = member.Name
                        }));
                    }
                case CustomerLoginResults.CustomerNotExist:
                    return Ok(MessageReturn.Error(_localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist")));

                case CustomerLoginResults.Deleted:
                    return Ok(MessageReturn.Error(_localizationService.GetResource("Account.Login.WrongCredentials.Deleted")));

                case CustomerLoginResults.NotActive:
                    return Ok(MessageReturn.Error(_localizationService.GetResource("Account.Login.WrongCredentials.NotActive")));

                case CustomerLoginResults.NotRegistered:
                    return Ok(MessageReturn.Error(_localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered")));

                case CustomerLoginResults.LockedOut:
                    return Ok(MessageReturn.Error(_localizationService.GetResource("Account.Login.WrongCredentials.LockedOut")));

                case CustomerLoginResults.WrongPassword:
                default:
                    return Ok(MessageReturn.Error(_localizationService.GetResource("Account.Login.WrongCredentials")));
            }
        }

        [HttpPost]
        public virtual async System.Threading.Tasks.Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber))
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.Login.Required.Phone")));
            if (string.IsNullOrEmpty(model.Password))
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.Login.Required.Password")));
            model.PhoneNumber = model.PhoneNumber.Trim().Replace("+84", "0");
            if (_customerService.CheckPhoneNumber(model.PhoneNumber))
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.Login.Exist.PhoneNumber")));
            var otp = _otpService.GetOtpByPhoneNumber(model.PhoneNumber);
            if(otp == null || !otp.Success)
                return Ok(MessageReturn.Error(_localizationService.GetResource("PhoneNumber.success.fail")));
            var customer = _customerService.InsertGuestCustomer();
            string email = "phone" + model.PhoneNumber + "@phone.skle.vn";
            var registrationRequest = new CustomerRegistrationRequest(customer,
                    email,
                    model.PhoneNumber,
                    model.PhoneNumber,
                    model.Password,
                    _customerSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    true);
            var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
            if (registrationResult.Success)
            {
                var members = _memberService.GetAllMember(fireBaseId: model.FirebaseId);
                foreach (var item in members)
                {
                    item.FirebaseId = null;
                    _memberService.Update(item, "Remove FirebaseId");
                }
                var member = new Member()
                {
                    Name = model.Name,
                    CustomerId = customer.Id,
                    CreatedAt = DateTime.Now,
                    StatusId = (int)ENStatusMember.Active,
                    FirebaseId = model.FirebaseId
                };
                _memberService.Insert(member, "Tạo mới tài khoản");
                var docRef = firestoreDb.Collection("users").Document(member.Id + "");
                Dictionary<string, object> update = new Dictionary<string, object>
                        {
                            { "userName", member.Name },
                        };
                await docRef.SetAsync(update, SetOptions.MergeAll);
                return Ok(MessageReturn.Success("Ok", new
                {
                    MemberId = member.Id,
                    Token = _commonFactory.GenerateJWTToken(member.Id, DateTime.Now.AddMonths(4)),
                }));
            }
            return Ok(MessageReturn.Error(String.Join(",", registrationResult.Errors)));
        }

        [HttpPost]
        public virtual IActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            var customer = _customerService.GetCustomerByPhoneNumber(model.PhoneNumber);
            if (customer != null && customer.Active && !customer.Deleted)
            {
                //send sms
                string token = _commonFactory.GenerateToken(customer.CustomerGuid.ToString());
                return Ok(SendOTP(model.PhoneNumber, token));
            }
            else
            {
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.PasswordRecovery.PhoneNumberNotFound")));
            }
        }

        [HttpPost]
        public virtual IActionResult PasswordRecoveryConfirm(PasswordRecoveryModel model)
        {
            var customer = _commonFactory.DecodeToken(model.Token);
            if (customer == null)
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.PasswordRecovery.TokenFail")));
            var otp = _otpService.GetOtpByPhoneNumber(customer.PhoneNumber);
            if (otp == null || !model.Otp.Equals(otp.OptCode) || !model.Token.Equals(otp.Token))
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.PasswordRecovery.OptFail")));
            if (otp.CreatedAt < DateTime.Now.AddMinutes(TIME_TOKEN))
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.PasswordRecovery.OptExpired")));
            otp.Success = true;
            _otpService.Update(otp);

            return Ok(MessageReturn.Success("Ok"));
        }

        [HttpPost]
        public virtual IActionResult RenewPassword(PasswordRecoveryModel model)
        {
            var customer = _commonFactory.DecodeToken(model.Token);
            if (customer == null)
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.PasswordRecovery.TokenFail")));
            var otp = _otpService.GetOtpByToken(model.Token);

            if (otp == null || !otp.Success)
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.PasswordRecovery.OptFail")));

            var response = _customerRegistrationService.ChangePassword(new ChangePasswordRequest(customer.Email,
                false, _customerSettings.DefaultPasswordFormat, model.NewPassword));

            if (response.Success)
            {
                return Ok(MessageReturn.Success("Ok"));
            }
            else
            {
                return Ok(MessageReturn.Error(response.Errors.FirstOrDefault()));
            }
        }

        [HttpPost]
        public virtual IActionResult ChangePassword(PasswordRecoveryModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            if (string.IsNullOrEmpty(model.OldPassword))
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.ChangePassword.Required.Password")));

            var customer = _customerService.GetCustomerById(_memberService.GetMemberById(currentMemberId).CustomerId);

            var response = _customerRegistrationService.ChangePassword(new ChangePasswordRequest(customer.Email,
                true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword));

            if (response.Success)
            {
                return Ok(MessageReturn.Success("Ok"));
            }
            else
            {
                return Ok(MessageReturn.Error(response.Errors.FirstOrDefault()));
            }
        }

        [HttpPost]
        public virtual IActionResult CheckOldPassword(PasswordRecoveryModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var customer = _customerService.GetCustomerById(_memberService.GetMemberById(currentMemberId).CustomerId);
            if (_customerRegistrationService.CheckOldPassword(customer.Id, model.OldPassword))
                return Ok(MessageReturn.Success("Ok"));
            return Ok(MessageReturn.Error(_localizationService.GetResource("Account.ChangePassword.Errors.OldPasswordDoesntMatch")));
        }

        #endregion login / Register / Recovery / Change Password

        #region Send Request / Cancel Request / Confirm Request / Delete Request / Delete Friend

        [HttpPost]
        public virtual IActionResult SendFriendRequest(FriendRequestModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var qr = _memberService.GetFriendRequestByFromIdAndToId(currentMemberId, model.ToId);
            var qr2 = _memberService.GetFriendRequestByFromIdAndToId(model.ToId, currentMemberId);
            if (qr2 != null)
                _memberService.DeleteFriendRequest(qr2.Id);
            if (qr == null)
            {
                _memberService.Insert(new FriendRequest()
                {
                    FromId = currentMemberId,
                    ToId = model.ToId
                });

                var mem1 = _memberService.GetMemberById(currentMemberId);
                _notificationService.Insert(new NotiMobi()
                {
                    BranchId = (int)ENTypeNotification.member,
                    Content = _localizationService.GetResource("member.SendFriendRequest"),
                    currentMemberId = currentMemberId,
                    MemberId = model.ToId,
                    //PictureUrl = _pictureService.GetPictureUrl(mem1.AvatarId, defaultPictureType: Core.Domain.Media.PictureType.Avatar),
                    Name = mem1.Name,
                    TypeId = (int)ENTypeNotiMobi.loimoi_ketban
                });
                return Ok(MessageReturn.Success("Ok"));
            }
            return BadRequest();
        }

        [HttpPost]
        public virtual IActionResult CancelFriendRequest(FriendRequestModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var qr = _memberService.GetFriendRequestByFromIdAndToId(currentMemberId, model.ToId);
            if (qr == null || qr.Deleted)
                return BadRequest();

            _memberService.DeleteFriendRequest(qr.Id);

            return Ok(MessageReturn.Success("Ok"));
        }

        [HttpPost]
        public virtual IActionResult ConfirmFriendRequest(FriendRequestModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var qr = _memberService.GetFriendRequestByFromIdAndToId(model.ToId, currentMemberId);
            if (qr.Deleted)
                return BadRequest();

            _memberService.DeleteFriendRequest(qr.Id);
            _memberService.Insert(new Friend()
            {
                FromId = model.ToId,
                ToId = currentMemberId
            });

            var mem1 = _memberService.GetMemberById(currentMemberId);
            var mem2 = _memberService.GetMemberById(model.ToId);

            _notificationService.Insert(new NotiMobi()
            {
                BranchId = (int)ENTypeNotification.member,
                Content = _localizationService.GetResource("member.ConfirmFriendRequest"),
                currentMemberId = currentMemberId,
                MemberId = model.ToId,
                //PictureUrl = _pictureService.GetPictureUrl(mem1.AvatarId, defaultPictureType: Core.Domain.Media.PictureType.Avatar),
                Name = mem1.Name,
                TypeId = (int)ENTypeNotiMobi.xacnhan_ketban
            });

            mem1.CountFriend++;
            _memberService.Update(mem1);

            _memberService.Insert(new MemberLog()
            {
                MemberId = mem1.Id,
                Content = "Đã kết bạn với Id: " + mem2.Id,
                After = JsonConvert.SerializeObject(mem1),
                CreatedAt = DateTime.Now,
                StatusId = (int)ENStatusLog.Insert
            });

            mem2.CountFriend++;
            _memberService.Update(mem2);

            _memberService.Insert(new MemberLog()
            {
                MemberId = mem2.Id,
                Content = "Đã được Id: " + mem1.Id + " xác nhận kết bạn",
                After = JsonConvert.SerializeObject(mem2),
                CreatedAt = DateTime.Now,
                StatusId = (int)ENStatusLog.Insert
            });
            return Ok(MessageReturn.Success("Ok"));
        }

        [HttpPost]
        public virtual IActionResult DeleteFriendRequest(FriendRequestModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var qr = _memberService.GetFriendRequestByFromIdAndToId(model.ToId, currentMemberId);

            qr.Deleted = true;

            _memberService.Update(qr);

            return Ok(MessageReturn.Success("Ok"));
        }

        [HttpPost]
        public virtual IActionResult DeleteFriend(FriendRequestModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var fr = _memberService.GetFriendByFromIdAndToId(currentMemberId, model.ToId);
            if (fr == null)
                return BadRequest();

            _memberService.DeleteFriend(fr.Id);

            var mem1 = _memberService.GetMemberById(currentMemberId);
            var mem2 = _memberService.GetMemberById(model.ToId);

            mem1.CountFriend--;
            _memberService.Update(mem1);

            _memberService.Insert(new MemberLog()
            {
                MemberId = mem1.Id,
                Content = "Đã huỷ kết bạn với Id: " + mem2.Id,
                After = JsonConvert.SerializeObject(mem1),
                CreatedAt = DateTime.Now,
                StatusId = (int)ENStatusLog.Insert
            });

            mem2.CountFriend--;
            _memberService.Update(mem2);

            _memberService.Insert(new MemberLog()
            {
                MemberId = mem2.Id,
                Content = "Đã bị Id: " + mem1.Id + " huỷ kết bạn",
                After = JsonConvert.SerializeObject(mem2),
                CreatedAt = DateTime.Now,
                StatusId = (int)ENStatusLog.Insert
            });

            return Ok(MessageReturn.Success("Ok"));
        }

        #endregion Send Request / Cancel Request / Confirm Request / Delete Request / Delete Friend

        #region Add Back List / Delete Back List / Check BackList / add Report

        private FirestoreDb firestoreDb => FirestoreDb.Create("skle-6e296", new FirestoreClientBuilder { JsonCredentials = System.IO.File.ReadAllText(_fileProvider.MapPath("firebase-adminsdk.json")) }.Build());

        [HttpPost]
        public virtual async System.Threading.Tasks.Task<IActionResult> AddBackListAsync(FriendRequestModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            _memberService.Insert(new MemberBackList()
            {
                FromId = currentMemberId,
                ToId = model.ToId,
                CreatedAt = DateTime.Now
            });
            bool bl = currentMemberId < model.ToId;
            string doc = string.Empty;
            bool isBlockFirst;
            bool isBlockLast;
            if (bl)
            {
                doc = currentMemberId + "," + model.ToId;
                isBlockFirst = true;
                isBlockLast = _memberService.CheckMemberBackListFrom(model.ToId, currentMemberId);
            }
            else
            {
                doc = model.ToId + "," + currentMemberId;
                isBlockLast = true;
                isBlockFirst = _memberService.CheckMemberBackListFrom(currentMemberId, model.ToId);
            }
            var docRef = firestoreDb.Collection("chatRooms").Document(doc);
            Dictionary<string, object> update = new Dictionary<string, object>
            {
                { "isBlockFirst", isBlockFirst },
                { "isBlockLast", isBlockLast },
            };
            await docRef.SetAsync(update, SetOptions.MergeAll);
            return Ok(MessageReturn.Success("Ok"));
        }

        [HttpPost]
        public virtual async System.Threading.Tasks.Task<IActionResult> DeleteBackListAsync(FriendRequestModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var mbl = _memberService.GetMemberBackListByFromIdAndToId(currentMemberId, model.ToId);
            if (mbl == null)
                return BadRequest();
            _memberService.DeleteMemberBackList(mbl.Id);

            bool bl = currentMemberId < model.ToId;
            string doc = string.Empty;
            bool isBlockFirst = false;
            bool isBlockLast = false;
            if (bl)
            {
                doc = currentMemberId + "," + model.ToId;
                isBlockFirst = false;
                isBlockLast = _memberService.CheckMemberBackListFrom(model.ToId, currentMemberId);
            }
            else
            {
                doc = model.ToId + "," + currentMemberId;
                isBlockLast = false;
                isBlockFirst = _memberService.CheckMemberBackListFrom(currentMemberId, model.ToId);
            }
            var docRef = firestoreDb.Collection("chatRooms").Document(doc);
            Dictionary<string, object> update = new Dictionary<string, object>
            {
                { "isBlockFirst", isBlockFirst },
                { "isBlockLast", isBlockLast },
            };
            await docRef.SetAsync(update, SetOptions.MergeAll);
            return Ok(MessageReturn.Success("Ok"));
        }

        [HttpPost]
        public virtual IActionResult AddReport(ReportModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            _postService.Insert(new Report
            {
                Content = model.Content,
                FormId = currentMemberId,
                isNew = true,
                TargetId = model.TargetId,
                Title = model.Title,
                CreatedAt = DateTime.Now
            });

            return Ok(MessageReturn.Success("Ok"));
        }

        #endregion Add Back List / Delete Back List / Check BackList

        #region Group

        [HttpPost]
        public virtual IActionResult JojnGroupRequest(GroupRequestModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();
            var group = _groupService.GetGroupById(model.GroupId);
            int stt = group.isApproval ? (int)ENStatusGroupRequest.wait_confirm : (int)ENStatusGroupRequest.confirm;
            if (group == null)
                return BadRequest();
            var groupMemberRequest = _groupService.GetGroupMemberByGroupIdAndMemberId(currentMemberId, model.GroupId);
            if (groupMemberRequest == null)
            {
                _groupService.Insert(new GroupMember()
                {
                    GroupId = model.GroupId,
                    MemberId = currentMemberId,
                    StatusId = stt,
                    CreatedAt = DateTime.Now
                });
                if (!group.isApproval)
                {
                    group.CountMember++;
                    _groupService.Update(group, "Tài khoản id: " + currentMemberId + " đã tham gia nhóm");
                    var member = _memberService.GetMemberById(currentMemberId);
                    member.CountGroup++;
                    _memberService.Update(member);
                    _groupService.UpdateCountNewPostGroup(currentMemberId, group.Id);
                }

                return Ok(MessageReturn.Success("Ok", stt));
            }
            else
            {
                if (groupMemberRequest.CreatedAt < DateTime.Now.AddMonths(-1))
                {
                    groupMemberRequest.StatusId = stt;
                    groupMemberRequest.CreatedAt = DateTime.Now;
                    _groupService.Update(groupMemberRequest);

                    return Ok(MessageReturn.Success("Ok", stt));
                }
                return Ok(MessageReturn.Error(_localizationService.GetResource("Group.JojnGroup.Wait")));
            }
        }

        [HttpPost]
        public virtual IActionResult InviteFriendJojnGroup(GroupRequestModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();
            var group = _groupService.GetGroupById(model.GroupId);
            int stt = group.isApproval ? (int)ENStatusGroupRequest.wait_confirm : (int)ENStatusGroupRequest.confirm;
            if (group == null)
                return BadRequest();
            var groupMemberRequest = _groupService.GetGroupMemberByGroupIdAndMemberId(currentMemberId, model.GroupId);
            if (groupMemberRequest != null || groupMemberRequest.StatusId != (int)ENStatusGroupRequest.confirm)
            {
                _groupService.Insert(new GroupMember()
                {
                    GroupId = model.GroupId,
                    MemberId = model.MemberId,
                    StatusId = (int)ENStatusGroupRequest.confirm,
                    CreatedAt = DateTime.Now
                });

                group.CountMember++;
                _groupService.Update(group, "Tài khoản #" + currentMemberId + " đã mời #" + model.MemberId + " vào nhóm");
                var member = _memberService.GetMemberById(currentMemberId);
                member.CountGroup++;
                _memberService.Update(member);
                _groupService.UpdateCountNewPostGroup(currentMemberId, group.Id);

                _notificationService.Insert(new NotiMobi()
                {
                    BranchId = (int)ENTypeNotification.member,
                    Content = _localizationService.GetResource("invite.group") + " " + group.Name,
                    GroupId = model.GroupId,
                    currentMemberId = currentMemberId,
                    MemberId = model.MemberId,
                    //PictureUrl = _pictureService.GetPictureUrl(group.AvatarId, defaultPictureType: Core.Domain.Media.PictureType.Avatar),
                    Name = member.Name,

                    TypeId = (int)ENTypeNotiMobi.invite_group
                });
                return Ok(MessageReturn.Success("Ok"));
            }

            return BadRequest();
        }

        [HttpPost]
        public virtual IActionResult LeaveGroup(GroupRequestModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var group = _groupService.GetGroupById(model.GroupId);
            if (group == null)
                return BadRequest();

            var groupMemberRequest = _groupService.GetGroupMemberByGroupIdAndMemberId(currentMemberId, model.GroupId);
            if (groupMemberRequest == null)
                return BadRequest();
            switch (groupMemberRequest.StatusId)
            {
                case (int)ENStatusGroupRequest.confirm:
                    _groupService.DeleteGroupMember(groupMemberRequest.Id);
                    group.CountMember--;
                    _groupService.Update(group, "Thành viên có Id:" + currentMemberId + " đã rời khỏi nhóm");

                    var member = _memberService.GetMemberById(currentMemberId);
                    member.CountGroup--;
                    _memberService.Update(member, "Rời khỏi nhóm: id = " + group.Id);

                    return Ok(MessageReturn.Success("Ok"));

                case (int)ENStatusGroupRequest.wait_confirm:
                    _groupService.DeleteGroupMember(groupMemberRequest.Id);
                    return Ok(MessageReturn.Success("Ok"));

                default:
                    return BadRequest();
            }
        }

        #endregion Group

        #region Update Member

        [HttpPost]
        public virtual async System.Threading.Tasks.Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var member = _memberService.GetMemberById(currentMemberId);
            var customer = _customerService.GetCustomerById(member.CustomerId);

            if (!member.Name.Equals(model.Name.Trim()))
            {
                var docRef = firestoreDb.Collection("users").Document(member.Id + "");
                Dictionary<string, object> update = new Dictionary<string, object>
                {
                    { "userName", model.Name.Trim() },
                };
                await docRef.SetAsync(update, SetOptions.MergeAll);
            }
            member.Name = model.Name.Trim();

            member.Email = model.Email?.Trim();

            if (!CommonHelper.IsValidEmail(model.Email?.Trim()))
            {
                customer.Email = model.Email?.Trim();
                _customerService.UpdateCustomer(customer);
            }

            if (!string.IsNullOrEmpty(model.Birthday))
            {
                try
                {
                    DateTime dt = DateTime.ParseExact(model.Birthday, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    member.Birthday = dt;
                }
                catch
                {
                    return Ok(MessageReturn.Error("Định dạng ngày không hợp lệ"));
                }
            }

            member.GenderId = model.GenderId;
            member.ProvinceId = model.ProvinceId;
            member.PhoneSet = model.PhoneSet;

            _memberService.Update(member);

            _memberService.Insert(new MemberLog()
            {
                MemberId = member.Id,
                Content = "Cập nhật thông tin tài khoản",
                After = JsonConvert.SerializeObject(member),
                StatusId = (int)ENStatusLog.Update,
                CreatedAt = DateTime.Now
            });
            return Ok(MessageReturn.Success("Ok"));
        }

        [HttpPost]
        public virtual IActionResult UpdatePicture([FromBody] UpdatePictureModel model)
        {
            var picture = _pictureService.InsertPicture(model.PictureBinary, model.MimeType, Guid.NewGuid().ToString(), validateBinary: true, isNew: false);
            if (picture == null)
                return Ok(MessageReturn.Error("Lỗi"));

            var member = _memberService.GetMemberById(currentMemberId);
            if (model.isUpAvartar)
            {
                member.AvatarId = picture.Id;
            }
            else
            {
                member.CoveId = picture.Id;
            }
            _memberService.Update(member);
            return Ok(MessageReturn.Success("Ok"));
        }

        #endregion Update Member

        #region Update Field

        [HttpPost]
        public virtual IActionResult UpdateMemberField([FromBody] UpdateFieldModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var fs = _memberService.GetAllMemberField(currentMemberId).ToList();

            foreach (var item in model.List)
            {
                if (fs.Count(aaaaaa => aaaaaa.FieldId == item) == 0)
                {
                    _memberService.Insert(new MemberField()
                    {
                        MemberId = currentMemberId,
                        FieldId = item
                    });
                }
            }
            foreach (var item in fs)
            {
                if (!model.List.Contains(item.FieldId))
                {
                    _memberService.Delete(item);
                }
            }
            return Ok(MessageReturn.Success());
        }

        #endregion Update Field

        #region Post

        [HttpPost]
        public virtual IActionResult UpdateLikePost([FromBody] LikePostModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var post = _postService.GetPostById(model.PostId);

            var postlike = _postService.GetAllPostLike(model.PostId).FirstOrDefault(s => s.MemberId == currentMemberId);
            if (postlike == null)
            {
                if (_postService.Insert(new PostLike()
                {
                    MemberId = currentMemberId,
                    PostId = post.Id,
                    CreatedAt = DateTime.Now
                }))
                {
                    post.CountLike++;
                    _postService.Update(post);
                    var mem1 = _memberService.GetMemberById(currentMemberId);
                    if (post.MemberId != currentMemberId)
                        _notificationService.Insert(new NotiMobi()
                        {
                            BranchId = (int)ENTypeNotification.member,
                            Content = _localizationService.GetResource("member.like.post"),
                            currentMemberId = currentMemberId,
                            PostId = post.Id,
                            MemberId = post.MemberId,
                            //PictureUrl = _pictureService.GetPictureUrl(mem1.AvatarId, defaultPictureType: Core.Domain.Media.PictureType.Avatar),
                            Name = mem1.Name,
                            TypeId = (int)ENTypeNotiMobi.like
                        });
                }
            }
            else if (!postlike.isLike)
            {
                postlike.isLike = true;
                _postService.Update(postlike);

                post.CountLike++;
                _postService.Update(post);
            }
            else
            {
                if (_postService.DeletePostLike(postlike.Id))
                {
                    post.CountLike--;
                    _postService.Update(post);
                }
            }
            return Ok(MessageReturn.Success());
        }

        [HttpPost]
        public virtual IActionResult UpdateSpamPost([FromBody] LikePostModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var post = _postService.GetPostById(model.PostId);

            if (post.MemberId == currentMemberId)
                return BadRequest();

            var postspam = _postService.GetAllPostSpam(model.PostId).FirstOrDefault(s => s.MemberId == currentMemberId);
            if (postspam == null)
            {
                if (_postService.Insert(new PostSpam()
                {
                    MemberId = currentMemberId,
                    PostId = post.Id,
                    CreatedAt = DateTime.Now
                }))
                {
                    post.CountSpam++;
                    _postService.Update(post);
                }
            }
            else
            {
                if (_postService.DeletePostSpam(postspam.Id))
                {
                    post.CountSpam--;
                    _postService.Update(post);
                }
            }

            return Ok(MessageReturn.Success());
        }

        [HttpPost]
        public virtual IActionResult UpdateHiddenPost([FromBody] LikePostModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();
            var hp = _postService.GetPostHidden(currentMemberId, model.ToId, model.PostId);
            if (hp == null)
            {
                _postService.Insert(new PostHidden() { FormId = currentMemberId, TargetId = model.PostId, ToId = model.ToId });
            }
            else
            {
                _postService.Delete(hp);
            }

            return Ok(MessageReturn.Success());
        }

        #endregion Post

        #region message

        [HttpPost]
        public virtual IActionResult InsertMessageToGroup([FromBody] MessageToGroupModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            if (model.GroupId == 0 || string.IsNullOrEmpty(model.Content))
                return BadRequest();

            var entity = new MessageToGroup()
            {
                MemberId = currentMemberId,
                CreatedAt = DateTime.Now,
                GroupId = model.GroupId,
                Content = model.Content
            };

            _notificationService.Insert(entity);
            var currentMember = _memberService.GetMemberById(currentMemberId);
            _notificationService.Insert(new NotiMobi()
            {
                BranchId = (int)ENTypeNotification.group,
                Content = model.Content,
                GroupId = model.GroupId,
                currentMemberId = currentMemberId,
                //PictureUrl = _pictureService.GetPictureUrl(currentMember.AvatarId, defaultPictureType: Core.Domain.Media.PictureType.Avatar),
                Name = currentMember.Name,
                MessageId = entity.Id,
                TypeId = (int)ENTypeNotiMobi.tin_nhan
            });
            var members = _groupService.GetAllGroupMember(GroupId: entity.GroupId).Where(a => a.MemberId != currentMemberId).Select(s => s.MemberId);

            foreach (var item in members)
            {
                if (!_memberService.CheckMemberBackList(currentMemberId, item))
                    _notificationService
                        .Insert(new MemberMessage() { MemberId = item, MessageId = entity.Id });
            }

            return Ok(MessageReturn.Success());
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [Consumes("multipart/form-data")]
        public virtual IActionResult InsertPicture([FromForm] AddPostModel model)
        {
            var list = new List<string>();

            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();
            try
            {
                if (model.Files == null)
                    model.Files = _httpContextAccessor.HttpContext.Request.Form?.Files;
            }
            catch
            {
            }

            foreach (var item in model.Files)
            {
                switch (item.ContentType.Split("/").First())
                {
                    case "image":
                        var picture = _pictureService.InsertPicture(item);
                        list.Add(_pictureService.GetPictureUrl(picture.Id));
                        break;
                }
            }

            return Ok(MessageReturn.Success("Ok", list));
        }

        #endregion message

        #region filebase

        [HttpPost]
        public virtual IActionResult FirebaseId(AddFilebabseIdModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var member = _memberService.GetMemberById(currentMemberId);
            member.FirebaseId = model.FirebaseId;
            _memberService.Update(member, "Cập nhật FilebaseId");
            return Ok(MessageReturn.Success());
        }

        #endregion filebase

        #region OTP

        [HttpPost]
        public virtual IActionResult SendOtp(SendSMS model)
        {
            return Ok(SendOTP(model.Phone, _commonFactory.GenerateToken(model.Phone)));
        }

        [HttpPost]
        public virtual IActionResult ConfirmOtp(SendSMS model)
        {
            var otp = _otpService.GetOtpByToken(model.Token);
            if (otp == null || !model.Otp.Equals(otp.OptCode))
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.PasswordRecovery.OptFail")));
            if (otp.CreatedAt < DateTime.Now.AddMinutes(TIME_TOKEN))
                return Ok(MessageReturn.Error(_localizationService.GetResource("Account.PasswordRecovery.OptExpired")));
            otp.Success = true;
            _otpService.Update(otp);

            return Ok(MessageReturn.Success("Ok", true));
        }

        #endregion OTP
    }
}