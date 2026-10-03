internal sealed class Enemy
{
    public GridCell Cell { get; }
    public int Health { get; private set; } = 3;

    public Enemy(GridCell initialCell)
    {
        Cell = initialCell;
    }

    public void TakeDamage(int amount)
    {
        Health = Math.Max(0, Health - amount);
    }

}
