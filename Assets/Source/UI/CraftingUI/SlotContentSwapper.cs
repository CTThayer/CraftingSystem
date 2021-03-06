﻿using System.Collections;
using System.Collections.Generic;

public class SlotContentSwapper
{
    public void Swap(ItemSlot A, ItemSlot B)
    {
        if (A is EquipmentSlot)
        {
            if (B is EquipmentSlot)
                SwapEquipmentSlotAndEquipmentSlot(A, B);
            else
                SwapItemSlotAndEquipmentSlot(B, A);
        }
        else if (A is PartSlot_Test)
        { 
            if (B is PartSlot_Test)
                SwapPartSlotAndPartSlot(A, B);
            else
                SwapItemSlotAndPartSlot(B, A);
        }
        else if (A is ResourceSlot)
        {
            if (B is ItemSlot)
                SwapItemSlotAndResourceSlot(B, A);
        }
        else
        {
            if (B is EquipmentSlot)
                SwapItemSlotAndEquipmentSlot(A, B);
            else if (B is PartSlot_Test)
                SwapItemSlotAndPartSlot(A, B);
            else if (B is ResourceSlot)
                SwapItemSlotAndResourceSlot(A, B);
            else
                SwapItemSlotAndItemSlot(A, B);
        }
    }

    public void SwapItemSlotAndItemSlot(ItemSlot A, ItemSlot B)
    {
        if (A.CanReceiveItem(B.storedItem) && B.CanReceiveItem(A.storedItem))
        {
            Storable sA = A.RemoveFromSlot();
            Storable sB = B.RemoveFromSlot();
            A.AddToSlot(sB);
            B.AddToSlot(sA);
        }
    }

    public void SwapItemSlotAndEquipmentSlot(ItemSlot A, ItemSlot B)
    {
        EquipmentSlot E = B as EquipmentSlot;
        if (E != null)
        {
            if (A.CanReceiveItem(B.storedItem) && E.CanReceiveItem(A.storedItem))
            {
                Storable sA = A.RemoveFromSlot();
                Storable sE = E.RemoveFromSlot();
                A.AddToSlot(sE);
                E.AddToSlot(sA);
            }
        }
    }

    public void SwapItemSlotAndPartSlot(ItemSlot A, ItemSlot B)
    {
        PartSlot_Test P = B as PartSlot_Test;
        if (P != null)
        {
            if (A.CanReceiveItem(B.storedItem) && P.CanReceiveItem(A.storedItem))
            {
                Storable sA = A.RemoveFromSlot();
                Storable sP = P.RemoveFromSlot();
                A.AddToSlot(sP);
                P.AddToSlot(sA);
            }
        }
    }

    public void SwapEquipmentSlotAndEquipmentSlot(ItemSlot A, ItemSlot B)
    {
        EquipmentSlot EA = B as EquipmentSlot;
        EquipmentSlot EB = B as EquipmentSlot;
        if (EA != null && EB != null)
        {
            if (EA.CanReceiveItem(EB.storedItem) && EB.CanReceiveItem(EA.storedItem))
            {
                Storable sA = EA.RemoveFromSlot();
                Storable sB = EB.RemoveFromSlot();
                EA.AddToSlot(sB);
                EB.AddToSlot(sA);
            }
        }
    }

    public void SwapPartSlotAndPartSlot(ItemSlot A, ItemSlot B)
    {
        PartSlot_Test PA = B as PartSlot_Test;
        PartSlot_Test PB = B as PartSlot_Test;
        if (PA != null && PB != null)
        {
            if (PA.CanReceiveItem(PB.storedItem) && PB.CanReceiveItem(PA.storedItem))
            {
                Storable sA = PA.RemoveFromSlot();
                Storable sB = PB.RemoveFromSlot();
                PA.AddToSlot(sA);
                PB.AddToSlot(sB);
            }
        }
    }

    public void SwapItemSlotAndResourceSlot(ItemSlot A, ItemSlot B)
    {
        ResourceSlot RB = B as ResourceSlot;
        if (RB != null)
        {
            if (A.CanReceiveItem(B.storedItem) && RB.CanReceiveItem(A.storedItem))
            {
                Storable sA = A.RemoveFromSlot();
                Storable sB = RB.RemoveFromSlot();
                A.AddToSlot(sB);
                RB.AddToSlot(sA);
            }
        }
    }

}
