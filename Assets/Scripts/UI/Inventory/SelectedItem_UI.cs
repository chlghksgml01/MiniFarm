using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItem_UI : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI textUI;

    public CollectableType type;
    private int count;

    public Sprite Icon
    {
        get { return iconImage.sprite; }
        set { iconImage.sprite = value; }
    }

    public int Count
    {
        get { return count; }
        set
        {
            count = value;
            if (count > 1)
                textUI.text = count.ToString();
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
        count = 0;
        type = CollectableType.NONE;
        iconImage.sprite = null;
        gameObject.SetActive(false);
    }
}
