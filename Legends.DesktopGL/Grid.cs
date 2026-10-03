internal sealed class Grid
{
    public int Columns { get; }
    public int Rows { get; }

    public Grid(int columns, int rows)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columns);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rows);

        Columns = columns;
        Rows = rows;
    }

    public bool ContainsCell(GridCell cell) =>
        cell.Column >= 0 &&
        cell.Column < Columns &&
        cell.Row >= 0 &&
        cell.Row < Rows;
}
