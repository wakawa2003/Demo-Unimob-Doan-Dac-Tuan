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

            CheckUniqueUpgradeList();
        }

        public void CheckUniqueUpgradeList()
        {
            var upgradesByName = new Dictionary<string, List<IUpgradeStrategy>>();

            foreach (var item in UpgradeList)
            {
                var upgrade = item.Value;
                if (upgrade == null)
                {
                    Debug.LogError("UpgradeList contains a null IUpgradeStrategy.", this);
                    continue;
                }

                if (!upgradesByName.TryGetValue(upgrade.Name, out var upgrades))
                {
                    upgrades = new List<IUpgradeStrategy>();
                    upgradesByName.Add(upgrade.Name, upgrades);
                }

                upgrades.Add(upgrade);
            }

            foreach (var item in upgradesByName)
            {
                if (item.Value.Count < 2)
                    continue;

                var duplicateFiles = string.Join(", ", item.Value.Select(GetUpgradeSource));
                Debug.LogError($"Duplicate IUpgradeStrategy Name '{item.Key}' found in: {duplicateFiles}", this);
            }

            string GetUpgradeSource(IUpgradeStrategy upgrade)
            {
#if UNITY_EDITOR
                if (upgrade is UnityEngine.Object unityObject)
                {
                    var assetPath = UnityEditor.AssetDatabase.GetAssetPath(unityObject);
                    if (!string.IsNullOrEmpty(assetPath))
                        return assetPath;
                }
#endif
                return upgrade is UnityEngine.Object obj
                    ? obj.name
                    : upgrade.GetType().FullName;
            }

        }

        public ResourceConfig GetResourceConfig(string id)
        {
            return ResourceConfigs.First(_ => _.ID == id);
        }
    }
}
