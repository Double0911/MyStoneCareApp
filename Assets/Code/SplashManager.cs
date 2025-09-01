using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Code
{
    public class SplashManager : MonoBehaviour
    {
        void Start()
        {
            // 開始時可以放一些初始化代碼
            Debug.Log("Splash Screen Initialized");
        }
        void Update()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0))
            {
                // 進入主遊戲
                SceneManager.LoadScene("MainScene");
            }
        }
    }

}
