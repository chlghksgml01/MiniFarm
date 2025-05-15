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
        // ���� ��ũ�� -> �������� 
        if (Input.mouseScrollDelta.y > 0)
        {
            selectedSlotIdx--;
            CheckSlot();
            DrawSelectedUI();
        }
        // �Ʒ��� ��ũ�� -> ���������� 
        else if (Input.mouseScrollDelta.y < 0)
        {
            selectedSlotIdx++;
            DrawSelectedUI();
        }
    }

    private void CheckSlot()
    {
        // ������ ���Կ� ���� �ִٸ�
        if (!GameManager.Instance.player.inventory.slots[selectedSlotIdx].IsEmpty())
        {
            // �ִϸ��̼� ����
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
