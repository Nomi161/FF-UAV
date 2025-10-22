using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 消火弾コントローラー
/// </summary>
public class FireSuppressionGrenadeControlle : MonoBehaviour
{
    FLASH_STATE state = FLASH_STATE.None;   // 火炎の強度(初期値)
    public GameObject flamePrefab;          // 炎のプレハブ
    [System.NonSerialized] public float deleteTime = 5.0f;  // グレネードが削除されるまでの時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Start()
    {
        Destroy(gameObject, deleteTime);

    }

    public void Update()
    {

    }

    /// <summary>
    /// 接触判定
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision={collision.gameObject.name}");
        Destroy(gameObject);
    }
}
