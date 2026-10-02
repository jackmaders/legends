internal sealed class Enemy
{
    public GridCell Cell { get; }

    public Enemy(GridCell initialCell)
    {
        Cell = initialCell;
    }
}
