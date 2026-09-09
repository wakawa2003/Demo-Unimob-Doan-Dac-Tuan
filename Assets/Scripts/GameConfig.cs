using System.Linq;
using UnityEngine;

namespace MyGameNamespace
{
    public class GameConfig : MonoBehaviour
    {

        public ResourceConfig[] ResourceConfigs;
        #region Singleton
        private static GameConfig ins;
        public static GameConfig Ins
        {
            get
            {
                if (ins == null)
                {
                    var a = FindObjectOfType<GameConfig>();
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

        public ResourceConfig GetResourceConfig(string id)
        {
            return ResourceConfigs.First(_ => _.ID == id);
        }
    }
}
