using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private Text itemNameText;
    [SerializeField] private Text itemStatsText;

    private StringBuilder sb = new StringBuilder();

    public void ShowTooltip(Storable storableItem)
    {
        Item storedItem = storableItem.item.GetComponent<Item>();
        if (storedItem != null)
        {
            itemNameText.text = storedItem.itemName;
            sb.Length = 0;
            AddStatToText("Base Value", storedItem.baseValue);
            AddStatToText("Mass", storedItem.mass);
            AddStatToText("Volume", storedItem.volume);
        }
        else
        {
            ItemPart storedItemPart = storableItem.item.GetComponent<ItemPart>();
            if (storedItemPart != null)
            {
                itemNameText.text = storedItemPart.partType;
                sb.Length = 0;
                AddStatToText("Part Quality", storedItemPart.partQuality);
                AddStatToText("Max Durability", storedItemPart.maxDurability);
                AddStatToText("Current Durability", storedItemPart.currentDurability);
            }
        }
        itemStatsText.text = sb.ToString();

        this.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        this.gameObject.SetActive(false);
    }

    private void AddStatToText(string statName, float value)
    {
        if (sb.Length > 0)
            sb.AppendLine();
        sb.Append(statName);
        sb.Append(" ");
        sb.Append(value);
    }

    private void AddStatToText(string statName, int value)
    {
        if (sb.Length > 0)
            sb.AppendLine();
        sb.Append(statName);
        sb.Append(": ");
        sb.Append(value);
    }

}
