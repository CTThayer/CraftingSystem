using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PartSlotTooltip : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text infoText;

    private StringBuilder sb = new StringBuilder();

    public void ShowTooltip(PartSlot partSlot)
    {
        if (partSlot != null)
        {
            if (partSlot.storedItem != null)
            {
                ItemPart storedItemPart = partSlot.storedItem.gameObject.GetComponent<ItemPart>();
                if (storedItemPart != null)
                {
                    nameText.text = storedItemPart.gameObject.name;
                    sb.Length = 0;
                    AddStatToText("Mass", storedItemPart.physicalStats.mass);
                    AddStatToText("Volume", storedItemPart.physicalStats.volume);
                    AddStatToText("Part Quality", storedItemPart.partQuality);
                    AddStatToText("Max Durability", storedItemPart.maxDurability);
                    AddStatToText("Current Durability", storedItemPart.currentDurability);
                }
                infoText.text = sb.ToString();
            }
            else
            {
                nameText.text = "Part Slot";
                sb.Length = 0;
                sb.Append("Accepts: ");
                for (int i = 0; i < partSlot.allowedPartTypes.Length; i++)
                {
                    sb.AppendLine();
                    sb.Append("\t");
                    sb.Append(partSlot.allowedPartTypes[i]);
                }
                infoText.text = sb.ToString();
            }
            this.gameObject.SetActive(true);
        }
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
