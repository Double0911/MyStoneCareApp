using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{

    public class MenuController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup menuBodyCanvasGroup;  // 選單
        [SerializeField] private CanvasGroup pullBottonCanvasGroup; // 拉選單的按鈕
        [SerializeField] private RectTransform menuBodyRectTransform;
        [SerializeField] private bool isMenuOpen = false; // 記錄選單狀態
        private float fadeDuration = 0.5f; // 漸變時間
        private float moveSpeed = 150f;



        void Start()
        {
            // 初始時確保選單是隱藏的
            menuBodyCanvasGroup.alpha = 0;
            menuBodyCanvasGroup.interactable = false;
            menuBodyCanvasGroup.blocksRaycasts = false;
        }

        public void ToggleMenu()
        {
            StopAllCoroutines();
            StartCoroutine(ControllMenu(isMenuOpen));  //在背景執行 1.製作動畫效果 2.讓事件延遲發生 3.持續執行某個行為
            isMenuOpen = !isMenuOpen;
        }

        IEnumerator ControllMenu(bool isMenuOpen)
        {
            Vector2 targetPosition = isMenuOpen ? new Vector2(0f, -1220f) : new Vector2(0f, -915f);
            StartCoroutine(FadeMenu(isMenuOpen));
            while (Vector2.Distance(menuBodyRectTransform.anchoredPosition, targetPosition) > 0.01f)
            {
                menuBodyRectTransform.anchoredPosition = Vector2.MoveTowards(menuBodyRectTransform.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            yield return null;
        }

        IEnumerator FadeMenu(bool isMenuOpen)
        {
            float startAlpha = menuBodyCanvasGroup.alpha;
            float endAlpha = isMenuOpen ? 0 : 1;
            float timer = 0;

            // 拉menu的按鈕與menu的隱形是相反的
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                menuBodyCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
                pullBottonCanvasGroup.alpha = Mathf.Lerp(endAlpha, startAlpha, timer / fadeDuration);
                yield return null;
            }
            menuBodyCanvasGroup.alpha = endAlpha;
            menuBodyCanvasGroup.interactable = !isMenuOpen;
            menuBodyCanvasGroup.blocksRaycasts = !isMenuOpen;

            pullBottonCanvasGroup.alpha = startAlpha;
            pullBottonCanvasGroup.interactable = isMenuOpen;
            pullBottonCanvasGroup.blocksRaycasts = isMenuOpen;
        }

        public void CloseMenuInstant()
        {
            StopAllCoroutines();
            StartCoroutine(ControllMenu(true));
            menuBodyRectTransform.anchoredPosition = new Vector2(0f, -1220f);
            pullBottonCanvasGroup.alpha = 1f;
            pullBottonCanvasGroup.interactable = true;
            pullBottonCanvasGroup.blocksRaycasts = true;
            isMenuOpen = false;
        }
    }
}