using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private bool isPlay = false;
    [SerializeField] private float totalTime;
    private Text textShowTime;
    public float TotalTime { get => totalTime; set => totalTime = value; }
    public bool IsPlay { get => isPlay; set => isPlay = value; }

    // Update is called once per frame
    void Update()
    {
        if (IsPlay)
        {
            totalTime += Time.deltaTime;
            ShowTime();
        }
    }
    public void StartTimer(Text textShowTime)
    {
        totalTime = 0f;
        IsPlay = true;
        this.textShowTime = textShowTime;
    }
    public float StopTimer()
    {
        IsPlay = false;
        this.textShowTime = null;
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
    private void ShowTime()
    {
        this.textShowTime.text = GetTimeString(TotalTime);
    }
}
