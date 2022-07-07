using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GiftsFast : MonoBehaviour
{
    [System.Serializable]
    private class ItemProgress
    {
        public string id;
        public GameObject textDay;
        public Image image;
    };

    private Booter dailyGiftBooter = null;
    private string idCollect = null;

    [SerializeField] private ItemProgress[] listItemProgress = null;
    public void OnShowing(Tuple<string, Booter> tuple)
    {
        idCollect = tuple.Item1;
        dailyGiftBooter = tuple.Item2;
        bool active = true;

        foreach (var itemProgress in listItemProgress)
        {
            itemProgress.textDay.SetActive(false);
            if (active) itemProgress.image.color = Color.green;
            if (itemProgress.id.Equals(tuple.Item1))
            {
                active = false;
                itemProgress.textDay.SetActive(true);
            }
        }
    }
    public void Close(int status)
    {
        PlayerPrefs.SetInt("StatusGiftFast", status);
        // Debug.Log(PlayerPrefs.GetInt("StatusGiftFast"));
        PopupContainer.Instance.CloseCurrentPopup();
        AudioManager.Instance.Play_Click_Button_Sound();

    }
}
