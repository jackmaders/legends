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
    public bool HasMovedThisTurn { get; private set; }
    public bool HasAttackedThisTurn { get; private set; }
    public bool IsBattleOver => !Hero.IsAlive || !Enemy.IsAlive;


    public BattleState()
    {
        Grid = new Grid(columns: 15, rows: 14);
        Hero = new Hero(new GridCell(6, 7));
        Enemy = new Enemy(new GridCell(6, 9));
    }

    public void MoveHeroTo(GridCell destination)

    {
        var enemyOccupiesDestination = Enemy.IsAlive && Enemy.Cell == destination;

        if (IsBattleOver ||
            CurrentTurn != TurnOwner.Hero ||
            HasMovedThisTurn ||
            !Grid.ContainsCell(destination) ||
            destination == Hero.Cell ||
            enemyOccupiesDestination)
        {
            return;
        }

        Hero.MoveTo(destination);
        HasMovedThisTurn = true;
        EndTurnIfBothActionsUsed();
    }

    public void AttackEnemy()
    {
        if (IsBattleOver ||
            CurrentTurn != TurnOwner.Hero ||
            HasAttackedThisTurn ||
            !Enemy.IsAlive)
        {
            return;
        }

        Enemy.TakeDamage(1);
        HasAttackedThisTurn = true;
        EndTurnIfBothActionsUsed();
    }


    public void AttackHero()
    {
        if (IsBattleOver ||
            CurrentTurn != TurnOwner.Enemy ||
            HasAttackedThisTurn ||
            !Hero.IsAlive)
        {
            return;
        }

        Hero.TakeDamage(1);
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

    public void MoveEnemyTo(GridCell destination)
    {
        if (IsBattleOver ||
            CurrentTurn != TurnOwner.Enemy ||
            HasMovedThisTurn ||
            !Enemy.IsAlive ||
            !Grid.ContainsCell(destination) ||
            destination == Enemy.Cell ||
            destination == Hero.Cell)
        {
            return;
        }

        Enemy.MoveTo(destination);
        HasMovedThisTurn = true;
        EndTurnIfBothActionsUsed();
    }

    private void EndTurnIfBothActionsUsed()
    {
        if (HasMovedThisTurn && HasAttackedThisTurn)
        {
            EndTurn();
        }
    }

}
