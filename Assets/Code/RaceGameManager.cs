using System.Collections;
using System.Collections.Generic;
using Assets.Code.Class;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static Assets.Code.Class.RaceGameClass;

namespace Assets.Code
{
    public class RaceGameManager : MonoBehaviour
    {
        [SerializeField] public RaceGameObject raceGameObject;
        [SerializeField] public RaceGameUI raceGameUI;
        [SerializeField] public RaceGameFinish raceGameFinish;

        [SerializeField] private SpriteRenderer mainGroupPetStoneSpriteRenderer;
        [SerializeField] private float StartingPoint = -4f;
        [SerializeField] private float originalSpeed = 0f;
        private float supportCoolDownTime = 5f;
        private bool gameStarted = false;
        private Dictionary<int, int> rankToExp;
        private Dictionary<GameObject, Coroutine> runnerCoroutines = new Dictionary<GameObject, Coroutine>();
        private Coroutine footPrintRoutine;

        void Start()
        {
            raceGameFinish = new RaceGameFinish();
            raceGameObject.allRunnersGameObject = GameObject.FindGameObjectsWithTag("Runner"); // 自動抓Tag是Runner的物件
            originalSpeed = raceGameObject.petStoneRunner.GetComponent<RaceGameRunner>().petSpeed;
            raceGameObject.runnerRankingList = new List<GameObject>();
        }

        void OnEnable()
        {
            gameStarted = false;
            raceGameObject.timeCountdownPanel.SetActive(false);
            raceGameObject.endingWindowPanel.SetActive(false);
            raceGameObject.supportButtonPanel.SetActive(false);
            raceGameObject.closeButton.SetActive(true);
            raceGameObject.petStoneRunner.GetComponent<SpriteRenderer>().sprite = mainGroupPetStoneSpriteRenderer.sprite;
            raceGameObject.petStoneRunner.GetComponent<RaceGameRunner>().runnerName = raceGameUI.petName.text;
        }

        void Update()
        {
            if (gameStarted == false)
            {
                CheckClickScreen();
                return;
            }

            if (gameStarted)
            {
                CheckFinishLine();
                if (raceGameFinish.finishRunnerCount == raceGameObject.allRunnersGameObject.Length)
                {
                    ShowEndingWindow();
                }
            }
        }

        private void CheckClickScreen()
        {
            // 1. 滑鼠檢查
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return; // 滑鼠點到 UI，不處理

                raceGameObject.clickHintPanel.SetActive(false);
                raceGameObject.closeButton.SetActive(false);
                gameStarted = true;
                StartCoroutine(StartRace());
                return;
            }

            // 2. 觸控檢查（手機）
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return; // 觸控點到 UI，不處理

