using System;
using System.Collections.Generic;
using System.Text;

namespace PlasticNotificationSystem
{
    interface INotifier
    {
        public void NotifyTrigger(ITriggerEvent Event);
        public void ParseJsonString(string JsonString);
    }
}
