internal sealed class BattleState
{
    public Grid Grid { get; }
    public Hero Hero { get; }

    public BattleState()
    {
        Grid = new Grid(columns: 15, rows: 14);
        Hero = new Hero(new GridCell(7, 6));
    }

    public void MoveHeroTo(GridCell destination)
    {
        if (Grid.ContainsCell(destination))
        {
            Hero.MoveTo(destination);
        }
    }
}
