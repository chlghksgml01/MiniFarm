using System.Collections.Generic;
using UnityEngine;

public class ToolBar_UI : MonoBehaviour
{
    private Slot_UI selectedSlot;
    private int slotCount;
    private int selectedSlotIdx = 0;
    private float initialSelectedUIPosX;

    [SerializeField] public List<Slot_UI> slotsUIs;
    [SerializeField] private float nextSelectedUIDistance = 102.5f;
    [SerializeField] private GameObject selectedUI;

    private void Awake()
    {
        slotCount = slotsUIs.Count;
        initialSelectedUIPosX = selectedUI.transform.position.x;
    }

    private void Update()
    {
        // 위로 스크롤 -> 왼쪽으로 
        if (Input.mouseScrollDelta.y > 0)
        {
            selectedSlotIdx--;
            CheckSlot();
            DrawSelectedUI();
        }
        // 아래로 스크롤 -> 오른쪽으로 
        else if (Input.mouseScrollDelta.y < 0)
        {
            selectedSlotIdx++;
            DrawSelectedUI();
        }
    }

    private void CheckSlot()
    {
        // 선택한 슬롯에 뭔가 있다면
        if (!GameManager.Instance.player.inventory.slots[selectedSlotIdx].IsEmpty())
        {
            // 애니메이션 변경
        }
    }

    private void DrawSelectedUI()
    {
        if (selectedSlotIdx < 0)
            selectedSlotIdx = slotCount - 1;
        if (selectedSlotIdx >= slotCount)
            selectedSlotIdx = 0;

        Vector3 nextSelectedUIPos = selectedUI.transform.position;
        nextSelectedUIPos.x = initialSelectedUIPosX + nextSelectedUIDistance * selectedSlotIdx;
        selectedUI.transform.position = nextSelectedUIPos;
    }
}
