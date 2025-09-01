using GoogleMobileAds.Api;
using UnityEngine;

namespace Assets.Code
{
    public class Begin : MonoBehaviour
    {
        [SerializeField] private GameObject mainGameGameObject;
        [SerializeField] private GameObject raceGameGameObject;

        void Start()
        {
            raceGameGameObject.SetActive(false);
            mainGameGameObject.SetActive(true);

            MobileAds.Initialize(initStatus => {
                Debug.Log("AdMob Initialized");
            });
        }
    }
}
