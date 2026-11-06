using UnityEngine;

public class GoalPost : MonoBehaviour
{
    [SerializeField] int clearSceneIndex = 3;
    bool isCleared = false;

    //何かがエリア内に入ったら
    private void OnTriggerEnter(Collider collision)
    {
        //触れたのがプレイヤーかどうか
        if(!isCleared && collision.CompareTag("Player"))
        {
            isCleared = true;
            //マウスカーソルのロックを解除
            Cursor.lockState = CursorLockMode.None;
            //マウスカーソルを表示
            Cursor.visible = true;

            FadeManager.instance.FadeAndLoadScnee(clearSceneIndex);
        }
    }
}
