using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CharacterGrid : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // Start is called before the first frame update

    [SerializeField] private Camera cam;
    [Header("Container")]
    [SerializeField] private ButtonInGameContainer buttonInGameContainer;
    private RectTransform gridContainer;
    private RectTransform gridOverlayContainer;
    private RectTransform gridUnderlayContainer;
    private RectTransform highlighLetterContainer;
    [SerializeField] private Timer timer = null;
    [SerializeField] private Text textTime = null;


    [Header("Board Settings")]
    [SerializeField] private float maxCellSize = 200;
    [SerializeField] private SelectedWord selectedWord = null;
    [SerializeField] private GameObject characterGridItemPrefab = null;



    [Header("Letter Settings")]
    [SerializeField] private Font letterFont = null;
    [SerializeField] private RectTransform[] positionKill = null;
    private int letterFontSize = 150;
    private Color highlightedTextColor;
    private Sprite letterHighlightedsprite;
    private Vector2 letterOffsetInCell = Vector2.zero;


    [Header("Highlight Settings")]
    [SerializeField] private Sprite highlightSprite = null;
    [SerializeField] private float highlightExtraSize = -35f;


    [Header("Highlight Letter Settings")]
    [SerializeField] private Sprite highlightLetterSprite = null;
    [SerializeField] private float highlightLetterSize = 0f;
    [SerializeField] private Color highlightLetterColor = Color.white;


    #region Member Variables
    private Board currentBoard;
    private int indexColor;
    private CharacterGridItem startCharacter;
    private CharacterGridItem lastEndCharacter;
    private Image selectingHighlight;
    private List<List<CharacterGridItem>> characterItems;
    private List<Image> highlights;
    private List<Image> LetterHints;
    private List<Position> locationUnused;

    private float currentScale;
    private float currentCellSize;

    private bool isSelecting;
    private int selectingPointerId;

    private bool activeRotating = false;
    private bool rotating = false;
    #endregion

    #region Properties
    private float ScaledHighlighExtraSize { get { return highlightExtraSize * currentScale; } }
    private Vector2 ScaledLetterOffsetInCell { get { return letterOffsetInCell * currentScale; } }
    private float ScaledHightlightLetterSize { get { return highlightLetterSize * currentScale; } }
    private float CellFullWidth { get { return currentCellSize; } }
    private float CellFullHeight { get { return currentCellSize; } }
    #endregion




    public void OnPointerDown(PointerEventData eventData)
    {
        if (selectingPointerId != -1) return;


        CharacterGridItem characterItem = GetCharacterItemAtPosition(eventData.position);
        if (characterItem != null)
        {
            isSelecting = true;
            selectingPointerId = eventData.pointerId;
            startCharacter = characterItem;
            lastEndCharacter = characterItem;
            //active và set color cho Highlight khi người chơi chọn 1 từ
            AssignHighlighColor(selectingHighlight);
            selectingHighlight.gameObject.SetActive(true);

            UpdateSelectingHighlight(eventData.position);
            UpdateSelectedWord();
            AudioManager.Instance.Play("highlight");
        }

        // Debug.Log(characterItem.Col);



    }
    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag: ");
        if (eventData.pointerId != selectingPointerId) return;
        UpdateSelectingHighlight(eventData.position);
        UpdateSelectedWord();

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != selectingPointerId) return;

        if (startCharacter != null && lastEndCharacter != null)
        {
            // trả lại màu đen cho chữ trên màn chơi
            // set IsHighlighted = false
            SetTextColor(startCharacter, lastEndCharacter, true, true, null);

            // VỊ trí bắt đầu và kết thúc
            Position wordStartPosition = new Position(startCharacter.Row, startCharacter.Col);
            Position wordEndPosition = new Position(lastEndCharacter.Row, lastEndCharacter.Col);

            //get text từ vị trí bắt đầu tới vị trí kết thúc
            string highlightedWord = GetWord(wordStartPosition, wordEndPosition);

            // Gọi GameManager kiểm tra xem word có đúng với yêu cầu k, nếu đúng sẽ trả lại word
            string foundWord = GameScreen.Instance.OnWordSelected(highlightedWord);

            // kiểm tra foundWord không phải null hoặc chuỗi rỗng
            if (!string.IsNullOrEmpty(foundWord))
            {
                ShowWord(wordStartPosition, wordEndPosition, foundWord, true, GameScreen.Instance.GetPositionWord(foundWord));
                selectedWord.Clear(true);
                AudioManager.Instance.Play("word-found");
            }
            else selectedWord.Clear();
        }
        else selectedWord.Clear();

        // End selecting and hide the select highlight
        // selectingPointerId = -1;
        isSelecting = false;
        selectingPointerId = -1;
        startCharacter = null;
        lastEndCharacter = null;
        selectingHighlight.gameObject.SetActive(false);
    }


    // Khởi tạo component
    public void Initialize()
    {
        //Chứa tất cả các chữ trên màn chơi
        gridContainer = CreateContainer("grid_container", typeof(RectTransform), typeof(GridLayoutGroup), typeof(CanvasGroup));

        // Chứa tất cả các phần hiện nên phía trên màn chơi. (chữ bay lên khi hoàn thành đúng)
        gridOverlayContainer = CreateContainer("grid_overlay_container", typeof(RectTransform));


        //Chứa các phần màu highlight cho các từ đã được chọn đúng
        gridUnderlayContainer = CreateContainer("grid_underlay_container", typeof(RectTransform));

        gridUnderlayContainer.SetAsFirstSibling();

        // Gợi ý các chữ cái mà người chơi cần tìm kiếm (Khoanh tròn các các chữ cái có trong màn chơi)
        highlighLetterContainer = CreateContainer("highligh_letter_container", typeof(RectTransform));

        characterItems = new List<List<CharacterGridItem>>();
        highlights = new List<Image>();
        LetterHints = new List<Image>();

        // khởi tạo highlight cơ bản để dử dụng khi người chơi chọn 1 từ
        selectingHighlight = CreateNewHighlight();
        selectingHighlight.gameObject.SetActive(false);

        selectingPointerId = -1;
    }

    public void SetUp(Board board, Dictionary<char, Sprite> dicWord)
    {
        Clear();
        currentCellSize = SetupGridContainer(board.rows, board.cols);
        currentScale = currentCellSize / maxCellSize;
        // Debug.Log("currentCellSize: " + currentCellSize + "   currentScale: " + currentScale);
        // Debug.Log("listWord.DicWord.Count: " + dicWord.Count);
        // Debug.Log(dicWord.Count);

        for (int i = 0; i < board.boardCharacters.Count; i++)
        {
            characterItems.Add(new List<CharacterGridItem>());
            for (int j = 0; j < board.boardCharacters[i].Count; j++)
            {
                char text = board.boardCharacters[i][j];
                GameObject _characterItem = Instantiate(characterGridItemPrefab, Vector3.zero, Quaternion.identity, gridContainer);
                CharacterGridItem _characterItemScript = _characterItem.GetComponent<CharacterGridItem>();
                _characterItemScript.Setup(text, new Vector3(currentScale, currentScale, 1f), ScaledLetterOffsetInCell, i, j, dicWord[text]);
                characterItems[i].Add(_characterItemScript);
            }
        }
        timer.StartTimer(textTime);

        currentBoard = board;
        StartCoroutine(SetUpValue(board));

    }
    // set chữ lên màn chơi
    IEnumerator SetUpValue(Board board)
    {
        // ScreenManager.Instance.ActiveLoading();
        yield return new WaitForSeconds(0.5f);
        // ScreenManager.Instance.DeactivateLoading();
        // Debug.Log("board.foundWords: " + board.foundWords.Count);
        foreach (string foundWord in board.foundWords) SetWordFound(foundWord, board.listWordDeleted);

        foreach (char letter in board.letterHintsUsed) ShowLetterHint(letter);

        foreach (string word in board.recommendWords) ShowWordRecommend(word);

        foreach (Position position in board.locationUnuseds) characterItems[position.row][position.col].SetWordUnuseds();

        locationUnused = UnusedLocation();

        GameScreen.Instance.CharacterItems = characterItems;

        GameScreen.Instance.ActionButtonRecommendWord();
        buttonInGameContainer.SetInteractableButtonClearWords(locationUnused.Count == 0 ? false : true);
    }
    private void AssignHighlighColor(Image highlight)
    {
        Color color = Color.white;

        if (GameDefine.COLOR_TEXT_BOARD.Length > 0)
        {

            indexColor = Random.Range(0, GameDefine.COLOR_TEXT_BOARD.Length);
            highlightedTextColor = GameDefine.COLOR_TEXT_BOARD[indexColor];
            letterHighlightedsprite = DataController.Instance.ArrSquare[indexColor];
            color = GameDefine.COLOR_LINE[indexColor];
        }
        else
        {
            Debug.LogError("[CharacterGrid] Highlight Colors is empty.");
        }

        highlight.color = color;
    }

    private void UpdateSelectingHighlight(Vector2 screenPosition)
    {
        if (isSelecting)
        {
            CharacterGridItem endCharacter = GetCharacterItemAtPosition(screenPosition);

            Position positionEnd = null;
            if (endCharacter != null) positionEnd = GetPositionEnd(endCharacter);
            else endCharacter = lastEndCharacter;
            if (positionEnd != null) endCharacter = characterItems[positionEnd.row][positionEnd.col];
            endCharacter = GetEndCharacter(startCharacter, endCharacter);


            if (lastEndCharacter != null) SetTextColor(startCharacter, lastEndCharacter, false, true, null);
            // Set kích thước, vị trí , dóc nghiêng của Highlight
            PositionHighlight(selectingHighlight, startCharacter, endCharacter);

            // tìm và set màu chữ từ vị trí bắt đầu đến vị trí kết thúc
            // set IsHighlighted = false
            SetTextColor(startCharacter, endCharacter, false, false, null);

            // If the new end character is different then the last play a sound
            if (lastEndCharacter != endCharacter) AudioManager.Instance.Play("highlight");


            // Set the last end character so if the player drags outside the grid container then we have somewhere to drag to
            lastEndCharacter = endCharacter;
        }
    }

    private Position GetPositionEnd(CharacterGridItem endCharacter)
    {
        int startRow = startCharacter.Row;
        int startCol = startCharacter.Col;

        int endRow = endCharacter.Row;
        int endCol = endCharacter.Col;

        int rowDiff = endRow - startRow;
        int colDiff = endCol - startCol;

        // check đường đi Highlight là thẳng, ngang hay chéo
        if (rowDiff != colDiff && rowDiff != 0 && colDiff != 0)
        {
            // Lấy ra một vị trí kết thúc cuối cùng hợp lý nhất
            if (Mathf.Abs(colDiff) > Mathf.Abs(rowDiff))
            {
                if (Mathf.Abs(colDiff) - Mathf.Abs(rowDiff) > Mathf.Abs(rowDiff)) rowDiff = 0;
                else colDiff = AssignKeepSign(colDiff, rowDiff);
            }
            else
            {
                if (Mathf.Abs(rowDiff) - Mathf.Abs(colDiff) > Mathf.Abs(colDiff)) colDiff = 0;
                else colDiff = AssignKeepSign(colDiff, rowDiff);
            }
            if (startCol + colDiff < 0)
            {
                colDiff = colDiff - (startCol + colDiff);
                rowDiff = AssignKeepSign(rowDiff, Mathf.Abs(colDiff));
            }
            else if (startCol + colDiff >= currentBoard.cols)
            {
                colDiff = colDiff - (startCol + colDiff - currentBoard.cols + 1);
                rowDiff = AssignKeepSign(rowDiff, Mathf.Abs(colDiff));
            }
            return new Position((startRow + rowDiff), (startCol + colDiff));
        }
        return null;
    }

    private CharacterGridItem GetEndCharacter(CharacterGridItem start, CharacterGridItem end)
    {

        int rowInc = (start.Row == end.Row) ? 0 : (start.Row < end.Row ? 1 : -1);
        int colInc = (start.Col == end.Col) ? 0 : (start.Col < end.Col ? 1 : -1);
        int incAmount = Mathf.Max(Mathf.Abs(start.Row - end.Row), Mathf.Abs(start.Col - end.Col));
        CharacterGridItem character = null;

        for (int i = 0; i <= incAmount; i++)
        {
            CharacterGridItem characterGridItem = characterItems[start.Row + i * rowInc][start.Col + i * colInc];
            if (!characterGridItem.IsActive) return character;
            character = characterGridItem;
        }
        return character;
    }

    // tìm và set màu chữ từ vị trí bắt đầu đến vị trí kết thúc
    private void SetTextColor(CharacterGridItem start, CharacterGridItem end, bool onPointerUp, bool unDoColor, Sprite sprite)
    {
        int rowInc = (start.Row == end.Row) ? 0 : (start.Row < end.Row ? 1 : -1);
        int colInc = (start.Col == end.Col) ? 0 : (start.Col < end.Col ? 1 : -1);
        int incAmount = Mathf.Max(Mathf.Abs(start.Row - end.Row), Mathf.Abs(start.Col - end.Col));

        for (int i = 0; i <= incAmount; i++)
        {
            CharacterGridItem characterGridItem = characterItems[start.Row + i * rowInc][start.Col + i * colInc];
            if (unDoColor) characterGridItem.UnDoColor();
            else
            {
                if (onPointerUp) characterGridItem.SetChoose(highlightedTextColor, letterHighlightedsprite);
                else characterGridItem.SetColor(highlightedTextColor, letterHighlightedsprite);
            }
        }
    }
    private void PositionHighlight(Image highlight, CharacterGridItem start, CharacterGridItem end)
    {
        RectTransform highlightRectT = highlight.transform as RectTransform;
        Vector2 startPosition = (start.transform as RectTransform).anchoredPosition;
        Vector2 endPosition = (end.transform as RectTransform).anchoredPosition;

        float distance = Vector2.Distance(startPosition, endPosition);
        float highlightWidth = currentCellSize + distance + ScaledHighlighExtraSize;
        float highlightHeight = currentCellSize + ScaledHighlighExtraSize;
        float scale = highlightHeight / highlight.sprite.rect.height;

        // Set position and size
        highlightRectT.anchoredPosition = startPosition + (endPosition - startPosition) / 2f;
        highlightRectT.localScale = new Vector3(scale, scale);
        highlightRectT.sizeDelta = new Vector2(highlightWidth / scale, highlight.sprite.rect.height);

        // Set angle
        float angle = Vector2.Angle(new Vector2(1f, 0f), endPosition - startPosition);

        if (startPosition.y > endPosition.y) angle = -angle;

        highlightRectT.eulerAngles = new Vector3(0f, 0f, angle);
    }


    //Set text header (SelectedWord)
    private void UpdateSelectedWord()
    {
        if (startCharacter != null && lastEndCharacter != null)
        {
            Position wordStartPosition = new Position(startCharacter.Row, startCharacter.Col);
            Position wordEndPosition = new Position(lastEndCharacter.Row, lastEndCharacter.Col);

            selectedWord.SetSelectedWord(GetWord(wordStartPosition, wordEndPosition), indexColor);
        }
        else
        {
            // Debug.Log("============ ELSE  ==========");
            selectedWord.Clear();
        }
    }

    //get text từ vị trí bắt đầu tới vị trí kết thúc
    private string GetWord(Position start, Position end)
    {
        int rowInc = (start.row == end.row) ? 0 : (start.row < end.row ? 1 : -1);
        int colInc = (start.col == end.col) ? 0 : (start.col < end.col ? 1 : -1);
        int incAmount = Mathf.Max(Mathf.Abs(start.row - end.row), Mathf.Abs(start.col - end.col));

        string word = "";

        for (int i = 0; i <= incAmount; i++)
        {
            word = word + currentBoard.boardCharacters[start.row + i * rowInc][start.col + i * colInc];
        }
        return word;
    }



    //Hàm bổ trợ
    // Get chữ từ vị trí
    private CharacterGridItem GetCharacterItemAtPosition(Vector2 screenPoint)
    {
        for (int i = 0; i < characterItems.Count; i++)
        {
            for (int j = 0; j < characterItems[i].Count; j++)
            {
                Vector2 localPoint = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(characterItems[i][j].transform as RectTransform, screenPoint, cam, out localPoint);
                localPoint.x += CellFullWidth / 2f;
                localPoint.y += CellFullHeight / 2f;

                if (localPoint.x >= 0 && localPoint.y >= 0 && localPoint.x < CellFullWidth && localPoint.y < CellFullHeight && characterItems[i][j].IsActive)
                {
                    return characterItems[i][j];
                }
            }
        }
        return null;
    }
    private int AssignKeepSign(int a, int b)
    {
        return (a / Mathf.Abs(a)) * Mathf.Abs(b);
    }
    private Image CreateHighlightLetterImage()
    {
        GameObject highlightImageObj = new GameObject("highligh_image_obj");
        RectTransform highlightImageObjT = highlightImageObj.AddComponent<RectTransform>();
        Image highlightLetterImage = highlightImageObj.AddComponent<Image>();

        highlightLetterImage.sprite = highlightLetterSprite;
        highlightLetterImage.color = highlightLetterColor;

        highlightImageObjT.sizeDelta = new Vector2(highlightLetterSize, highlightLetterSize);
        highlightImageObjT.anchorMin = new Vector2(0f, 1f);
        highlightImageObjT.anchorMax = new Vector2(0f, 1f);
        highlightImageObjT.SetParent(highlighLetterContainer);

        highlightLetterImage.transform.localScale = Vector3.one;

        return highlightLetterImage;
    }
    private Image CreateNewHighlight()
    {
        GameObject highlightObject = new GameObject("highlight");
        RectTransform highlightRectT = highlightObject.AddComponent<RectTransform>();
        Image highlightImage = highlightObject.AddComponent<Image>();

        highlightRectT.anchorMin = new Vector2(0f, 1f);
        highlightRectT.anchorMax = new Vector2(0f, 1f);
        highlightRectT.SetParent(gridUnderlayContainer, false);

        highlightImage.type = Image.Type.Sliced;
        highlightImage.fillCenter = true;
        highlightImage.sprite = highlightSprite;

        AssignHighlighColor(highlightImage);

        if (selectingHighlight != null)
        {
            selectingHighlight.transform.SetAsLastSibling();
        }

        return highlightImage;
    }
    private RectTransform CreateContainer(string name, params System.Type[] types)
    {
        GameObject containerObj = new GameObject(name, types);
        RectTransform container = containerObj.GetComponent<RectTransform>();

        container.SetParent(transform, false);
        container.anchoredPosition = Vector2.zero;
        container.anchorMin = Vector2.zero;
        container.anchorMax = Vector2.one;
        container.offsetMin = Vector2.zero;
        container.offsetMax = Vector2.zero;

        return container;
    }

    private void ShowWord(Position wordStartPosition, Position wordEndPosition, string word, bool useSelectedColor, Vector3 toPosition)
    {
        CharacterGridItem startCharacter = characterItems[wordStartPosition.row][wordStartPosition.col];
        CharacterGridItem endCharacter = characterItems[wordEndPosition.row][wordEndPosition.col];

        Image highlight = HighlightWord(wordStartPosition, wordEndPosition, useSelectedColor);

        // Create the floating text in the middle of the highlighted word
        Vector2 startPosition = (startCharacter.transform as RectTransform).anchoredPosition;
        Vector2 endPosition = (endCharacter.transform as RectTransform).anchoredPosition;
        Vector2 center = endPosition + (startPosition - endPosition) / 2f;

        Text floatingText = CreateFloatingText(word, highlight.color, center);

        Color toColor = new Color(floatingText.color.r, floatingText.color.g, floatingText.color.b, 0f);
        // Debug.Log("toPosition: " + toPosition);
        floatingText.transform.DOMove(toPosition, 1f);
        floatingText.transform.DOScale(new Vector3(0.3f, 0.3f, 1), 1f)
        // Sau khi chữ bay lên
        .OnComplete(() =>
        {
            GameScreen.Instance.WordListContainer_SetWordFound(word);
            GameScreen.Instance.WordListContainer_PlusWord();
            var boardCompleted = GameScreen.Instance.CheckBoardCompleted();
            if (boardCompleted && timer.IsPlay)
            {
                var totalTime = timer.StopTimer();
                DataController.Instance.SetTimeCompleteLevel(totalTime);
                Debug.Log("totalTime: " + totalTime);
            }
            Destroy(floatingText.gameObject);
        });
    }
    public Image HighlightWord(Position start, Position end, bool useSelectedColour)
    {
        // Debug.Log("start: " + start.Log() + "  start:  " + start.Log());
        var test = DataController.Instance.ArrSquare[indexColor];

        Image highlight = CreateNewHighlight();

        highlights.Add(highlight);

        CharacterGridItem startCharacterItem = characterItems[start.row][start.col];
        CharacterGridItem endCharacterItem = characterItems[end.row][end.col];

        // Set kích thước, vị trí , dóc nghiêng của Highlight
        PositionHighlight(highlight, startCharacterItem, endCharacterItem);

        // tìm và set màu chữ từ vị trí bắt đầu đến vị trí kết thúc
        // set IsHighlighted = true
        SetTextColor(startCharacterItem, endCharacterItem, true, false, test);

        // if (useSelectedColour && SelectingHighlight != null)
        // {
        //     highlight.color = SelectingHighlight.color;
        // }
        return highlight;
    }


    //word bay len
    private Text CreateFloatingText(string text, Color color, Vector2 position)
    {
        GameObject floatingTextObject = new GameObject("found_word_floating_text", typeof(Shadow));
        RectTransform floatingTextRectT = floatingTextObject.AddComponent<RectTransform>();
        Text floatingText = floatingTextObject.AddComponent<Text>();

        floatingText.text = text;
        floatingText.font = letterFont;
        floatingText.fontSize = letterFontSize;
        floatingText.color = color;

        floatingTextRectT.anchoredPosition = position;
        floatingTextRectT.localScale = new Vector3(currentScale, currentScale, 1f);
        floatingTextRectT.anchorMin = new Vector2(0f, 1f);
        floatingTextRectT.anchorMax = new Vector2(0f, 1f);
        floatingTextRectT.SetParent(gridOverlayContainer, false);

        floatingTextObject.transform.Rotate(transform.eulerAngles);

        ContentSizeFitter csf = floatingTextObject.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        return floatingText;

    }
    private float SetupGridContainer(int rows, int columns)
    {
        GridLayoutGroup gridLayoutGroup = gridContainer.GetComponent<GridLayoutGroup>();

        float cellWidth = gridContainer.rect.width / (float)(columns + 0.6f);
        float cellHeight = gridContainer.rect.height / (float)(rows + 0.6f);
        float cellSize = Mathf.Min(cellWidth, cellHeight, maxCellSize);

        float maxSpaceW = gridContainer.rect.width - cellSize * columns;
        float maxSpaceH = gridContainer.rect.height - cellSize * rows;
        float spaceW = maxSpaceW / (float)(columns + 1);
        float spaceH = maxSpaceH / (float)(rows + 1);
        // Debug.Log("cellWidth: " + cellWidth + "   cellHeight: " + cellHeight + "   cellSize: " + cellSize);

        // Debug.Log(string.Format("gridContainer width: {0}, height: {1}", gridContainer.rect.width, gridContainer.rect.height));


        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(spaceW, spaceH);
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;
        return cellSize;
    }

    public void ShowWordRecommend(string word)
    {
        if (currentBoard == null)
        {
            return;
        }
        for (int i = 0; i < currentBoard.wordPlacements.Count; i++)
        {
            Board.WordPlacement wordPlacement = currentBoard.wordPlacements[i];

            if (word == wordPlacement.word)
            {
                Position startPosition = wordPlacement.startingPosition;

                CharacterGridItem characterItem = characterItems[startPosition.row][startPosition.col];
                // Debug.Log("word:  " + word + "    startPosition: " + startPosition.Log() + "   characterItem: " + characterItem.Log());

                Image highlight = HighlightWord(startPosition, startPosition, false);

                // wordListContainer.SetWordRecommend(word, highlight.color);
                GameScreen.Instance.wordListContainer_SetWordRecommend(word, indexColor);
                break;
            }
        }


    }


    public bool ClearWords()
    {
        // Debug.Log("locationUnused.Count: " + locationUnused.Count);
        if (locationUnused.Count <= 0) return false;
        if (locationUnused.Count <= 4)
        {
            for (int i = 0; i < locationUnused.Count; i++)
            {
                Debug.Log(i);
                FlyWord(locationUnused[i], positionKill[i].position);
            }
            locationUnused.Clear();
            buttonInGameContainer.SetInteractableButtonClearWords(false);
        }
        else
        {
            int i = 3;
            while (i >= 0)
            {
                int index = Random.Range(0, locationUnused.Count);
                Position position = locationUnused[index];
                FlyWord(position, positionKill[i].position);
                locationUnused.RemoveAt(index);
                i--;
            }
        }
        // GameManager.Instance.SetBoardInProgress();
        return true;
    }
    private void FlyWord(Position index, Vector3 positionKill)
    {
        GameScreen.Instance.SetLocationUnusedInBoard(index);
        CharacterGridItem characterItem = characterItems[index.row][index.col];
        characterItem.FlyWord(gridOverlayContainer, positionKill);

    }
    public List<Position> UnusedLocation()
    {
        Dictionary<string, Position> locationUse = new Dictionary<string, Position>();
        List<Position> locationUnused = new List<Position>();

        for (int i = 0; i < currentBoard.wordPlacements.Count; i++)
        {
            Board.WordPlacement wordPlacement = currentBoard.wordPlacements[i];
            int j = wordPlacement.word.Length;
            int x = wordPlacement.startingPosition.row;
            int y = wordPlacement.startingPosition.col;
            while (j > 0)
            {
                string str = "row: " + x + "  col: " + y;
                Position position = new Position(x, y);
                if (!locationUse.ContainsKey(str)) locationUse.Add(str, position);
                x += wordPlacement.verticalDirection;
                y += wordPlacement.horizontalDirection;
                j--;
            }
        }
        for (int i = 0; i < characterItems.Count; i++)
        {
            for (int j = 0; j < characterItems[i].Count; j++)
            {
                string str = "row: " + i + "  col: " + j;

                Position position = new Position(i, j);
                CharacterGridItem characterItem = characterItems[position.row][position.col];
                if (!locationUse.ContainsKey(str) && characterItem.IsActive)
                {
                    locationUnused.Add(position);
                }
            }
        }
        return locationUnused;
    }
    public void CloneText(CharacterGridItem characterItem)
    {
        GameObject duplicate = Instantiate(characterItem.gameObject);

        duplicate.transform.SetParent(gridOverlayContainer);



    }

    public void ShowWordHint(string word)
    {
        if (currentBoard == null)
        {
            return;
        }

        for (int i = 0; i < currentBoard.wordPlacements.Count; i++)
        {
            Board.WordPlacement wordPlacement = currentBoard.wordPlacements[i];

            if (word == wordPlacement.word)
            {
                Position startPosition = wordPlacement.startingPosition;
                Position endPosition = new Position(startPosition.row + wordPlacement.verticalDirection * (word.Length - 1), startPosition.col + wordPlacement.horizontalDirection * (word.Length - 1));

                ShowWord(startPosition, endPosition, word, false, GameScreen.Instance.GetPositionWord(word));

                break;
            }
        }
    }
    public void ShowLetterHint(char letterToShow)
    {

        foreach (var row in characterItems)
        {
            foreach (var item in row)
            {
                char letter = item.Text;
                if (letter.Equals(letterToShow) && item.IsActive)
                {
                    Image highlightImage = CreateHighlightLetterImage();
                    LetterHints.Add(highlightImage);

                    Vector2 position = (item.transform as RectTransform).anchoredPosition;
                    position.y += ScaledHightlightLetterSize / 15;

                    RectTransform highlightImageT = highlightImage.GetComponent<RectTransform>();

                    highlightImageT.sizeDelta = new Vector2(ScaledHightlightLetterSize, ScaledHightlightLetterSize);
                    highlightImageT.anchoredPosition = position;


                }
            }
        }
    }

    public void SetWordFound(string word, HashSet<string> listWordDeleted)
    {
        if (currentBoard == null)
        {
            return;
        }

        for (int i = 0; i < currentBoard.wordPlacements.Count; i++)
        {
            Board.WordPlacement wordPlacement = currentBoard.wordPlacements[i];

            if (word == wordPlacement.word)
            {
                Position startPosition = wordPlacement.startingPosition;
                Position endPosition = new Position(startPosition.row + wordPlacement.verticalDirection * (word.Length - 1), startPosition.col + wordPlacement.horizontalDirection * (word.Length - 1));
                // Debug.Log("startPosition: " + startPosition.Log() + "  endPosition:  " + endPosition.Log());
                Image highlightWord = HighlightWord(startPosition, endPosition, false);
                // wordListContainer.SetWordFound(word);
                if (!listWordDeleted.Contains(word)) GameScreen.Instance.WordListContainer_SetWordFound(word);

                break;
            }
        }
    }

    public void Clear()
    {
        foreach (var row in characterItems)
        {
            foreach (var item in row)
            {
                Destroy(item.gameObject);

            }
        }

        // for (int i = 0; i < characterItems.Count; i++)
        // {
        //     for (int j = 0; j < characterItems[i].Count; j++)
        //     {
        //         Destroy(characterItems[i][j].gameObject);
        //     }

        // }
        foreach (var item in highlights)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in LetterHints)
        {
            Destroy(item.gameObject);
        }

        characterItems.Clear();
        highlights.Clear();
        LetterHints.Clear();
        ClearRotating();

    }


    public void Rotating()
    {
        if (activeRotating) return;
        foreach (Transform child in gridContainer.transform)
        {
            child.DOLocalRotate(new Vector3(0, 0, rotating == false ? 180 : 360), 2f)
            .SetDelay(0.3f);
        }
        var z = transform.eulerAngles.z - 180;
        activeRotating = true;
        Sequence rotatingTransform = DOTween.Sequence();
        rotatingTransform.Append(transform.DOScale(new Vector3(0.7f, 0.7f, 1f), 0.3f))
        .Append(transform.DORotate(new Vector3(0, 0, rotating == false ? -180 : 0), 2f))
        .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f))
        .OnComplete(() => activeRotating = false);

        rotating = !rotating;
    }

    public void SuggestManyWords(float time, List<string> nonFoundWordsChoose)
    {
        IEnumerator coroutine = SuggestManyWordsTemp(time, nonFoundWordsChoose);
        StartCoroutine(coroutine);
    }
    private IEnumerator SuggestManyWordsTemp(float time, List<string> nonFoundWordsChoose)
    {
        yield return new WaitForSeconds(time);
        foreach (string word in nonFoundWordsChoose)
        {
            ShowWordRecommend(word);
        }
        AudioManager.Instance.Play("hint-used");
    }

    private void ClearRotating()
    {
        transform.Rotate(Vector3.zero);
        foreach (Transform child in gridContainer.transform)
        {
            child.Rotate(Vector3.zero);
        }
    }

    public void SetInteractableButtonSuggestManyWords(bool status)
    {
        buttonInGameContainer.SetInteractableButtonSuggestManyWords(status);
    }
    public void SetInteractableButtonRecommendWord(bool status)
    {
        buttonInGameContainer.SetInteractableButtonRecommendWord(status);
    }



    public List<char> GetListLetterExist()
    {
        List<char> letters = new List<char>();
        foreach (var row in characterItems)
        {
            foreach (var item in row)
            {
                char letter = item.Text;
                if (!letters.Contains(letter) && !currentBoard.letterHintsUsed.Contains(letter) && item.IsActive)
                {
                    letters.Add(letter);
                }
            }
        }
        return letters;
    }
}
