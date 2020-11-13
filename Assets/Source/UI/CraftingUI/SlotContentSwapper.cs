using System.Collections;
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
        else if (A is PartSlot)
        { 
            if (B is PartSlot)
                SwapPartSlotAndPartSlot(A, B);
            else
                SwapItemSlotAndPartSlot(B, A);
        }
        else
        {
            if (B is EquipmentSlot)
                SwapItemSlotAndEquipmentSlot(A, B);
            else if (B is PartSlot)
                SwapItemSlotAndPartSlot(A, B);
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

            //Storable temp = A.storedItem;
            //A.storedItem = B.storedItem;
            //B.storedItem = temp;
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

                //Storable temp = E.UnequipFromBone();
                //Equipable equipableItem = A.storedItem as Equipable;
                //E.storedItem = A.storedItem;
                //A.storedItem = temp;
            }
        }
    }

    public void SwapItemSlotAndPartSlot(ItemSlot A, ItemSlot B)
    {
        PartSlot P = B as PartSlot;
        if (P != null)
        {
            if (A.CanReceiveItem(B.storedItem) && P.CanReceiveItem(A.storedItem))
            {
                Storable sA = A.RemoveFromSlot();
                Storable sP = P.RemoveFromSlot();
                A.AddToSlot(sP);
                P.AddToSlot(sA);

                //Storable temp = P.storedItem;
                //A.storedItem = P.storedItem;
                //A.storedItem = temp;
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

                //Equipable equipableItemA = EA.storedItem as Equipable;
                //Equipable equipableItemB = EB.storedItem as Equipable;
                //EA.UnequipFromBone();
                //EB.UnequipFromBone();
                //EA.EquipToBone(equipableItemB);
                //EB.EquipToBone(equipableItemA);
            }
        }
    }

    public void SwapPartSlotAndPartSlot(ItemSlot A, ItemSlot B)
    {
        PartSlot PA = B as PartSlot;
        PartSlot PB = B as PartSlot;
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

}
