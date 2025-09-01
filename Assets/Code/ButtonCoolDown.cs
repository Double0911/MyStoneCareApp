using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class ButtonCooldown : MonoBehaviour
    {
        [SerializeField] private Button targetButton;
        [SerializeField] private Image cooldownMask;
        [SerializeField] private TextMeshProUGUI cooldownText;

        public bool isCoolingDown = false;
        private float cooldownTimer = 0f;

        void Start()
        {
            cooldownText.gameObject.SetActive(false);
            cooldownMask.fillAmount = 0f;
        }

        public IEnumerator StartCooldown(float cooldownDuration, bool canInteractable)
        {
            if (isCoolingDown) yield break;
            if (!canInteractable)
            {
                targetButton.interactable = false;
            }
            StartCoroutine(CooldownMaskRoutine(cooldownDuration, canInteractable));
            StartCoroutine(ShowCoolDownTimeText(cooldownDuration));
        }

        public void StopCooldown()
        {
            if (isCoolingDown)
            {
                StopAllCoroutines();
                cooldownMask.fillAmount = 0f;
                cooldownText.gameObject.SetActive(false);
                isCoolingDown = false;
                targetButton.interactable = true;
            }
        }

        private IEnumerator ShowCoolDownTimeText(float cooldownDuration)
        {
            float timeLeft = cooldownDuration;
            cooldownText.gameObject.SetActive(true);

            while (timeLeft > 0f)
            {
                timeLeft -= Time.deltaTime;
                int minutes = Mathf.FloorToInt(timeLeft / 60f);
                int seconds = Mathf.FloorToInt(timeLeft % 60f);
                cooldownText.text = $"{minutes:00}:{seconds:00}";

                yield return null;
            }

            cooldownText.gameObject.SetActive(false);
        }

        private IEnumerator CooldownMaskRoutine(float cooldownDuration, bool canInteractable)
        {
            cooldownTimer = cooldownDuration;
            isCoolingDown = true;

            while (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                float ratio = cooldownTimer / cooldownDuration;
                cooldownMask.fillAmount = ratio;
                yield return null;
            }

            cooldownMask.fillAmount = 0f;
            isCoolingDown = false;
            targetButton.interactable = true;
        }


        

    }
}
