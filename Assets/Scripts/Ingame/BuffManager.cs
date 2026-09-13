using System.Collections.Generic;
using UnityEngine;

namespace MyGameNamespace
{
    public class BuffManager : MonoBehaviour
    {

        #region Singleton
        private static BuffManager ins;
        public static BuffManager Ins
        {
            get
            {
                if (ins == null)
                {
                    var a = FindObjectOfType<BuffManager>();
                    a?.Awake();
                }
                return ins;
            }
            set => ins = value;
        }
        #endregion

        private void Awake()
        {
            #region Singleton
            if (ins == null)
                ins = this;
            else
            {
                if (ins != this)
                    Destroy(gameObject);
                return;
            }
            #endregion
        }
        public List<string> BuffPlantByPlantID_x2_List = new();
        public List<string> BuffPlantByPlantID_x3_List = new();

        public bool IsGlobalProfitX2 = false;
        public bool IsGlobalProfitX3 = false;

        public class BuffForPlant_x2_x3 : IPlant
        {

            public ICarryable Carryable => throw new System.NotImplementedException();

            public string ResourcesID { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

            public Transform transform => throw new System.NotImplementedException();

            public List<ILevelPlant> LevelPlants => throw new System.NotImplementedException();

            public ILevelPlant LevelPlantStrategy { get; set; }
            public int Level { get; set; }
            public int _CoinEarn { get => getCoin(); set { } }


            public float _Duration { get; set; }
            public IPlant Decore { get; set; }
            public IPlant PrevDecore { get; set; }
            public string PlantID { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

            public void SetLevel(ILevelPlant levelPlant)
            {
            }

            public virtual int getCoin()
            {

                // Debug.Log($"get coin in buff");
                // return 0;
                var root = (this as IPlant).GetRoot();
                var idResourcesRoot = root.PlantID;
                int mul = 1;

                var x2 = BuffManager.Ins.BuffPlantByPlantID_x2_List.Find(_ => _ == idResourcesRoot);
                if (x2 != null)
                    mul *= 2;

                var x3 = BuffManager.Ins.BuffPlantByPlantID_x3_List.Find(_ => _ == idResourcesRoot);
                if (x3 != null)
                    mul *= 3;

                if (BuffManager.Ins.IsGlobalProfitX2)
                    mul *= 2;

                if (BuffManager.Ins.IsGlobalProfitX3)
                    mul *= 3;

                mul -= 1;//phai tru di 1 don vi de bu vao gia tri goc
                return root._CoinEarn * mul;
            }

        }

    }
}
