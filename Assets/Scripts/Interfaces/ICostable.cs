using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGameNamespace
{
    public interface ICostable
    {
        public void Setup(int cost);
        public int Cost { get; set; }
    }
}