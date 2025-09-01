using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Code.Class
{
    public class RaceGameClass
    {
        [Serializable]
        public class RaceGameUI
        {
            public TextMeshProUGUI timeCountdown;
            public TextMeshProUGUI petName;
            public TextMeshProUGUI resultText;
        }

        [Serializable]
        public class RaceGameObject
        {
            [SerializeField] public GameObject mainGroupGameObject;
            [SerializeField] public GameObject raceGameGameObject;
            [SerializeField] public GameObject petStoneRunner;
            [SerializeField] public GameObject petStone;
            [SerializeField] public GameObject supportButtonPanel;
            [SerializeField] public GameObject endingWindowPanel;
            [SerializeField] public GameObject clickHintPanel;
            [SerializeField] public GameObject timeCountdownPanel;
            [SerializeField] public GameObject closeButton;
            [HideInInspector] public GameObject[] allRunnersGameObject;
            [HideInInspector] public List<GameObject> runnerRankingList;
            [HideInInspector] public List<GameObject> runnersList;
        }

        public class RaceGameFinish
        {
            public int finishRunnerCount = 0;
            public float finishLine = 3.7f;
        }
    }
}
