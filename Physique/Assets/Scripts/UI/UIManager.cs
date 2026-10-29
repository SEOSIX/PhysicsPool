using UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private UIGame _gameUI;


    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _gameUI = GetComponent<UIGame>();
    }
}
