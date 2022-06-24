using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Responsive : MonoBehaviour
{
    public static Responsive Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    [SerializeField] RectTransform listContainerRT = null;
    [SerializeField] VerticalLayoutGroup viewWordList = null;
    [SerializeField] RectTransform headerContainerRT = null;
    [SerializeField] RectTransform gridContainerRT = null;
    [SerializeField] RectTransform ButtonContainerRT = null;

    private bool isSmallScreen = false;
    public bool IsSmallScreen { get => isSmallScreen; set => isSmallScreen = value; }

    void Start()
    {
        Debug.Log(string.Format("CharacterGridContainer width: {0}, height: {1}", gridContainerRT.rect.width, gridContainerRT.rect.height));
        Debug.Log(string.Format("Screen width: {0}, height: {1}, ratio: {2}", Screen.width, Screen.height, (float)Screen.height / Screen.width));
        var ratio = (float)Screen.height / Screen.width -0.1f;
        Debug.Log(ratio);
        // Debug.Log((float)(16f / 9f));
        // Debug.Log((float)(20f / 9f));
        // if (ratio < (float)(16f / 9f))
        // {
        //     // Debug.Log("3:4");
        //     // gridContainerRT.sizeDelta = new Vector2(740f, 755f);
        //     // gridContainerRT.anchoredPosition = new Vector3(0f, -573.28f, 0);


        //     // listContainerRT.sizeDelta = new Vector2(735.25f, 222.93f);
        //     // listContainerRT.anchoredPosition = new Vector3(0f, 45f, 0);

        //     // ButtonContainerRT.anchoredPosition = new Vector3(0f, 150f, 0);
        //     // ButtonContainerRT.sizeDelta = new Vector2(740f, 120f);
        //     // GridLayoutGroup gridLayout = ButtonContainerRT.GetComponent<GridLayoutGroup>();
        //     // gridLayout.cellSize = new Vector2(100, 105);
        //     // gridLayout

        //     // gridContainerRT.anchoredPosition = new Vector3(0f, -640f, 0);
        // }
        // else if (ratio < (float)(20f / 9f))
        // {
        //     // Debug.Log("16:9");
        //     // gridContainerRT.sizeDelta = new Vector2(1003f, 1006f);
        // }
        // else
        // {
        //     Debug.Log("20:9");

        //     gridContainerRT.sizeDelta = new Vector2(800f, 791f);
        //     gridContainerRT.anchoredPosition = new Vector3(0f, -769f, 0);


        //     listContainerRT.sizeDelta = new Vector2(800f, 250f);
        //     listContainerRT.anchoredPosition = new Vector3(0f, -73f, 0);

            
        //     headerContainerRT.sizeDelta = new Vector2(800f, 94f);
        //     headerContainerRT.anchoredPosition = new Vector3(0f, -304f, 0);

        //     ButtonContainerRT.anchoredPosition = new Vector3(0f, 372f, 0);
        //     ButtonContainerRT.sizeDelta = new Vector2(800f, 120f);
        //     GridLayoutGroup gridLayout = ButtonContainerRT.GetComponent<GridLayoutGroup>();
        //     gridLayout.cellSize = new Vector2(115f, 110.922f);
        //     gridLayout.spacing = new Vector2(15f, 0f);

        //     viewWordList.spacing = 10;

        //     IsSmallScreen = true;
        // }

         if (ratio > (float)(16f / 9f)){
             Debug.Log("20:9");

            gridContainerRT.sizeDelta = new Vector2(800f, 791f);
            gridContainerRT.anchoredPosition = new Vector3(0f, -769f, 0);


            listContainerRT.sizeDelta = new Vector2(800f, 250f);
            listContainerRT.anchoredPosition = new Vector3(0f, -73f, 0);

            
            headerContainerRT.sizeDelta = new Vector2(800f, 94f);
            headerContainerRT.anchoredPosition = new Vector3(0f, -304f, 0);

            ButtonContainerRT.anchoredPosition = new Vector3(0f, 372f, 0);
            ButtonContainerRT.sizeDelta = new Vector2(800f, 120f);
            GridLayoutGroup gridLayout = ButtonContainerRT.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(115f, 110.922f);
            gridLayout.spacing = new Vector2(15f, 0f);

            viewWordList.spacing = 10;

            IsSmallScreen = true;
         }
        Debug.Log(string.Format("resize width: {0}, height: {1}", gridContainerRT.rect.width, gridContainerRT.rect.height));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
