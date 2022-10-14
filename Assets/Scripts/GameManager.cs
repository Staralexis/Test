using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    float timer = 180f;
    float score = 1800f;
    int[] currentTab = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    //int[] goal = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    [SerializeField] Text timertext;
    [SerializeField] Text scoreText;
    public Image[] board;
    public Sprite[] androidSprite;
    public Sprite[] appleSprite;

    public int selectedTile = -1;

    public bool isGameEnded = false;

    public Vector2 firstPressPos;
    public Vector2 secondPressPos;
    public Vector2 currentSwipe;

    // Start is called before the first frame update
    void Start()
    {
        Shuffle();
    }

    private void Shuffle()
    {
       
        for(int i=0;i<30; i++)
        {
            int j;
            for (j=0; j<9; j++)
            {
                if(currentTab[j]==5)
                {
                   
                    break;
                }
            }
            int random = Random.Range(0, 4);
            int temp;
            switch (random)
            {
                case 0: //up
                    if (j <= 3) continue;
                    temp = currentTab[j];
                    currentTab[j] = currentTab[j - 3];
                    currentTab[j - 3] = temp;
                    break;
                case 1: //down
                    if (j > 5) continue;
                    temp = currentTab[j];
                    currentTab[j] = currentTab[j + 3];
                    currentTab[j + 3] = temp;
                    break;
                case 2: //right
                    if (j == 2 || j == 5 || j == 8) continue;
                    temp = currentTab[j];
                    currentTab[j] = currentTab[j + 1];
                    currentTab[j + 1] = temp;
                   
                    break;
                case 3: //left
                    if (j == 0 || j == 3 || j == 6) continue;
                    temp = currentTab[j];
                    currentTab[j] = currentTab[j - 1];
                    currentTab[j - 1] = temp;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ///////INPUT/////////
        ///
        if(Input.GetMouseButtonUp(0) && !isGameEnded)
        {
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //normalize the 2d vector
            currentSwipe.Normalize();

            //swipe upwards
            if (currentSwipe.y > 0 && (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f))
            {
                if (selectedTile > 2 && currentTab[selectedTile - 3] == 5)
                {
                    int temp = currentTab[selectedTile];
                    currentTab[selectedTile] = currentTab[selectedTile - 3];
                    currentTab[selectedTile - 3] = temp;
                }
            }
            //swipe down
            else if (currentSwipe.y < 0 && (currentSwipe.x > -0.5f && currentSwipe.x < 0.5f))
            {
                if (selectedTile < 6 && currentTab[selectedTile + 3] == 5)
                {
                    int temp = currentTab[selectedTile];
                    currentTab[selectedTile] = currentTab[selectedTile + 3];
                    currentTab[selectedTile + 3] = temp;
                }
            }
            //swipe left
            else if (currentSwipe.x < 0 && (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f))
            {
                if (selectedTile != 0 && selectedTile != 3 && selectedTile != 6 && currentTab[selectedTile - 1] == 5)
                {
                    int temp = currentTab[selectedTile];
                    currentTab[selectedTile] = currentTab[selectedTile - 1];
                    currentTab[selectedTile - 1] = temp;
                }
            }
            //swipe right
            else if (currentSwipe.x > 0 && (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f))
            {
                if (selectedTile != 2 && selectedTile != 5 && selectedTile != 8 && currentTab[selectedTile + 1] == 5)
                {
                    int temp = currentTab[selectedTile];
                    currentTab[selectedTile] = currentTab[selectedTile + 1];
                    currentTab[selectedTile + 1] = temp;
                }
            }

            
        }


        isGameEnded = true;
        for(int i = 1; i<10; i++)
        {
            if(currentTab[i-1] != i)
            {
                isGameEnded = false;
            }
        }


        ///////// DISPLAY ////////
        if (!isGameEnded)
        {
            timer -= Time.deltaTime;
            score -= Time.deltaTime * 10;
        }
        else
        {
            scoreText.enabled = true;
            scoreText.text = "score : " + ((int)score).ToString();
            if ((int)score > PlayerPrefs.GetInt("highScore"))
            {
                PlayerPrefs.SetInt("highScore", (int)score);
            }
        }
        timertext.text = CreateStringForTimer(timer);
        if(timer<0)
        {
            isGameEnded = true;
            timertext.text = "0 : 0";
            timertext.color = Color.red;
            score = 0;
        }

        if (Application.platform == RuntimePlatform.Android)
            for (int i = 0; i < board.Length; i++)
            {
                board[i].sprite = androidSprite[currentTab[i] - 1];
            }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            for (int i = 0; i < board.Length; i++)
            {
                board[i].sprite = appleSprite[currentTab[i] - 1];
            }
        else
            for (int i = 0; i < board.Length; i++)
            {
                board[i].sprite = appleSprite[currentTab[i]-1];
            }



    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == 5)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    string CreateStringForTimer(float timer)
    {
        int timerInt = (int) timer;
        string newString = ""+ timerInt / 60+" : "+ timerInt % 60;
        



        return newString;
    }

    public void SelectTile1()
    {
        selectedTile = 0;
        firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    public void SelectTile2()
    {
        selectedTile = 1;
        firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    public void SelectTile3()
    {
        selectedTile = 2;
        firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    public void SelectTile4()
    {
        selectedTile = 3;
        firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    public void SelectTile5()
    {
        selectedTile = 4;
        firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    public void SelectTile6()
    {
        selectedTile = 5;
        firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    public void SelectTile7()
    {
        selectedTile = 6;
        firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    public void SelectTile8()
    {
        selectedTile = 7;
        firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
    public void SelectTile9()
    {
        selectedTile = 8;
        firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

}
