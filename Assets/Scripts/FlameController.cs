using System.Collections;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 炎の勢い
/// </summary>
public enum FLASH_STATE
{
    None,           // 非着火段階(初期値、炎のエフェクトは見えない状態)
    Spark,          // 火花、小さな点火。炎はほぼ見えない。
    Flicker,        // ゆらめく炎。炎の長さは約60cm未満。
    Flame,          // 安定した炎。炎の長さは約1m前後。
    Blaze,          // 激しく燃える炎。炎の長さは1m以上、広範囲に広がる。
    Inferno         // 制御不能な大火災。建物全体が燃えるレベル。
}

/// <summary>
/// 延焼方向
/// </summary>
public enum FIRE_SPREAD_DIRECTION
{
    Horizon,    // 水平方向
    Vertical    // 垂直方向
}

/// <summary>
/// 炎の表現コントローラ
/// </summary>
public class FlameController : MonoBehaviour
{
    public int no;
    public bool initiallyOnFire = false;       // 最初から発火している状態(false=未発火/true=発火)
    public GameObject flameEffect;              // 炎のプレハブ

    [System.NonSerialized] public float fireSpreadAreaH = 3.5f;        // 幅方向延焼範囲設定(m)
    [System.NonSerialized] public float fireSpreadAreaV = 3.0f;        // 高さ方向延焼範囲設定(m)
    public FLASH_STATE fireSpradLevel = FLASH_STATE.Blaze; // 延焼を開始する炎の状態
    public int fireSpreadMax = 1;               // 延焼最大数
    public int groupNo = 0;                     // グループ番号(階層の設定等に使用する)
    public int localNo = 0;                     // グループ内のローカル番号
    [SerializeField]

    [System.NonSerialized]
    public FLASH_STATE flashState = FLASH_STATE.None;  // 炎の強度状態
    public bool ignition = false;            // 発火状態(false=未発火/true=発火)
    [SerializeField]

    // 出火状態に移行する各時間(秒)
    float sparkTime = 1.0f;          // 火花、小さな点火。炎はほぼ見えない。
    float flickerTime = 10.0f;        // ゆらめく炎。炎の長さは約60cm未満。
    float flameTime = 10.0f;          // 安定した炎。炎の長さは約1m前後。
    float blazeTime = 10.0f;          // 激しく燃える炎。炎の長さは1m以上、広範囲に広がる。
    float infernoTime = 10.0f;        // 制御不能な大火災。建物全体が燃えるレベル。

    float timer = 0f;                 // 状態更新タイマー
    GameObject flame;                 // 炎

    // 出火状態に移行する各時間カウンタ(秒)
    float cntIgnition = 10.0f;      // 着火段階(火災が発生した最初の瞬間。小規模な炎が発生し、燃焼が局所的に始まる。消火器などで容易に鎮火可能な段階。)
    float cntGrowth = 10.0f;        // 成長段階(炎が拡大し、周囲の可燃物に燃え移る。室内温度が急上昇し、煙が天井付近に溜まり始める。)
    float cntFlashover = 10.0f;     // フラッシュオーバー（Flashover）に至る可能性がある危険な段階。
                                    //    int flashCount = 0;

    int fireSpradCount = 0;         // 延焼回数
    protected bool adjacentFireSpred = false;  // 類焼の有無(false=類焼無し/true=類焼有り) 

    protected string localName = "DefaultFlame";    // Flameのインスタンスを識別する名前

    /// <summary>
    /// 発火状態の読み出し
    /// </summary>
    /// <returns></returns>
    public bool GetIgnition()
    {
        return ignition;
    }

