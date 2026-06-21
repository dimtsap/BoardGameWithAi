namespace Game2048.Engine;

public class Board
{
    public Board(int rows, int columns)
    {        
        this.Tiles = new int?[rows, columns];
        this.Rows = rows;
        this.Columns = columns;
        this.InitializeTiles();        
    }

    private List<int> EmptySpaces()
    {
        var emptyIndices = new List<int>();
        for (int i = 0; i < this.Rows; i++)
        {
            for (int j = 0; j < this.Columns; j++)
            {
                if (this.Tiles[i, j] == null)
                {
                    emptyIndices.Add(i * this.Columns + j);
                }
            }
        }

        return emptyIndices;
    }

    public bool CanMoveHorizontally()
    {
        int?[] row = new int?[this.Columns];
        for (int i = 0; i < this.Rows; i++)
        {
            this.ExtractRow(i, row);
            if (row.Any(x => x == null))
                return true;
            for (int j = 0; j < this.Columns - 1; j++)
            {
                if (row[j] == row[j + 1])
                {
                    return true;
                }
            }
            Array.Clear(row, 0, row.Length);
        }
        return false;
    }

    public bool CanMoveVertically()
    {
        int?[] column = new int?[this.Rows];
        for (int i = 0; i < this.Columns; i++)
        {
            this.ExtractColumn(i, column);
            if (column.Any(x => x == null))
                return true;
            for (int j = 0; j < this.Rows - 1; j++)
            {
                if (column[j] == column[j + 1])
                {
                    return true;
                }
            }
            Array.Clear(column, 0, column.Length);
        }
        return false;
    }

    public GameState CheckWin()
    {
        for (int i = 0; i < this.Rows; i++)
        {
            for (int j = 0; j < this.Columns; j++)
            {
                if (this.Tiles[i, j].HasValue && this.Tiles[i, j] == 2048)
                {
                    return GameState.Won;
                }
            }
        }
        return GameState.Continue;
    }

    public void AddRandomTiles()
    {
        var emptySpaces = this.EmptySpaces();

        var rnd = new Random();
        var randomIndex1 = rnd.Next(0, emptySpaces.Count);
        var index1 = emptySpaces[randomIndex1];
        emptySpaces.RemoveAt(randomIndex1);

        var row = index1 / this.Columns;
        var col = index1 % this.Columns;
        this.Tiles[row, col] = rnd.Next(0, 2) == 0 ? 2 : 4;
    }

    public void InitializeTiles()
    {
        Random rnd = new Random();
        var nInitialValues = rnd.Next(1, this.Rows * this.Columns);

        var indices = new List<int>();
        while (indices.Count < nInitialValues)
        {
            var index = rnd.Next(0, this.Rows * this.Columns);
            if (!indices.Contains(index))
            {
                indices.Add(index);
            }
        }

        foreach (var index in indices)
        {
            var row = index / this.Columns;
            var col = index % this.Columns;
            this.Tiles[row, col] = 2;
        }
    }

    public int?[,] Tiles { get; private set; }
    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public void MoveUp()
    {
        int?[] column = new int?[this.Rows];
        for (int j = 0; j < Columns; j++)
        {
            var nValues = this.ExtractColumn(j, column);

            this.Condense(column, nValues);

            this.AssignColumn(j, column);

            Array.Clear(column, 0, column.Length);
        }
    }

    public void MoveDown()
    {
        int?[] column = new int?[this.Rows];
        for (int j = 0; j < this.Columns; j++)
        {
            var nValues = this.ExtractColumn(j, column, true);

            this.Condense(column, nValues);

            this.AssignColumn(j, column, true);

            Array.Clear(column, 0, column.Length);
        }
    }

    public void MoveLeft()
    {
        int?[] row = new int?[this.Columns];
        for (int i = 0; i < this.Rows; i++)
        {
            var nValues = this.ExtractRow(i, row, false);

            this.Condense(row, nValues);

            this.AssignRow(i, row, false);

            Array.Clear(row, 0, row.Length);
        }
    }

    public void MoveRight()
    {
        int?[] row = new int?[this.Columns];
        for (int i = 0; i < this.Rows; i++)
        {
            var nValues = this.ExtractRow(i, row, true);

            this.Condense(row, nValues);

            this.AssignRow(i, row, true);

            Array.Clear(row, 0, row.Length);
        }
    }

    private void AssignRow(int indexRow, int?[] row, bool reverse = false)
    {
        var conditionReverse = (int x) => x >= 0;
        var conditionForward = (int x) => x < this.Columns;

        (int finalIndex, Func<int, bool> condition, int step) = reverse
            ? (this.Columns - 1, conditionReverse, -1)
            : (0, conditionForward, 1);

        for (int j = 0; j < this.Columns; j++)
        {
            if (row[j] == null)
            {
                continue;
            }
            this.Tiles[indexRow, finalIndex] = row[j];
            finalIndex += step;
        }
        for (int f = finalIndex; condition(f); f += step)
        {
            this.Tiles[indexRow, f] = null;
        }
    }

    private void AssignColumn(int indexColumn, int?[] column, bool reverse = false)
    {
        var conditionReverse = (int x) => x >= 0;
        var conditionForward = (int x) => x < this.Rows;

        (int finalIndex, Func<int, bool> condition, int step) = reverse
            ? (this.Rows - 1, conditionReverse, -1)
            : (0, conditionForward, 1);

        for (int i = 0; i < this.Rows; i++)
        {
            if (column[i] == null)
            {
                continue;
            }
            this.Tiles[finalIndex, indexColumn] = column[i];
            finalIndex+= step;
        }
        for (int f = finalIndex; condition(f); f += step)
        {
            this.Tiles[f, indexColumn] = null;
        }
    }

    private void Condense(int?[] array, int nValues)
    {
        var nPairs = nValues - 1;
        for (int indexPairs = 0; indexPairs < nPairs; indexPairs++)
        {
            if (array[indexPairs] == array[indexPairs + 1])
            {
                array[indexPairs] *= 2;
                array[indexPairs + 1] = null;
            }
        }
    }

    private int ExtractRow(int indexRow, int?[] row, bool reverse = false)
    {
        var conditionReverse = (int x) => x >= 0;
        var conditionForward = (int x) => x < this.Columns;

        (int indexStart, Func<int, bool> condition, int step) = reverse
            ? (this.Columns - 1, conditionReverse, -1)
            : (0, conditionForward, 1);

        int counterValues = 0;
        for (int j = indexStart; condition(j); j += step)
        {
            if (this.Tiles[indexRow, j] == null)
            {
                continue;
            }
            row[counterValues++] = this.Tiles[indexRow, j];
        }
        return counterValues;
    }

    private int ExtractColumn(int indexColumn, int?[] column, bool reverse = false)
    {
        var conditionReverse = (int x) => x >= 0;
        var conditionForward = (int x) => x < this.Rows;

        (int indexStart, Func<int, bool> condition, int step) = reverse
            ? (this.Rows - 1, conditionReverse, -1)
            : (0, conditionForward, 1);

        int counterValues = 0;
        for (int i = indexStart; condition(i); i += step)
        {
            if (this.Tiles[i, indexColumn] == null)
            {
                continue;
            }
            column[counterValues++] = this.Tiles[i, indexColumn];
        }
        return counterValues;
    }
}
