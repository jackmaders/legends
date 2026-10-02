using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Create the game object; `using` disposes it when Run returns.
using var game = new LegendsGame();

// Start MonoGame's lifecycle: initialize the game, then update and draw until it exits.
game.Run();

// Extend MonoGame's Game class with this game's setup and callbacks.
internal sealed class LegendsGame : Game
{
    // Keep the graphics manager available for display settings such as size and fullscreen mode.
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;
    private Texture2D _circle = null!;
    private readonly Grid _grid = new(columns: 15, rows: 14);
    private const int CellSize = 40;
    private const int GridLeft = 340;
    private const int GridTop = 80;
    private GridCell? _hoveredCell;
    private bool _heroSelected;
    private ButtonState _previousLeftButton = ButtonState.Released;
    private readonly Hero _hero = new(new GridCell(7, 6));




    // Attach graphics setup to this game before Run begins initialization.
    public LegendsGame()
    {
        // Request a 1280-by-720 back buffer for the game window.
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720
        };

        IsMouseVisible = true;
        Window.Title = "Legends";
    }

    protected override void LoadContent()
    {
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

        _hoveredCell = GetGridCellAt(mousePosition);

        var justPressed =
            mouse.LeftButton == ButtonState.Pressed &&
            _previousLeftButton == ButtonState.Released;

        if (justPressed && _hoveredCell.HasValue)
        {
            _heroSelected = _hoveredCell.Value == _hero.Cell;
        }

        _previousLeftButton = mouse.LeftButton;


        base.Update(gameTime);
    }


    private GridCell? GetGridCellAt(Point screenPosition)
    {
        var gridPixelBounds = new Rectangle(
            GridLeft,
            GridTop,
            _grid.Columns * CellSize,
            _grid.Rows * CellSize);

        if (!gridPixelBounds.Contains(screenPosition))
        {
            return null;
        }

        var cell = new GridCell(
            (screenPosition.X - GridLeft) / CellSize,
            (screenPosition.Y - GridTop) / CellSize);


        if (!_grid.ContainsCell(cell))
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
        _spriteBatch.Draw(_pixel, new Rectangle(40, 40, 80, 80), Color.Orange);
        _spriteBatch.Draw(_circle, new Rectangle(160, 40, 80, 80), Color.Green);

        for (var row = 0; row < _grid.Rows; row++)
        {
            for (var column = 0; column < _grid.Columns; column++)
            {
                var x = GridLeft + column * CellSize;
                var y = GridTop + row * CellSize;
                var color = (row + column) % 2 == 0 ? Color.Gray : Color.DarkGray;

                var cell = new GridCell(column, row);
                if (_heroSelected && _hero.Cell == cell)
                {
                    color = Color.Orange;
                }
                else if (_hoveredCell == cell)
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

        var markerBounds = new Rectangle(
            GridLeft + _hero.Cell.Column * CellSize + inset,
            GridTop + _hero.Cell.Row * CellSize + inset,
            markerSize,
            markerSize);

        _spriteBatch.Draw(_circle, markerBounds, Color.Red);



        _spriteBatch.End();

        // Let Game draw any registered DrawableGameComponents, using the same timing data.
        base.Draw(gameTime);
    }
}
