using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static Unity.Collections.Unicode;

namespace Assets.Code
{
    public class RaceGameRunner : MonoBehaviour
    {
        [SerializeField] private float currentSpeed;
        [SerializeField] private GameObject footprintPrefab;  // 拖入預製物
        [SerializeField] private Transform haveTrailRunnerTransform;  // 寵物的位置
        [SerializeField] private Transform trailParent;   // 放痕跡的 UI 範圍（可選）
        public float petSpeed = 0.5f;
        public string runnerName;
        public bool isStartSignalGiven = false;
        public bool isPetStone;
        public bool haveFootPrint;

        public IEnumerator PetStoneRunning()
        {
            while (!isStartSignalGiven) yield return null;

            while (true)
            {
                transform.Translate(Vector2.up * petSpeed * Time.deltaTime);
                yield return null;
            }
        }

        public IEnumerator StartRunning()
        {
            while (!isStartSignalGiven) yield return null;

            List<SpeedChange> speedPlans = GenerateRandomSpeedPlan();
            foreach (var plan in speedPlans)
            {
                if (!plan.applied)
                {
                    currentSpeed = plan.speed;
                    plan.applied = true;

                    float elapsed = 0f;
                    //跑步
                    while (elapsed < plan.time)
                    {
                        transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                }
            }
        }

        //走路會留下痕跡的方法
        public IEnumerator StartRunningWithFootPrint()
        {
            while (!isStartSignalGiven) yield return null;

            List<SpeedChange> speedPlans = GenerateRandomSpeedPlan();
            foreach (var plan in speedPlans)
            {
                if (!plan.applied)
                {
                    currentSpeed = plan.speed;
                    plan.applied = true;

                    float elapsed = 0f;
                    //跑步
                    while (elapsed < plan.time)
                    {
                        transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }
                }
            }
        }


        public IEnumerator CreateFootPrint()
        {
            while (!isStartSignalGiven) yield return null;

            GameObject trail = Instantiate(footprintPrefab, trailParent);
            trail.transform.position = haveTrailRunnerTransform.position;
            Destroy(trail, 2f);
            int count = 1;
            while (true)
            {
                trail = Instantiate(footprintPrefab, trailParent);
                trail.transform.position = haveTrailRunnerTransform.position;

                // 2 秒後自動刪除
                StartCoroutine(FadeAndDestroy(trail, 3f));
                count++;
                yield return new WaitForSeconds(0.8f);
            }
        }

        IEnumerator FadeAndDestroy(GameObject trail, float duration)
        {
            SpriteRenderer sr = trail.GetComponent<SpriteRenderer>();
            Color originalColor = sr.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(trail);
        }

        private List<SpeedChange> GenerateRandomSpeedPlan()
        {
            int count = 10;
            List<SpeedChange> speedPlan = new List<SpeedChange>();

            for (int i = 0; i < count; i++)
            {
                float lastingTime = Random.Range(1f, 3f); // 此速度持續時間
                float newSpeed = Random.Range(0.1f, 1f); // 新速度
                speedPlan.Add(new SpeedChange(lastingTime, newSpeed));
            }
            speedPlan.Add(new SpeedChange(50f, 0.5f)); //確保一定能跑到終點

            return speedPlan;
        }

        class SpeedChange
        {
            public float time;
            public float speed;
            public bool applied;

            public SpeedChange(float time, float speed)
            {
                this.time = time;
                this.speed = speed;
                this.applied = false;
            }
        }



    }
}
