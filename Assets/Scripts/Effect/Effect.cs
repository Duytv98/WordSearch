using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Effect : MonoBehaviour
{
    [SerializeField] Transform rocket = null;
    [SerializeField] ParticleSystem rocketExplosion = null;
    public void PlayRocket(float time)
    {
        rocket.gameObject.SetActive(true);
        rocket.DOMoveY(2.1f, time)
        .SetEase(Ease.InCubic)
        .OnComplete(() =>
        {
            rocketExplosion.Play();
            rocket.gameObject.SetActive(false);
            rocket.localPosition = Vector3.zero;

        });
    }
}
