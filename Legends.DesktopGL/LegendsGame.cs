using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Extend MonoGame's Game class with this game's setup and callbacks.
internal sealed class LegendsGame : Game
{
    // Keep the graphics manager available for display settings such as size and fullscreen mode.
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;
    private Texture2D _circle = null!;
    private const int CellSize = 40;
    private const int GridLeft = 340;
    private const int GridTop = 80;
    private GridCell? _hoveredCell;
    private bool _heroSelected;
    private bool _enemySelected;
    private ButtonState _previousLeftButton = ButtonState.Released;
    private KeyboardState _previousKeyboardState;
    private readonly BattleState _battleState = new();
    private SpriteFont _healthFont = null!;

    // Attach graphics setup to this game before Run begins initialization.
    public LegendsGame()
    {
        // Request a 1280-by-720 back buffer for the game window.
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720
        };

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Legends";
    }

    protected override void LoadContent()
    {

        _healthFont = Content.Load<SpriteFont>("Fonts/Health");


        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        const int size = 64;
        _circle = new Texture2D(GraphicsDevice, size, size);
        var pixels = new Color[size * size];

        var center = (size - 1) / 2f;
        var radius = size / 2f;

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var dx = x - center;
                var dy = y - center;
                var insideCircle = dx * dx + dy * dy <= radius * radius;

                pixels[y * size + x] = insideCircle ? Color.White : Color.Transparent;
            }
        }

        _circle.SetData(pixels);

    }

    protected override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var mousePosition = new Point(mouse.X, mouse.Y);

        var keyboard = Keyboard.GetState();
        var spaceJustPressed = keyboard.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space);

        if (spaceJustPressed)
        {
            _battleState.EndTurn();
            _heroSelected = false;
            _enemySelected = false;
        }

        _hoveredCell = GetGridCellAt(mousePosition);

        var justPressed =
            mouse.LeftButton == ButtonState.Pressed &&
            _previousLeftButton == ButtonState.Released;

        if (justPressed && _hoveredCell.HasValue && !_battleState.IsBattleOver)
        {
            var clickedCell = _hoveredCell.Value;

            if (_heroSelected)
            {
                if (clickedCell == _battleState.Enemy.Cell && _battleState.Enemy.IsAlive)
                {
                    _battleState.AttackEnemy();
                }
                else
                {
                    _battleState.MoveHeroTo(clickedCell);
                }

                _heroSelected = false;
            }
            else if (_enemySelected)
            {
                if (clickedCell == _battleState.Hero.Cell && _battleState.Hero.IsAlive)
                {
                    _battleState.AttackHero();
                }
                else
                {
                    _battleState.MoveEnemyTo(clickedCell);
                }

                _enemySelected = false;
            }
            else
            {
                _heroSelected =
                    _battleState.CurrentTurn == TurnOwner.Hero &&
                    _battleState.Hero.IsAlive &&
                    clickedCell == _battleState.Hero.Cell;

                _enemySelected =
                    _battleState.CurrentTurn == TurnOwner.Enemy &&
                    _battleState.Enemy.IsAlive &&
                    clickedCell == _battleState.Enemy.Cell;
            }
        }


        _previousLeftButton = mouse.LeftButton;
        _previousKeyboardState = keyboard;


        base.Update(gameTime);
    }


    private GridCell? GetGridCellAt(Point screenPosition)
    {
        var gridPixelBounds = new Rectangle(
            GridLeft,
            GridTop,
            _battleState.Grid.Columns * CellSize,
            _battleState.Grid.Rows * CellSize);

        if (!gridPixelBounds.Contains(screenPosition))
        {
            return null;
        }

        var cell = new GridCell(
            (screenPosition.X - GridLeft) / CellSize,
            (screenPosition.Y - GridTop) / CellSize);


        if (!_battleState.Grid.ContainsCell(cell))
        {
            return null;
        }

        return cell;
    }




    // MonoGame calls Draw when it renders a frame; GameTime carries frame timing information.
    protected override void Draw(GameTime gameTime)
    {
        // Clear the current render target (the window back buffer) to solid blue.
        // Game exposes GraphicsDevice; the manager initializes and configures it.
        GraphicsDevice.Clear(Color.CornflowerBlue);


        _spriteBatch.Begin();

        var turnText = _battleState.IsBattleOver
            ? "Battle over"
            : $"{_battleState.CurrentTurn} turn";

        _spriteBatch.DrawString(_healthFont, turnText, new Vector2(40, 160), Color.White);


        _spriteBatch.Draw(_pixel, new Rectangle(40, 40, 80, 80), Color.Orange);
        _spriteBatch.Draw(_circle, new Rectangle(160, 40, 80, 80), Color.Green);

        for (var row = 0; row < _battleState.Grid.Rows; row++)
        {
            for (var column = 0; column < _battleState.Grid.Columns; column++)
            {
                var x = GridLeft + column * CellSize;
                var y = GridTop + row * CellSize;
                var color = (row + column) % 2 == 0 ? Color.Gray : Color.DarkGray;

                var cell = new GridCell(column, row);
                if (_heroSelected && _battleState.Hero.Cell == cell)
                {
                    color = Color.Orange;
                }
                else if (_enemySelected && _battleState.Enemy.Cell == cell)
                {

                    color = Color.Pink;

                }
                if (_hoveredCell == cell)
                {
                    color = Color.Yellow;
                }




                _spriteBatch.Draw(
                    _pixel,
                    new Rectangle(x, y, CellSize, CellSize),
                    color);
            }
        }

        const int inset = 4;
        var markerSize = CellSize - inset * 2;

        var heroMarker = new Rectangle(
            GridLeft + _battleState.Hero.Cell.Column * CellSize + inset,
            GridTop + _battleState.Hero.Cell.Row * CellSize + inset,
            markerSize,
            markerSize);


        var enemyMarker = new Rectangle(
            GridLeft + _battleState.Enemy.Cell.Column * CellSize + inset,
            GridTop + _battleState.Enemy.Cell.Row * CellSize + inset,
            markerSize,
            markerSize);

        var enemy = _battleState.Enemy;

        if (enemy.IsAlive)
        {
            _spriteBatch.Draw(_circle, enemyMarker, Color.Green);

            var hpText = enemy.Health.ToString();
            var textSize = _healthFont.MeasureString(hpText);
            var textPosition = new Vector2(
                enemyMarker.Center.X - textSize.X / 2f,
                enemyMarker.Center.Y - textSize.Y / 2f);

            _spriteBatch.DrawString(_healthFont, hpText, textPosition, Color.White);
        }

        var hero = _battleState.Hero;
        if (hero.IsAlive)
        {
            _spriteBatch.Draw(_circle, heroMarker, Color.Red);

            var hpText = hero.Health.ToString();
            var textSize = _healthFont.MeasureString(hpText);
            var textPosition = new Vector2(
                heroMarker.Center.X - textSize.X / 2f,
                heroMarker.Center.Y - textSize.Y / 2f);

            _spriteBatch.DrawString(_healthFont, hpText, textPosition, Color.White);
        }





        _spriteBatch.End();

        // Let Game draw any registered DrawableGameComponents, using the same timing data.
        base.Draw(gameTime);
    }
}
