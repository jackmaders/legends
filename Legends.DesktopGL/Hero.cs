internal sealed class Hero
{
    public GridCell Cell { get; private set; }

    public Hero(GridCell initialCell)
    {
        Cell = initialCell;
    }
}
