using System;
using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject gate;             // 消火弾発射ゲート
    public GameObject fsGrenade;        // 消火弾プレハブ
    public GameObject aimPoint;         // エイムポイント

    public float maxDistance = 100f;    // Rayの最大距離
    float grenadeSpeed = 150f;          // 弾速
    bool hitState = false;              // Rayが何かに触れているか(false=非接触/true=接触)
    bool coolingState = false;          // クーリングオフ状態

    // ターゲットポイントを格納する変数
    private Vector3 targetPoint;
    int myLayerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Shot());
        myLayerMask = ~(1 << 9);
        Debug.Log($"LayerMask={Convert.ToString(myLayerMask, 2)}");
    }

    // Update is called once per frame
    void Update()
    {

        // Raycastの結果を格納する変数
        RaycastHit hit;

        // Raycastを実行
        // Physics.Raycast(Vector3 origin(rayの開始地点), Vector3 direction(rayの向き),RaycastHit hitInfo(当たったオブジェクトの情報を格納), float distance(rayの発射距離), int layerMask(レイヤマスクの設定));
        if (Physics.Raycast(gate.transform.position, gate.transform.forward, out hit, maxDistance))
        {
            // 何かに衝突した場合、その衝突地点がターゲットポイント
            targetPoint = hit.point;

            // エイムポイントを表示する
            Vector3 aimVecter3 = new Vector3(targetPoint.x, targetPoint.y, targetPoint.z -1.0f);
            aimPoint.SetActive(true);                           // オブジェクトをアクティブにする
            aimPoint.transform.position = aimVecter3;            // 当たった場所に移動させる
            aimPoint.transform.LookAt(Camera.main.transform);   // カメラの方向を向かせる

            if (!hitState)
            {
                hitState = true;
                Debug.Log("Ray hits");
            }

            // デバッグ表示(Rayの軌跡を線で表示する。Rayが何かに触れている時は緑)
            Debug.DrawLine(gate.transform.position, hit.point, Color.green);
            if (!coolingState)
            {
                Debug.Log("call Shot()");
                coolingState = true;
            }
        }
        else
        {
            // エイムポイントを非表示にする
            aimPoint.SetActive(false);

            // 何にも衝突しなかった場合、レイの最大距離分だけ進んだ地点がターゲットポイント
            targetPoint = gate.transform.position + gate.transform.forward * maxDistance;

            if (hitState)
            {
                hitState = false;
                Debug.Log("Ray miss");
            }

            // デバッグ表示(Rayの軌跡を線で表示する。レイが何にも触れていないときは赤)
            Debug.DrawLine(gate.transform.position, targetPoint, Color.red);
        }

        // ここで aimPoint を使って照準の位置を更新したり、弾を飛ばしたりする処理を行う
        // 例: Debug.Log("ターゲットポイント: " + targetPoint);

    }

    /// <summary>
    /// ショットコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator Shot()
    {
        while (true)
        {

            if (Input.GetButtonDown("Jump"))
            {
                Debug.Log("Shot()");
                //        Transform gate = transform.Find("Gate");
                // 消火弾を作成する
                GameObject grenade = Instantiate(fsGrenade, gate.transform.position, Quaternion.identity); // 新しい回転を適用
                //               Debug.Log("grenade");
                // 弾のRigidbodyを読みだす
                               Rigidbody grenadeRbody = grenade.GetComponent<Rigidbody>();
                               grenadeRbody.useGravity = false;  // 光線兵器なら重力の影響をOFFにする
                               grenadeRbody.position = gate.transform.position;

                // SEの再生
                //           SEPlay(SEType.Shot);

                                grenadeRbody.AddForce(gate.transform.forward * grenadeSpeed, ForceMode.Impulse);  // ゲートの正面から弾を撃ちだす
                //                grenadeRbody.AddForce((gate.transform.position - gate.transform.position).normalized * grenadeSpeed, ForceMode.Impulse);  // プレイヤーに向けて弾を撃ちだす

            }
            yield return null;

        }
    }

}