                raceGameObject.clickHintPanel.SetActive(false);
                raceGameObject.closeButton.SetActive(false);
                gameStarted = true;
                StartCoroutine(StartRace());
                return;
            }
        }

        public void ClickCancelButton()
        {
            raceGameObject.mainGroupGameObject.SetActive(true);
            raceGameObject.raceGameGameObject.SetActive(false);
        }

        private IEnumerator WaitForThreeSeconds()
        {
            // 等待三秒鐘
            float waitTime = 3f;
            float elapsedTime = 0f;
            int time = 3;
            raceGameObject.timeCountdownPanel.SetActive(true);
            while (elapsedTime < waitTime)
            {
                raceGameUI.timeCountdown.text = $"{time}";
                Debug.Log($"倒數計時：{time}秒");

                yield return new WaitForSeconds(1f); // 等 1 秒

                time--; // 每秒減一
                elapsedTime += 1f;
            }
            raceGameObject.timeCountdownPanel.SetActive(false);
        }

        private void CheckFinishLine()
        {
            for (int i = raceGameObject.runnersList.Count - 1; i >= 0; i--)
            {
                GameObject runner = raceGameObject.runnersList[i];

                if (runner.transform.position.y >= raceGameFinish.finishLine)
                {
                    if (runner.GetComponent<RaceGameRunner>().haveFootPrint)
                    {
                        StopCoroutine(runnerCoroutines[runner]);
                        StopCoroutine(footPrintRoutine);
                    }
                    else
                    {
                        StopCoroutine(runnerCoroutines[runner]);
                    }
                    raceGameObject.runnersList.RemoveAt(i);
                    raceGameFinish.finishRunnerCount++;
                    raceGameObject.runnerRankingList.Add(runner);
                }
            }
        }

        private IEnumerator StartRace()
        {
            Coroutine routine;
            raceGameObject.runnersList = new List<GameObject>(raceGameObject.allRunnersGameObject);
            foreach (GameObject runner in raceGameObject.allRunnersGameObject)
            {
                if (runner.GetComponent<RaceGameRunner>().haveFootPrint)
                {
                    routine = StartCoroutine(runner.GetComponent<RaceGameRunner>().StartRunningWithFootPrint());
                    footPrintRoutine = StartCoroutine(runner.GetComponent<RaceGameRunner>().CreateFootPrint());
                }
                else if (runner.name.Contains("Stone"))
                {
                    routine = StartCoroutine(runner.GetComponent<RaceGameRunner>().PetStoneRunning());
                }
                else
                {
                    routine = StartCoroutine(runner.GetComponent<RaceGameRunner>().StartRunning());
                }
                runnerCoroutines[runner] = routine;
            }

            yield return StartCoroutine(WaitForThreeSeconds());

            foreach (GameObject runner in raceGameObject.allRunnersGameObject)
            {
                runner.GetComponent<RaceGameRunner>().isStartSignalGiven = true;
            }
            raceGameObject.supportButtonPanel.SetActive(true);
        }

        public void SpeedUpButton()
        {
            float chance = Random.value; // 會得到 0.0 ~ 1.0 的隨機值

            if (chance < 0.3f) // 30% 機率停下來
            {
                StartCoroutine(PauseRunning());
                Debug.Log("😵 停下來了！");
            }
            else // 剩下 70%
            {
                StartCoroutine(BoostSpeed()); // 你的加速邏輯
                Debug.Log("🚀 加速成功！");
            }
            StartCoroutine(raceGameObject.supportButtonPanel.GetComponent<ButtonCooldown>().StartCooldown(supportCoolDownTime, false));
        }

        private IEnumerator PauseRunning()
        {
            float randomTime = Random.Range(1f, 2f);
            raceGameObject.petStoneRunner.GetComponent<RaceGameRunner>().petSpeed = 0f;

            Debug.Log($"😴 停止跑步 {randomTime} 秒");
            yield return new WaitForSeconds(randomTime);

            raceGameObject.petStoneRunner.GetComponent<RaceGameRunner>().petSpeed = originalSpeed;
            Debug.Log("PauseRunning結束");
        }

        private IEnumerator BoostSpeed()
        {
            float randomTime = Random.Range(0.5f, 2f);
            float randomSpeed = Random.Range(0.4f, 1.5f);
            raceGameObject.petStoneRunner.GetComponent<RaceGameRunner>().petSpeed = randomSpeed;

            Debug.Log($"開始加速 {randomTime} 秒");
            yield return new WaitForSeconds(randomTime);

            raceGameObject.petStoneRunner.GetComponent<RaceGameRunner>().petSpeed = originalSpeed;
            Debug.Log("BoostSpeed結束");
        }

        private void ShowEndingWindow()
        {
            ShowRaceResult();
            raceGameObject.endingWindowPanel.SetActive(true);
            gameStarted = false;

        }

        private void ShowRaceResult()
        {
            string result = "";
            for (int i = 0; i < raceGameObject.runnerRankingList.Count; i++)
            {
                string name = raceGameObject.runnerRankingList[i].GetComponent<RaceGameRunner>().runnerName;
                result += $"{i + 1} 名 - {name}\n";
                if (raceGameObject.runnerRankingList[i].GetComponent<RaceGameRunner>().isPetStone)
                {
                    raceGameObject.petStone.GetComponent<Stone>().growthValue += GetExperience(i);
                    raceGameObject.petStone.GetComponent<Stone>().OnExpGained();
                    Debug.Log("獲取經驗" + GetExperience(i));
                }
            }

            raceGameUI.resultText.text = result;
        }

        public void ResetRaceGame()
        {
            gameStarted = false;
            raceGameObject.endingWindowPanel.SetActive(false);
            raceGameObject.supportButtonPanel.SetActive(false);
            raceGameObject.clickHintPanel.SetActive(true);
            raceGameUI.resultText.text = "";
            raceGameObject.runnerRankingList.Clear();
            raceGameFinish.finishRunnerCount = 0;
            foreach (GameObject runner in raceGameObject.allRunnersGameObject)
            {
                runner.transform.position = new Vector2(runner.transform.position.x, StartingPoint);
                runner.GetComponent<RaceGameRunner>().isStartSignalGiven = false;
            }
        }

        public void CloseRaceGame()
        {
            gameStarted = false;
            ResetRaceGame();

            raceGameObject.raceGameGameObject.SetActive(false);
            raceGameObject.mainGroupGameObject.SetActive(true);
        }

        private int GetExperience(int rank)
        {
            rankToExp = new Dictionary<int, int>()
        {
            { 0, 50 },
            { 1, 30 },
            { 2, 15 },
            { 3, 5 }
        };

            if (rankToExp.TryGetValue(rank, out int exp))
                return exp;
            else
                return 0;
        }

    }
}
