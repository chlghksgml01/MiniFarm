using TMPro;
using UnityEngine;

public class SelectedItem_UI : MonoBehaviour
{
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (text.text == "1")
            text.text = "";
    }
}
