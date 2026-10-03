internal sealed class Hero
{
    public GridCell Cell { get; private set; }
    public int Health { get; private set; } = 3;
    public bool IsAlive => Health > 0;

    public Hero(GridCell initialCell)
    {
        Cell = initialCell;
    }

    public void MoveTo(GridCell cell)
    {
        Cell = cell;
    }


    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);
    }

}
