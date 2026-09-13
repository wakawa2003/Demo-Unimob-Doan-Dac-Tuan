using System.Collections.Generic;
using System.Linq;
using AYellowpaper;
using EasyDI;
using UnityEngine;

namespace MyGameNamespace
{
    public class GameConfig : MonoBehaviour
    {

        [SerializeField] private List<InterfaceReference<IUpgradeStrategy>> upgradeList;

        public ResourceConfig[] ResourceConfigs;
        public List<InterfaceReference<IUpgradeStrategy>> UpgradeList
        {
            get
            {
                var injector = FindAnyObjectByType<SceneContext>();
                foreach (var item in upgradeList)
                {
                    injector.InjectFor(item.Value);
                }
                return upgradeList;
            }
        }
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
