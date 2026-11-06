using UnityEngine;
using System.Collections.Generic;

public class GunController : MonoBehaviour, IVisionable
{
    [Header("弾の設定")]
    //弾のプレハブ
    [SerializeField] GameObject bulletPrefab;
    //初速
    [SerializeField] float initSpeed = 30.0f;
    //発射位置
    [SerializeField] Transform shotPoint;
    //減速率
    [SerializeField] float dragPerFrame = 0.98f;
    //下限速度
    [SerializeField] float minSpeed = 4.0f;
    //飛距離を保存
    float bulletLenght = 0.0f;

    [Header("リコイル設定")]
    //銃のモデル
    [SerializeField] Transform gunModel;
    //リコイル角度
    [SerializeField] float recoilAngle = 10.0f;
    //リコイルの戻る速さ
    [SerializeField] float recoverSpeed = 10.0f;
    //元の角度を保存する変数
    Quaternion defaultRotation;

    [Header("照準設定")]
    //ドットのプレハブ
    [SerializeField] GameObject dotPrefab;
    //ドットのインスタンス
    GameObject spawnedSightDot;
    //Bullet以外を対象にするマスクを作成
    [SerializeField] private LayerMask layerMask;

    [Header("レイ設定")]
    //レイの反射回数
    [SerializeField] int maxRayRef = 2;
    //レイ保存用の変数
    LineRenderer lineRenderer;

    [Header("オブジェクトプール設定")]
    //生成する弾数
    [SerializeField] int maxAmmo = 10;
    //現在の弾数を保存する変数
    int currentAmmo;
    //初期の生成数
    int boolSize;
    //弾を保存するリスト
    List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        //レイを取得
        lineRenderer = GetComponent<LineRenderer>();
        //最初はオフに
        lineRenderer.enabled = false;
    }

    void Start()
    {
        //飛距離を計算
        bulletLenght = CaluculateTotalDistance();
        //元の角度を保存
        defaultRotation = gunModel.localRotation;

        //ドットの生成
        spawnedSightDot = Instantiate(dotPrefab);
        //初期は非表示に
        spawnedSightDot.SetActive(false);

        //弾数を初期化
        currentAmmo = maxAmmo;

        //オブジェクトプールの初期化
        boolSize = maxAmmo / 2;
        for(int i = 0;i< boolSize;i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            //オブジェクトを非表示に
            obj.SetActive(false);
            //プールに追加
            pool.Add(obj);
        }
    }

    private void OnEnable()
    {
        PlayerController.OnVisionAction += OnVisionToggle;
    }

    void OnDisable()
    {
        PlayerController.OnVisionAction -= OnVisionToggle;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && currentAmmo != 0)
        {
            Shot();
        }

        //リコイルを戻す処理
        gunModel.localRotation = Quaternion.Slerp(
            gunModel.localRotation,
            defaultRotation,
            Time.deltaTime * recoverSpeed
        );

        UpdateSight();

        //レイが有効な時だけ処理
        if (lineRenderer != null && lineRenderer.enabled)
        {
            UpdatePredictionRay();
        }
    }

    //弾を発射する関数
    void Shot()
    {
        //弾を取得
        GameObject bullet = GetBullet(shotPoint.position, shotPoint.rotation);
        //リキッドボディを取得
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        //弾を前方に飛ばす
        rb.linearVelocity = shotPoint.forward * initSpeed;
        //Bulletのコンポーネントを取得
        Bullet bl = bullet.GetComponent<Bullet>();
        //スピードの初期設定
        bl.SetUp(initSpeed, dragPerFrame, minSpeed);

        //リコイルの処理
        gunModel.localRotation *= Quaternion.Euler(-recoilAngle, 0, 0);

        //弾数を減らす処理
        currentAmmo--;
        UpdateAmmoCountUI();
    }

    //照準のアップデート
    void UpdateSight()
    {
        //Gunから前方にレイを飛ばす
        if (Physics.Raycast(shotPoint.position, shotPoint.forward, out RaycastHit hit, bulletLenght, layerMask))
        {
            //何かに当たればサイトを移動
            spawnedSightDot.SetActive(true);
            //サイトを壁の少し上に移動
            spawnedSightDot.transform.position = hit.point + hit.normal * 0.02f;
            //向きを合わせる
            spawnedSightDot.transform.rotation = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(0,180,0);
        }
        else
        {
            //非表示
            spawnedSightDot.SetActive(false);
        }
    }

    //レイの描画
    void UpdatePredictionRay()
    {
        LineRenderer line = GetComponent<LineRenderer>();
        line.positionCount = 1;
        line.SetPosition(0, shotPoint.position);

        Vector3 currentPos = shotPoint.position;
        Vector3 currentDir = shotPoint.forward;

        for(int i = 0;i < maxRayRef;i++)
        {
            //レイを飛ばす
            if (Physics.Raycast(currentPos, currentDir, out RaycastHit hit, bulletLenght,layerMask))
            {
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, hit.point);

                //法線ベクトルを使って反射ベクトルを計算
                currentDir = Vector3.Reflect(currentDir, hit.normal);
                currentPos = hit.point;
            }
            //何にも当たらなかったら
            else
            {
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, currentPos + currentDir * bulletLenght);
                break;
            }
        }
    }

    //オブジェクトを取得する関数
    GameObject GetBullet(Vector3 position,Quaternion rotation)
    {
        //プールを全選択
        foreach(GameObject obj in pool)
        {
            //非アクティブなオブジェクトを探す
            if (!obj.activeInHierarchy)
            {
                //場所と向きを調整
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                //オブジェクトを有効化
                obj.SetActive(true);
                return obj;
            }
        }

        //数が足りない場合は新規に生成
        GameObject newObj = Instantiate(bulletPrefab, position, rotation,null);
        //プールに追加
        pool.Add(newObj);
        return newObj;
    }

    //IVisionableの実装
    public void OnVisionToggle(bool isVisionMode)
    {
        //透視中だけレイをオンにする
        GetComponent<LineRenderer>().enabled = isVisionMode;
    }

    //初速、減速率、下限速度から射程を計算
    float CaluculateTotalDistance()
    {
        float dt = Time.fixedDeltaTime;
        //減速率が1以上に設定されてるなら無限
        if (dragPerFrame >= 1.0) return float.PositiveInfinity;
        //実際の飛距離を計算
        return dt * ((initSpeed * dragPerFrame) - minSpeed) / (1f - dragPerFrame);
    }

    //弾数UIの更新
    void UpdateAmmoCountUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateAmmo(currentAmmo);
        }
    }
}
