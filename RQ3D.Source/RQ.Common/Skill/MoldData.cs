using RQ.Base.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace RQ.Base.Skill
{
    /// <summary>
    /// Container class to contain the mold and its related shards and shard counts. Used to update the HUD and other tasks.
    /// </summary>
    public class MoldData
    {
        public MoldConfig MoldConfig { get; set; }
        public List<ItemDesc> shardConfigs { get; set; }
    }
}
