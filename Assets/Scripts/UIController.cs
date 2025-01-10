using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    
    public int curWeaponSpriteInd = 0;
    public float displayProgress = 0f;
    public Image weaponUI;
    public RectTransform progressBarFill;
    public RectTransform progressBarFrame;

    public int nextGameTip = 0;
    public bool showingGameTip = false;
    public GameObject gameTipUI;

    public Sprite[] gameTipSprites;
    public Sprite[] weaponUIVariations;
    

    public void ChangeWeaponUI(int ind) {
        if (ind < 0 || ind >= weaponUIVariations.Length) return;

        curWeaponSpriteInd = ind;
        weaponUI.sprite = weaponUIVariations[ind];
    }

    public void ChangeProgressBar(float p) {
        float newWidth = progressBarFrame.rect.width * p * 1.0229f;
        progressBarFill.sizeDelta = new Vector2(newWidth, progressBarFill.sizeDelta.y);
    }

    void Start()
    {
    }

    public void ShowGameTip(){
        if (nextGameTip >= gameTipSprites.Length) return;

        Image gameTipImage = gameTipUI.GetComponent<Image>();
        gameTipImage.sprite = gameTipSprites[nextGameTip];
        Animator gameTipAnimator = gameTipUI.GetComponent<Animator>();


        showingGameTip = true;
        gameTipUI.SetActive(true);
        gameTipAnimator.SetTrigger("ShowTip");
    }


    public void HideGameTip(){
        showingGameTip = false;
        gameTipUI.SetActive(false);
        nextGameTip++;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeProgressBar(displayProgress);
    }
}
