using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField]
    private GameObject uniBar;
    private RectTransform uniTrans;
    private Image uniImage;
    [SerializeField]
    private GameObject magicBar;
    private Image magicImage;
    private RectTransform magicTrans;
    [SerializeField]
    private GameObject physBar;
    private Image physImage;
    private RectTransform physTrans;

    public float offset;

    public void UpdateValues(float uni, float phys, float magic)
    {
        uni = uni < 0 ? 0 : uni;
        SetBarPosition(uniImage, uni);
        SetBarPosition(magicImage, magic, magicTrans, uni);
        SetBarPosition(physImage, phys, physTrans , uni);
    }

    private void SetBarPosition(Image img, float percentage, RectTransform trans = null, float startPercentage = 0)
    {
        img.fillAmount = percentage;
        if(trans)
        {
            //763 is the width of the bar in pixels
            trans.anchoredPosition = new Vector3(offset* startPercentage,0,0) ;
        }
    }

    private void Start()
    {
        uniImage = uniBar.GetComponent<Image>();
        physImage = physBar.GetComponent<Image>();
        magicImage = magicBar.GetComponent<Image>();
        uniTrans = uniBar.GetComponent<RectTransform>();
        physTrans = physBar.GetComponent<RectTransform>();
        magicTrans = magicBar.GetComponent<RectTransform>();
    }
}
