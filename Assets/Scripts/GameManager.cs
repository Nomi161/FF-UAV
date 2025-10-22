using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] flameList;
    static public bool isGusting = false;

    int numberOfRoomsOnTheFloor = 6;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 v3 = new Vector3(20, 10, -20);
        foreach (var flameCont in flameList)
        {
            //flameCont.transform.position = v3;
            //flameCont.SetActive(true);
            GameObject bullet = Instantiate(flameCont, v3,Quaternion.identity); // 新しい回転を適用
            v3.x += 10;
            Debug.Log($"new flame {v3}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
