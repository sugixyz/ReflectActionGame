using UnityEngine;
using System.Collections;

public abstract class MovableEntity : MonoBehaviour
{
    [Header("移動の共通設定")]
    //移動にかかる時間
    [SerializeField] protected float moveDuration;
    //元に戻るまでの時間
    [SerializeField] protected float waitDuration;
    //イージングの制御
    [SerializeField] protected AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    //初期の位置
    protected Vector3 defaultPos;
    //起動時の位置
    protected Vector3 triggerPos;
    //起動しているかどうか
    protected bool isTrigger = false;
    //現在のコルーチンを取得する変数
    protected Coroutine currentCoroutine = null;

    //共通のイージング処理
    protected IEnumerator TriggerMove(bool targetState)
    {
        isTrigger = targetState;
        Vector3 from = transform.position;
        //trueなら起動位置へ、falseなら初期位置へ
        Vector3 to = isTrigger ? triggerPos : defaultPos;
        float time = moveDuration;
        float elapsed = 0.0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = easeCurve.Evaluate(elapsed / time);
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
        transform.position = to;
    }
}
