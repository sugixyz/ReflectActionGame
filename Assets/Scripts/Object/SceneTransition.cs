using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    //îÚÇŒÇµÇΩÇ¢ÉVÅ[Éìî‘çÜ
    [SerializeField] private int targetSceneIndex;

    void Start()
    {
        Button bt = GetComponent<Button>();
        if(bt != null)
        {
            bt.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        if (FadeManager.instance != null)
        {
            FadeManager.instance.FadeAndLoadScnee(targetSceneIndex);
        }
    }
}
