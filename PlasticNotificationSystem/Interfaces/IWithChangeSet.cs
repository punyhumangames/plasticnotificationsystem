using System;
using System.Collections.Generic;
using System.Text;

namespace PlasticNotificationSystem
{
    public interface IWithChangeSet : IWithRepository
    {
        public string ChangeId
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
