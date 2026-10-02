internal sealed class BattleState
{
    public Grid Grid { get; }
    public Hero Hero { get; }
    public Enemy Enemy { get; }

    public BattleState()
    {
        Grid = new Grid(columns: 15, rows: 14);
        Hero = new Hero(new GridCell(6, 7));
        Enemy = new Enemy(new GridCell(6, 9));
    }

    public void MoveHeroTo(GridCell destination)
    {
        if (Grid.ContainsCell(destination))
        {
            Hero.MoveTo(destination);
        }
    }
}
