using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    /// <summary>
    /// Generate a solvable puzzle with random values
    /// </summary>
    /// <remarks>
    /// When a puzzle is generated, that same puzzle is cloned and then solved.
    /// That way when the generated puzzle is being played, it can be compared to the solved puzzle on progress.
    /// It is possible that a generated puzzle could have more than one solvable solution
    /// </remarks>
    /// <history>
    /// Date        Author                  Description
    /// 2019.09.26  Lehan, Ryan             Initial Development
    /// </history>
    public class Generator
    {
        #region Accessors / Mutators
        public byte NumberOfRandomValues { get; private set; }
        public Puzzle GeneratedPuzzle { get; private set; }
        public Puzzle SolvedPuzzle { get; private set; }
        #endregion

        #region Constructors
        public Generator(byte numOfRandomValues)
        {
            // Validate parameters
            if (numOfRandomValues <= 0 || numOfRandomValues > Puzzle.MAX_CELLS)
                throw new ArgumentOutOfRangeException(nameof(numOfRandomValues));

            NumberOfRandomValues = numOfRandomValues;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Populate puzzle with random values
        /// </summary>
        /// <remarks>
        /// This will perform some validations to make sure the random value can be placed within the specified cell
        /// </remarks>
        /// <param name="puzzle"></param>
        private void Populate(Puzzle puzzle)
        {
            int value = 0;
            Random random = new Random();

            // Loop for the number of random number values to generate
            for (int i = 0; i < NumberOfRandomValues; i++)
            {
                Puzzle.Cell cell = GetRandomCell(puzzle);

                // Loop until we have a valid random number in a random cell
                do
                {
                    cell.Value = null;                                          // Clear out previous value
                    cell = GetRandomCell(puzzle);
                    value = random.Next(Puzzle.MIN_VALUE, Puzzle.MAX_VALUE + 1);
                    cell.Value = Convert.ToByte(value);
                } while (Validator.IsExistValue(puzzle, cell));

                // Lock cell so that value cannot be changed
                cell.IsLocked = true;
            }
        }

        /// <summary>
        /// This will randomly pick a cell from the given puzzle
        /// </summary>
        /// <remarks>
        /// If the picked cell has a value or is locked, then a new cell will be randomly chosen
        /// </remarks>
        /// <param name="puzzle"></param>
        /// <returns></returns>
        private Puzzle.Cell GetRandomCell(Puzzle puzzle)
        {
            int row = 0;
            int col = 0;
            Puzzle.Cell cell = null;
            Random random = new Random();

            do
            {
                row = random.Next(0, Puzzle.PUZZLE_GRID_SIZE);
                col = random.Next(0, Puzzle.PUZZLE_GRID_SIZE);
                cell = puzzle.GetCell(row, col);
            } while (cell.IsLocked || cell.Value.HasValue);

            return cell;
        }


        public void Generate()
        {
            // Need to make sure that we have a solvable puzzle
            do
            {
                // Clear existing puzzles
                GeneratedPuzzle = null;
                SolvedPuzzle = null;

                GeneratedPuzzle = new Puzzle();
                Populate(GeneratedPuzzle);

                // Solve the generated puzzle and test to make sure it is solved
                SolvedPuzzle = Solver.Solve(GeneratedPuzzle);

            } while (!Validator.IsSolved(SolvedPuzzle));
        }
        #endregion
    }
}
