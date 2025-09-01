using System;
using System.Collections;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Code.Class.StoneClass;

namespace Assets.Code
{
    public class Stone : MonoBehaviour
    {
        [SerializeField] private PetUIButton petUIButton;
        private double lastPatTime = 0f, lastWaterTime = 0f, lastFeedTime = 0f;
        [SerializeField] public SpriteRenderer petRenderer;  // 角色的 SpriteRenderer
        [SerializeField] private Sprite[] petSprites;         // 存放不同型態的圖片
        private int petStage = 1;
        public int growthValue = 0;
        private int[] evolutionValues = { 200, 300, 400, 500, 600, 700, 800, 900, 1000 }; // 每個階段的需求值
        private int maxStage = 10; // 最高等級
        [SerializeField] private TextMeshProUGUI petLevelText; // UI 文字
        [SerializeField] private Slider expSlider; // 經驗值進度條
        [SerializeField] private TextMeshProUGUI expHint; // 經驗值進度條
        [SerializeField] public TextMeshProUGUI petNameText; // UI 文字
        [SerializeField] private Image levelUpImage;
        [SerializeField] private Transform petTransform;
        [SerializeField] private GameObject adConfirmWindow;
        [SerializeField] private GameObject warningPanel;
        private GameObject adSourceButtonObject;
        private float uiOffsetX = 150f;
        private float uiOffsetY = 75f;
        private float patCoolDownTime = 120f;
        private float waterCoolDownTime = 250f;
        private float feedCoolDownTime = 300f;


        void Start()
        {
            LoadData(); // 載入存檔
            Invoke("CheckPetUIButtonCooldownTime", 0.01f); // 檢查按鈕冷卻時間
            UpdateSliderUI();
            UpdatePetAppearance();

            adConfirmWindow.SetActive(false);
        }

        void Update()
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(petTransform.position);

            petUIButton.petUIRectTransform.position = screenPos;
        }

        //摸摸
        public void Pat()
        {
            if (petUIButton.patButton.GetComponent<ButtonCooldown>().isCoolingDown == true)
            {
                adSourceButtonObject = petUIButton.patButton;
                adConfirmWindow.SetActive(true);
                return;
            }

            Double currentTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            if (IsActionAvailable(1, currentTime))
            {
                StartCoroutine(petUIButton.patButton.GetComponent<ButtonCooldown>().StartCooldown(patCoolDownTime, true));
                lastPatTime = currentTime;
                growthValue += 10;
                OnExpGained();
                PlayerPrefs.SetString("LastPatTime", lastPatTime.ToString());
                PlayerPrefs.Save();
            }
        }

        //喝水
        public void GiveWater()
        {
            if (petUIButton.waterButton.GetComponent<ButtonCooldown>().isCoolingDown == true)
            {
                adSourceButtonObject = petUIButton.waterButton;
                adConfirmWindow.SetActive(true);
                return;
            }

            Double currentTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            if (IsActionAvailable(2, currentTime))
            {
                StartCoroutine(petUIButton.waterButton.GetComponent<ButtonCooldown>().StartCooldown(waterCoolDownTime, true));
                lastWaterTime = currentTime;
                growthValue += 20;
                OnExpGained();
                PlayerPrefs.SetString("LastWaterTime", lastWaterTime.ToString());
                PlayerPrefs.Save();
            }
        }

        //吃飯
        public void Feed()
        {
            if (petUIButton.feedButton.GetComponent<ButtonCooldown>().isCoolingDown == true)
            {
                adSourceButtonObject = petUIButton.feedButton;
                adConfirmWindow.SetActive(true);
                return;
            }

            double currentTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            if (IsActionAvailable(3, currentTime))
            {
                StartCoroutine(petUIButton.feedButton.GetComponent<ButtonCooldown>().StartCooldown(feedCoolDownTime, true));
                lastFeedTime = currentTime;
                growthValue += 50;
                OnExpGained();
                PlayerPrefs.SetString("LastFeedTime", lastFeedTime.ToString());
                PlayerPrefs.Save();
            }
        }

        bool IsActionAvailable(int type, double time)
        {
            string lastTimeString = "";
            float coolDownTine = 0f;
            //1 = 摸摸, 2 = 喝水, 3 = 吃飯 
            if (type == 1)
            {
                lastTimeString = "LastPatTime";
                coolDownTine = patCoolDownTime;
            }
            else if (type == 2)
            {
                lastTimeString = "LastWaterTime";
                coolDownTine = waterCoolDownTime;
            }
            else if (type == 3)
            {
                lastTimeString = "LastFeedTime";
                coolDownTine = feedCoolDownTime;
            }
            double lastTime = PlayerPrefs.GetFloat(lastTimeString, 0);
            return (time - lastTime) >= coolDownTine;
        }

        //廣告確認視窗
        public void OnAdConfirmWindowClose()
        {
            adConfirmWindow.SetActive(false);
        }

        public void OnAdConfirmWindowConfirm()
        {
            Debug.Log(adSourceButtonObject.name);
            AdManager.Instance.ShowRewardedAd(() =>
            {
                adSourceButtonObject.GetComponent<ButtonCooldown>().StopCooldown();
                adConfirmWindow.SetActive(false);
                return;
            });

            StartCoroutine(warningPanel.GetComponent<ShowWarning>().ShowWaningCanvasGroup());
        }

        void UpdateSliderUI()
        {
            petLevelText.text = $"Lv. {petStage}"; // 顯示等級
            expSlider.value = GetCurrentExpRatio();    // 更新經驗條
            if (petStage >= maxStage)
            {
                expHint.text = "MAX";
            }
            else
            {
                expHint.text = $"{growthValue} / {evolutionValues[petStage - 1]}";
            }
        }

