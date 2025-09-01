using System.Collections;
using Assets.Code.Config;
using GoogleMobileAds.Api;
using UnityEngine;
using static Assets.Code.AdManager;
using static Assets.Code.Config.AdConfig;

namespace Assets.Code
{
    public class AdManager : MonoBehaviour
    {
        private RewardedAd rewardedAd;
        private string adUnitId = "";
        public static AdManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 場景切換也不會被摧毀
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            //AdMob廣告 ID
#if UNITY_EDITOR
            adUnitId = "ca-app-pub-3940256099942544/6300978111"; // 測試用 ID
#elif UNITY_ANDROID
        adUnitId = AdConfig.AdMod_ID; // 正式 ID
#else
        adUnitId = "unexpected_platform";
#endif

            MobileAds.Initialize(initStatus => { });
            LoadRewardedAd();
        }

        public void LoadRewardedAd()
        {
            var adRequest = new AdRequest();

            RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("獎勵廣告載入失敗: " + error);
                }

                rewardedAd = ad;
                Debug.Log("獎勵廣告載入成功");

                rewardedAd.OnAdFullScreenContentFailed += (AdError err) =>
                {
                    Debug.LogError("廣告播放失敗: " + err);
                    LoadRewardedAd();
                };

                // 廣告全螢幕內容關閉事件
                rewardedAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("廣告已關閉");
                    LoadRewardedAd();  // 自動載入下一支
                };
            });
        }

        public void ShowRewardedAd(System.Action onSuccess)
        {
            if (rewardedAd != null)
            {
                rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log("廣告完成，玩家獲得獎勵");
                    onSuccess?.Invoke(); // 呼叫回傳
                });
            }
            else
            {
                Debug.LogWarning("廣告還沒載入!");
            }
        }
    }
}