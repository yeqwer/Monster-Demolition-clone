using System.Collections.Generic;

public class EquipmentInventory
{
    private Queue<EquipmentType> _equipment = new Queue<EquipmentType>();

    public int ItemsCount => _equipment.Count;

    public bool PeekFirstItem(out EquipmentType type)
    {
        type = 0;
        if (_equipment.Count <= 0)
            return false;

        type = _equipment.Peek();
        return true;
    }

    public void RemoveFirstItem()
    {
        if (_equipment.Count <= 0)
            return;

        _equipment.Dequeue();
    }

    public void AddItemToEnd(EquipmentType type)
    {
        _equipment.Enqueue(type);
    }

    public bool TakeFirstItem(out EquipmentType type)
    {
        type = 0;
        if (ItemsCount <= 0)
            return false;

        type = _equipment.Dequeue();
        return true;
    }
}
