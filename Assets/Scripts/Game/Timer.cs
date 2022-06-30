using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private bool isPlay = false;
    [SerializeField] private float totalTime;
    public float TotalTime { get => totalTime; set => totalTime = value; }

    // Update is called once per frame
    void Update()
    {
        if (isPlay)
        {
            totalTime += Time.deltaTime;
            Debug.Log(GetCurrentTime());
        }
    }
    public void StartTimer()
    {
        totalTime = 0f;
        isPlay = true;
    }
    public float StopTimer()
    {
        isPlay = false;
        return TotalTime;
    }
    public string GetCurrentTime()
    {
        return GetTimeString(TotalTime);

    }
    private string GetTimeString(float TotalTime)
    {
        TimeSpan time = TimeSpan.FromSeconds(TotalTime);
        if (TotalTime >= 3600) return time.ToString(@"hh\:mm\:ss");
        return time.ToString(@"mm\:ss");
    }
}
