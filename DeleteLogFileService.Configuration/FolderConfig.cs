using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DeleteLogFileService.Configuration
{
    public class FolderConfig : ConfigurationElement
    {
        public FolderConfig() { }

        public FolderConfig(string path, int days)
        {
            Path = path;
            RemainDays = days;
        }

        [ConfigurationProperty("path", DefaultValue = null, IsRequired = true, IsKey = true)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("remaindays", DefaultValue = null, IsRequired = false, IsKey = false)]
        public int RemainDays
        {
            get { return (int)this["remaindays"]; }
            set { this["remaindays"] = value; }
        }

        [ConfigurationProperty("pattern", DefaultValue = null, IsRequired = false, IsKey = false)]
        public string Pattern
        {
            get { return (string)this["pattern"]; }
            set { this["pattern"] = value; }
        }
    }
}
