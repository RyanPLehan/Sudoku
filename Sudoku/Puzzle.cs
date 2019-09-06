using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    /// <summary>
    /// Classic Sudoku puzzle including solving routine that utilizes the Backtracking algorithm (recursive and non-recursive routines)
    /// </summary>
    /// <history>
    /// Date        Author                  Description
    /// 2019.09.01  Lehan, Ryan             Complete re-write from original
    /// </history>
    public class Puzzle
    {
        #region Constants
        public const byte MIN_VALUE = 1;
        public const byte MAX_VALUE = 9;                // 9
        public const byte PUZZLE_GRID_SIZE = 9;         // 9x9 grid 9
        public const byte QUADRIENT_GRID_SIZE = 3;      // 3x3 grid 3
        #endregion

        #region Classes
        public class Cell
        {
            private byte? _value;
            public int Row { get; private set; }
            public int Column { get; private set; }
            public bool IsLocked { get; set; }
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
        }
        #endregion

        #region Class Members
        /// <summary>
        /// By making this private, it is hiding the implementation and not allowing an outside consumer replace a valid cell with an invalid cell
        /// </summary>
        private Cell[,] _puzzle;
        #endregion

        #region Constructors
        public Puzzle()
        {
            _puzzle = Initialize();
        }
        #endregion

        #region Methods
        #region --Private
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
        /// This will transpose the 2 dimensional array to a single dimensional array
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="rowLength"></param>
        /// <param name="colIndex"></param>
        /// <param name="colLength"></param>
        /// <returns></returns>
        private Cell[] Transpose(int rowIndex, int rowLength, int colIndex, int colLength)
        {
            // Calculate maximum array size
            Cell[] cells = new Cell[rowLength * colLength];

            int rowMax = rowIndex + rowLength;
            int colMax = colIndex + colLength;

            // Iterate through puzzle and reference to (not clone) existing Cells
            int idx = 0;
            for (int row = rowIndex; row < rowMax; row++)
                for (int col = colIndex; col < colMax; col++)
                {
                    cells[idx++] = _puzzle[row, col];
                }

            return cells;
        }


        #region -- Validation Routines --
        /// <summary>
        /// Checks to see if a section of cells is valid.  A section of cells would be a single row, single column or single quadrient of cells.
        /// </summary>
        /// <remarks>
        /// All cells must have a value and the values must be unique within the section of cells
        /// </remarks>
        /// <param name="cells"></param>
        /// <returns></returns>
        private bool IsValid(IEnumerable<Cell> cells)
        {
            bool ret = true;
            bool[] verifier = new bool[MAX_VALUE];

            /*
             * The idea is simple... 
             * Use a verifying array to hold true/false values.
             * Iterate through the section of cells
             * When iterating through the cells array, use the cell's value (minus 1) as the index to the verifier array to set the value to true
             * After the cells has been iterated all the way through, then check the verifier array.
             * If all values are true, then the section of cells is valid
             */

            // Iterate through the section of cells 
            foreach (Cell cell in cells)
            {
                if (cell.Value.HasValue)
                    verifier[cell.Value.Value - 1] = true;
            }


            // Now iterate through verifier
            // Use the logical operator as a quick checker
            foreach (bool b in verifier)
                ret = ret && b;

            return ret;
        }


        /// <summary>
        /// Determine if the value exists within the existing array of cells
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cells"></param>
        /// <returns></returns>
        private bool IsExistValue(byte value, IEnumerable<Cell> cells)
        {
            return cells.Any(x => x.Value.GetValueOrDefault() == value);
        }


        /// <summary>
        /// Checks to see if cell's value already exists in the cell's current row, column and quadrient
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool IsExistValue(Cell cell)
        {
            bool ret = false;
            Cell[] cells = null;

            // Check current row to determine if the cell's value already exists by iterating through all the columns in that row
            // Need to remove the cell that we are checking against
            cells = Transpose(cell.Row, 1, 0, PUZZLE_GRID_SIZE)
                        .Where(x => x.Column != cell.Column)
                        .ToArray();
            ret = ret || IsExistValue(cell.Value.GetValueOrDefault(), cells);


            // Check current column to determine if the cell's value already exists by iterating through all the rows in that column
            // Need to remove the cell that we are checking against
            cells = Transpose(0, PUZZLE_GRID_SIZE, cell.Column, 1)
                        .Where(x => x.Row != cell.Row)
                        .ToArray();
            ret = ret || IsExistValue(cell.Value.GetValueOrDefault(), cells);


            // Check current quadrient to determine if the cell's value already exists by iterating through all the rows and columns in that quadrient
            // Determine quadrient
            int quadRow = (cell.Row / QUADRIENT_GRID_SIZE);
            int quadCol = (cell.Column / QUADRIENT_GRID_SIZE);

            // Need to remove the cell that we are checking against
            cells = Transpose((quadRow * QUADRIENT_GRID_SIZE), QUADRIENT_GRID_SIZE, (quadCol * QUADRIENT_GRID_SIZE), QUADRIENT_GRID_SIZE)
                        .Where(x => !((x.Row == cell.Row) && (x.Column == cell.Column)))
                        .ToArray();
            ret = ret || IsExistValue(cell.Value.GetValueOrDefault(), cells);


            return ret;
        }
        #endregion


        #region -- Solving Routines --
        /// <summary>
        /// This will sove the puzzle by using a recursive Backtracking algorthim
        /// </summary>
        /// <remarks>
        /// This is a brute force routine
        /// </remarks>
        /// <param name="puzzle"></param>
        /// <param name="index"></param>
        private void Solve(Cell[] puzzle, int index)
        {
            int minIndex = puzzle.GetLowerBound(0);
            int maxIndex = puzzle.GetUpperBound(0);

            // Check the index, and exit if the index is outside the boundries
            if (index < minIndex || index > maxIndex)
                return;

            // Much easier to reference a single cell in the beginning than array[index] the whole time
            Cell puzzleCell = puzzle[index];

            // Need to bypass if this cell is Locked.
            if (puzzleCell.IsLocked)
            {
                // Call recursively sending the next index to process
                Solve(puzzle, index + 1);
            }
            else
            {
                // Re-Initialize the cell's value
                puzzleCell.Value = null;

                // Check to see if the puzzle is solved, if so, break out of the routine all together
                // Iterate through all the possible values for this cell
                while (!IsSolved())
                {
                    if (puzzleCell.Value.GetValueOrDefault() < MAX_VALUE)
                    {
                        // Increment current cell
                        // Note: The Increment method has logic to determine if the cell is Locked already
                        puzzleCell.Increment();

                        // Determine if we stay on the current cell (to obtain a valid value) or move to the next cell
                        // Call recursively sending the next index to process
                        if (!IsExistValue(puzzleCell))
                            Solve(puzzle, index + 1);
                    }
                    else
                    {
                        // Re-Initialize the cell's value if the current value is at the max value
                        puzzleCell.Value = null;

                        // By breaking out of the loop, we are forcing the method to return to the previous call
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This will sove the puzzle by using a non-recursive Backtracking algorthim
        /// </summary>
        /// <remarks>
        /// This is a brute force routine
        /// </remarks>
        /// <param name="puzzle"></param>
        private void Solve(Cell[] puzzle)
        {
            int minIndex = puzzle.GetLowerBound(0);
            int maxIndex = puzzle.GetUpperBound(0);
            int index = minIndex;
            int directionFactor = 0;        // 1 to move forward, -1 to move backward

            // Loop until we have either solved or exhusted our routine
            while (!IsSolved() && index >= minIndex )
            {
                // Change the index by the direction factor
                index += directionFactor;

                // Check to see if we have gone beyond the boundries, change the index factor
                if (index > maxIndex)
                {
                    index = maxIndex;
                    directionFactor = -1;
                }

                // Much easier to reference a single cell in the beginning than array[index] the whole time
                Cell puzzleCell = puzzle[index];

                // Need to bypass if this cell is IsLocked. (if cell is locked keep the directionFactor the same)
                if (!puzzleCell.IsLocked)
                {
                    // Iterate through all the possible values for this cell
                    if (puzzleCell.Value.GetValueOrDefault() < MAX_VALUE)
                    {
                        // Increment current cell
                        // Note: The Increment method has logic to determine if the cell is Locked already
                        puzzleCell.Increment();

                        // Determine if we stay on the current cell (to obtain a valid value) or move to the next cell
                        if (IsExistValue(puzzleCell))
                            directionFactor = 0;        // Stay on current cell
                        else                            
                            directionFactor = 1;        // Move to next cell
                    }
                    else
                    {
                        // Re-Initialize the cell's value if the current value is at the max value
                        puzzleCell.Value = null;

                        // Set the direction factor to the previous cell
                        directionFactor = -1;
                    }
                }
                else
                {
                    // The index will not change if the first cell is locked and the directionFactor is zero
                    if (directionFactor == 0)
                        directionFactor = 1;
                }
            }
        }
        #endregion
        #endregion

        #region --Public
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
        /// This will check to see if ALL the cells in the puzzle has a value
        /// </summary>
        /// <returns></returns>
        public bool IsComplete()
        {
            IEnumerable<Cell> cells = GetCells();
            return !cells.Any(x => x.Value == null);
        }


        /// <summary>
        /// This will check to see if the puzzle is solved
        /// </summary>
        /// <returns></returns>
        public bool IsSolved()
        {
            bool ret = true;

            // First check to make sure puzzle is complete
            ret = ret && IsComplete();

            // Check each row by copying the row data (ie columns) to a single array
            // Notice that the ret value is also being checked
            for (int row = 0; row < PUZZLE_GRID_SIZE && ret; row++)
                ret = ret && IsValid(Transpose(row, 1, 0, PUZZLE_GRID_SIZE));

            // Check each col by copying the col data (ie rows) to a single array
            // Notice that the ret value is also being checked
            for (int col = 0; col < PUZZLE_GRID_SIZE && ret; col++)
                ret = ret && IsValid(Transpose(0, PUZZLE_GRID_SIZE, col, 1));


            // There are 9 quardrients in the puzzle.
            // The quadrients are arranged in a 3x3 formation
            // Each quadrient is a 3x3 grid of cells
            for (int quadRow = 0; quadRow < QUADRIENT_GRID_SIZE && ret; quadRow++)
                for (int quadCol = 0; quadCol < QUADRIENT_GRID_SIZE && ret; quadCol++)
                {
                    ret = ret && IsValid(Transpose((quadRow * QUADRIENT_GRID_SIZE),
                                                    QUADRIENT_GRID_SIZE,
                                                    (quadCol * QUADRIENT_GRID_SIZE),
                                                    QUADRIENT_GRID_SIZE));
                }

            return ret;
        }

        /// <summary>
        /// This will take the current puzzle and solve it
        /// </summary>
        /// <remarks>
        /// This assumes that the puzzle is solvable.  Meaning, there was no malicious attempt to lock cell values so that the solver would be in an infinite loop 
        /// </remarks>
        public void Solve()
        {
            Cell[] puzzle = null;

            // Clear all non-Locked values
            Clear();

            // Much easier to work with a single dimensional array, so lets transpose it
            puzzle = Transpose(0, PUZZLE_GRID_SIZE, 0, PUZZLE_GRID_SIZE);

            // Call recursive function to do all the work
            // Solve(puzzle, 0);

            // Call non-recursive function to do all the work
            Solve(puzzle);
        }
        #endregion
        #endregion
    }
}
