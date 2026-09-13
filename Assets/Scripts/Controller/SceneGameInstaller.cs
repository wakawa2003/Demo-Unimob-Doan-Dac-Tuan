using EasyDI;
using TuanTool;
using UnityEngine;

namespace MyGameNamespace
{
    public class SceneGameInstaller : MonoInstaller
    {

        [SerializeField] private GameObject UserData;
        [SerializeField] private GameObject PoolManager;
        public override void InstallBinding()
        {
            ContainerBinding.Bind<IUserData>().To<UserDataController>().FromInstance(UserData.GetComponent<UserDataController>());
            ContainerBinding.Bind<IPoolMannager>().To<Pool>().FromInstance(PoolManager.GetComponent<Pool>());
        }


    }
}
