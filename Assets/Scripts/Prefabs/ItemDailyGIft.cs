using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDailyGIft : MonoBehaviour
{
    [SerializeField] private Image background = null;
    [SerializeField] private Image backgroundHeader = null;
    [SerializeField] private TextMeshProUGUI textDay = null;
    [SerializeField] private TextMeshProUGUI countdownTime = null;
    [SerializeField] private Image contentIcon = null;
    [SerializeField] private Image check = null;
    private string strGift;

    public void SetName(string text)
    {
        textDay.text = text;
        check.gameObject.SetActive(false);
        countdownTime.gameObject.SetActive(false);

    }
    public void SetDayPassed(Sprite bg, Sprite bgHeader, Sprite icon, Sprite spCheck, Color color)
    {

        background.sprite = bg;
        backgroundHeader.sprite = bgHeader;
        contentIcon.sprite = icon;
        check.sprite = spCheck;

        background.SetNativeSize();
        backgroundHeader.SetNativeSize();
        contentIcon.SetNativeSize();
        check.gameObject.SetActive(true);
        check.SetNativeSize();
        SetAImage(contentIcon, 0.34f);
        textDay.color = color;
    }
    public void SetGiftNext(Sprite bg, Sprite bgHeader, Sprite icon, string time, Color color)
    {
        background.sprite = bg;
        backgroundHeader.sprite = bgHeader;
        contentIcon.sprite = icon;

        background.SetNativeSize();
        backgroundHeader.SetNativeSize();
        contentIcon.SetNativeSize();
        SetAImage(contentIcon, 1f);
        check.gameObject.SetActive(false);
        countdownTime.gameObject.SetActive(true);
        countdownTime.text = time;
        textDay.color = color;
    }
    public void SetTodayCollected(Sprite bg, Sprite bgHeader, Sprite spCheck, Color color)
    {

        background.sprite = bg;
        backgroundHeader.sprite = bgHeader;
        check.sprite = spCheck;

        background.SetNativeSize();
        backgroundHeader.SetNativeSize();
        check.SetNativeSize();
        contentIcon.gameObject.SetActive(false);
        countdownTime.gameObject.SetActive(false);
        check.gameObject.SetActive(true);
        textDay.color = color;
    }
    public void SetGiftFuture(Sprite bg, Sprite bgHeader, Sprite icon, Color color)
    {
        background.sprite = bg;
        backgroundHeader.sprite = bgHeader;
        contentIcon.sprite = icon;

        background.SetNativeSize();
        backgroundHeader.SetNativeSize();
        contentIcon.SetNativeSize();
        SetAImage(contentIcon, 1f);
        check.gameObject.SetActive(false);
        textDay.color = color;
    }

    private Sprite GetGiftSprite()
    {
        return null;
    }
    private void SetAImage(Image image, float a)
    {
        image.gameObject.SetActive(true);
        var color = contentIcon.color;
        contentIcon.color = new UnityEngine.Color(color.r, color.g, color.b, a);
    }
    public void OnClick()
    {
        DailyGift.Instance.OnClick(gameObject.name);
    }
}
