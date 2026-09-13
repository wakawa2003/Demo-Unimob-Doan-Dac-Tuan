using System;
using TuanTool;
using TuanTool.Popup;
using UnityEngine;

namespace MyGameNamespace
{
    public class UpgradePopup : Popup
    {

        [SerializeField] private GameObject slotPrefabs;

        public override void Show(Action onComplete = null)
        {
            base.Show(onComplete);
            Refresh();
        }

        private void Refresh()
        {
            slotPrefabs.SetActive(false);
            slotPrefabs.transform.parent.DeleteAllChild(slotPrefabs.transform);

            foreach (var item in GameConfig.Ins.UpgradeList)
            {
                if (item.Value.IsCanBuy())
                {
                    var newSlot = Instantiate(slotPrefabs, slotPrefabs.transform.parent);
                    newSlot.gameObject.SetActive(true);
                    newSlot.GetComponent<UpgradeItemView>().Setup(item.Value, delegate
                    {
                        // Refresh();
                    });
                }
            }
        }
    }
}
