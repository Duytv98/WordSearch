using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gift : MonoBehaviour
{

    [SerializeField] private Text headerText = null;
    [SerializeField] private Text messageText = null;
    public void OnShowing(string headerText, string messageText)
    {
        this.headerText.text = headerText;
        this.messageText.text = messageText;
    }
}
