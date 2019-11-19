using System;
using System.Collections.Generic;
using System.Text;

namespace PlasticNotificationSystem.TriggerEvents
{
    public class MakeRepositoryTrigger : ITriggerEvent, IWithRepository
    {
        public string Title
        {
            get;
            protected set;
        }
        public string Body
        {
            get;
            protected set;
        }
        public string Server
        {
            get;
            private set;
        }
        public string ClientMachine
        {
            get;
            private set;
        }
        public string Author
        {
            get;
            private set;
        }

        public string Repository
        {
            get;
            private set;
        }

        public virtual void Parse(string data)
        {
            Author = Environment.GetEnvironmentVariable("PLASTIC_USER") ?? "#ERR_NoAuthor";
            Server = Environment.GetEnvironmentVariable("PLASTIC_SERVER") ?? "#ERR_NoServer";
            ClientMachine = Environment.GetEnvironmentVariable("PLASTIC_CLIENTMACHINE") ?? "#ERR_NoServer";
            Repository = Environment.GetEnvironmentVariable("PLASTIC_REPOSITORY_NAME") ?? "#ERR_Repository";

            Title = "Repository Created";
            Body = Repository;
                                   
        }
    }
}
