using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Code
{
    public class MainMenuButton : MonoBehaviour
    {
        [SerializeField] private GameObject stone;
        [SerializeField] private GameObject mainSceneGameObject;
        [SerializeField] private GameObject raceGameGameObject;
        [SerializeField] private GameObject renameWindow;
        [SerializeField] private GameObject menu;

        public void ResetButtonClicked()
        {
            stone.GetComponent<Stone>().DeletedData();
            menu.GetComponent<MenuController>().CloseMenuInstant();
        }

        public void RaceGameButtonClicked()
        {
            mainSceneGameObject.SetActive(false);
            raceGameGameObject.SetActive(true);
            menu.GetComponent<MenuController>().CloseMenuInstant();
        }

        public void RenameButtonClicked()
        {
            renameWindow.GetComponent<PetRenameUI>().ShowRenameWindow();
            menu.GetComponent<MenuController>().CloseMenuInstant();
        }
    }
}
