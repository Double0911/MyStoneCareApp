using UnityEngine;
using TMPro;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public class PetRenameUI : MonoBehaviour
    {
        [SerializeField] private GameObject renameWindow;
        [SerializeField] private GameObject warningPanel;
        [SerializeField] private TMP_InputField nameInputField; //輸入框 
        [SerializeField] private TextMeshProUGUI warningText;
        public Stone stoneScript;

        void Start()
        {
            warningPanel.GetComponent<CanvasGroup>().alpha = 0;
            warningPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void ShowRenameWindow()
        {
            renameWindow.SetActive(true);
            warningText.text = "";
            nameInputField.text = "";
            warningPanel.GetComponent<CanvasGroup>().alpha = 0;
            warningPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void HideRenameWindow()
        {
            renameWindow.SetActive(false);
        }

        public void ApplyName()
        {
            string input = nameInputField.text.Trim();
            int length = input.Trim().Length;

            if (length < 2)
            {
                warningText.text = "名字過短，最少2個字";
                StartCoroutine(warningPanel.GetComponent<ShowWarning>().ShowWaningCanvasGroup());
                return;
            }
            if (length > 6)
            {
                warningText.text = "名字過長，最多6個字";
                StartCoroutine(warningPanel.GetComponent<ShowWarning>().ShowWaningCanvasGroup());
                return;
            }

            stoneScript.petNameText.text = input;
            PlayerPrefs.SetString("PetName", input);
            PlayerPrefs.Save();
            HideRenameWindow(); // 成功就關閉視窗
        }

    }
}