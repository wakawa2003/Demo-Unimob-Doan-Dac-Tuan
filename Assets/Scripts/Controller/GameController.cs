using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor.PackageManager;
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
        [SerializeField] private Transform endPosCustomer;

        [Header("Deliver Configs")]
        [SerializeField] private GameObject deliverPrefabs;
        [SerializeField] private Transform initPosSpawnDeliver;
        [SerializeField] private Transform endPosRunDeliver;

        List<IDeliver> DeliverWaitingList = new List<IDeliver>();
        List<ICustomer> CustomerWaitingList = new List<ICustomer>();

        void OnDestroy()
        {
            EventDispatcher.OnCarryableSpawneed.RemoveListener(OnCarryableSpawneed);
            EventDispatcher.OnDeliverHasCarryable.RemoveListener(OnDeliverHasCarryable);
            EventDispatcher.OnCustomerReadyToTake.RemoveListener(OnCustomerReadyToTake);
            EventDispatcher.onCustomerTakeCarryable.RemoveListener(onCustomerTakeCarryable);
            EventDispatcher.OnCustomerInit.RemoveListener(onCustomerInit);
            EventDispatcher.OnDeliverInit.RemoveListener(onDeliverInit);
        }



        void Start()
        {
            Input.simulateMouseWithTouches = true;
            EventDispatcher.OnCarryableSpawneed.AddListener(OnCarryableSpawneed);
            EventDispatcher.OnDeliverHasCarryable.AddListener(OnDeliverHasCarryable);
            EventDispatcher.OnCustomerReadyToTake.AddListener(OnCustomerReadyToTake);
            EventDispatcher.onCustomerTakeCarryable.AddListener(onCustomerTakeCarryable);
            EventDispatcher.OnCustomerInit.AddListener(onCustomerInit);
            EventDispatcher.OnDeliverInit.AddListener(onDeliverInit);

            SpawnNewCustomer(initPosSpawnCustomer.position, initPosSpawnCustomer.rotation, true).Forget();
        }

        private void onCustomerTakeCarryable((ICustomer customer, ICarrier carrier, ICarryable carryable) data)
        {
            SpawnNewCustomer(initPosSpawnCustomer.position, initPosSpawnCustomer.rotation, false).Forget();
        }

        private void onDeliverInit(IDeliver arg0)
        {

        }

        private void onCustomerInit(ICustomer arg0)
        {

        }

        private void OnDeliverHasCarryable((ICarryable carryable, IDeliver deliver) arg0)
        {
            Debug.Log($"OnDeliverHasCarryable");
            if (!DeliverWaitingList.Contains(arg0.deliver))
                DeliverWaitingList.Add(arg0.deliver);
            CheckCollect().Forget();
        }

        private void OnCustomerReadyToTake(ICustomer customer)
        {
            Debug.Log($"OnCustomerReadyToTake");
            if (!CustomerWaitingList.Contains(customer))
                CustomerWaitingList.Add(customer);
            CheckCollect().Forget();
        }

        private async UniTask CheckCollect()
        {
            if (CustomerWaitingList.Count() > 0 && DeliverWaitingList.Count() > 0)
            {

                Debug.Log($"check collect");
                var customer = CustomerWaitingList.First();
                var deliver = DeliverWaitingList.First();
                CustomerWaitingList.Remove(customer);
                DeliverWaitingList.Remove(deliver);

                //tim ra slot chua customer
                var s = this.slotServerData.ToList().Find(_ =>
                {
                    if (_.Customer != null && _.Deliver == null)
                        return _.Customer?.transform == customer.transform;
                    return false;
                });


                s.Deliver = deliver;
                if (s == null)
                    Debug.LogError($"Loi khong tim thay slot");
                await deliver.RunToPosition(s.PositonDeliver.position, s.PositonDeliver.eulerAngles, destroyCancellationToken);
                await customer.TakeCarryable(deliver, deliver.Carryable, destroyCancellationToken);
                s.Customer = null;
                s.Deliver = null;
            }
        }

        private async void OnCarryableSpawneed((ICarryable carryable, Transform transformTake) data)
        {
            var newDeliver = Instantiate(deliverPrefabs, initPosSpawnDeliver.position, Quaternion.identity);

            newDeliver.GetComponent<IDeliver>().Setup(endPosRunDeliver.position);
            //chay den cho noi can lay
            await newDeliver.GetComponent<IDeliver>().RunToPosition(data.transformTake.position, data.transformTake.eulerAngles, destroyCancellationToken);

            //lay carryable va chay anim
            await newDeliver.GetComponent<ICarrier>().TakeCarryable(data.carryable.Owner, data.carryable, destroyCancellationToken);
        }

        public async UniTask<SlotServeData> SpawnNewCustomer(Vector3 startPos, Quaternion rotation, bool isInstant)
        {
            var emptySlot = slotServerData.ToList().Find(_ => _.Customer == null);
            if (emptySlot != null)
            {

                var pos = isInstant ? emptySlot.PositionCustomer.position : startPos;
                var newCustomer = Instantiate(customerPrefabs, pos, emptySlot.PositionCustomer.transform.rotation);
                emptySlot.Customer = newCustomer;
                newCustomer.GetComponent<ICustomer>().Setup(endPosCustomer.position);

                await newCustomer.GetComponent<ICustomer>().RunToPosition(emptySlot.PositionCustomer.position, emptySlot.PositionCustomer.transform.eulerAngles, isInstant, destroyCancellationToken);
            }
            return emptySlot;
        }


        [System.Serializable]
        public class SlotServeData
        {
            [ReadOnly] public GameObject Customer;
            [FormerlySerializedAs("Position")]
            public Transform PositionCustomer;
            [ReadOnly] public ICarrier Deliver;
            public Transform PositonDeliver;
        }
    }
}
