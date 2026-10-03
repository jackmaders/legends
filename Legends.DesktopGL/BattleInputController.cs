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
        if (SelectedUnit.HasValue)
        {
            if (_battleState.IsOpponentCell(clickedCell))
            {
                _battleState.AttackOpponent();
            }
            else
            {
                _battleState.MoveCurrentUnitTo(clickedCell);
            }

            SelectedUnit = null;
        }
        else if (clickedCell == _battleState.CurrentUnitCell)
        {
            SelectedUnit = _battleState.CurrentTurn;
        }
    }
}
