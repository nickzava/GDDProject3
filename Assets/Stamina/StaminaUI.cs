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
        SetBarPosition(magicImage, magic == 0 ? 0 : magic + uni);
        SetBarPosition(physImage, phys == 0 ? 0 : phys + uni);
    }

    private void SetBarPosition(Image img, float percentage)
    {
        img.fillAmount = percentage;
    }

    private void Start()
    {
        uniImage = uniBar.GetComponent<Image>();
        physImage = physBar.GetComponent<Image>();
        magicImage = magicBar.GetComponent<Image>();
        //uniTrans = uniBar.GetComponent<RectTransform>();
        //physTrans = physBar.GetComponent<RectTransform>();
        //magicTrans = magicBar.GetComponent<RectTransform>();
    }
}
