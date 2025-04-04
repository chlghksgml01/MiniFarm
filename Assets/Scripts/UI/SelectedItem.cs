using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI textUI;

    public CollectableType type;


    void Update()
    {
        if (textUI.text == "1")
            textUI.text = "";
    }
}
