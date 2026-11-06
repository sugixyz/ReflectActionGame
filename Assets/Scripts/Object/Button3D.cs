using System.Collections;
using UnityEngine;

public class Button3D : MovableEntity, IHitReceiver,IReflectable,IEasingMovable
{
    [Header("基本設定")]
    //起動時の色
    [SerializeField] Color activeColor = Color.green;
    //移動距離
    [SerializeField] float triggerPositionOffset = 1.0f;

    [Header("連動設定")]
    //動かす対象の壁
    [SerializeField] GameObject[] targetObjects;
    //壁が動くまでの遅延
    [SerializeField] float DelayDuration = 0.0f;

    //初期の色を保存する変数
    Color defaultColor;
    //マテリアルを保存する変数
    Renderer myRenderer;
    //ターゲットの壁を保存する変数
    IEasingMovable targetMove;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        defaultColor = myRenderer.material.color;
        defaultPos = transform.position;
        Vector3 pushDirection = -transform.up;
        triggerPos = defaultPos + pushDirection * triggerPositionOffset;
        targetMove = null;
    }

    //プレイヤーに触れた時の処理
    private void OnCollisionEnter(Collision collision)
    {
        //触れたのがプレイヤーか
        if (collision.gameObject.CompareTag("Player"))
        {
            OnHit();
        }
    }

    //IHitReceiverの実装
    public void OnHit()
    {
        //ボタンを動かす
        Move();
    }

    //IEasingMovableの実装
    public void Move()
    {
        //コルーチンが動いてないなら移動する
        if(currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(ButtonSequence());
            //壁を動かすまでのディレイ
            StartCoroutine(MoveDeray());
        }
    }

    //壁を動かすまでのディレイ
    private IEnumerator MoveDeray()
    {
        yield return new WaitForSeconds(DelayDuration);
        foreach( GameObject wall in targetObjects)
        {
            if(wall != null)
            {
                targetMove = wall.GetComponent<IEasingMovable>();
                targetMove.Move();
            }
        }
    }

    //ボタンの動作時間管理用の処理
    private IEnumerator ButtonSequence()
    {
        //色を変える
        myRenderer.material.color = activeColor;
        //へこませる
        yield return TriggerMove(true);

        //待機
        yield return new WaitForSeconds(waitDuration);

        //戻す
        yield return TriggerMove(false);
        //色を戻す
        myRenderer.material.color = defaultColor;

        currentCoroutine = null;
    }
}
