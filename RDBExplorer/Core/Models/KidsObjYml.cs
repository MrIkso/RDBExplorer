using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Core.Models
{
    public class NameInfo
    {
        public string LocalName { get; set; }
        public string FullName { get; set; }
        public uint Hash { get; set; }
    }

    public class TypeEntry
    {
        public NameInfo Name { get; set; }
        public List<TypeEntry> Parents { get; set; }
        public List<object> Properties { get; set; }
    }


    internal class KidsObjYml
    {
        public List<TypeEntry> Types { get; set; }
    }
}
