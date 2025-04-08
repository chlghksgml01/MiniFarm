using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    static ItemDropper instance;
    public static ItemDropper Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("ItemDropper");
                instance = obj.AddComponent<ItemDropper>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

}
