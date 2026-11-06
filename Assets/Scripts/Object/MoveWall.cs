using UnityEngine;
using System.Collections;

public class MoveWall : MovableEntity,IEasingMovable
{
    //壁の動きの種類
    public enum MoveMode
    {
        TOGGLE,
        ONCE,
        AUTO_RETURN
    }

    [Header("動作設定")]
    //ムーブモードを保存する変数
    [SerializeField] MoveMode mode = MoveMode.TOGGLE;
    //移動距離
    [SerializeField] Vector3 positionOffset;
    
    
    void Start()
    {
        defaultPos = transform.position;
        triggerPos = defaultPos + positionOffset;
    }

    //IEasingMovableの実装
    public void Move()
    {
        //コルーチンが動いてないなら処理を進める
        if (currentCoroutine == null)
        {
            switch (mode)
            {
                case MoveMode.ONCE:
                    //起動していないときに１度だけ起動
                    if (!isTrigger)
                    {
                        currentCoroutine = StartCoroutine(MoveSequence(true));
                    }
                    break;
                case MoveMode.TOGGLE:
                    //反転させて起動
                    currentCoroutine = StartCoroutine(MoveSequence(!isTrigger));
                    break;
                case MoveMode.AUTO_RETURN:
                    currentCoroutine = StartCoroutine(AutoReturnSequence());
                    break;
            }
        }
    }

    //動作時間管理用の処理
    private IEnumerator MoveSequence(bool targetState)
    {
        //移動させる
        yield return TriggerMove(targetState);

        currentCoroutine = null;
    }

    //自動動作用の時間管理用の処理
    private IEnumerator AutoReturnSequence()
    {
        //移動させる
        yield return TriggerMove(true);

        //待機
        yield return new WaitForSeconds(waitDuration);

        //戻す
        yield return TriggerMove(false);

        currentCoroutine = null;
    }

    //接触処理
    private void OnCollisionEnter(Collision collision)
    {
        //接触したのがプレイヤーなら
        if(collision.gameObject.CompareTag("Player"))
        {
            //プレイヤーを自分の子にする
            collision.transform.SetParent(transform);
        }
    }

    //接触終わったら
    private void OnCollisionExit(Collision collision)
    {
        //プレイヤーが離れたら
        if(collision.gameObject.CompareTag("Player"))
        {
            //親子関係を解除
            collision.transform.SetParent(null);
        }
    }
}
