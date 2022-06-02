using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CharacterGrid : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // Start is called before the first frame update

    [SerializeField] private Camera cam;
    private enum HighlighPosition
    {
        AboveLetters,
        BelowLetters
    }

    [SerializeField] private WordListContainer wordListContainer = null;
    [SerializeField] private float maxCellSize = 200;
    [SerializeField] private SelectedWord selectedWord = null;


    [Header("Letter Settings")]
    [SerializeField] private Font letterFont = null;
    [SerializeField] private int letterFontSize = 150;
    [SerializeField] private Color letterColor = Color.black;
    [SerializeField] private Color letterHighlightedColor = Color.white;
    [SerializeField] private Vector2 letterOffsetInCell = Vector2.zero;

    [Header("Highlight Settings")]
    [SerializeField] private HighlighPosition highlightPosition = HighlighPosition.AboveLetters;
    [SerializeField] private Sprite highlightSprite = null;
    [SerializeField] private float highlightExtraSize = -35f;
    [SerializeField] private List<Color> highlightColors = null;



    [Header("Highlight Letter Settings")]
    [SerializeField] private Sprite highlightLetterSprite = null;
    [SerializeField] private float highlightLetterSize = 0f;
    [SerializeField] private Color highlightLetterColor = Color.white;

    private Image selectingHighlight;

    [SerializeField] private GameObject characterGridItemPrefab = null;

    private Board currentBoard;

    [Header("Container")]
    private RectTransform gridContainer;
    private RectTransform gridOverlayContainer;
    private RectTransform gridUnderlayContainer;
    private RectTransform highlighLetterContainer;

    private List<List<char>> boardCharacters = null;
    private List<List<CharacterGridItem>> characterItems;
    private List<Image> highlights;
    private List<Image> LetterHints;


    private float currentScale;
    private float currentCellSize;

    private bool isSelecting;
    private int selectingPointerId;
    private bool ActiveEvent = true;
    private CharacterGridItem startCharacter;
    private CharacterGridItem lastEndCharacter;


    private float ScaledHighlighExtraSize { get { return highlightExtraSize * currentScale; } }
    private Vector2 ScaledLetterOffsetInCell { get { return letterOffsetInCell * currentScale; } }
    private float ScaledHightlightLetterSize { get { return highlightLetterSize * currentScale; } }
    private float CellFullWidth { get { return currentCellSize; } }
    private float CellFullHeight { get { return currentCellSize; } }

    public Image SelectingHighlight { get => selectingHighlight; set => selectingHighlight = value; }

    private bool activeRotating = false;
    private bool rotating = false;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!ActiveEvent)
        {
            // There is already a mouse/pointer highlighting words 
            return;
        }
        if (GameManager.Instance.ActiveGameState == GameManager.GameState.BoardActive)
        {
            CharacterGridItem characterItem = GetCharacterItemAtPosition(eventData.position);
            if (characterItem != null)
            {
                isSelecting = true;
                startCharacter = characterItem;
                lastEndCharacter = characterItem;
                //active và set color cho Highlight khi người chơi chọn 1 từ
                AssignHighlighColor(SelectingHighlight);
                SelectingHighlight.gameObject.SetActive(true);

                UpdateSelectingHighlight(eventData.position);
                UpdateSelectedWord();
                AudioManager.Instance.Play("highlight");
            }

            // Debug.Log(characterItem.Col);
        }

    }
    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag: ");
        if (!ActiveEvent)
        {
            return;
        }

        if (GameManager.Instance.ActiveGameState == GameManager.GameState.BoardActive)
        {
            UpdateSelectingHighlight(eventData.position);
            UpdateSelectedWord();
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!ActiveEvent)
        {
            return;

        }

        if (startCharacter != null && lastEndCharacter != null && GameManager.Instance.ActiveGameState == GameManager.GameState.BoardActive)
        {
            // trả lại màu đen cho chữ trên màn chơi
            SetTextColor(startCharacter, lastEndCharacter, letterColor, false, true);

            // VỊ trí bắt đầu và kết thúc
            Position wordStartPosition = new Position(startCharacter.Row, startCharacter.Col);
            Position wordEndPosition = new Position(lastEndCharacter.Row, lastEndCharacter.Col);

            //get text từ vị trí bắt đầu tới vị trí kết thúc
            string highlightedWord = GetWord(wordStartPosition, wordEndPosition);

            // Gọi GameManager kiểm tra xem word có đúng với yêu cầu k, nếu đúng sẽ trả lại word
            string foundWord = GameManager.Instance.OnWordSelected(highlightedWord);

            // kiểm tra foundWord không phải null hoặc chuỗi rỗng
            if (!string.IsNullOrEmpty(foundWord))
            {
                ShowWord(wordStartPosition, wordEndPosition, foundWord, true, GameManager.Instance.GetPositionWord(foundWord));
                selectedWord.Clear(true);
                AudioManager.Instance.Play("word-found");
            }
            else selectedWord.Clear();
        }
        else selectedWord.Clear();

        // End selecting and hide the select highlight
        // selectingPointerId = -1;
        isSelecting = false;
        startCharacter = null;
        lastEndCharacter = null;
        SelectingHighlight.gameObject.SetActive(false);
    }


    // Khởi tạo component
    public void Initialize()
    {
        //Chứa tất cả các chữ trên màn chơi
        gridContainer = CreateContainer("grid_container", typeof(RectTransform), typeof(GridLayoutGroup), typeof(CanvasGroup));

        // Chứa tất cả các phần hiện nên phía trên màn chơi. (chữ bay lên khi hoàn thành đúng)
        gridOverlayContainer = CreateContainer("grid_overlay_container", typeof(RectTransform));

        if (highlightPosition == HighlighPosition.BelowLetters)
        {
            //Chứa các phần màu highlight cho các từ đã được chọn đúng
            gridUnderlayContainer = CreateContainer("grid_underlay_container", typeof(RectTransform));

            gridUnderlayContainer.SetAsFirstSibling();
        }
        // Gợi ý các chữ cái mà người chơi cần tìm kiếm (Khoanh tròn các các chữ cái có trong màn chơi)
        highlighLetterContainer = CreateContainer("highligh_letter_container", typeof(RectTransform));

        characterItems = new List<List<CharacterGridItem>>();
        highlights = new List<Image>();
        LetterHints = new List<Image>();

        // khởi tạo highlight cơ bản để dử dụng khi người chơi chọn 1 từ
        SelectingHighlight = CreateNewHighlight();
        SelectingHighlight.gameObject.SetActive(false);
    }

    public void SetUp(Board board)
    {
        Clear();
        currentCellSize = SetupGridContainer(board.rows, board.cols);
        currentScale = currentCellSize / maxCellSize;

        for (int i = 0; i < board.boardCharacters.Count; i++)
        {
            characterItems.Add(new List<CharacterGridItem>());
            for (int j = 0; j < board.boardCharacters[i].Count; j++)
            {
                char text = board.boardCharacters[i][j];
                GameObject _characterItem = Instantiate(characterGridItemPrefab, Vector3.zero, Quaternion.identity, gridContainer);
                CharacterGridItem _characterItemScript = _characterItem.GetComponent<CharacterGridItem>();
                _characterItemScript.Setup(text, letterColor, new Vector3(currentScale, currentScale, 1f), ScaledLetterOffsetInCell);

                _characterItemScript.Row = i;
                _characterItemScript.Col = j;
                _characterItemScript.IsHighlighted = false;
                // Debug.Log(_characterItemScript.Log());
                characterItems[i].Add(_characterItemScript);
            }
        }
        // selectingPointerId = -1;

        currentBoard = board;
        // if (board.foundWords.Count != 0 && board.recommendWords.Count != 0)
        // {
        //     StartCoroutine(SetUpValue(board));
        // }
        StartCoroutine(SetUpValue(board));

    }
    // set chữ lên màn chơi
    IEnumerator SetUpValue(Board board)
    {
        GameManager.Instance.ActiveLoading();
        yield return new WaitForSeconds(0.01f);
        GameManager.Instance.DeactivateLoading();
        // Debug.Log("board.foundWords: " + board.foundWords.Count);
        foreach (string foundWord in board.foundWords)
        {
            SetWordFound(foundWord, board.listWordDeleted);
        }
        // Debug.Log("board.letterHintsUsed: " + board.letterHintsUsed.Count);
        foreach (char letter in board.letterHintsUsed)
        {
            ShowLetterHint(letter);
        }
        // Debug.Log("count: " + board.recommendWords);
        foreach (string word in board.recommendWords)
        {
            ShowWordRecommend(word);
        }
        // Debug.Log("số từ bay đi: " + board.locationUnuseds.Count);
        foreach (Position position in board.locationUnuseds)
        {
            CharacterGridItem characterItem = characterItems[position.row][position.col];
            characterItem.SetWordUnuseds();
        }
    }





    //set clolor Highligh
    private void AssignHighlighColor(Image highlight)
    {
        Color color = Color.white;

        if (highlightColors.Count > 0)
        {
            color = highlightColors[Random.Range(0, highlightColors.Count)];
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

            // If endCharacter is null then the mouse position must be off the grid container
            if (endCharacter != null)
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
                        if (Mathf.Abs(colDiff) - Mathf.Abs(rowDiff) > Mathf.Abs(rowDiff))
                        {
                            rowDiff = 0;
                        }
                        else
                        {
                            colDiff = AssignKeepSign(colDiff, rowDiff);
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(rowDiff) - Mathf.Abs(colDiff) > Mathf.Abs(colDiff))
                        {
                            colDiff = 0;
                        }
                        else
                        {
                            colDiff = AssignKeepSign(colDiff, rowDiff);
                        }
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

                    endCharacter = characterItems[startRow + rowDiff][startCol + colDiff];
                }
            }
            else
            {
                // Use the last selected end character
                endCharacter = lastEndCharacter;
            }

            if (lastEndCharacter != null)
            {
                // tìm và set màu chữ từ vị trí bắt đầu đến vị trí kết thúc
                SetTextColor(startCharacter, lastEndCharacter, letterColor, false);
            }

            // Set kích thước, vị trí , dóc nghiêng của Highlight
            PositionHighlight(SelectingHighlight, startCharacter, endCharacter);

            // tìm và set màu chữ từ vị trí bắt đầu đến vị trí kết thúc
            SetTextColor(startCharacter, endCharacter, letterHighlightedColor, false);

            // If the new end character is different then the last play a sound
            if (lastEndCharacter != endCharacter)
            {
                AudioManager.Instance.Play("highlight");
            }

            // Set the last end character so if the player drags outside the grid container then we have somewhere to drag to
            lastEndCharacter = endCharacter;
        }
    }

    // tìm và set màu chữ từ vị trí bắt đầu đến vị trí kết thúc
    private void SetTextColor(CharacterGridItem start, CharacterGridItem end, Color color, bool isHighlighted, bool onPointerUp = false)
    {
        int rowInc = (start.Row == end.Row) ? 0 : (start.Row < end.Row ? 1 : -1);
        int colInc = (start.Col == end.Col) ? 0 : (start.Col < end.Col ? 1 : -1);
        int incAmount = Mathf.Max(Mathf.Abs(start.Row - end.Row), Mathf.Abs(start.Col - end.Col));

        for (int i = 0; i <= incAmount; i++)
        {
            CharacterGridItem characterGridItem = characterItems[start.Row + i * rowInc][start.Col + i * colInc];
            if (characterGridItem.IsHighlighted)
            {
                characterGridItem.characterText.color = letterHighlightedColor;
            }
            else
            {
                if (isHighlighted)
                {
                    characterGridItem.IsHighlighted = isHighlighted;
                }
                // if(!characterGridItem.IsActive)
                // characterGridItem.characterText.color = characterGridItem.IsActive ? color : Color.grey;
                if (onPointerUp) characterGridItem.characterText.color = characterGridItem.IsActive ? color : Color.grey;
                else characterGridItem.characterText.color = color;
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
        // Debug.Log("currentCellSize: " + currentCellSize + "   ScaledHighlighExtraSize: " + ScaledHighlighExtraSize + "  currentScale: " + currentScale);
        // Debug.Log("highlightHeight: " + highlightHeight + "   highlight.sprite.rect.height: " + highlight.sprite.rect.height + "  scale: " + scale);

        // Set position and size
        highlightRectT.anchoredPosition = startPosition + (endPosition - startPosition) / 2f;
        // Debug.Log("start: " + start.Log());
        // Debug.Log("end: " + end.Log());
        // Debug.Log("startPosition: " + startPosition + "      endPosition: " + endPosition);
        // Debug.Log("anchoredPosition: " + (startPosition + (endPosition - startPosition) / 2f));
        // Now Set the size of the highlight
        highlightRectT.localScale = new Vector3(scale, scale);
        highlightRectT.sizeDelta = new Vector2(highlightWidth / scale, highlight.sprite.rect.height);

        // Set angle
        float angle = Vector2.Angle(new Vector2(1f, 0f), endPosition - startPosition);

        if (startPosition.y > endPosition.y)
        {
            angle = -angle;
        }

        highlightRectT.eulerAngles = new Vector3(0f, 0f, angle);
    }


    //Set text header (SelectedWord)
    private void UpdateSelectedWord()
    {
        if (startCharacter != null && lastEndCharacter != null)
        {
            Position wordStartPosition = new Position(startCharacter.Row, startCharacter.Col);
            Position wordEndPosition = new Position(lastEndCharacter.Row, lastEndCharacter.Col);

            selectedWord.SetSelectedWord(GetWord(wordStartPosition, wordEndPosition), SelectingHighlight.color);
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

                if (localPoint.x >= 0 && localPoint.y >= 0 && localPoint.x < CellFullWidth && localPoint.y < CellFullHeight)
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
        highlightRectT.SetParent(highlightPosition == HighlighPosition.AboveLetters ? gridOverlayContainer : gridUnderlayContainer, false);

        highlightImage.type = Image.Type.Sliced;
        highlightImage.fillCenter = true;
        highlightImage.sprite = highlightSprite;

        AssignHighlighColor(highlightImage);

        if (SelectingHighlight != null)
        {
            SelectingHighlight.transform.SetAsLastSibling();
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
            GameManager.Instance.WordListContainer_SetWordFound(word);
            GameManager.Instance.WordListContainer_PlusWord();
            GameManager.Instance.CheckBoardCompleted();
            Destroy(floatingText.gameObject);
        });








        //
        //
        //
        //
        //

    }
    public Image HighlightWord(Position start, Position end, bool useSelectedColour)
    {
        // Debug.Log("start: " + start.Log() + "  start:  " + start.Log());

        Image highlight = CreateNewHighlight();

        highlights.Add(highlight);

        CharacterGridItem startCharacterItem = characterItems[start.row][start.col];
        CharacterGridItem endCharacterItem = characterItems[end.row][end.col];

        // Set kích thước, vị trí , dóc nghiêng của Highlight
        PositionHighlight(highlight, startCharacterItem, endCharacterItem);

        // tìm và set màu chữ từ vị trí bắt đầu đến vị trí kết thúc
        SetTextColor(startCharacterItem, endCharacterItem, letterHighlightedColor, true);

        if (useSelectedColour && SelectingHighlight != null)
        {
            highlight.color = SelectingHighlight.color;
        }
        return highlight;
    }

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

        float cellWidth = gridContainer.rect.width / (float)columns;
        float cellHeight = gridContainer.rect.height / (float)rows;
        float cellSize = Mathf.Min(cellWidth, cellHeight, maxCellSize);

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
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
                GameManager.Instance.wordListContainer_SetWordRecommend(word, highlight.color);
                break;
            }
        }


    }


    public bool ClearWords()
    {
        List<Position> locationUnused = UnusedLocation();
        // Debug.Log("locationUnused.Count: " + locationUnused.Count);
        if (locationUnused.Count <= 0) return false;
        if (locationUnused.Count <= 4)
        {
            foreach (Position position in locationUnused)
            {
                GameManager.Instance.SetLocationUnusedInBoard(position);
                CharacterGridItem characterItem = characterItems[position.row][position.col];
                characterItem.FlyWord();
            }
        }
        else
        {
            int i = 4;
            while (i > 0)
            {
                int index = Random.Range(0, locationUnused.Count);
                Position position = locationUnused[index];
                GameManager.Instance.SetLocationUnusedInBoard(position);
                CharacterGridItem characterItem = characterItems[position.row][position.col];
                characterItem.FlyWord();
                locationUnused.RemoveAt(index);
                // Debug.Log("i: " + i);
                i--;
            }
        }
        GameManager.Instance.SaveCurrentBoard();
        return true;
    }
    public List<Position> UnusedLocation()
    {
        Dictionary<string, Position> locationUse = new Dictionary<string, Position>();
        List<Position> locationUnused = new List<Position>();

        for (int i = 0; i < currentBoard.wordPlacements.Count; i++)
        {
            Board.WordPlacement wordPlacement = currentBoard.wordPlacements[i];
            // Debug.Log(JsonUtility.ToJson(wordPlacement));
            int j = wordPlacement.word.Length;
            // Debug.Log("Số lượng chữ: " + j);
            int x = wordPlacement.startingPosition.row;
            int y = wordPlacement.startingPosition.col;
            while (j > 0)
            {
                string str = "row: " + x + "  col: " + y;
                // Debug.Log(str);
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
                    characterItem.Clone(gridOverlayContainer);
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

                ShowWord(startPosition, endPosition, word, false, GameManager.Instance.GetPositionWord(word));

                break;
            }
        }
    }
    public void ShowLetterHint(char letterToShow)
    {
        for (int row = 0; row < currentBoard.rows; row++)
        {
            for (int col = 0; col < currentBoard.cols; col++)
            {
                char letter = currentBoard.boardCharacters[row][col];

                if (letter == letterToShow)
                {
                    Image highlightImage = CreateHighlightLetterImage();
                    LetterHints.Add(highlightImage);

                    CharacterGridItem characterGridItem = characterItems[row][col];

                    Vector2 position = (characterGridItem.transform as RectTransform).anchoredPosition;

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
                if (!listWordDeleted.Contains(word)) GameManager.Instance.WordListContainer_SetWordFound(word);

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


}
