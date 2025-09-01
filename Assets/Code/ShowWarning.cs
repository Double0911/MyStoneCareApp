using System.Collections;
using UnityEngine;

namespace Assets.Code
{
    public class ShowWarning : MonoBehaviour
    {
        private float fadeDuration = 0.2f; // 漸變時間

        public IEnumerator ShowWaningCanvasGroup()
        {
            yield return StartCoroutine(ShowWarningCanvasGroup(true));

            // 停留
            yield return new WaitForSeconds(1.5f);

            // 淡出
            yield return StartCoroutine(ShowWarningCanvasGroup(false));
        }

        IEnumerator ShowWarningCanvasGroup(bool showWarning)
        {
            CanvasGroup warningCanvasGroup = this.GetComponent<CanvasGroup>();
            float startAlpha = warningCanvasGroup.alpha;
            float endAlpha = showWarning ? 1 : 0;
            float timer = 0;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                warningCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
                yield return null;
            }
            warningCanvasGroup.blocksRaycasts = showWarning;
            warningCanvasGroup.alpha = endAlpha;
        }
    }
}
