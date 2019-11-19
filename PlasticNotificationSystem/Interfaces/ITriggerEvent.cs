using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace PlasticNotificationSystem
{
    public interface ITriggerEvent
    {
        public string Title
        {
            get;
        }

        public string Body
        {
            get;
        }

        public string Server
        {
            get;
        }

        public string ClientMachine
        {
            get;
        }
        public string Author
        {
            get;
        }

        public void Parse(string data);
    }
}
