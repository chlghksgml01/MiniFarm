using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : MonoBehaviour
{
    public Image iconImage;
    [SerializeField] TextMeshProUGUI textUI;
    private int quantity;
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
    public CollectableType type;

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
