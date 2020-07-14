using UnityEngine;

public class UIRoot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    	UnityEngine.Debug.Log(UIPanelType.LoginPanel + "=====UIPanelType.LoginPanel========");
        UIManager.Instance.PushPanel(UIPanelType.LoginPanel);
    }
}
