﻿namespace Round.NET.AvaloniaApp.EncodeCalculator.Models.ItemManage;

public class Deduplication
{
    public static bool DeduplicationItem(string Name)
    {
        foreach (var item in ItemMange.Items)
        {
            if (item.FuncItem.Name == Name)
            {
                return true;
            }
        }
        return false;
    }
}