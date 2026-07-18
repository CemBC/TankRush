class ClassicTetrisGame : TetrisGame
    {
        private Tetromino _activeTetromino;
        private int _activeRow;
        private int _activeColumn;
        private bool _hasActive;

        protected override bool PlaceTetromino(Tetromino tetromino, int getRow, int getColumn)
        {
            if (!CheckTetromino(tetromino, getRow, getColumn))
                return false;

            _activeTetromino = tetromino;
            _activeRow = getRow;
            _activeColumn = getColumn;
            _hasActive = true;

            PaintTetromino(_activeTetromino, _activeRow, _activeColumn, true);
            return true;
        }

        protected override void Left()
        {
            if (!_hasActive) return;
            TryMove(0, -1);
        }

        protected override void Right()
        {
            if (!_hasActive) return;
            TryMove(0, +1);
        }

        protected override void Rotate()
{
    if (!_hasActive) return;

    PaintTetromino(_activeTetromino, _activeRow, _activeColumn, false);

    var rotated = RotateCCW(_activeTetromino);

    if (CheckTetromino(rotated, _activeRow, _activeColumn))
    {
        _activeTetromino = rotated;
        PaintTetromino(_activeTetromino, _activeRow, _activeColumn, true);
    }
    else
    {
        PaintTetromino(_activeTetromino, _activeRow, _activeColumn, true);
    }
}


        protected override bool Tick()
        {
            if (_hasActive)
            {
                // 1 adım düşür
                var moved = TryMove(-1, 0);

                if (!moved)
                {
                    // artık düşemiyor => yerleşti
                    _hasActive = false;

                    // tamamlanan satırları temizle
                    ClearFullRows();
                }

                return true;
            }
            else
            {
                // aktif yoksa => sıradaki tetrominoyu spawn et
                var next = GetNextTetromino();
                var cell = GetPlacementCell(next);

                // yerleştiremiyorsak oyun biter
                return PlaceTetromino(next, cell.GetRow(), cell.GetColumn());
            }
        }

        private bool TryMove(int deltaRow, int deltaCol)
        {
            // aktif parçayı geçici kaldır
            PaintTetromino(_activeTetromino, _activeRow, _activeColumn, false);

            var newRow = _activeRow + deltaRow;
            var newCol = _activeColumn + deltaCol;

            if (CheckTetromino(_activeTetromino, newRow, newCol))
            {
                _activeRow = newRow;
                _activeColumn = newCol;
                PaintTetromino(_activeTetromino, _activeRow, _activeColumn, true);
                return true;
            }

            // hareket yok => geri koy
            PaintTetromino(_activeTetromino, _activeRow, _activeColumn, true);
            return false;
        }

        private void PaintTetromino(Tetromino tetromino, int row, int col, bool occupied)
        {
            var tMap = tetromino.GetTetrominoMap();
            for (var i = 0; i < tMap.GetLength(0); i++)
            {
                for (var j = 0; j < tMap.GetLength(1); j++)
                {
                    if (!tMap[i, j]) continue;

                    var cell = Map.GetCell(row + i, col + j);
                    // normalde null gelmez (CheckTetromino ile garanti), ama güvenli olsun:
                    if (cell != null) cell.SetOccupied(occupied);
                }
            }
        }

        private void ClearFullRows()
        {
            for (var r = 0; r < Map.GetHeight(); r++)
            {
                if (IsRowFull(r))
                {
                    RemoveRow(r);
                    r--; // aynı index'e tekrar bak (çünkü üstler aşağı indi)
                }
            }
        }

        private bool IsRowFull(int row)
        {
            for (var c = 0; c < Map.GetWidth(); c++)
            {
                if (!Map.GetCell(row, c).IsOccupied())
                    return false;
            }
            return true;
        }

        private void RemoveRow(int row)
        {
            // row ve üstünü aşağı kaydır
            for (var r = row; r < Map.GetHeight() - 1; r++)
            {
                for (var c = 0; c < Map.GetWidth(); c++)
                {
                    var above = Map.GetCell(r + 1, c).IsOccupied();
                    Map.GetCell(r, c).SetOccupied(above);
                }
            }

            // en üst satırı boşalt
            var top = Map.GetHeight() - 1;
            for (var c = 0; c < Map.GetWidth(); c++)
                Map.GetCell(top, c).SetOccupied(false);
        }

        private Tetromino RotateCCW(Tetromino t)
{
    int n = t.GetScale();
    var src = t.GetTetrominoMap();     // src[row, col]  row=0 bottom
    var rot = new bool[n, n];

    // CCW rotation (row=0 bottom coordinate system):
    // rot[newRow, newCol] = src[newCol, n-1-newRow]
    for (int r = 0; r < n; r++)
    {
        for (int c = 0; c < n; c++)
        {
            rot[r, c] = src[c, n - 1 - r];
        }
    }

    // Tetromino(string[]) constructor expects rows top->bottom,
    // and internally maps them to row indices (scale - i - 1).
    var lines = new string[n];
    for (int i = 0; i < n; i++)
    {
        int internalRow = n - 1 - i; // top row first
        var chars = new char[n];
        for (int j = 0; j < n; j++)
        {
            chars[j] = rot[internalRow, j] ? '#' : '.';
        }
        lines[i] = new string(chars);
    }

    return new Tetromino(lines);
}
    }

    
