internal sealed class Enemy(GridCell initialCell)
{
    public GridCell Cell { get; private set; } = initialCell;
    public int Health { get; private set; } = 3;
    public bool IsAlive => Health > 0;

    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);
    }

    public void MoveTo(GridCell cell)
    {
        Cell = cell;
    }



}
