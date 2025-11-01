using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    playing,
    pause,
    option,
    gameover,
    gameclear,
}

public enum Phase
{
    gate,
    enemy,
    boss
}


public class GameManager : MonoBehaviour
{
    //<<< ゲーム変数 >>>
    public static GameState gameState;          // ゲームの状態
    public static Phase missionPhase;
    public static float atitude;                // 高度
    public static float batteryLevel;           // バッテリー残量
    public static int grenadeRemainingNum;      // 消火弾残数
    public static bool isGusting = false;       // 嵐の風発生中

    /// <summary>
    /// 火種の配置を設定する
    /// </summary>
    [SerializeField]
    public GameObject fireSource;           // 火点となるオブジェクト
    private float fireSourceVectX = -5.1f;  // 最初の火点のX座標
    private float fireSourceVectY = 123.8f; // 最初の火点のY座標
    private float fireSourceVectZ = 26.4f;  // 最初の火点のZ座標
    private int fireCountV = 2;             // 火点の高さ方向の数
    private int fireCountH = 3;             // 火点の幅方向の数
    private float fireSpacingH = 3.5f;      // 火点同士の幅方向の間隔(m)
    private float fireSpacingV = 3.0f;      // 火点同士の高さ方向の間隔(m)
    private List<GameObject> fireSourceList = new List<GameObject>();   // 火点となるオブジェクトのリスト   

    [Header("各種設定値")]
    [SerializeField]  public float batteryCapacity = 1000.0f;   // バッテリー容量
    [SerializeField]  public int numberOfGrenadesCarried = 100; // 消火弾搭載数

    float timeLimit;            // 制限時間
    float timeLimitCounter;     // 制限時間カウンタ

    int fsGrenadeCapacity;      // 消火弾搭載量

    float windSpeed;            // 風速
    int windDirction;           // 風向

    int numberOfRoomsOnTheFloor = 6;

    private void Awake()
    {
        //<<< ゲーム変数の初期化 >>>
        gameState = GameState.playing;                  // ゲームの状態
        missionPhase = Phase.gate;
        atitude = 0.0f;                                 // 高度
        batteryLevel = batteryCapacity;                 // バッテリー残量
        grenadeRemainingNum = numberOfGrenadesCarried;  // 消火弾残数
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 火種の配置(とりあえず)
        IgnitionSourcePlacement();

        gameState = GameState.playing;
        //最初にゲートの数を把握
//        numOfGate = GameObject.FindGameObjectsWithTag("Gate").Length;

        //シーン情報の取得
        Scene currentScene = SceneManager.GetActiveScene();
        //シーン名の取得
        string sceneName = currentScene.name;

        switch (sceneName)
        {
            case "BossStage":
                missionPhase = Phase.boss;
//                onBossStage = true;
//                SoundManager.instance.PlayBgm(BGMType.Boss);
                break;
            case "MainStage":
                missionPhase = Phase.gate;
//                onNormalStage = true;
//                SoundManager.instance.PlayBgm(BGMType.Stage);
                break;
            case "Opening":
                //SoundManager.instance.PlayBgm(BGMType.Opening);
                break;
            case "Ending":
                //SoundManager.instance.PlayBgm(BGMType.Ending);
                break;
            case "Title":
                Cursor.lockState = CursorLockMode.None; //画面中心にカーソルのロック解除
                Cursor.visible = true; //カーソルを表示
                //SoundManager.instance.PlayBgm(BGMType.Title);
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //if (numOfGate <= 0 && !onBossStage && onNormalStage)
        //{
        //    missionPhase = Phase.enemy;
        //    if (enemyList.Count <= 0)
        //    {
        //        //すべての敵を殲滅したらボスステージへ
        //        StartCoroutine(StartBossStage());
        //    }
        //}

        if (gameState == GameState.gameclear)
        {

            //ボスを殲滅したらエンディングへ
            StartCoroutine(StartEnding());
        }

    }

    /// <summary>
    /// 火種の配置
    /// </summary>
    void IgnitionSourcePlacement()
    {

        //<<< 火点オブジェクトの作成と配置 >>>
        Vector3 v3 = new Vector3(
                            fireSourceVectX,    // 最初の火点のX座標
                            fireSourceVectY,    // 最初の火点のY座標
                            fireSourceVectZ);   // 最初の火点のZ座標
        int groupNo = 0;
        int localNo = 0;
        bool firstFireSource = false;   // 最初の火種を作成した
        for (int i = 0; i < fireCountV; i++)        // 火点の高さ方向の数
        {
            for (int j = 0; j < fireCountH; j++)    // 火点の幅方向の数
            {
                //flameCont.transform.position = v3;
                //flameCont.SetActive(true);
                // オブジェクトを生成する
                GameObject fireSourceObj = Instantiate(fireSource, v3, Quaternion.identity); // flameオブジェクトを作成する
                // スクリプトのインスタンスを取得する
                FlameController cnt = fireSourceObj.GetComponent<FlameController>();
                // 最初に生成したオブジェクトを火種として点火指定する
                if (!firstFireSource)
                {
                    cnt.initiallyOnFire = true; // 最初から発火している状態(false=未発火/true=発火)
                    firstFireSource = true;     // 最初の火種を作成した
                }
                cnt.groupNo = groupNo;              // 階層別にグループ番号を設定する
                fireSourceObj.SetActive(true);
                // 火点の幅位置の更新
                v3.x += fireSpacingH;      // 火点同士の幅方向の間隔(m);
                Debug.Log($"new flame {v3}");
                fireSourceList.Add(fireSourceObj);   // 火点となるオブジェクトのリスト
                cnt.localNo = localNo;
                localNo++;
            }
            // 火点の高さ位置の更新
            v3.x = fireSourceVectX;     // 最初の火点のX座標
            v3.y += fireSpacingV;       // 火点同士の高さ方向の間隔(m)
            groupNo++;                    // 次の階層を表すグループ番号を更新する
        }
    }


    IEnumerator StartBossStage()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("BossStage");
    }

    IEnumerator StartEnding()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Ending");
    }
}
