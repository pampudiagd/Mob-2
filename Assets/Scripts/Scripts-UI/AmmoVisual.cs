using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//TODO: Add stuff to Player.cs to increase and decrease ammo correctly.
public class AmmoVisual : MonoBehaviour
{

    //Set up the ammo sprites
    [SerializeField] private Sprite ammo0Sprite;
    [SerializeField] private Sprite ammo1Sprite;
    [SerializeField] private Sprite ammo2Sprite;
    [SerializeField] private Sprite ammo3Sprite;
    [SerializeField] private Sprite ammo4Sprite;

    //Set up variables for later (start/update)
    float maxAmmo = 0; //how many diamonds to draw
    Player playerCS; //will hold player.cs reference
    float lastAmmoFraction = 0; //determine how much to fill the most recent ammo unit (0=empty, 4=full)

    //The following will be used in the formula for where to draw the UI
    int iAdjust = 0;
    int yCoord = 0;

    //AmmoImage class, represents one diamond of ammo
    public class AmmoImage
    {
        //define variables
        public AmmoVisual ammoVisual; //reference class itself to access private variables (sprites)
        public Image ammoImage;
        public int ammoFraction;

        //have our class take the above variables as a parameter
        public AmmoImage(AmmoVisual ammoVisual, Image ammoImage, int ammoFraction)
        {
            this.ammoVisual = ammoVisual;
            this.ammoImage = ammoImage;
            this.ammoFraction = ammoFraction;
        }

        //determine which sprite to show
        public void SetAmmoFractions(int fractions)
        {
            switch (fractions)
            {
                case 0: ammoImage.sprite = ammoVisual.ammo0Sprite; ammoFraction = 0; break; //empty
                case 1: ammoImage.sprite = ammoVisual.ammo1Sprite; ammoFraction = 1; break; //1/4
                case 2: ammoImage.sprite = ammoVisual.ammo2Sprite; ammoFraction = 2; break; //1/2
                case 3: ammoImage.sprite = ammoVisual.ammo3Sprite; ammoFraction = 3; break; //3/4
                case 4: ammoImage.sprite = ammoVisual.ammo4Sprite; ammoFraction = 4; break; //full
            }
        }
    }

    //List of ammo images (empty to full)
    public List<AmmoImage> ammoImageList;

    //Function to update ammo.
    //This is the same as the function to update hearts, as both work in increments of 4.
    public void UpdateAmmo(float currentAmmo)
    {
        for (int i = 0; i < ammoImageList.Count; i++)
        {
            int fullAmmoThreshold = (i + 1) * 4;
            int ammoStart = i * 4;

            if (currentAmmo >= fullAmmoThreshold)
            {
                ammoImageList[i].SetAmmoFractions(4); // Full
            }
            else if (currentAmmo > ammoStart)
            {
                int partial = Mathf.FloorToInt(currentAmmo - ammoStart);
                ammoImageList[i].SetAmmoFractions(partial); // Partial
            }
            else
            {
                ammoImageList[i].SetAmmoFractions(0); // Empty
            }
        }
    }

    public void Awake()
    {
        ammoImageList = new List<AmmoImage>();
    }

    //assign the ammo sprite to an ammo GameObject, return image
    public AmmoImage CreateAmmoImage(Vector2 anchoredPosition)
    {
        //Create GameObject
        GameObject ammoGameObject = new GameObject("Ammo", typeof(Image));

        //Set as child of transform
        ammoGameObject.transform.SetParent(this.transform, false);
        ammoGameObject.transform.localPosition = Vector3.zero; //vector3 = 3d, UI goes "above" gameplay
        ammoGameObject.transform.localScale = new Vector3(2f, 2f, 1f);

        //Locate and size ammo
        ammoGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        ammoGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);

        //Set ammo sprite
        Image ammoImageUI = ammoGameObject.GetComponent<Image>(); //making this a variable makes the code cleaner
        ammoImageUI.sprite = ammo0Sprite;

        //constructor to receive ammo image
        AmmoImage ammoImage = new AmmoImage(this, ammoImageUI, 4); //use "this" for HeartsVisual parameter
        ammoImageList.Add(ammoImage);

        return ammoImage;
    }


    // S T A R T
    // Start is called before the first frame update
    void Start()
    {

        //Get Player object
        GameObject playerObject = GameObject.Find("Player");

        //Get Player script
        if (playerObject != null)
        {
            playerCS = playerObject.GetComponent<Player>();

            if (playerCS.ammoMax % 4 == 0)
            {
                maxAmmo = playerCS.ammoMax / 4;
            }
            else
            {
                Debug.Log("Player max ammo should be a multiple of 4");
            }
        }

        //Draw rows 1 and 2, up to 6 ammo diamonds each
        for (int i=0;i<maxAmmo;i++)
        {
            //Adjust drawing location based on row
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

            //Now for the once-per-diamond stuff
            //If current ammo is enough that the current diamond being drawn is full
            if (playerCS.ammoCount >= (i*4) + 4)
            {
                //create image
                CreateAmmoImage(new Vector2(225 + 30 * (i+iAdjust), yCoord)).SetAmmoFractions(4);
            }

            //And, if the current diamond is partly full:
            else if ((i * 4) < playerCS.ammoCount && playerCS.ammoCount < (i * 4) + 4)
            {
                //set value of lastAmmoFraction
                lastAmmoFraction = Mathf.Floor(playerCS.ammoCount - (i * 4));

                //create image
                CreateAmmoImage(new Vector2(225 + 30 * (i+iAdjust), yCoord)).SetAmmoFractions((int)lastAmmoFraction);
            }

            //And, if the current diamond is empty:
            else if (playerCS.ammoCount <= (i * 4))
            {
                //create image
                CreateAmmoImage(new Vector2(225 + 30 * (i+iAdjust), yCoord)).SetAmmoFractions(0);
            }
        }
    }


    // U P D A T E
    // Update is called once per frame
    void Update()
    {
        
    }
}
