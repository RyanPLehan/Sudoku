using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    /// <summary>
    /// Solver for a Sudoku puzzle 
    /// </summary>
    /// <history>
    /// Date        Author                  Description
    /// 2019.09.26  Lehan, Ryan             Re-factored into separation of concerns from monolithic class
    /// </history>
    public static class Solver
    {       
        /// <summary>
        /// This is the starting method for solving the puzzle via a recusive algorthim
        /// </summary>
        /// <remarks>
        /// This method only starts the recursion, the method itself, is not recursed
        /// </remarks>
        /// <param name="puzzle"></param>
        private static void SolveRecursive(Puzzle puzzle)
        {
            // Much easier to work with a single dimensional array, so lets transpose it
            Puzzle.Cell[] cells = Utils.Transpose(puzzle.ToArray(),
                                                  0,
                                                  Puzzle.PUZZLE_GRID_SIZE,
                                                  0,
                                                  Puzzle.PUZZLE_GRID_SIZE);

            // Call recursive function to do all the work
            SolveRecursive(puzzle, cells, 0);
        }

        /// <summary>
        /// This will solve the puzzle by using a recursive Backtracking algorthim
        /// </summary>
        /// <remarks>
        /// This is a brute force routine
        /// </remarks>
        /// <param name="puzzle"></param>
        /// <param name="cells"></param>
        /// <param name="index"></param>
        private static void SolveRecursive(Puzzle puzzle, Puzzle.Cell[] cells, int index)
        {
            int minIndex = cells.GetLowerBound(0);
            int maxIndex = cells.GetUpperBound(0);

            // Check the index, and exit if the index is outside the boundries
            if (index < minIndex || index > maxIndex)
                return;

            // Much easier to reference a single cell in the beginning than array[index] the whole time
            Puzzle.Cell cell = cells[index];

            // Need to bypass if this cell is Locked.
            if (cell.IsLocked)
            {
                // Call recursively sending the next index to process
                SolveRecursive(puzzle, cells, index + 1);
            }
            else
            {
                // Re-Initialize the cell's value
                cell.Value = null;

                // Check to see if the puzzle is solved, if so, break out of the routine all together
                // Iterate through all the possible values for this cell
                while (!Validator.IsSolved(puzzle))
                {
                    if (cell.Value.GetValueOrDefault() < Puzzle.MAX_VALUE)
                    {
                        // Increment current cell
                        // Note: The Increment method has logic to determine if the cell is Locked already
                        cell.Increment();

                        // Determine if we stay on the current cell (to obtain a valid value) or move to the next cell
                        // Call recursively sending the next index to process
                        if (!Validator.IsExistValue(puzzle, cell))
                            SolveRecursive(puzzle, cells, index + 1);
                    }
                    else
                    {
                        // Re-Initialize the cell's value if the current value is at the max value
                        cell.Value = null;

                        // By breaking out of the loop, we are forcing the method to return to the previous call
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This will solve the puzzle by using a non-recursive Backtracking algorthim
        /// </summary>
        /// <remarks>
        /// This is a brute force routine
        /// </remarks>
        /// <param name="puzzle"></param>
        /// <param name="cells"></param>
        private static void SolveNonRecursive(Puzzle puzzle)
        {
            // Much easier to work with a single dimensional array, so lets transpose it
            Puzzle.Cell[] cells = Utils.Transpose(puzzle.ToArray(),
                                                  0,
                                                  Puzzle.PUZZLE_GRID_SIZE,
                                                  0,
                                                  Puzzle.PUZZLE_GRID_SIZE);

            int minIndex = cells.GetLowerBound(0);
            int maxIndex = cells.GetUpperBound(0);
            int index = minIndex;
            int directionFactor = 0;        // 1 to move forward, -1 to move backward

            // Loop until we have either solved or exhusted our routine
            while (!Validator.IsSolved(puzzle) && index >= minIndex)
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
                Puzzle.Cell cell = cells[index];

                // Need to bypass if this cell is IsLocked. (if cell is locked keep the directionFactor the same)
                if (!cell.IsLocked)
                {
                    // Iterate through all the possible values for this cell
                    if (cell.Value.GetValueOrDefault() < Puzzle.MAX_VALUE)
                    {
                        // Increment current cell
                        // Note: The Increment method has logic to determine if the cell is Locked already
                        cell.Increment();

                        // Determine if we stay on the current cell (to obtain a valid value) or move to the next cell
                        if (Validator.IsExistValue(puzzle, cell))
                            directionFactor = 0;        // Stay on current cell
                        else
                            directionFactor = 1;        // Move to next cell
                    }
                    else
                    {
                        // Re-Initialize the cell's value if the current value is at the max value
                        cell.Value = null;

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


        /// <summary>
        /// This will take the current puzzle and solve it
        /// </summary>
        /// <remarks>
        /// This assumes that the puzzle is solvable.  Meaning, there was no malicious attempt to lock cell values so that the solver would be in an infinite loop 
        /// </remarks>
        public static Puzzle Solve(Puzzle puzzle)
        {
            Puzzle ret = puzzle.Clone();

            // Clear all non-Locked values
            ret.Clear();

            // Call recursive function to do all the work
            // SolveRecursive(ret);

            // Call non-recursive function to do all the work
            SolveNonRecursive(ret);

            return ret;
        }

    }
}
