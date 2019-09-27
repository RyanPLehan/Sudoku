using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    /// <summary>
    /// Sudoku puzzle 
    /// </summary>
    /// <history>
    /// Date        Author                  Description
    /// 2019.09.26  Lehan, Ryan             Re-factored into separation of concerns from monolithic class
    /// </history>
    public class Puzzle
    {
        #region Constants
        public const byte MIN_VALUE = 1;
        public const byte MAX_VALUE = 9;                // 9
        public const byte PUZZLE_GRID_SIZE = 9;         // 9x9 grid 9
        public const byte QUADRIENT_GRID_SIZE = 3;      // 3x3 grid 3
        public const byte MAX_CELLS = PUZZLE_GRID_SIZE * PUZZLE_GRID_SIZE;
        #endregion

        #region Classes
        public class Cell
        {

            private byte? _value;
            public int Row { get; private set; }
            public int Column { get; private set; }
            public bool IsLocked { get; set; }


            #region Constructors
            /// <summary>
            /// Constructor that can only be instaniated within this package
            /// </summary>
            /// <param name="row"></param>
            /// <param name="col"></param>
            internal Cell(int row, int col)
            {
                Row = row;
                Column = col;
                Clear();
            }
            #endregion

            #region Properties
            public byte? Value
            {
                get
                { return _value; }

                set
                {
                    if (value != null &&
                        (value < MIN_VALUE || value > MAX_VALUE))
                        throw new ArgumentOutOfRangeException(nameof(value));

                    if (!IsLocked)
                        _value = value;
                }
            }
            #endregion

            #region Methods
            /// <summary>
            /// This will reset the cell's Value and IsLocked properties to their default settings
            /// </summary>
            internal void Clear()
            {
                _value = null;
                IsLocked = false;
            }

            /// <summary>
            /// This will increment the cell's value by 1.
            /// If the value is null, it will be initialized to the minimum value
            /// If the value is currently at the maximum value, then it will be reset to minimum value
            /// </summary>
            internal void Increment()
            {
                if (!IsLocked)
                {
                    if (_value == null || _value == MAX_VALUE)
                        _value = MIN_VALUE;
                    else
                        _value++;
                }
            }

            /// <summary>
            /// Convert to string
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Value?.ToString() ?? String.Empty;
            }
            #endregion
        }
        #endregion

        private Cell[,] _puzzle;


        #region Constructors
        public Puzzle()
        {
            _puzzle = Initialize();
        }
        #endregion

        #region Methods
        private void Clear(bool includeLockedValues)
        {
            IEnumerable<Cell> cells = GetCells();           
            foreach (Cell cell in cells)
            {
                if (!cell.IsLocked || includeLockedValues)
                    cell.Clear();
            }
        }

        /// <summary>
        /// Create and Initialize_puzzle
        /// </summary>
        /// <returns></returns>
        private Cell[,] Initialize()
        {
            // Create puzzle
            Cell[,] puzzle = new Cell[PUZZLE_GRID_SIZE, PUZZLE_GRID_SIZE];

            // Populate puzzle with Cell information
            for (int row = 0; row < PUZZLE_GRID_SIZE; row++)
                for (int col = 0; col < PUZZLE_GRID_SIZE; col++)
                    puzzle[row, col] = new Cell(row, col);

            return puzzle;
        }



        /// <summary>
        /// Clear the entire puzzle of all non-Locked values
        /// </summary>
        public void Clear()
        {
            Clear(false);
        }


        /// <summary>
        /// Clear the entire puzzle of all values including Locked values
        /// </summary>
        public void ClearAll()
        {
            Clear(true);
        }

        /// <summary>
        /// Clone this puzzle.
        /// </summary>
        /// <remarks>
        /// This is useful for keeping a solved puzzle, ie to use for hints, and the one the player is working on
        /// </remarks>
        /// <returns>Puzzle</returns>
        public Puzzle Clone()
        {
            Puzzle clonePuzzle = new Puzzle();
            Cell cloneCell = null;

            // Iterate through this puzzle and set the values of the cloned puzzle
            IEnumerable<Cell> cells = GetCells();
            foreach (Cell cell in cells)
            {
                // Technically, we can access the private member _puzzle within the clonePuzzle.  Some people view it as bad practice
                // cloneCell = clonePuzzle._puzzle[cell.Row, cell.Column];

                cloneCell = clonePuzzle.GetCell(cell.Row, cell.Column);
                cloneCell.Value = cell.Value;
                cloneCell.IsLocked = cell.IsLocked;
            }

            return clonePuzzle;
        }

        /// <summary>
        /// Return a single cell
        /// </summary>
        /// <param name="row">Row value zero based</param>
        /// <param name="column">Column value zero based</param>
        /// <returns></returns>
        public Cell GetCell(int row, int column)
        {
            // Validate parameters
            if (row < 0 ||
                row >= PUZZLE_GRID_SIZE)
                throw new ArgumentOutOfRangeException(nameof(row));

            if (column < 0 ||
                column >= PUZZLE_GRID_SIZE)
                throw new ArgumentOutOfRangeException(nameof(column));


            return _puzzle[row, column];
        }


        /// <summary>
        /// Return an enumerable list of cells
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cell> GetCells()
        {
            for (int row = 0; row < PUZZLE_GRID_SIZE; row++)
                for (int col = 0; col < PUZZLE_GRID_SIZE; col++)
                {
                    yield return _puzzle[row, col];
                }
        }


        /// <summary>
        /// Returns 2 dimensional cloned array
        /// </summary>
        /// <returns></returns>
        public Cell[,] ToArray()
        {
            return _puzzle;
        }
        #endregion
    }
}
