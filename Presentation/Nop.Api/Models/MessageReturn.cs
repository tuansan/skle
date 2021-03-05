namespace Nop.Api.Models
{
    public class MessageReturn
    {
        public MessageReturn()
        {
        }

        public MessageReturn(bool _isSuccess, string _msg)
        {
            isSuccess = _isSuccess;
            message = _msg;
        }

        public string message { get; set; }
        public bool isSuccess { get; set; }
        public dynamic objectInfo { get; set; }

        public static MessageReturn Success(string _msg = "Ok", dynamic _objectInfo = null)
        {
            var msgret = new MessageReturn(true, _msg);
            msgret.objectInfo = _objectInfo;
            return msgret;
        }

        public static MessageReturn Error(string _msg, dynamic _objectInfo = null)
        {
            var msgret = new MessageReturn(false, _msg);
            msgret.objectInfo = _objectInfo;
            return msgret;
        }
    }
}