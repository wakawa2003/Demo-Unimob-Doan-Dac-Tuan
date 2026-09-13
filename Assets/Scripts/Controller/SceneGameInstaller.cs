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
            ContainerBinding.Bind<IUserData>().To<UserDataController>().FromInstance(UserData.GetComponent<UserDataController>());
            ContainerBinding.Bind<IPoolMannager>().To<Pool>().FromInstance(PoolManager.GetComponent<Pool>());

            ContainerBinding.Decore<IPlant>().To<BuffManager.BuffForPlant_x2_x3>().CustomGetInstance((a, b) => new BuffManager.BuffForPlant_x2_x3());
            // ContainerBinding.Decore<IPlant>().To<BuffManager.BuffForPlant_x3>().CustomGetInstance((a, b) => new BuffManager.BuffForPlant_x3());

        }



    }
}
