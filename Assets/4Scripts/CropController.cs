
using UnityEngine;


public class CropController : MonoBehaviour
{
    [SerializeField] private ScriptableCropData cropData;

    private Sprite cropImage;

    private int currentGrowthLevel = 0;
    private int growthDays = 0;

    private void Awake()
    {
        cropImage = GetComponent<SpriteRenderer>().sprite;
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void Grow()
    {
        // ´Ù ÀÚ¶úÀ¸¸é return
        if (currentGrowthLevel >= cropData.growthLevel)
            return;

        growthDays++;

        if (growthDays >= cropData.growthDurations[currentGrowthLevel])
        {
            growthDays = 0;
            currentGrowthLevel++;
            //GameManager.Instance.tileManager.GrowCrop();
        }
    }

    public void Water()
    {
        //cropImage = cropData.wetCropImage[currentGrowthLevel];
    }
}