using System;
using System.Collections.Generic;
using System.Text;

namespace PlasticNotificationSystem
{
    public enum FileOperation
    {
        Add,
        Change,
        Move,
        Delete,
        Unknown
    }

    public enum FileType
    {
        File,
        Directory,
        Unknown
    }

    public interface IWithFileChange : IWithChangeSet
    {
        public FileOperation FileOperation
        {
            get;
        }

        public FileType FileType
        {
            get;
        }

        public string FileName
        {
            get;
        }

    }
}
