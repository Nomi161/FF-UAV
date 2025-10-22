using UnityEngine;

enum FLASH_STATE
{
    None,           // 非着火段階(初期値、炎のエフェクトは見えない状態)
    Spark,          // 火花、小さな点火。炎はほぼ見えない。
    Flicker,        // ゆらめく炎。炎の長さは約60cm未満。
    Flame,          // 安定した炎。炎の長さは約1m前後。
    Blaze,          // 激しく燃える炎。炎の長さは1m以上、広範囲に広がる。
    Inferno         // 制御不能な大火災。建物全体が燃えるレベル。
}


/// <summary>
/// 炎の表現コントローラ
/// </summary>
public class FlameController : MonoBehaviour
{
    public GameObject flamePrefab; // 炎のプレハブ

    FLASH_STATE flashState = FLASH_STATE.None;  // 炎の強度状態

    // 出火状態に移行する各時間[s]
    float sparkTime = 10.0f;          // 火花、小さな点火。炎はほぼ見えない。
    float flickerTime = 10.0f;        // ゆらめく炎。炎の長さは約60cm未満。
    float flameTime = 10.0f;          // 安定した炎。炎の長さは約1m前後。
    float blazeTime = 10.0f;          // 激しく燃える炎。炎の長さは1m以上、広範囲に広がる。
    float infernoTime = 10.0f;        // 制御不能な大火災。建物全体が燃えるレベル。

    float timer = 0f;               // 状態更新タイマー
    bool ignition = false;          // 発火状態(false=未発火/true=発火)
    GameObject flame;               // 炎

    // 出火状態に移行する各時間カウンタ[s]
    float cntIgnition = 10.0f;     // 着火火段階(火災が発生した最初の瞬間。小規模な炎が発生し、燃焼が局所的に始まる。消火器などで容易に鎮火可能な段階。)
    float cntGrowth = 10.0f;       // 成長段階(炎が拡大し、周囲の可燃物に燃え移る。室内温度が急上昇し、煙が天井付近に溜まり始める。)
    float cntFlashover = 10.0f;    // フラッシュオーバー（Flashover）に至る可能性がある危険な段階。
                                   //    int flashCount = 0;
                                   // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // オブジェクトの取得
        flame = Instantiate(flamePrefab, transform.position, Quaternion.identity);
        //       flame.SetActive(false);
        flame.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        ignition = true;      // 発火状態(false=未発火/true=発火)
        Debug.Log("flame ignition");
    }

    // Update is called once per frame
    void Update()
    {
        if (flame != null && ignition && flashState != FLASH_STATE.Inferno)      // 発火状態(false=未発火/true=発火)
        {
            timer += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (flame != null && ignition)      // 発火状態(false=未発火/true=発火)
        {
            // 出火状態に移行する各時間カウンタ[s]
            switch (flashState)  // 炎の強度状態
            {
                case FLASH_STATE.None:                   // 非着火段階(初期値、炎のエフェクトは見えない状態)
                    if (sparkTime <= timer)
                    {
                        flame.SetActive(true);
                        timer = 0.0f;
                        flashState = FLASH_STATE.Spark;  // 炎の強度状態
                        Debug.Log("FLASH_STATE.Spark");
                    }
                    break;
                case FLASH_STATE.Spark:          // 火花、小さな点火。炎はほぼ見えない。
                    if (flickerTime <= timer)
                    {
                        flame.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        timer = 0.0f;
                        flashState = FLASH_STATE.Flicker;  // 炎の強度状態
                        Debug.Log("FLASH_STATE.Flicker");
                    }
                    break;
                case FLASH_STATE.Flicker:        // ゆらめく炎。炎の長さは約60cm未満。
                    if (flameTime <= timer)
                    {
                        flame.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                        timer = 0.0f;
                        flashState = FLASH_STATE.Flame;  // 炎の強度状態
                        Debug.Log("FLASH_STATE.Flame");
                    }
                    break;
                case FLASH_STATE.Flame:          // 安定した炎。炎の長さは約1m前後。
                    if (blazeTime <= timer)
                    {
                        flame.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        timer = 0.0f;
                        flashState = FLASH_STATE.Blaze;  // 炎の強度状態
                        Debug.Log("FLASH_STATE.Blaze");
                    }
                    break;
                case FLASH_STATE.Blaze:          // 激しく燃える炎。炎の長さは1m以上、広範囲に広がる。
                    if (infernoTime <= timer)
                    {
                        flame.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                        timer = 0.0f;
                        flashState = FLASH_STATE.Inferno;  // 炎の強度状態
                        Debug.Log("FLASH_STATE.Inferno");
                    }
                    break;
                case FLASH_STATE.Inferno:        // 制御不能な大火災。建物全体が燃えるレベル。
                    if (infernoTime <= timer)
                    {
                        flame.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
                        timer = 0.0f;
                        flashState = FLASH_STATE.Inferno;  // 炎の強度状態
                    }
                    break;
                default:
                    break;
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
                    Debug.Log("Down FLASH_STATE.None");
                    break;
                case FLASH_STATE.Flicker:        // ゆらめく炎。炎の長さは約60cm未満。
                    flashState = FLASH_STATE.Spark;
                    timer = 0.0f;
                    flame.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    Debug.Log("Down FLASH_STATE.Spark");
                    break;
                case FLASH_STATE.Flame:          // 安定した炎。炎の長さは約1m前後。
                    flashState = FLASH_STATE.Flicker;
                    timer = 0.0f;
                    flame.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    Debug.Log("Down FLASH_STATE.Flame");
                    break;
                case FLASH_STATE.Blaze:          // 激しく燃える炎。炎の長さは1m以上、広範囲に広がる。
                    flashState = FLASH_STATE.Flame;
                    timer = 0.0f;
                    flame.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    Debug.Log("Down FLASH_STATE.Blaze");
                    break;
                case FLASH_STATE.Inferno:        // 制御不能な大火災。建物全体が燃えるレベル。
                    flashState = FLASH_STATE.Blaze;
                    timer = 0.0f;
                    flame.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                    Debug.Log("Down FLASH_STATE.Inferno");
                    break;
                default:
                    break;
            }
        }
    }

}

