using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MyGameNamespace
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private SlotServeData[] slotServerData;
        [Header("Customer Configs")]
        [SerializeField] private Transform initPosSpawnCustomer;
        [SerializeField] private GameObject customerPrefabs;

        [Header("Deliver Configs")]
        [SerializeField] private GameObject deliverPrefabs;
        [SerializeField] private Transform initPosSpawnDeliver;
        [SerializeField] private Transform endPosRunDeliver;


        void Start()
        {
            Input.simulateMouseWithTouches = true;
            SpawnNewCustomer(initPosSpawnCustomer.position, initPosSpawnCustomer.rotation, 0).Forget();
            EventDispatcher.OnCarryableSpawneed.AddListener(OnCarryableSpawneed);
        }

        void OnDestroy()
        {
            EventDispatcher.OnCarryableSpawneed.RemoveListener(OnCarryableSpawneed);
        }


        private async void OnCarryableSpawneed((ICarryable carryable, Transform transformTake) data)
        {
            var newDeliver = Instantiate(deliverPrefabs, initPosSpawnDeliver.position, Quaternion.identity);
            //chay den cho noi can lay
            await newDeliver.GetComponent<IDeliver>().RunToPosition(data.transformTake.position, data.transformTake.eulerAngles, destroyCancellationToken);

            //lay carryable va chay anim
            await newDeliver.GetComponent<ICarrier>().TakeCarryable(data.carryable, destroyCancellationToken);
        }

        public async UniTask<SlotServeData> SpawnNewCustomer(Vector3 startPos, Quaternion rotation, float duration)
        {
            var emptySlot = slotServerData.ToList().Find(_ => _.Customer == null);
            if (emptySlot != null)
            {
                var newCustomer = Instantiate(customerPrefabs, startPos, rotation);
                emptySlot.Customer = newCustomer;
                await newCustomer.GetComponent<ICustomer>().MoveTotarget(emptySlot.PositionCustomer, duration, destroyCancellationToken);
            }
            return emptySlot;
        }


        [System.Serializable]
        public class SlotServeData
        {
            [ReadOnly] public GameObject Customer;
            [FormerlySerializedAs("Position")]
            public Transform PositionCustomer;
            [ReadOnly] public GameObject Deliver;
            public Transform PositonDeliver;
        }
    }
}
