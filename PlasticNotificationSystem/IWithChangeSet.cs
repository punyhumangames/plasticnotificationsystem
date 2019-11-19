using System;
using System.Collections.Generic;
using System.Text;

namespace PlasticNotificationSystem
{
    public interface IWithChangeSet
    {
        public string ChangeId
        {
            get;
        }
        public string Repository
        {
            get;
        }
        public string Branch
        {
            get;
        }
        public string Server
        {
            get;
        }
    }
}
