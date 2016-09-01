using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DeleteLogFileService.Configuration
{
    public class FolderConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("settings", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(FolderConfigCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public FolderConfigCollection Settings
        {
            get
            {
                return (FolderConfigCollection)base["settings"];
            }
        }
    }
}
