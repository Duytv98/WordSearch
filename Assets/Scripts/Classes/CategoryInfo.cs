using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void SelectedChangedDelegate(bool val);
[System.Serializable]
public class CategoryInfo
{
    #region Enums

    public enum LockType
    {
        None,
        Coins,
        Keys,
        IAP
    }

    #endregion

    public string displayName;
    public string saveId;
    public Sprite icon;
    public Color categoryColor;
    public LockType lockType;
    public int unlockAmount;
    public List<TextAsset> levelFiles;

    public SelectedChangedDelegate selectedChanged;
    private bool isSelected = false;
    public bool Selected
    {
        get { return isSelected; }
        set
        {
            // if the value has changed
            if (isSelected != value)
            {
                // update the state and call the selection handler if it exists
                isSelected = value;
                if (selectedChanged != null) selectedChanged(isSelected);
            }
        }
    }
}
