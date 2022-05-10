using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{

    [SerializeField] private RectTransform barFillArea = null;
    [SerializeField] private RectTransform bar = null;
    [SerializeField] private float minSize = 60;

    private bool setOnUpdate;
    private float setProgress;


    private void Update()
    {
        if (setOnUpdate)
        {
            StartCoroutine(SetNextFrame(setProgress));
            setOnUpdate = false;
        }
    }



    public void SetProgress(float progress)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(SetNextFrame(progress));
        }
        else
        {
            setOnUpdate = true;
            setProgress = progress;
        }
    }

    private IEnumerator SetNextFrame(float progress)
    {
        yield return new WaitForEndOfFrame();

        bar.sizeDelta = new Vector2(GetBarWidth(progress), bar.sizeDelta.y);
    }

    public void SetProgressAnimated(float fromProgress, float toProgress, float animDuration, float startDelay)
    {

        float fromBarWidth = GetBarWidth(fromProgress);
        float toBarWidth = GetBarWidth(toProgress);

        bar.sizeDelta = new Vector2(fromBarWidth, bar.sizeDelta.y);

    }

    private float GetBarWidth(float progress)
    {
        float fillWidth = barFillArea.rect.width - minSize;

        return minSize + fillWidth * progress;
    }

}
