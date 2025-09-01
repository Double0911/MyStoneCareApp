using System.Collections;
using UnityEngine;

public class StoneWander : MonoBehaviour
{
    private float moveSpeed = 2f; // 寵物移動速度
    private Vector2 maxRange = new Vector2(0.7f, 3f);
    private Vector2 minRange = new Vector2(-0.8f, -2f);
    private Vector2 targetPosition; // 目標位置

    private void OnEnable()
    {
        ChooseNewTarget();
        StartCoroutine(MoveAndWait());
    }

    IEnumerator MoveAndWait()
    {
        while (true)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            // 檢查是否已經到達目標點
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                yield return new WaitForSeconds(Random.Range(1f, 3f)); // 停頓 1 秒
                ChooseNewTarget(); // 換下一個目標
            }

            yield return null; // 等待下一幀再檢查
        }
    }

    private void ChooseNewTarget()
    {
        // 設定新目標，確保在範圍內
        float randomX = Random.Range(minRange.x, maxRange.x);
        float randomY = Random.Range(minRange.y, maxRange.y);
        targetPosition = new Vector2(randomX, randomY);
    }


}

