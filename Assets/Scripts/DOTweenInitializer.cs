using UnityEngine;
using DG.Tweening;

/// <summary>
/// Initialize DOTween at startup to avoid module errors
/// </summary>
public class DOTweenInitializer : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeDOTween()
    {
        // Force DOTween initialization
        DOTween.Init(false, true, LogBehaviour.Verbose);
        
        Debug.Log("DOTween manually initialized");
    }
}
