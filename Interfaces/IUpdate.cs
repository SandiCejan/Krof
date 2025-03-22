using Microsoft.Xna.Framework;
using System;

namespace KrofEngine
{
    internal interface IUpdate : IUpdateable, IComparable<IUpdate>
    {
        public new int CompareTo(IUpdate other)
        {
            if (other == null) return 1;
            return UpdateOrder.CompareTo(other.UpdateOrder);
        }
    }
}
