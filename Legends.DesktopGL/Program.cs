

// Create the game object; `using` disposes it when Run returns.
using var game = new LegendsGame();

// Start MonoGame's lifecycle: initialize the game, then update and draw until it exits.
game.Run();
