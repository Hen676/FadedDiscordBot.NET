using System.Collections.Generic;

namespace FadedVanguardBot0._1.Models.Config
{
    public class ReactionRoleConfig
    {
        public ulong? Message { get; set; }
        public List<ulong>  Roles { get; set; } = new List<ulong>();
    }
}
