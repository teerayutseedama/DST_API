using static System.Runtime.InteropServices.JavaScript.JSType;
using Twilio.TwiML.Messaging;

namespace Disaster_Prediction_Alert_System_API.DataView
{
    public class IHttpResponse
    {
        public int Status { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; }
        public IHttpResponse() {
            SetAction(MessageType.Save);
        }
        public void SetAction(MessageType type)
        {
            if (type == MessageType.Save)
            {
                Status = 200;
                Message = "Save Data Success";
                Error = null;
            }else if (type==MessageType.Fail)
            {
                Status = 400;
                Message = "Save Data Fail";
                Error = null;
            }
           
            else
            {
                Status = 200;
                Error = null;
            }
        }
    }
    public enum MessageType
    {
        Save = 1,
        Fail=2
    }
}
