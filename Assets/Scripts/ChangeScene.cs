using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;

    public void Load()
    {
        GameManager.batteryLevel = 1000.0f;           // バッテリー残量
        GameManager.grenadeRemainingNum = 100;      // 消火弾残数
        SceneManager.LoadScene(sceneName);
    }
}