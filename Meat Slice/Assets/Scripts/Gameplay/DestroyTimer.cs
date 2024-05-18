using System;
using System.Collections;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public Action destroyAction;

    public IEnumerator StartTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        destroyAction();
        Destroy(gameObject);
    }
}
