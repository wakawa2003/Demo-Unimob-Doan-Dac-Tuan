using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGameNamespace
{
    public class Root : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            DOVirtual.DelayedCall(0.1f, delegate
            {

                SceneManager.LoadScene(1);
            });
        }


    }
}
