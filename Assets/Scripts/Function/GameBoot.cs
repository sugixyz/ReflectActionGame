using UnityEngine;

public class GameBoot : MonoBehaviour
{
    private void Awake()
    {
        //起動時一回だけ実行
        Application.targetFrameRate = 60;

        //オブジェクトが消えないように
        DontDestroyOnLoad(gameObject);
    }
}
