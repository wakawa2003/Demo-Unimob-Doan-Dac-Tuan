using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EasyDI;
using Sirenix.OdinInspector;
using TuanTool;
using Unity.Collections;
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

        [Inject] IUserData userData;
        [Inject] IPoolMannager poolMannager;

        List<IDeliver> DeliverWaitingList = new List<IDeliver>();
        List<ICustomer> CustomerWaitingList = new List<ICustomer>();
        List<CarryableWaitToDeliverData> CaryableWaitToDeliverDatasList = new();

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
            var cost = data.carryable.GetComponent<ICostable>();
            userData.AddCoin(cost.Cost);
            var slot = slotServerData.First(_ => _.Customer?.transform == data.customer.transform);
            if (poolMannager == null)
                Debug.LogError($"nulll");
            poolMannager.Instantiate("coinRewardFX", slot.PositionRewardCoin.position, slot.PositionRewardCoin.rotation).GetComponentInChildren<ParticleSystem>().Play(true);
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

                //xoá carryable da co nguoi deliver đi
                var find = CaryableWaitToDeliverDatasList.Find(_ => _.Deliver == deliver);
                CaryableWaitToDeliverDatasList.Remove(find);

                if (s == null)
                    Debug.LogError($"Loi khong tim thay slot");
                await deliver.RunToPosition(s.PositonDeliver.position, s.PositonDeliver.eulerAngles, destroyCancellationToken);
                await customer.TakeCarryable(deliver, deliver.Carryable, destroyCancellationToken);
                s.Customer = null;
                s.Deliver = null;
                await CheckSpawnNewDeliver();
            }
        }

        private async void OnCarryableSpawneed((ICarrier carrier, ICarryable carryable, Transform transformTake) data)
        {
            var find = CaryableWaitToDeliverDatasList.Find(_ => _.carrier == data.carrier && _.carryable == data.carryable);
            if (find == null)
                CaryableWaitToDeliverDatasList.Add(new CarryableWaitToDeliverData { carrier = data.carrier, carryable = data.carryable, transformTake = data.transformTake });
            await CheckSpawnNewDeliver();
        }

        private async UniTask CheckSpawnNewDeliver()
        {
            //chi spawn deliver cho caryable chua co deliver thoi

            foreach (var data in CaryableWaitToDeliverDatasList.ToList())
            {
                if (data.Deliver == null)
                    if (CaryableWaitToDeliverDatasList.Find(_ => _.carrier == data.carrier && _.Deliver != null) == null)//kiem tra khong trung lap nhung carrier da co deliver roi.
                    {
                        var newDeliver = Instantiate(deliverPrefabs, initPosSpawnDeliver.position, Quaternion.identity).GetComponent<IDeliver>();
                        newDeliver.Setup(endPosRunDeliver.position);
                        data.Deliver = newDeliver;
                        //chay den cho noi can lay
                        await newDeliver.RunToPosition(data.transformTake.position, data.transformTake.eulerAngles, destroyCancellationToken);

                        //lay carryable va chay anim
                        await newDeliver.TakeCarryable(data.carryable.Owner, data.carryable, destroyCancellationToken);
                    }
            }
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
        public class CarryableWaitToDeliverData
        {
            public ICarrier carrier;
            public ICarryable carryable;
            public Transform transformTake;

            public ICarrier Deliver;
        }

        [System.Serializable]
        public class SlotServeData
        {
            [Sirenix.OdinInspector.ReadOnly] public GameObject Customer;
            [FormerlySerializedAs("Position")]
            public Transform PositionCustomer;
            [Sirenix.OdinInspector.ReadOnly] public ICarrier Deliver;
            public Transform PositonDeliver;

            public Transform PositionRewardCoin;
        }
    }
}
