using System;
using System.Linq;
using System.Collections.Generic;


namespace Sudoku
{
    /// <summary>
    /// Valdiation routines for a Sudoku puzzle 
    /// </summary>
    /// <history>
    /// Date        Author                  Description
    /// 2019.09.26  Lehan, Ryan             Re-factored into separation of concerns from monolithic class
    /// </history>
    public static class Validator
    {
        /// <summary>
        /// Determine if the value exists within the existing array of cells
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cells"></param>
        /// <returns></returns>
        private static bool IsExistValue(byte value, IEnumerable<Puzzle.Cell> cells)
        {
            return cells.Any(x => x.Value.GetValueOrDefault() == value);
        }


       
        /// <summary>
        /// Checks to see if cell's value already exists in the cell's current row, column and quadrient
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        internal static bool IsExistValue(Puzzle puzzle, Puzzle.Cell cell)
        {
            bool ret = false;
            Puzzle.Cell[] cells = null;
            Puzzle.Cell[] filteredCells = null;

            // Check current row to determine if the cell's value already exists by iterating through all the columns in that row
            // Need to remove the cell that we are checking against
            cells = Utils.Transpose<Puzzle.Cell>(puzzle.ToArray(),
                                                 cell.Row,
                                                 1,
                                                 0,
                                                 Puzzle.PUZZLE_GRID_SIZE);

            // Need to remove the cell that we are checking against
            filteredCells = cells.Where(x => x.Column != cell.Column)
                                 .ToArray();
            ret = ret || IsExistValue(cell.Value.GetValueOrDefault(), filteredCells);


            // Check current column to determine if the cell's value already exists by iterating through all the rows in that column
            // Need to remove the cell that we are checking against
            cells = Utils.Transpose<Puzzle.Cell>(puzzle.ToArray(),
                                                 0,
                                                 Puzzle.PUZZLE_GRID_SIZE,
                                                 cell.Column,
                                                 1);

            // Need to remove the cell that we are checking against
            filteredCells = cells.Where(x => x.Row != cell.Row)
                                 .ToArray();
            ret = ret || IsExistValue(cell.Value.GetValueOrDefault(), filteredCells);


            // Check current quadrient to determine if the cell's value already exists by iterating through all the rows and columns in that quadrient
            // Determine quadrient
            int quadRow = (cell.Row / Puzzle.QUADRIENT_GRID_SIZE);
            int quadCol = (cell.Column / Puzzle.QUADRIENT_GRID_SIZE);

            // Need to remove the cell that we are checking against
            cells = Utils.Transpose<Puzzle.Cell>(puzzle.ToArray(),
                                                (quadRow * Puzzle.QUADRIENT_GRID_SIZE),
                                                Puzzle.QUADRIENT_GRID_SIZE,
                                                (quadCol * Puzzle.QUADRIENT_GRID_SIZE),
                                                Puzzle.QUADRIENT_GRID_SIZE);

            // Need to remove the cell that we are checking against
            filteredCells = cells.Where(x => !((x.Row == cell.Row) && (x.Column == cell.Column)))
                                 .ToArray();
            ret = ret || IsExistValue(cell.Value.GetValueOrDefault(), filteredCells);


            return ret;
        }

        /// <summary>
        /// Checks to see if a section of cells is valid.  A section of cells would be a single row, single column or single quadrient of cells.
        /// </summary>
        /// <remarks>
        /// All cells must have a value and the values must be unique within the section of cells
        /// </remarks>
        /// <param name="cells"></param>
        /// <returns></returns>
        internal static bool IsValid(IEnumerable<Puzzle.Cell> cells)
        {
            bool ret = true;
            bool[] verifier = new bool[Puzzle.MAX_VALUE];

            /*
             * The idea is simple... 
             * Use a verifying array to hold true/false values.
             * Iterate through the section of cells
             * When iterating through the cells array, use the cell's value (minus 1) as the index to the verifier array to set the value to true
             * After the cells has been iterated all the way through, then check the verifier array.
             * If all values are true, then the section of cells is valid
             */

            // Iterate through the section of cells 
            foreach (Puzzle.Cell cell in cells)
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
        /// This will check to see if ALL the cells in the puzzle has a value
        /// </summary>
        /// <returns></returns>
        public static bool IsComplete(Puzzle puzzle)
        {
            IEnumerable<Puzzle.Cell> cells = puzzle.GetCells();
            return !cells.Any(x => x.Value == null);
        }


        /// <summary>
        /// This will check to see if the puzzle is solved
        /// </summary>
        /// <returns></returns>
        public static bool IsSolved(Puzzle puzzle)
        {
            bool ret = true;

            // First check to make sure puzzle is complete
            ret = ret && Validator.IsComplete(puzzle);

            // Check each row by copying the row data (ie columns) to a single array
            // Notice that the ret value is also being checked
            for (int row = 0; row < Puzzle.PUZZLE_GRID_SIZE && ret; row++)
                ret = ret && Validator.IsValid(Utils.Transpose<Puzzle.Cell>(puzzle.ToArray(),
                                                                            row,
                                                                            1,
                                                                            0,
                                                                            Puzzle.PUZZLE_GRID_SIZE));

            // Check each col by copying the col data (ie rows) to a single array
            // Notice that the ret value is also being checked
            for (int col = 0; col < Puzzle.PUZZLE_GRID_SIZE && ret; col++)
                ret = ret && Validator.IsValid(Utils.Transpose<Puzzle.Cell>(puzzle.ToArray(),
                                                                            0,
                                                                            Puzzle.PUZZLE_GRID_SIZE,
                                                                            col,
                                                                            1));


            // There are 9 quardrients in the puzzle.
            // The quadrients are arranged in a 3x3 formation
            // Each quadrient is a 3x3 grid of cells
            for (int quadRow = 0; quadRow < Puzzle.QUADRIENT_GRID_SIZE && ret; quadRow++)
                for (int quadCol = 0; quadCol < Puzzle.QUADRIENT_GRID_SIZE && ret; quadCol++)
                {
                    ret = ret && Validator.IsValid(Utils.Transpose<Puzzle.Cell>(puzzle.ToArray(),
                                                                                (quadRow * Puzzle.QUADRIENT_GRID_SIZE),
                                                                                Puzzle.QUADRIENT_GRID_SIZE,
                                                                                (quadCol * Puzzle.QUADRIENT_GRID_SIZE),
                                                                                Puzzle.QUADRIENT_GRID_SIZE));
                }

            return ret;
        }

    }
}
