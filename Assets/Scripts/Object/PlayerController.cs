using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("基本設定")]
    //移動速度
    [SerializeField] float moveSpeed = 5.0f;
    //ジャンプ力
    [SerializeField] float jumpPower = 5.0f;
    //HP
    [SerializeField] int maxHp = 100;
    //現在のHPを保存する変数
    int currentHp;
    //エネルギー
    [SerializeField] float maxEnergy = 100.0f;
    //現在のエネルギーを保存する変数
    float currentEnergy;
    //受けるダメージ
    [SerializeField] int damage = 25;
    //リキッドボディを取得する変数
    Rigidbody rb;
    //透視モード起動用のイベント
    public static event Action<bool> OnVisionAction;
    //透視モード中かどうか
    bool isPerspective = false;

    [Header("カメラ設定")]
    //マウス感度
    [SerializeField] float mouseSensitivity = 2.0f;
    //カメラの上下回転制限
    //上限
    [SerializeField] float upAngleLimit = 60.0f;
    //下限
    [SerializeField] float lowAngleLimit = -45.0f;
    //現在のカメラ角度を保存する変数
    float currentCameraAngle = 0.0f;
    //カメラを取得する変数
    Transform cameraTransform;

    [Header("地面判定設定")]
    //地面に触れているか
    bool isGrounded;
    //足元へのずれ
    Vector3 groundCheckOffset = new Vector3(0, -1.0f, 0);
    //判定のサイズ
    [SerializeField] Vector3 groundCheckSize = new Vector3(0.5f, 0.1f, 0.5f);
    //地面判定のレイヤー
    [SerializeField] LayerMask groundLayer;

    void Start()
    {
        //HPを設定
        currentHp = maxHp;
        UpdateHpUI();
        //エネルギーを設定
        currentEnergy = maxEnergy;
        UpdateEnergyUI();

        //カメラの取得
        cameraTransform = GetComponentInChildren<Camera>().transform;
        //カメラを目線に移動
        cameraTransform.localPosition = new Vector3(0, 0.8f, 0);

        //リキッドボディの取得
        rb = GetComponent<Rigidbody>();

        //マウスカーソルのロック
        Cursor.lockState = CursorLockMode.Locked;
        //マウスカーソルを非表示に
        Cursor.visible = false;
    }

    void Update()
    {
        Rotation();
        Move();
        Perspective();
        CheckGround();

        //スペースでジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        //透視モードのエネルギー管理
        if (isPerspective && currentEnergy > 0)
        {
            ConsumeEnergy();
        }
        else if (!isPerspective && currentEnergy < maxEnergy)
        {
            RegenerateEnergy();
        }
        UpdateEnergyUI();

        //Escでマウスカーソルのロック解除
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    //視点移動
    void Rotation()
    {
        //左右の回転
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        //Y軸回転
        transform.Rotate(0, mouseX, 0);

        //上下の回転
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentCameraAngle -= mouseY;
        //カメラの回転制限
        currentCameraAngle = Mathf.Clamp(currentCameraAngle, lowAngleLimit, upAngleLimit);

        //カメラの角度を更新
        cameraTransform.localRotation = Quaternion.Euler(currentCameraAngle, 0, 0);
    }
    
    //移動処理
    void Move()
    {
        //入力の取得
        //X軸方向　A/D
        float x = Input.GetAxis("Horizontal");
        //Z軸方向　W/S
        float z = Input.GetAxis("Vertical");

        //移動ベクトル
        Vector3 moveVelocity = (transform.right * x) + (transform.forward * z);

        //速度設定
        rb.linearVelocity = new Vector3(moveVelocity.x * moveSpeed, rb.linearVelocity.y, moveVelocity.z * moveSpeed);
    }

    //ジャンプ
    void Jump()
    {
        //上方向の力を加える
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        isGrounded = false;
    }

    //エネルギーの消費処理
    void ConsumeEnergy()
    {
        //1秒間で20消費
        currentEnergy -= Time.deltaTime * 20;
        //制限をかける
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    //エネルギーの回復処理
    void RegenerateEnergy()
    {
        //1秒間で10回復
        currentEnergy += Time.deltaTime * 10;
        //制限をかける
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
    }

    //透視
    void Perspective()
    {
        //右クリックを押したとき
        if(Input.GetMouseButtonDown(1))
        {
            //イベント発行
            //イベント予約がなければスルー
            //透視
            OnVisionAction?.Invoke(true);
            isPerspective = true;
        }

        //右クリックを離したとき、もしくはエネルギーがなくなったら
        if(Input.GetMouseButtonUp(1) || currentEnergy <= 0)
        {
            //イベント発行
            //イベント予約がなければスルー
            //通常視点
            OnVisionAction?.Invoke(false);
            isPerspective = false;
        }
    }

    //足元に地面があるかどうか
    private void CheckGround()
    {
        //地面の判定
        isGrounded = Physics.CheckBox(
            transform.position + groundCheckOffset,
            groundCheckSize / 2,
            Quaternion.identity,
            groundLayer
        );
    }

    //敵に触れた時の処理
    private void OnTriggerEnter(Collider other)
    {
        //当たったのが敵か判定
        if (other.CompareTag("Enemy"))
        {
            //ダメージを受ける
            TakeDamage(damage);
        }
    }

    //IDamageableの実装
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        //制限をかける
        currentHp = Math.Clamp(currentHp, 0, maxHp);

        UpdateHpUI();

        //ゲームオーバー処理
        if(currentHp <= 0)
        {
            FadeManager.instance.FadeAndLoadScnee(2);
            //マウスカーソルのロックを解除
            Cursor.lockState = CursorLockMode.None;
            //マウスカーソルを表示
            Cursor.visible = true;
        }
    }

    //HpUIの更新
    void UpdateHpUI()
    {
        if(UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHP(currentHp, maxHp);
        }
    }

    //エネルギーUIの更新
    void UpdateEnergyUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateEnergy(currentEnergy, maxEnergy);
        }
    }
}
