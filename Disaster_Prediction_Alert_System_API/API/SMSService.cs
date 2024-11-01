using Twilio.Types;
using Twilio;
using System;
using System.Collections.Generic;
using Twilio.Rest.Api.V2010.Account;

namespace Disaster_Prediction_Alert_System_API.API
{
    public class SMSService
    {
        string PhoneNumber = "";
       private  string _accountSid = "";
       private string _authToken = "";
        public SMSService(string sendToPhoneNumber,string accountId,string token)
        {
            PhoneNumber = sendToPhoneNumber;
            _accountSid = accountId;
            _authToken = token;
        }
        public bool SendMessage(string TextMessage)
        {
           
           
            TwilioClient.Init(_accountSid, _authToken);
            var messageOptions = new CreateMessageOptions(
              new PhoneNumber(PhoneNumber));
            messageOptions.From = new PhoneNumber("+14437323122");
            messageOptions.Body = TextMessage;
            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);
            if (message.Status == MessageResource.StatusEnum.Queued)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

