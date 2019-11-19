using System;
using System.Collections.Generic;
using System.Text;

namespace PlasticNotificationSystem
{
    public interface IWithDetails
    {
        public IEnumerable<object> Details
        {
            get;
        }
    }
}
