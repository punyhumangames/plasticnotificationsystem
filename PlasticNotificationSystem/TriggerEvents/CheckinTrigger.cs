using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PlasticNotificationSystem.TriggerEvents
{

    class ItemData : IWithFileChange
    {
        public FileOperation FileOperation
        {
            get;
            private set;
        }

        public FileType FileType
        {
            get;
            private set;
        }
        public string FileName
        {
            get;
            private set;
        }
        public string ChangeId
        {
            get;
            private set;
        }
        public string Repository
        {
            get;
            private set;
        }
        public string Branch
        {
            get;
            private set;
        }
        public string Server
        {
            get;
            private set;
        }
        public void Parse(string Line)
        {

            //Parse the example.  
            //Format is 
            /*
                status item_path item_type#br:branch;changeset:cset_id@rep:rep_name@repserver:server
             */
            //Examples:
            /*
                CH "/" DIR#br:/main/scm001;changeset:61@rep:doom3src@repserver:HERMES:8087
                CH "/search" DIR#br:/main/scm001;changeset:61@rep:doom3src@repserver:HERMES:8087
                CH "/search/search.h" FILE#br:/main/scm001;changeset:61@rep:doom3src@repserver:HERMES:8087
             */
                         
            string Status = Line.Substring(0, Line.IndexOf(' '));
            Line = Line.Substring(Status.Length).Trim();

            string Filename = Line.Substring(0, Line.LastIndexOf('"') + 1);
            Line = Line.Substring(Filename.Length).Trim();

            Filename = Filename.Replace("\"", "");


            string fileType = Line.Substring(0, Line.IndexOf("#") + 1);
            Line = Line.Substring(fileType.Length).Trim();

            fileType = fileType.Replace("#", "");

            string Branch = Line.Substring(0, Line.IndexOf(';') + 1);
            Line = Line.Substring(Branch.Length).Trim();

            Branch = Branch.Replace(";", "");
            Branch = Branch.Replace("br:", "");

            string Changeset = Line.Substring(0, Line.IndexOf('@') + 1);
            Line = Line.Substring(Changeset.Length);

            Changeset = Changeset.Replace("@", "");
            Changeset = Changeset.Replace("changeset:", "");

            string Repo = Line.Substring(0, Line.IndexOf('@') + 1);
            Line = Line.Substring(Repo.Length);

            Repo = Repo.Replace("@", "");
            Repo = Repo.Replace("rep:", "");

            string Server = Line;

            Server = Server.Replace("repserver", "");


            FileOperation = Status switch
            {
                "AD" => FileOperation.Add,
                "CH" => FileOperation.Change,
                "MV" => FileOperation.Move,
                "DE" => FileOperation.Delete,
                _ => FileOperation.Unknown,
            };

            this.FileType = fileType switch
            {
                "DIR" => FileType.Directory,
                "FILE" => FileType.File,
                _ => FileType.Unknown
            };

            this.FileName = Filename;            
            this.Branch = Branch;
            this.ChangeId = Changeset;
            this.Repository = Repo;
            this.Server = Server;                       
        }

        public override string ToString()
        {
            //MODE: File Branch@Repo
            return string.Format("{0}: \'{1}\'\t\t{2}@{3}", FileOperation.ToString(), FileName, Branch, Repository);
        }              

    }

    class CheckinTrigger : ITriggerEvent, IWithDetails, IWithComment
    {
        public string Title
        {
            get;
            private set;
        }

        public string Body
        {
            get;
            private set;
        }
                

        public IEnumerable<object> Details
        {
            get;
            private set;
        }

        public string Author
        {
            get;
            private set;
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

        public string Comment
        {
            get;
            private set;
        }

        public void Parse(string data)
        {
            string[] StringArray = data.Split(Environment.NewLine);
            List<ItemData> Items = new List<ItemData>();

            foreach(string s in StringArray)
            {
                ItemData dat = new ItemData();
                dat.Parse(s);

                Items.Add(dat);
            }           
            string ChangeSet = Environment.GetEnvironmentVariable("PLASTIC_CHANGESET") ?? "#ERR_NoChangeNum@Err@Err@err";

            
            string changeNum = ChangeSet.Substring(0, ChangeSet.IndexOf('@'));
            ChangeSet = ChangeSet.Substring(changeNum.Length + 1);

            changeNum = changeNum.Replace("cs:", "");

            string branch = ChangeSet.Substring(0, ChangeSet.IndexOf('@'));
            ChangeSet = ChangeSet.Substring(branch.Length + 1);

            branch = branch.Replace("br:", "");

            string repo = ChangeSet.Substring(0, ChangeSet.IndexOf('@'));
            ChangeSet = ChangeSet.Substring(repo.Length + 1);

            repo = repo.Replace("rep:", "");

            string repserver = ChangeSet;

            repserver = repserver.Replace("repserver:", "");

            Comment = Environment.GetEnvironmentVariable("PLASTIC_COMMENT") ?? "#ERR_NoDisc";
            Author = Environment.GetEnvironmentVariable("PLASTIC_USER") ?? "#ERR_NoAuthor"; 
            Server = Environment.GetEnvironmentVariable("PLASTIC_SERVER") ?? "#ERR_NoServer";
            ClientMachine = Environment.GetEnvironmentVariable("PLASTIC_CLIENTMACHINE") ?? "#ERR_NoServer";

            Title = string.Format("Change #{0} on {1} {2}", changeNum, repo, branch);
            Body = Comment;

            Details = Items;
           
        }
    }
}
