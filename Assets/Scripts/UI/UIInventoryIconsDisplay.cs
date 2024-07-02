using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutGroup))]
public class UIInventoryIconsDisplay : MonoBehaviour
{
    public GameObject slotTemplate;
    public uint maxSlots = 6;
    public PlayerInventory inventory;

    public GameObject[] slots;

    [Header("Paths")]
    public string iconPath;
    [HideInInspector] public string targetedItemList;

    void Reset()
    {
        slotTemplate = transform.GetChild(0).gameObject;
        inventory = FindObjectOfType<PlayerInventory>();
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (!inventory) Debug.LogWarning("No inventory attached to the UI icon display");

        Type t = typeof(PlayerInventory);
        FieldInfo field = t.GetField(targetedItemList, BindingFlags.Public | BindingFlags.Instance);

        if (field == null)
        {
            Debug.LogWarning("The list in the inventory is not found");
            return;
        }

        List<PlayerInventory.Slot> items = (List<PlayerInventory.Slot>)field.GetValue(inventory);

        for (int i = 0; i < items.Count; i++)
        {
            if (i >= slots.Length)
            {
                Debug.LogWarning(string.Format("You have {0} inventory slots, but only {1} slots on the UI", items.Count, slots.Length));
                break;
            }

            Item item = items[i].item;

            Transform iconObj = slots[i].transform.Find(iconPath);
            if (iconObj)
            {
                Image icon = iconObj.GetComponentInChildren<Image>();

                if (!item) icon.color = new Color(1, 1, 1, 0);
                else
                {
                    icon.color = new Color(1, 1, 1, 1);
                    if (icon) icon.sprite = item.data.icon;
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
