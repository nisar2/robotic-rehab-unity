using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
[DisallowMultipleComponent]
public class Page : MonoBehaviour
{
    public bool ExitOnNewPagePush = false;
    [SerializeField]
    private UnityEvent PrePushAction;
    [SerializeField]
    private UnityEvent PostPushAction;
    [SerializeField]
    private UnityEvent PrePopAction;
    [SerializeField]
    private UnityEvent PostPopAction;


    public void Enter()
    {
        PrePushAction?.Invoke();
        gameObject.SetActive(true);
    }

    public void Exit()
    {
		PrePopAction?.Invoke();
        gameObject.SetActive(false);
    }
}
