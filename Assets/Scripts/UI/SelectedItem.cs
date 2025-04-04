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
            if (quantity == 0)
                gameObject.SetActive(false);
        }
    }
    public CollectableType type;
}
