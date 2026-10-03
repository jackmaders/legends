using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

internal sealed class BattleRenderer : IDisposable
{
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _pixel;
    private readonly Texture2D _circle;
    private readonly SpriteFont _healthFont;

    public BattleRenderer(
        GraphicsDevice graphicsDevice,
        ContentManager content,
        BattleViewport viewport)
    {
        _healthFont = content.Load<SpriteFont>("Fonts/Health");
        _spriteBatch = new SpriteBatch(graphicsDevice);

        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        const int circleSize = 64;
        _circle = new Texture2D(graphicsDevice, circleSize, circleSize);
        var pixels = new Color[circleSize * circleSize];
        var center = (circleSize - 1) / 2f;
        var radius = circleSize / 2f;

        for (var y = 0; y < circleSize; y++)
        {
            for (var x = 0; x < circleSize; x++)
            {
                var dx = x - center;
                var dy = y - center;
                var insideCircle = dx * dx + dy * dy <= radius * radius;
                pixels[y * circleSize + x] = insideCircle ? Color.White : Color.Transparent;
            }
        }

        _circle.SetData(pixels);
    }

    public void Draw(BattleState battleState, GridCell? hoveredCell, TurnOwner? selectedUnit)
    {
        _spriteBatch.Begin();

        var turnText = battleState.IsBattleOver
            ? "Battle over"
            : $"{battleState.CurrentTurn} turn";
        _spriteBatch.DrawString(_healthFont, turnText, new Vector2(40, 160), Color.White);

        _spriteBatch.Draw(_pixel, new Rectangle(40, 40, 80, 80), Color.Orange);
        _spriteBatch.Draw(_circle, new Rectangle(160, 40, 80, 80), Color.Green);

        DrawGrid(battleState, hoveredCell, selectedUnit);
        DrawUnit(battleState.Enemy.Cell, battleState.Enemy.Health, battleState.Enemy.IsAlive, Color.Green);
        DrawUnit(battleState.Hero.Cell, battleState.Hero.Health, battleState.Hero.IsAlive, Color.Red);

        _spriteBatch.End();
    }

    private void DrawGrid(BattleState battleState, GridCell? hoveredCell, TurnOwner? selectedUnit)
    {
        for (var row = 0; row < battleState.Grid.Rows; row++)
        {
            for (var column = 0; column < battleState.Grid.Columns; column++)
            {
                var cell = new GridCell(column, row);
                var color = (row + column) % 2 == 0 ? Color.Gray : Color.DarkGray;

                if (selectedUnit == TurnOwner.Hero && battleState.Hero.Cell == cell)
                {
                    color = Color.Orange;
                }
                else if (selectedUnit == TurnOwner.Enemy && battleState.Enemy.Cell == cell)
                {
                    color = Color.Pink;
                }

                if (hoveredCell == cell)
                {
                    color = Color.Yellow;
                }

                _spriteBatch.Draw(_pixel, BattleViewport.GetCellBounds(cell), color);
            }
        }
    }

    private void DrawUnit(GridCell cell, int health, bool isAlive, Color color)
    {
        if (!isAlive)
        {
            return;
        }

        const int inset = 4;
        var cellBounds = BattleViewport.GetCellBounds(cell);
        var markerBounds = new Rectangle(
            cellBounds.X + inset,
            cellBounds.Y + inset,
            cellBounds.Width - inset * 2,
            cellBounds.Height - inset * 2);

        _spriteBatch.Draw(_circle, markerBounds, color);

        var healthText = health.ToString();
        var textSize = _healthFont.MeasureString(healthText);
        var textPosition = new Vector2(
            markerBounds.Center.X - textSize.X / 2f,
            markerBounds.Center.Y - textSize.Y / 2f);

        _spriteBatch.DrawString(_healthFont, healthText, textPosition, Color.White);
    }

    public void Dispose()
    {
        _spriteBatch.Dispose();
        _pixel.Dispose();
        _circle.Dispose();
    }
}
