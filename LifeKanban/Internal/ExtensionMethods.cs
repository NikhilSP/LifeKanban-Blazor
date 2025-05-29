namespace LifeKanban.Internal;

public static class ExtensionMethods
{
    public static void MoveItem<T>(this IList<T> list, int fromIndex, int toIndex)
    {
        var item = list[fromIndex];
        list.RemoveAt(fromIndex);
        list.Insert(fromIndex < toIndex ? toIndex - 1 : toIndex, item);
    }
}