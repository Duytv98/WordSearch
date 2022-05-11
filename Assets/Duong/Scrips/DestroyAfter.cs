using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float Time = 1;
    void OnEnable()
    {
        Invoke(nameof(Deactive),Time);
    }

    private void Deactive()
    {
        gameObject.SetActive(false);
    }
}
