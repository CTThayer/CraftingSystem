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
        itemNameText.text = storableItem.item.itemName;
        //itemStatsText.text += "Base Value: " + storableItem.item.baseValue + "\n";
        //itemStatsText.text += "Mass: " + storableItem.item.mass + "\n";
        //itemStatsText.text += "Volume: " + storableItem.item.volume + "\n";

        //sb.Clear();
        sb.Length = 0;
        AddStatToText("Base Value", storableItem.item.baseValue);
        AddStatToText("Mass", storableItem.item.mass);
        AddStatToText("Volume", storableItem.item.volume);

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