    /// <summary>
    ///  発火させる
    /// </summary>
    /// <returns></returns>
    public void StartIgnition()
    {
        Debug.Log($"StartIgnition() groupNo={groupNo} localNo={localNo} localName={localName} flameEffect={flameEffect}");
        // 炎のプレハブが生成されていなければ生成する
        if (flame == null)
        {
            if (flame == null) Debug.Log($"groupNo={groupNo} localNo={localNo} localName={localName} Create flame");

            // 炎プレハブオブジェクトの取得(自身の位置に移動する)
            flame = Instantiate(flameEffect, transform.position, Quaternion.identity);
            flame.SetActive(true);
            Debug.Log($"myPos={transform.position} flamePos={flame.transform.position}");
            //Vector3 flamePos = new Vector3(transform.position.x - 1.5f, transform.position.y, transform.position.z);
            //flame.transform.position = flamePos;
            flame.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        if (!ignition)
        {
            // 発火する
            if (flame == null) Debug.Log($"groupNo={groupNo} localNo={localNo} localName={localName} adjacentFireSpred");
            //        flame.SetActive(true);
            adjacentFireSpred = true;  // 類焼の有無(false=類焼無し/true=類焼有り) 
            ignition = true;
        }

    }

    private void Awake()
    {
        //flame = Instantiate(flamePrefab, transform.position, Quaternion.identity);
        ////       flame.SetActive(false);
        //flame.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Flameのインスタンスを識別する名前
        localName = "Flame " + Random.Range(0.0f, 5.0f).ToString();
        Debug.Log($"groupNo={groupNo} localNo={localNo} LocalName = {localName} initiallyOnFire ={initiallyOnFire} ignition={ignition}");

        // InspectorでInitiallyOnFireパラメータがONならば、最初から発火状態にする
        if (initiallyOnFire && !ignition)
        {
            // 発火させる
            StartIgnition();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (flame != null && ignition)      // 発火状態(false=未発火/true=発火)
        {
            timer += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (flame != null && ignition)      // 発火状態(false=未発火/true=発火)
        {
            //            Debug.Log($"localName = {localName} FixedUpdate() flame={flame} ignition={ignition}");
            int x = 0;
            if (flame == null)
            {
                x = 1;
            }
            if (flame != null && ignition)      // 発火状態(false=未発火/true=発火)
            {
                // 出火状態に移行する各時間カウンタ[s]
                switch (flashState)  // 炎の強度状態
                {
                    case FLASH_STATE.None:                   // 非着火段階(初期値、炎のエフェクトは見えない状態)
                        if (sparkTime <= timer)
                        {
                            //flame.SetActive(true);
                            flame.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
//                            Debug.Log($"groupNo={groupNo} localNo={localNo} localName={localName} sparkTime={sparkTime} timer={timer} FLASH_STATE.Spark");
                            timer = 0.0f;
                            flashState = FLASH_STATE.Spark;  // 炎の強度状態
                        }
                        break;
                    case FLASH_STATE.Spark:          // 火花、小さな点火。炎はほぼ見えない。
                        /// 延焼判定処理
                        /// 自身が発火状態ならば周囲の火種に延焼を広げる
//                        FirePropagationAssessment();
                        if (flickerTime <= timer)
                        {
                            flame.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
//                            Debug.Log($"lgroupNo={groupNo} localNo={localNo} ocalName={localName} flickerTime={flickerTime} timer={timer} FLASH_STATE.Flicker");
                            timer = 0.0f;
                            flashState = FLASH_STATE.Flicker;  // 炎の強度状態
                        }
                        break;
                    case FLASH_STATE.Flicker:        // ゆらめく炎。炎の長さは約60cm未満。
                        /// 延焼判定処理
                        /// 自身が発火状態ならば周囲の火種に延焼を広げる
//                        FirePropagationAssessment();
                        if (flameTime <= timer)
                        {
                            flame.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
//                            Debug.Log($"groupNo={groupNo} localNo={localNo} localName={localName} flameTime={flameTime} timer={timer} FLASH_STATE.Flame");
                            timer = 0.0f;
                            flashState = FLASH_STATE.Flame;  // 炎の強度状態
                        }
                        break;
                    case FLASH_STATE.Flame:          // 安定した炎。炎の長さは約1m前後。
                        /// 延焼判定処理
                        /// 自身が発火状態ならば周囲の火種に延焼を広げる
//                        FirePropagationAssessment();
                        if (blazeTime <= timer)
                        {
                            flame.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
//                            Debug.Log($"groupNo={groupNo} localNo={localNo} localName={localName} blazeTime={blazeTime} timer={timer} FLASH_STATE.Blaze");
                            timer = 0.0f;
                            flashState = FLASH_STATE.Blaze;  // 炎の強度状態
                        }
                        break;
                    case FLASH_STATE.Blaze:          // 激しく燃える炎。炎の長さは1m以上、広範囲に広がる。
                        /// 延焼判定処理
                        /// 自身が発火状態ならば周囲の火種に延焼を広げる
                        FirePropagationAssessment(FIRE_SPREAD_DIRECTION.Horizon);
                        if (infernoTime <= timer)
                        {
                            flame.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
//                            Debug.Log($"groupNo={groupNo} localNo={localNo} localName={localName} infernoTime={infernoTime} timer={timer} FLASH_STATE.Inferno");
                            timer = 0.0f;
                            flashState = FLASH_STATE.Inferno;  // 炎の強度状態
                        }
                        break;
                    case FLASH_STATE.Inferno:        // 制御不能な大火災。建物全体が燃えるレベル。
                        /// 延焼判定処理
                        /// 自身が発火状態ならば周囲の火種に延焼を広げる
                        FirePropagationAssessment(FIRE_SPREAD_DIRECTION.Vertical);
                        if (infernoTime <= timer)
                        {
                            flame.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                            timer = 0.0f;
                            flashState = FLASH_STATE.Inferno;  // 炎の強度状態
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 延焼判定処理
    /// 自身が発火状態ならば周囲の火種に延焼を広げる
    ///<paramref name="direction">延焼方向</paramref>
    /// </summary>
    void FirePropagationAssessment(FIRE_SPREAD_DIRECTION direction)
    {
        if (fireSpradCount >= fireSpreadMax)         // 延焼回数
        {
            return;
        }

        //<<< 延焼判定 >>>
        if (this.ignition)            // 発火状態(false=未発火/true=発火)
        {
            // タグに"Flame"が設定されているオブジェクトを検索する
            GameObject[] flames = GameObject.FindGameObjectsWithTag("Flame");
            //                Debug.Log($"flames={flames.Length}");
            int i = 0;
            // 取得したオブジェクトの数だけ処理を行う
            foreach (GameObject flameObj in flames)
            {
                bool ignition = false;  // 発火対象の有無

                //                Debug.Log($"no ={i} col.gameObject.GetComponent<FlameController>()");
                // 対象オブジェクトの"FlameController"スクリプトを取得する
                FlameController flameCnt = flameObj.GetComponent<FlameController>();

                // 自身も検出されるので、自身は除外する
                string lname = flameCnt.localName;
                //                Debug.Log($"LocalName2 = {flameCnt.localName}");
                // 対象は自分自身では無い？
                if (!localName.Equals(lname))
                {
                    // 相手が既に発火状態ならば処理を行わない
                    if (flameCnt.GetIgnition())            // 発火状態(false=未発火/true=発火))
                    {
                        continue;
                    }

                    //<<< 対象との距離と角度を求める >>>
                    float targetDistance = Vector3.Distance(transform.position, flameObj.transform.position);
                    Vector3 to = flameObj.transform.position - transform.position;
                    float angleX = Vector3.SignedAngle(transform.forward, to, Vector3.right);   // X軸を基準に角度を求める( 符号付き角度（-180° ～ 180°）)
                    float angleY = Vector3.SignedAngle(transform.forward, to, Vector3.up);      // Y軸を基準に角度を求める( 符号付き角度（-180° ～ 180°）)
                    float angleZ = Vector3.SignedAngle(transform.forward, to, Vector3.forward); // Z軸を基準に角度を求める( 符号付き角度（-180° ～ 180°）)
                    Debug.Log($"groupNo={flameCnt.groupNo} localNo={flameCnt.localNo} distance={targetDistance} x°={angleX} y°={angleY} z°={angleZ}");

                    //<<< 各距離に対して発火対象であるか確認する >>>
                    switch (direction)
                    {
                        case FIRE_SPREAD_DIRECTION.Horizon:    // 水平方向
                            if ((fireSpreadAreaH >= targetDistance) && (angleX == 90) && (angleY == 90) && (angleZ == 90))
                            {
                                ignition = true;  // 発火対象の有無
                            }
                            break;
                        case FIRE_SPREAD_DIRECTION.Vertical:   // 垂直方向
                            if ((fireSpreadAreaV >= targetDistance) && (angleX == -90) && (angleY == 90) && (angleZ == 90))
                            {
                                ignition = true;  // 発火対象の有無
                            }
                            break;
                    }
                    if (ignition)  // 発火対象の有無
                    {
                        // 発火させる
                        flameCnt.StartIgnition();
                    }


                }
            }
        }
    }

    /// <summary>
    /// コリジョン・イベントハンドラ
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FSGrenade"))
        {
            Debug.Log("FSGreanade Hit");
            // 出火状態に移行する各時間カウンタ[s]
            switch (flashState)  // 炎の強度状態
            {
                case FLASH_STATE.None:           // 非着火段階(初期値、炎のエフェクトは見えない状態)
                    break;
                case FLASH_STATE.Spark:          // 火花、小さな点火。炎はほぼ見えない。
                    flashState = FLASH_STATE.None;
                    timer = 0.0f;
                    flame.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
                    Debug.Log($"groupNo={groupNo} localNo={localNo} Down FLASH_STATE.None");
                    break;
                case FLASH_STATE.Flicker:        // ゆらめく炎。炎の長さは約60cm未満。
                    flashState = FLASH_STATE.Spark;
                    timer = 0.0f;
                    flame.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Debug.Log($"groupNo={groupNo} localNo={localNo} Down FLASH_STATE.Spark");
                    break;
                case FLASH_STATE.Flame:          // 安定した炎。炎の長さは約1m前後。
                    flashState = FLASH_STATE.Flicker;
                    timer = 0.0f;
                    flame.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    Debug.Log($"groupNo={groupNo} localNo={localNo} Down FLASH_STATE.Flame");
                    break;
                case FLASH_STATE.Blaze:          // 激しく燃える炎。炎の長さは1m以上、広範囲に広がる。
                    flashState = FLASH_STATE.Flame;
                    timer = 0.0f;
                    flame.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    Debug.Log($"groupNo={groupNo} localNo={localNo} Down FLASH_STATE.Blaze");
                    break;
                case FLASH_STATE.Inferno:        // 制御不能な大火災。建物全体が燃えるレベル。
                    flashState = FLASH_STATE.Blaze;
                    timer = 0.0f;
                    flame.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                    Debug.Log($"groupNo={groupNo} localNo={localNo} Down FLASH_STATE.Inferno");
                    break;
                default:
                    break;
            }
        }
    }

}

