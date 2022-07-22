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
        var ratio = (float)Screen.height / Screen.width;
        if (ratio > (float)(18f / 9f))
        {
            Debug.Log("20:9");

            gridContainerRT.sizeDelta = new Vector2(800f, 791f);
            gridContainerRT.anchoredPosition = new Vector3(0f, -803f, 0);

            listContainerRT.sizeDelta = new Vector2(800f, 250f);
            listContainerRT.anchoredPosition = new Vector3(0f, -73f, 0);

            headerContainerRT.sizeDelta = new Vector2(800f, 94f);
            headerContainerRT.anchoredPosition = new Vector3(0f, -317f, 0);

            ButtonContainerRT.anchoredPosition = new Vector3(0f, 300f, 0);
            ButtonContainerRT.sizeDelta = new Vector2(800f, 120f);
            GridLayoutGroup gridLayout = ButtonContainerRT.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(115f, 110.922f);
            gridLayout.spacing = new Vector2(15f, 0f);

            viewWordList.spacing = 10;

            IsSmallScreen = true;
        }
        else if (ratio > (float)(16f / 9f))
        {
            Debug.Log("18:9");

            gridContainerRT.sizeDelta = new Vector2(877f, 988f);
            gridContainerRT.anchoredPosition = new Vector3(0f, -875f, 0);

            listContainerRT.sizeDelta = new Vector2(871f, 296f);
            listContainerRT.anchoredPosition = new Vector3(0f, -35f, 0);

            headerContainerRT.sizeDelta = new Vector2(800f, 94f);
            headerContainerRT.anchoredPosition = new Vector3(0f, -304f, 0);

            ButtonContainerRT.anchoredPosition = new Vector3(0f, 160f, 0);
            ButtonContainerRT.sizeDelta = new Vector2(860f, 135f);
            GridLayoutGroup gridLayout = ButtonContainerRT.GetComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(118f, 120f);
            gridLayout.spacing = new Vector2(30f, 0f);

       

        }
        else
        {
            Debug.Log("Màn hình lớn hơn 16:9");
        }
        // Debug.Log(string.Format("resize width: {0}, height: {1}", gridContainerRT.rect.width, gridContainerRT.rect.height));
    }

}
