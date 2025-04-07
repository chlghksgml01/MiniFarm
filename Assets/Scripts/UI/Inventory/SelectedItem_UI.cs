using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItem_UI : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI textUI;

    public CollectableType type;
    private int quantity;

    public Sprite Icon
    {
        get { return iconImage.sprite; }
        set { iconImage.sprite = value; }
    }
    public int Quantity
    {
        get { return quantity; }
        set
        {
            quantity = value;
            if (quantity > 1)
                textUI.text = quantity.ToString();
            else
                textUI.text = "";
        }
    }

    private void Awake()
    {
        iconImage.raycastTarget = false;
    }

    public void SetEmpty()
    {
        quantity = 0;
        type = CollectableType.NONE;
        iconImage.sprite = null;
        gameObject.SetActive(false);
    }
}
