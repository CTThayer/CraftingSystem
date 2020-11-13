using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ISlotPanelIO
{
    bool SetDelegateActions(Action<ItemSlot>[] delegates);
    ItemSlot CanAdd(ItemSlot input);
}
