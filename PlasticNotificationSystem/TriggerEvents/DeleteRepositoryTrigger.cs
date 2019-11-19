using System;
using System.Collections.Generic;
using System.Text;

namespace PlasticNotificationSystem.TriggerEvents
{
    public class DeleteRepositoryTrigger : MakeRepositoryTrigger
    {
        public override void Parse(string data)
        {
            base.Parse(data);

            Title = "Repository Deleted";
        }
    }
}