        float GetCurrentExpRatio()
        {
            if (petStage >= maxStage) return 1f; // 滿級顯示100%
            int currentStageExp = evolutionValues[petStage - 1];
            return (growthValue / (float)currentStageExp);
        }

        void CheckEvolution()
        {
            if (petStage < maxStage && growthValue >= evolutionValues[petStage - 1])
            {
                petStage++;
                growthValue = 0;
                UpdatePetAppearance();
                StartCoroutine(ShowLevelUp());
            }
        }

        void UpdatePetAppearance()
        {
            if (petStage >= 1 && petStage <= petSprites.Length && petStage <= 10)
            {
                petRenderer.sprite = petSprites[petStage - 1]; // 直接修改 Sprite
                SetStoneUIPosition();
            }
        }

        public IEnumerator ShowLevelUp()
        {
            levelUpImage.gameObject.SetActive(true);
            CanvasGroup canvasGroup = levelUpImage.GetComponent<CanvasGroup>();
            RectTransform rect = levelUpImage.GetComponent<RectTransform>();

            // 初始狀態
            canvasGroup.alpha = 0;
            rect.localScale = Vector3.zero;
            rect.anchoredPosition = Vector2.zero;

            float duration = 1f;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                // 淡入、縮放、往上飄
                canvasGroup.alpha = Mathf.Lerp(0, 1, t);
                rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                rect.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(0, 100), t);

                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            // 淡出
            time = 0f;
            duration = 0.5f;
            Vector2 startPos = rect.anchoredPosition;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                canvasGroup.alpha = Mathf.Lerp(1, 0, t);
                rect.anchoredPosition = Vector2.Lerp(startPos, startPos + new Vector2(0, 50), t);

                yield return null;
            }

            levelUpImage.gameObject.SetActive(false);
        }

        public void OnExpGained()
        {
            CheckEvolution();
            UpdateSliderUI();
            SaveData();
        }

        void SaveData()
        {
            PlayerPrefs.SetInt("GrowthValue", growthValue);
            PlayerPrefs.SetInt("PetStage", petStage);
            PlayerPrefs.Save();
        }

        void LoadData()
        {
            growthValue = PlayerPrefs.GetInt("GrowthValue", 0);
            petStage = PlayerPrefs.GetInt("PetStage", 1);
            lastPatTime = double.Parse(PlayerPrefs.GetString("LastPatTime", "0"));
            lastWaterTime = double.Parse(PlayerPrefs.GetString("LastWaterTime", "0"));
            lastFeedTime = double.Parse(PlayerPrefs.GetString("LastFeedTime", "0"));
            petNameText.text = PlayerPrefs.GetString("PetName", "石頭醬");
        }


        public void DeletedData()
        {
            petStage = 1;
            growthValue = 0;
            UpdateSliderUI();
            UpdatePetAppearance();
            SaveData();
        }

        void CheckPetUIButtonCooldownTime()
        {

            double currentTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            float cooldownTime = 0f;

            if (currentTime - lastPatTime <= patCoolDownTime)
            {
                cooldownTime = patCoolDownTime - (float)(currentTime - lastPatTime);
                StartCoroutine(petUIButton.patButton.GetComponent<ButtonCooldown>().StartCooldown(cooldownTime, true));
            }

            if (currentTime - lastWaterTime <= waterCoolDownTime)
            {
                cooldownTime = waterCoolDownTime - (float)(currentTime - lastWaterTime);
                StartCoroutine(petUIButton.waterButton.GetComponent<ButtonCooldown>().StartCooldown(cooldownTime, true));
            }

            if (currentTime - lastFeedTime <= feedCoolDownTime)
            {
                cooldownTime = feedCoolDownTime - (float)(currentTime - lastFeedTime);
                StartCoroutine(petUIButton.feedButton.GetComponent<ButtonCooldown>().StartCooldown(cooldownTime, true));
            }
        }

        private void SetStoneUIPosition()
        {
            Bounds bounds = petRenderer.bounds; // Sprite的包圍盒（世界座標）
            Vector2 petRightWorldPos = new Vector2(bounds.max.x, bounds.center.y);
            Vector2 petLeftWorldPos = new Vector2(bounds.min.x, bounds.center.y);
            Vector2 petButtomWorldPos = new Vector2(bounds.center.x, bounds.min.y);

            //摸摸UI
            Vector2 petRightScreenPos = Camera.main.WorldToScreenPoint(petRightWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                petUIButton.petUIRectTransform,
                petRightScreenPos,
                null,
                out Vector2 rightUIPos
            );
            rightUIPos.x += uiOffsetX;
            petUIButton.patButtonGroup.GetComponent<RectTransform>().anchoredPosition = rightUIPos;

            //喝水UI
            Vector2 petLeftScreenPos = Camera.main.WorldToScreenPoint(petLeftWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                petUIButton.petUIRectTransform,
                petLeftScreenPos,
                null,
                out Vector2 leftUIPos
            );
            leftUIPos.x -= uiOffsetX;
            petUIButton.waterButtonGroup.GetComponent<RectTransform>().anchoredPosition = leftUIPos;

            //餵食物UI
            Vector2 petButtomScreenPos = Camera.main.WorldToScreenPoint(petButtomWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                petUIButton.petUIRectTransform,
                petButtomScreenPos,
                null,
                out Vector2 buttomUIPos
            );
            buttomUIPos.y -= uiOffsetY;
            petUIButton.feedButtonGroup.GetComponent<RectTransform>().anchoredPosition = buttomUIPos;
        }
    }
}
