using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public int curWeaponSpriteInd = 0;
    public float displayProgress = 0f;
    public Image weaponUI;
    public RectTransform progressBarFill;
    public RectTransform progressBarFrame;
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

    // Update is called once per frame
    void Update()
    {
        ChangeProgressBar(displayProgress);
    }
}
