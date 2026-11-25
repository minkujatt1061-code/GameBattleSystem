namespace GameBattleSystem.Algorithms;

public static class SortAlgorithms
{
    /// <summary>
    /// Manual bubble sort. Repeatedly swaps neighbouring items that are out of order;
    /// stops early if a full pass makes no swaps. Stable, so equal items keep their order.
    /// </summary>
    public static void BubbleSort<T>(IList<T> items, IComparer<T> comparer)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparer);

        for (int pass = 0; pass < items.Count - 1; pass++)
        {
            bool swapped = false;

            for (int i = 0; i < items.Count - 1 - pass; i++)
            {
                if (comparer.Compare(items[i], items[i + 1]) > 0)
                {
                    (items[i], items[i + 1]) = (items[i + 1], items[i]);
                    swapped = true;
                }
            }

            if (!swapped) break;
        }
    }
}
