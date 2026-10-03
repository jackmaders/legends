using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

internal sealed class LegendsGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly BattleState _battleState = new();
    private readonly BattleViewport _battleViewport = new();
    private readonly BattleInputController _battleInputController;
    private BattleRenderer _battleRenderer = null!;
    private GridCell? _hoveredCell;

    public LegendsGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 1280,
            PreferredBackBufferHeight = 720
        };

        _battleInputController = new BattleInputController(_battleState);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Legends";
    }

    protected override void LoadContent()
    {
        _battleRenderer = new BattleRenderer(GraphicsDevice, Content, _battleViewport);
    }

    protected override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var mousePosition = new Point(mouse.X, mouse.Y);
        _hoveredCell = BattleViewport.GetGridCellAt(mousePosition, _battleState.Grid);

        _battleInputController.Update(mouse, Keyboard.GetState(), _hoveredCell);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _battleRenderer.Draw(_battleState, _hoveredCell, _battleInputController.SelectedUnit);

        base.Draw(gameTime);
    }

    protected override void UnloadContent()
    {
        _battleRenderer.Dispose();
        base.UnloadContent();
    }
}
