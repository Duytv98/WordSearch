using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPopup : MonoBehaviour
{

    // [SerializeField] private ToggleSlider musicToggle = null;
    // [SerializeField] private ToggleSlider soundToggle = null;
      public void CloseSettingsPopup()
    {
        PopupContainer.Instance.ClosePopup();
    }
}
