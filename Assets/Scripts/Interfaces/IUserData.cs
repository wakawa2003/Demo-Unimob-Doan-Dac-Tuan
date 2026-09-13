using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace MyGameNamespace
{
    public interface IUserData
    {
        public long Coin { get; set; }
        public long Diamond { get; set; }

        public UnityEvent<(long Before, long after)> OnCoinChange { get; set; }
        public UnityEvent<(long Before, long after)> OnDiamondChange { get; set; }

        public long AddCoin(long amount)
        {
            long b = Coin;
            Coin += amount;
            Coin = math.max(0, Coin);
            OnCoinChange?.Invoke(new(b, Coin));
            return Coin;
        }

        public long AddDiamond(long amount)
        {
            long b = Diamond;
            Diamond += amount;
            Diamond = math.max(0, Diamond);
            OnDiamondChange?.Invoke(new(b, Diamond));
            return Diamond;
        }
    }
}