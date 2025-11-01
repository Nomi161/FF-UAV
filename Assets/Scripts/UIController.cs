using TMPro;
using Unity.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // 高度
    public TextMeshProUGUI atitudeValue;

    // バッテリー残量
    public TextMeshProUGUI batteryValue;
    public Slider batterySlider;

    // 消火弾残数
    public TextMeshProUGUI grenadeValue;
    public Slider grenadeSlider;

    // ミッション表示
    public TalkData missions;
    public GameObject missionPanel;
    public TextMeshProUGUI missionText;

    float currentAtitude;               // 現在の高度
    float currentBatteryLevel;          // 現在のバッテリー残量
    int currentGrenadeRemainingNum;     // 現在の消火弾残数

    // 情報表示パネル
    public GameObject atitudePanel;     // 高度
    public GameObject batteryPanel;     // バッテリー
    public GameObject grenadePanel;     // 消火弾
    public GameObject controllerPanel;  // 操作
    public GameObject cursorPanel;      // カーソル(エイムポイント)

    public GameObject gameOverPanel;    // GAME OVER

    bool isGameOverPanel;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //<<< 各コントロールの表示の初期化 >>>
        // 高度
        currentAtitude = transform.position.y;
        float  valAti = currentAtitude;
        atitudeValue.text = valAti.ToString();

        // バッテリー残量
        currentBatteryLevel = GameManager.batteryLevel;
        int val = (int)currentBatteryLevel;
        batteryValue.text = val.ToString();
        batterySlider.maxValue = val;
        batterySlider.value = currentBatteryLevel;
        Debug.Log($"batterySlider.maxValue={batterySlider.maxValue}");

        // 消火弾残数
        currentGrenadeRemainingNum = GameManager.grenadeRemainingNum;      // 消火弾残数
;       grenadeValue.text = currentGrenadeRemainingNum.ToString();
        grenadeSlider.maxValue = currentGrenadeRemainingNum;
        grenadeSlider.value = currentGrenadeRemainingNum;
        Debug.Log($"grenadeSlider.maxValue={grenadeSlider.maxValue}");

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState == GameState.gameover)
        {
            if (!isGameOverPanel)
            {
                isGameOverPanel = true;

                batteryPanel.SetActive(false);
                grenadePanel.SetActive(false);

                controllerPanel.SetActive(false);
                cursorPanel.SetActive(false);
                missionPanel.SetActive(false);

                gameOverPanel.SetActive(true);
            }
            return;
        }

        // 高度を更新する
        if (currentAtitude != GameManager.atitude)
        {
            currentAtitude = GameManager.atitude;
            float val = currentAtitude;
            atitudeValue.text = val.ToString("F1");
        }

        // バッテリー残量を更新する
        if (currentBatteryLevel != GameManager.batteryLevel)
        {
            currentBatteryLevel = GameManager.batteryLevel;
            int val = (int)currentBatteryLevel;
            batteryValue.text = val.ToString();
            batterySlider.value = currentBatteryLevel;
//!!!            Debug.Log($"currentBatteryLevel={currentBatteryLevel}");
        }

        // 消火弾残量を更新する
        if (currentGrenadeRemainingNum != GameManager.grenadeRemainingNum)
        {
            currentGrenadeRemainingNum = GameManager.grenadeRemainingNum;
            grenadeValue.text = currentGrenadeRemainingNum.ToString();
            grenadeSlider.value = currentGrenadeRemainingNum;
        }

        switch (GameManager.missionPhase)
        {
            case Phase.gate:
                missionText.text = missions.talkDatas[0].talk;
                break;
            case Phase.enemy:
                missionText.text = missions.talkDatas[1].talk;
                break;
            case Phase.boss:
                missionText.text = missions.talkDatas[2].talk;
                break;
        }

    }
}
