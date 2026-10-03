using Microsoft.Xna.Framework;

internal sealed class BattleViewport
{
    private const int CellSize = 40;
    private const int GridLeft = 340;
    private const int GridTop = 80;

    static public GridCell? GetGridCellAt(Point screenPosition, Grid grid)
    {
        var gridPixelBounds = new Rectangle(
            GridLeft,
            GridTop,
            grid.Columns * CellSize,
            grid.Rows * CellSize);

        if (!gridPixelBounds.Contains(screenPosition))
        {
            return null;
        }

        var cell = new GridCell(
            (screenPosition.X - GridLeft) / CellSize,
            (screenPosition.Y - GridTop) / CellSize);

        return grid.ContainsCell(cell) ? cell : null;
    }

    static public Rectangle GetCellBounds(GridCell cell)
    {
        return new Rectangle(
            GridLeft + cell.Column * CellSize,
            GridTop + cell.Row * CellSize,
            CellSize,
            CellSize);
    }
}
