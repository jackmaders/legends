using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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



    // MonoGame calls Draw when it renders a frame; GameTime carries frame timing information.
    protected override void Draw(GameTime gameTime)
    {
        // Clear the current render target (the window back buffer) to solid blue.
        // Game exposes GraphicsDevice; the manager initializes and configures it.
        GraphicsDevice.Clear(Color.CornflowerBlue);

        const int columns = 15;
        const int rows = 14;
        const int tileSize = 40;
        const int boardLeft = 340;
        const int boardTop = 80;



        _spriteBatch.Begin();
        _spriteBatch.Draw(_pixel, new Rectangle(40, 40, 80, 80), Color.Orange);
        _spriteBatch.Draw(_circle, new Rectangle(160, 40, 80, 80), Color.Green);

        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                var x = boardLeft + column * tileSize;
                var y = boardTop + row * tileSize;
                var color = (row + column) % 2 == 0 ? Color.Gray : Color.DarkGray;

                _spriteBatch.Draw(
                    _pixel,
                    new Rectangle(x, y, tileSize, tileSize),
                    color);
            }
        }

        _spriteBatch.End();

        // Let Game draw any registered DrawableGameComponents, using the same timing data.
        base.Draw(gameTime);
    }
}
