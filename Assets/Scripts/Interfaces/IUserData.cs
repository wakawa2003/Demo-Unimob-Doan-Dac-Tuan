using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MyGameNamespace
{
    public interface IUserData
    {
        public int Coin { get; set; }
        public int Diamond { get; set; }

        public UnityEvent<(int Before, int after)> OnCoinChange { get; set; }
        public UnityEvent<(int Before, int after)> OnDiamondChange { get; set; }

        public int AddCoin(int amount)
        {
            var b = Coin;
            Coin += amount;
            Coin = Mathf.Max(0, Coin);
            OnCoinChange?.Invoke(new(b, Coin));
            return Coin;
        }

        public int AddDiamond(int amount)
        {
            var b = Diamond;
            Diamond += amount;
            Diamond = Mathf.Max(0, Diamond);
            OnDiamondChange?.Invoke(new(b, Diamond));
            return Diamond;
        }
    }
}