using System;
using UnityEngine;

public class OnBurgerBunDestroyed : MonoBehaviour
{
    private Action onDestroyAction;

    // Init method instead of constructor
    public void Init(Action _onDestroyAction)
    {

        onDestroyAction = _onDestroyAction;
    }

    void OnDestroy()
    {
        AudioManager.iAudioManager.PlayDeathWhistle();
        onDestroyAction?.Invoke();
    }
}