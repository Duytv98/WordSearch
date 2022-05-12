using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Effect : MonoBehaviour
{
    [SerializeField] Transform rocket = null;
    [SerializeField] ParticleSystem rocketExplosion = null;
    public void PlayRocket()
    {
        rocket.gameObject.SetActive(true);
        rocket.DOMoveY(2.1f, 1f)
        .SetEase(Ease.InCubic)
        .OnComplete(() =>
        {
            rocketExplosion.Play();
            rocket.gameObject.SetActive(false);
            rocket.localPosition = Vector3.zero;

        });
    }
}
