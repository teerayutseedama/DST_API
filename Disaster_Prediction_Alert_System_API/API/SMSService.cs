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
        public SMSService(string sendToPhoneNumber)
        {
            PhoneNumber = sendToPhoneNumber;
        }
        public bool SendMessage(string TextMessage)
        {
           
            var accountSid = "AC3e20387a680f58140e3b962be546b98c";
            var authToken = "bd6728ab7331d73d889da40531ff6a3c";
            TwilioClient.Init(accountSid, authToken);
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

