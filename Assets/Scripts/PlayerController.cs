using System.Collections;
using UnityEngine;
/// <summary>
/// プレイヤーコントローラー
/// </summary>
public class PlayerController : MonoBehaviour
{
    GameObject gate;    // 消火弾発射ゲート
    // 移動設定
    [System.NonSerialized] public float moveSpeed = 100.0f;
    [System.NonSerialized] public float rotationSpeed = 30f;
    
    bool CanMove = true;
    int[] input = new int[6]; // Forward, Back, Left, Right, Up, Down

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < input.Length; i++)
        {
            input[i] = 0; // Forward, Back, Left, Right, Up, Down
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (CanMove)
        {
            UpdatePosition();
        }
    }

    /// <summary>
    /// 移動処理
    /// キー設定
    ///  W:Throttle(Open)[上昇]
    ///  S:Throttle(Close)[下降]
    ///  A:Yaw(Left)[左旋回]
    ///  D:Yaw(Right)[右旋回]
    ///  U:Pitch(Down)[前進]
    ///  J:Pitch(Up)[後退]
    ///  H:Roll(Left)[左移動]
    ///  K:Roll(Right)[右移動]
    /// </summary>
    void UpdatePosition()
    {
        //// Rotation
        ////if (canRotate)
        ////{
        //Quaternion AddRot = Quaternion.identity;
        //float yaw = 0;
        //float pitch = 0;
        //float roll = 0;
        ////if (CanRotateYaw)
        ////{
        //yaw = Input.GetAxis("Yaw") * (Time.fixedDeltaTime * rotationSpeed);
        //yaw = Input.GetAxis("Yaw") * (Time.fixedDeltaTime * rotationSpeed);
        ////}
        ////if (CanRotatePitch)
        ////{
        //pitch = Input.GetAxis("Pitch") * (Time.fixedDeltaTime * rotationSpeed);
        ////}
        ////if (CanRotateRoll)
        ////{
        //roll = Input.GetAxis("Roll") * (Time.fixedDeltaTime * rotationSpeed);
        ////}
        //AddRot.eulerAngles = new Vector3(-pitch, yaw, -roll);
        //GetComponent<Rigidbody>().rotation *= AddRot;
        //////}
        for (int i = 0; i < input.Length; i++)
        {
            input[i] = 0; // Forward, Back, Left, Right, Up, Down
        }

        // Rotation
        float yaw = 0;
        if (Input.GetKey(KeyCode.A))
        {
            //Debug.Log("KeyCode.A");
            yaw = -1 * (Time.deltaTime * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //Debug.Log("KeyCode.D");
            yaw = 1 * (Time.deltaTime * rotationSpeed);
        }
        Quaternion AddRot = Quaternion.identity;
        AddRot.eulerAngles = new Vector3(0, yaw, 0);
        GetComponent<Rigidbody>().rotation *= AddRot;

        // Translation
        //if (canTranslate)
        //{
        // Check key input
        if (Input.GetKey(KeyCode.U))
        {
            input[0] = 1;   // Forward
        }
        else if (Input.GetKey(KeyCode.J))
        {
            input[1] = 1;   // Back
        }

        if (Input.GetKey(KeyCode.H))
        {
            input[2] = 1;   // Left
        }
        else if (Input.GetKey(KeyCode.K))
        {
            input[3] = 1;   // Right
        }

        if (Input.GetKey(KeyCode.W))
        {
            input[4] = 1;   // Up
            //Debug.Log("KeyCode.W");
        }
        else if (Input.GetKey(KeyCode.S))
        {
            input[5] = 1;   // Down
            //Debug.Log("KeyCode.S");
        }

        int numInput = 0;
        for (int i = 0; i < input.Length; i++)
        {
            numInput += input[i];
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        // Add velocity to the gameobject
        float curSpeed = numInput > 0 ? moveSpeed : 0;
        Vector3 AddPos = input[0] * Vector3.forward + input[2] * Vector3.left + input[4] * Vector3.up
            + input[1] * Vector3.back + input[3] * Vector3.right + input[5] * Vector3.down;
        AddPos = GetComponent<Rigidbody>().rotation * AddPos;
        GetComponent<Rigidbody>().linearVelocity = AddPos * (Time.fixedDeltaTime * curSpeed);
    }


}
