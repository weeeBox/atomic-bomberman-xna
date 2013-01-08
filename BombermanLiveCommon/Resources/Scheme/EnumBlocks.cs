using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombermanLiveCommon.Resources.Scheme
{
    public enum EnumBlocks
    {
        BLOCK_BLANK = 0,     // no block
        BLOCK_BREAKABLE,     // breakable brick
        BLOCK_SOLID,         // solid brick
        BLOCK_EXTRA,         // map extra
        BLOCK_POWERUP,       // powerup
        BLOCK_BOMB,          // pulsating bomb
        BLOCK_FLAME          // flame
    };
}
