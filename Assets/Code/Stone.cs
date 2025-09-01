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
        [SerializeField] public SpriteRenderer petRenderer;  // ���⪺ SpriteRenderer
        [SerializeField] private Sprite[] petSprites;         // �s�񤣦P���A���Ϥ�
        private int petStage = 1;
        public int growthValue = 0;
        private int[] evolutionValues = { 200, 300, 400, 500, 600, 700, 800, 900, 1000 }; // �C�Ӷ��q���ݨD��
        private int maxStage = 10; // �̰�����
        [SerializeField] private TextMeshProUGUI petLevelText; // UI ��r
        [SerializeField] private Slider expSlider; // �g��ȶi�ױ�
        [SerializeField] private TextMeshProUGUI expHint; // �g��ȶi�ױ�
        [SerializeField] public TextMeshProUGUI petNameText; // UI ��r
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
            LoadData(); // ���J�s��
            Invoke("CheckPetUIButtonCooldownTime", 0.01f); // �ˬd���s�N�o�ɶ�
            UpdateSliderUI();
            UpdatePetAppearance();

            adConfirmWindow.SetActive(false);
        }

        void Update()
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(petTransform.position);

            petUIButton.petUIRectTransform.position = screenPos;
        }

        //�N�N
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

        //�ܤ�
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

        //�Y��
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
            //1 = �N�N, 2 = �ܤ�, 3 = �Y�� 
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

        //�s�i�T�{����
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
            petLevelText.text = $"Lv. {petStage}"; // ��ܵ���
            expSlider.value = GetCurrentExpRatio();    // ��s�g���
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
            if (petStage >= maxStage) return 1f; // �������100%
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
                petRenderer.sprite = petSprites[petStage - 1]; // �����ק� Sprite
                SetStoneUIPosition();
            }
        }

        public IEnumerator ShowLevelUp()
        {
            levelUpImage.gameObject.SetActive(true);
            CanvasGroup canvasGroup = levelUpImage.GetComponent<CanvasGroup>();
            RectTransform rect = levelUpImage.GetComponent<RectTransform>();

            // ��l���A
            canvasGroup.alpha = 0;
            rect.localScale = Vector3.zero;
            rect.anchoredPosition = Vector2.zero;

            float duration = 1f;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                // �H�J�B�Y��B���W��
                canvasGroup.alpha = Mathf.Lerp(0, 1, t);
                rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                rect.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(0, 100), t);

                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            // �H�X
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
            petNameText.text = PlayerPrefs.GetString("PetName", "���Y��");
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
            Bounds bounds = petRenderer.bounds; // Sprite���]�򲰡]�@�ɮy�С^
            Vector2 petRightWorldPos = new Vector2(bounds.max.x, bounds.center.y);
            Vector2 petLeftWorldPos = new Vector2(bounds.min.x, bounds.center.y);
            Vector2 petButtomWorldPos = new Vector2(bounds.center.x, bounds.min.y);

            //�N�NUI
            Vector2 petRightScreenPos = Camera.main.WorldToScreenPoint(petRightWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                petUIButton.petUIRectTransform,
                petRightScreenPos,
                null,
                out Vector2 rightUIPos
            );
            rightUIPos.x += uiOffsetX;
            petUIButton.patButtonGroup.GetComponent<RectTransform>().anchoredPosition = rightUIPos;

            //�ܤ�UI
            Vector2 petLeftScreenPos = Camera.main.WorldToScreenPoint(petLeftWorldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                petUIButton.petUIRectTransform,
                petLeftScreenPos,
                null,
                out Vector2 leftUIPos
            );
            leftUIPos.x -= uiOffsetX;
            petUIButton.waterButtonGroup.GetComponent<RectTransform>().anchoredPosition = leftUIPos;

            //������UI
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
