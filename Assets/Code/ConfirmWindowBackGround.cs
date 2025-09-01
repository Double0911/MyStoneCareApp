using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public class ConfirmWindowBackGround : MonoBehaviour, IPointerClickHandler
    {
        public GameObject windowPanel;  // 整個視窗物件組，包含內容跟背景

        // 點擊背景區域（視窗以外）會呼叫這裡
        public void OnPointerClick(PointerEventData eventData)
        {
            // 點擊了背景區域（非視窗本體）
            CloseWindow();
        }

        public void CloseWindow()
        {
            windowPanel.SetActive(false);
        }
    }
}
