internal enum TurnOwner
{
    Hero,
    Enemy
}

internal sealed class BattleState
{
    public Grid Grid { get; }
    public Hero Hero { get; }
    public Enemy Enemy { get; }
    public TurnOwner CurrentTurn { get; private set; } = TurnOwner.Hero;
    public GridCell CurrentUnitCell =>
        CurrentTurn == TurnOwner.Hero ? Hero.Cell : Enemy.Cell;
    public bool HasMovedThisTurn { get; private set; }
    public bool HasAttackedThisTurn { get; private set; }
    public bool IsBattleOver => !Hero.IsAlive || !Enemy.IsAlive;


    public BattleState()
    {
        Grid = new Grid(columns: 15, rows: 14);
        Hero = new Hero(new GridCell(6, 7));
        Enemy = new Enemy(new GridCell(6, 9));
    }

    public void MoveCurrentUnitTo(GridCell destination)
    {
        if (IsBattleOver ||
            HasMovedThisTurn ||
            !Grid.ContainsCell(destination) ||
            destination == CurrentUnitCell ||
            IsOpponentCell(destination))
        {
            return;
        }

        if (CurrentTurn == TurnOwner.Hero)
        {
            Hero.MoveTo(destination);
        }
        else
        {
            Enemy.MoveTo(destination);
        }

        HasMovedThisTurn = true;
        EndTurnIfBothActionsUsed();
    }

    public void AttackOpponent()
    {
        if (IsBattleOver || HasAttackedThisTurn)
        {
            return;
        }

        if (CurrentTurn == TurnOwner.Hero)
        {
            Enemy.TakeDamage(1);
        }
        else
        {
            Hero.TakeDamage(1);
        }

        HasAttackedThisTurn = true;
        EndTurnIfBothActionsUsed();
    }



    public void EndTurn()
    {
        if (IsBattleOver)
        {
            return;
        }

        CurrentTurn = CurrentTurn == TurnOwner.Hero
            ? TurnOwner.Enemy
            : TurnOwner.Hero;

        HasMovedThisTurn = false;
        HasAttackedThisTurn = false;
    }

    public bool IsOpponentCell(GridCell cell)
    {
        return CurrentTurn == TurnOwner.Hero
            ? Enemy.IsAlive && Enemy.Cell == cell
            : Hero.IsAlive && Hero.Cell == cell;
    }

    private void EndTurnIfBothActionsUsed()
    {
        if (HasMovedThisTurn && HasAttackedThisTurn)
        {
            EndTurn();
        }
    }

}
