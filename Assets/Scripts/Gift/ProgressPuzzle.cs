using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressPuzzle : MonoBehaviour
{
    // Start is called before the first frame update
    // [SerializeField] private RectTransform barFillArea = null;
    [SerializeField] private RectTransform process = null;
    [SerializeField] private Text currentProcess;
    public void Initialize(int curr, int total)
    {
        var localScale = process.localScale;
        float x = (float)curr / (float) total;
        process.localScale = new Vector3(x, localScale.y, localScale.z);

        currentProcess.text = curr + "/" + total;
        // Debug.Log(process.localScale);
        // Debug.Log("Curr: " + curr + "  total: " + total);
    }
}
