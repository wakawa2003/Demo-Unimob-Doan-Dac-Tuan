using System.Collections.Generic;
using EasyDI;
using TuanTool;
using UnityEngine;

namespace MyGameNamespace
{
    public class SceneGameInstaller : MonoInstaller
    {

        [SerializeField] private GameObject UserData;
        [SerializeField] private GameObject PoolManager;
        [SerializeField] private BuffManager BuffManager;

        public override void InstallBinding()
        {
            Debug.Log($"install binding");
            ContainerBinding.Bind<IUserData>().To<UserDataController>().FromInstance(UserData.GetComponent<UserDataController>());
            Debug.Log($"install binding");
            ContainerBinding.Bind<IPoolMannager>().To<Pool>().FromInstance(PoolManager.GetComponent<Pool>());
            Debug.Log($"install binding");

            // ContainerBinding.Decore<IPlant>().To<BuffManager.BuffForPlant_x2_x3>().CustomGetInstance((a, b) =>
            // {

            //     Debug.Log($"inject BuffForPlant_x2_x3: {a.GetType().FullName}");
            //     return new BuffManager.BuffForPlant_x2_x3();
            // });
            // ContainerBinding.Decore<IPlant>().To<BuffManager.BuffForPlant_x3>().CustomGetInstance((a, b) => new BuffManager.BuffForPlant_x3());

        }



    }
}
