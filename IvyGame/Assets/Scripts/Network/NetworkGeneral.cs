using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Network
{
    internal class NetworkGeneral
    {
        public static readonly int PacketTypesCount = Enum.GetValues(typeof(PacketType)).Length;
    }
}
