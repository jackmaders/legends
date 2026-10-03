using Microsoft.Xna.Framework.Input;

internal sealed class BattleInputController(BattleState battleState)
{
    private readonly BattleState _battleState = battleState;
    private ButtonState _previousLeftButton = ButtonState.Released;
    private KeyboardState _previousKeyboardState;

    public TurnOwner? SelectedUnit { get; private set; }


    public void Update(MouseState mouse, KeyboardState keyboard, GridCell? hoveredCell)
    {
        var spaceJustPressed =
            keyboard.IsKeyDown(Keys.Space) &&
            _previousKeyboardState.IsKeyUp(Keys.Space);

        if (spaceJustPressed)
        {
            _battleState.EndTurn();
            SelectedUnit = null;
        }

        var leftButtonJustPressed =
            mouse.LeftButton == ButtonState.Pressed &&
            _previousLeftButton == ButtonState.Released;

        if (leftButtonJustPressed && hoveredCell.HasValue && !_battleState.IsBattleOver)
        {
            HandleCellClick(hoveredCell.Value);
        }

        _previousLeftButton = mouse.LeftButton;
        _previousKeyboardState = keyboard;
    }

    private void HandleCellClick(GridCell clickedCell)
    {
        if (SelectedUnit == TurnOwner.Hero)
        {
            if (clickedCell == _battleState.Enemy.Cell && _battleState.Enemy.IsAlive)
            {
                _battleState.AttackEnemy();
            }
            else
            {
                _battleState.MoveHeroTo(clickedCell);
            }

            SelectedUnit = null;
        }
        else if (SelectedUnit == TurnOwner.Enemy)
        {
            if (clickedCell == _battleState.Hero.Cell && _battleState.Hero.IsAlive)
            {
                _battleState.AttackHero();
            }
            else
            {
                _battleState.MoveEnemyTo(clickedCell);
            }

            SelectedUnit = null;
        }
        else if (_battleState.CurrentTurn == TurnOwner.Hero &&
                 _battleState.Hero.IsAlive &&
                 clickedCell == _battleState.Hero.Cell)
        {
            SelectedUnit = TurnOwner.Hero;
        }
        else if (_battleState.CurrentTurn == TurnOwner.Enemy &&
                 _battleState.Enemy.IsAlive &&
                 clickedCell == _battleState.Enemy.Cell)
        {
            SelectedUnit = TurnOwner.Enemy;
        }
    }
}
