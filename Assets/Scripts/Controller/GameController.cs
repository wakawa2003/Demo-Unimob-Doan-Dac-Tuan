using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public class GameController : MonoBehaviour
    {
        [Header("Customer Configs")]
        [SerializeField] private Transform initPosSpawnCustomer;
        [SerializeField] private GameObject customerPrefabs;
        [SerializeField] private CustomerData[] customerDatas;

        void Start()
        {
            Input.simulateMouseWithTouches = true;
            SpawnNewCustomer(initPosSpawnCustomer.position, initPosSpawnCustomer.rotation, 0).Forget();
        }

        public async UniTask<CustomerData> SpawnNewCustomer(Vector3 startPos, Quaternion rotation, float duration)
        {
            var emptySlot = customerDatas.ToList().Find(_ => _.Customer == null);
            var newCustomer = Instantiate(customerPrefabs, startPos, rotation);
            emptySlot.Customer = newCustomer;

            await newCustomer.GetComponent<ICustomer>().MoveTotarget(emptySlot.Position, duration, destroyCancellationToken);
            return emptySlot;
        }


        [System.Serializable]
        public class CustomerData
        {
            public GameObject Customer;
            public Transform Position;
        }
    }
}
