using EasyDI;
using UnityEngine;

namespace MyGameNamespace
{
    public class SceneGameInstaller : MonoInstaller
    {

        [SerializeField] private GameObject UserData;
        public override void InstallBinding()
        {
            ContainerBinding.Bind<IUserData>().To<UserDataController>().FromInstance(UserData.GetComponent<UserDataController>());
        }


    }
}
