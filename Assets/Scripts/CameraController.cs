using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    Vector3 diff;       // ターゲットとの距離の差
    GameObject player;  // ターゲットとなるプレイヤー情報
    Vector3 targetPos;  // ターゲットの位置

    public float followSpeed = 8;   // カメラの補間スピード

    // カメラの初期位置
    public Vector3 defaultPos = new Vector3(0, 6, -10);
    public Vector3 defaultRotate = new Vector3(12, 0, 0);

    float distance = 5.0f;         // 対象の後方距離
    float height = 2.0f;           // カメラの高さ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // カメラを変数で決めた初期位置・角度にする
        transform.position = defaultPos;
        transform.rotation = Quaternion.Euler(defaultRotate);   // RotationはQuaternion型

        // プレイヤー情報の取得
        player = GameObject.FindGameObjectWithTag("Player");
        // プレイヤーの位置を読みだす
        targetPos = player.transform.position;
        Debug.Log($"targetPos={targetPos}");

        // プレイヤーとカメラの距離感を記憶しておく
        diff = player.transform.position - transform.position;
        targetPos.z += 100;
        transform.position = targetPos;
        transform.LookAt(player.transform);
        Debug.Log($"ownPos={transform.position}");
    }

    private void LateUpdate()
    {
        // プレイヤーが見つからなければ何もしない
        if (player == null) return;

        // 対象のY軸回転を取得
        Quaternion targetRotation = Quaternion.Euler(0, player.transform.eulerAngles.y, 0);

        // 後方方向を計算（Y軸回転に基づく）
        Vector3 behindPosition = player.transform.position - (targetRotation * Vector3.forward * distance);
        behindPosition.y += height;

        // カメラ位置をスムーズに更新
        transform.position = Vector3.Lerp(transform.position, behindPosition, Time.deltaTime * followSpeed);

        // 対象を常に見る
        transform.LookAt(player.transform.position + Vector3.up * height * 0.5f);

        //targetPos = player.transform.position;
        //Debug.Log($"targetPos={targetPos}");
        //targetPos.z -= 10;

        //線形補間を使って、カメラを目的の場所に動かす
        //Lerpメソッド（今の位置、ゴールとすべき位置、割合）
        //transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        //transform.LookAt(player.transform);
        //Debug.Log($"ownPos={transform.position}");
    }
    // Update is called once per frame
    void Update()
    {
    }
}
