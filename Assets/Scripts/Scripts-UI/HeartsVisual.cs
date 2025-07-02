using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsVisual : MonoBehaviour
{
    //Set up the heart sprites
    [SerializeField] private Sprite heart0Sprite;
    [SerializeField] private Sprite heart1Sprite;
    [SerializeField] private Sprite heart2Sprite;
    [SerializeField] private Sprite heart3Sprite;
    [SerializeField] private Sprite heart4Sprite;

    //Set up variables for later (start/update)
    float maxHearts = 0; //how many hearts to draw
    Player playerCS; //will hold player.CS reference
    float lastHeartFraction = 0; //use to fill heart between 0 and 4 (empty and full)

    //The following will be used in formula for where to draw hearts
    int iAdjust = 0;
    int yCoord = 0;

    //HeartImage class, represents a single heart
    public class HeartImage
    {
        //define variables
        public HeartsVisual heartsVisual; //reference class itself to access private variables (sprites)
        public Image heartImage;
        public int heartFraction;

        //have our class take the above variables as a parameter
        public HeartImage(HeartsVisual heartsVisual, Image heartImage, int heartFraction)
        {
            this.heartsVisual = heartsVisual;
            this.heartImage = heartImage;
            this.heartFraction = heartFraction;
        }

        //determine which sprite to show
        public void SetHeartFractions(int fractions)
        {
            switch (fractions)
            {
                case 0: heartImage.sprite = heartsVisual.heart0Sprite; heartFraction = 0; break; //empty
                case 1: heartImage.sprite = heartsVisual.heart1Sprite; heartFraction = 1; break; //1/4
                case 2: heartImage.sprite = heartsVisual.heart2Sprite; heartFraction = 2; break; //1/2
                case 3: heartImage.sprite = heartsVisual.heart3Sprite; heartFraction = 3; break; //3/4
                case 4: heartImage.sprite = heartsVisual.heart4Sprite; heartFraction = 4; break; //full

            }
        }
    }

    //List of heart images (empty to full)
    public List<HeartImage> heartImageList;

    //The following code comes from ChatGPT since I had to untangle some stuff with it.
    //This function should update hearts to reflect a given health value.
    public void UpdateHearts(float currentHealth)
    {
        Debug.Log($"HeartImageList Count: {heartImageList.Count}");
        for (int i = 0; i < heartImageList.Count; i++)
        {
            int fullHeartThreshold = (i + 1) * 4;
            int heartStart = i * 4;

            if (currentHealth >= fullHeartThreshold)
            {
                heartImageList[i].SetHeartFractions(4); // Full
            }
            else if (currentHealth > heartStart)
            {
                int partial = Mathf.FloorToInt(currentHealth - heartStart);
                heartImageList[i].SetHeartFractions(partial); // Partial
            }
            else
            {
                heartImageList[i].SetHeartFractions(0); // Empty
            }
        }
    }

    public void Awake()
    {
        heartImageList = new List<HeartImage>();
    }

    //assign the heart sprite to a heart GameObject, return image
    public HeartImage CreateHeartImage(Vector2 anchoredPosition)
    {
        //Create GameObject
        GameObject heartGameObject = new GameObject("Heart", typeof(Image));

        //Set as child of transform
        heartGameObject.transform.SetParent(this.transform, false);
        heartGameObject.transform.localPosition = Vector3.zero; //vector3 = 3d, UI goes "above" gameplay
        heartGameObject.transform.localScale = new Vector3(2f, 2f, 1f);

        //Locate and size heart
        heartGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        heartGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);

        //Set heart sprite
        Image heartImageUI = heartGameObject.GetComponent<Image>(); //making this a variable makes the code cleaner
        heartImageUI.sprite = heart0Sprite;

        //constructor to receive heart image
        HeartImage heartImage = new HeartImage(this, heartImageUI, 4); //use "this" for HeartsVisual parameter
        heartImageList.Add(heartImage);

        return heartImage;
    }



    //S T A R T
    // Start is called before the first frame update
    void Start()
    {
        //Set up variables

        //Get Player object
        GameObject playerObject = GameObject.Find("Player"); //I've heard this function is bad if called in Update()
                                                             //but OK when called in Start()

        //Get Player script
        if (playerObject != null)
        {
            playerCS = playerObject.GetComponent<Player>();
            Debug.Log("Player's max health:" + playerCS.healthMax);

            if (playerCS.healthMax % 4 == 0)
            {
                maxHearts = playerCS.healthMax / 4;
            }
            else
            {
                Debug.Log("Player max health should be a multiple of 4");
            }
        }

        //draw rows 1 and 2, up to 6 hearts each
        for (int i = 0; i < maxHearts; i++)
        {
            //adjust drawing location based on row
            if (i < 6)
            {
                iAdjust = 0;
                yCoord = 200;
            }
            else if (i < 12)
            {
                iAdjust = -6;
                yCoord = 175;
            }


            //Now for the once-per-heart stuff
            //If current health is enough that the current heart being drawn is full
            if (playerCS.healthCurrent >= (i * 4) + 4)
            {
                //create image
                CreateHeartImage(new Vector2(-375 + 30 * (i + iAdjust), yCoord)).SetHeartFractions(4);
            }
            //And, if the current heart is partly full:
            else if ((i * 4) < playerCS.healthCurrent && playerCS.healthCurrent < (i * 4) + 4)
            {
                //set value of lastHeartFraction
                lastHeartFraction = Mathf.Floor(playerCS.healthCurrent - (i * 4));

                //create image
                CreateHeartImage(new Vector2(-375 + 30 * (i + iAdjust), yCoord)).SetHeartFractions((int)lastHeartFraction);
            }
            //And, if the current heart is empty:
            else if (playerCS.healthCurrent <= (i * 4))
            {
                //create image
                CreateHeartImage(new Vector2(-375 + 30 * (i + iAdjust), yCoord)).SetHeartFractions(0);
            }
        }

    }



    // U P D A T E
    // Update is called once per frame
    void Update()
    {

    }
}
