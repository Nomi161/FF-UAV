using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class StormController : MonoBehaviour
{
    [Header("風の設定")]
    Vector3[] windDirection = {  // 風の方向
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
        Vector3.up,
        Vector3.down
    };
    float windStrength = 80f;               // 風の強さ
    float gustInterval = 3f;                // 突風の間隔
    float gustDuration = 1f;                // 突風の持続時間

    private float gustTimer;
    private bool isGusting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        gustTimer += Time.fixedDeltaTime;

        // 突風の生成可
        if (!isGusting & gustTimer >= gustInterval)
        {
            isGusting = true;
            GameManager.isGusting = true;
            gustTimer = 0f;
            StartCoroutine(ApplyGust());
        }
    }

    private IEnumerator ApplyGust()
    {
        float timer = 0f;

        // 突風の持続時間だけ突風を吹かせる
        int idx = Random.Range(0, windDirection.Length);    // 風の方向
        windStrength = Random.Range(1, 8) * 10.0f;          // 風の強さ
        gustDuration = Random.Range(1, 10);                 // 突風の持続時間
        while (timer < gustDuration)
        {
            // 特定の範囲内にいる敵を検知したり、エリア内のオブジェクトに何らかの処理を行いたい場合に使用します。﻿
            Collider[] affectedObjects = Physics.OverlapSphere(transform.position, 20f);
            foreach (Collider col in affectedObjects)
            {
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null)
                {
                    rb.AddForce(windDirection[idx].normalized * windStrength, ForceMode.Force);
                }
            }

            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        gustInterval = Random.Range(1, 10); // 次回突風の間隔
        isGusting = false;
        GameManager.isGusting = false;

    }
}
