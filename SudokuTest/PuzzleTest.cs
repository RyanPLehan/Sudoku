using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Sudoku;


namespace PuzzleTest
{
    [TestClass]
    public class PuzzleTest
    {
        private Puzzle _puzzle;

        /// <summary>
        /// Populate each Puzzle cell with a random value
        /// </summary>
        /// <param name="puzzle"></param>
        private void PopulateRandomValues(Puzzle puzzle)
        {
            IEnumerable<Puzzle.Cell> cells = _puzzle.GetCells();
            Random rand = new Random();

            foreach (var cell in cells)
            {
                cell.Value = Convert.ToByte((rand.Next() % Puzzle.MAX_VALUE) + 1);
            }
        }

        /// <summary>
        /// Populate each cell with a value such that the puzzle is considered solved
        /// </summary>
        /// <param name="puzzle"></param>
        private void PopulateSolvedValues(Puzzle puzzle)
        {
            byte[,] values = {
                                { 1,2,3,4,5,6,7,8,9 },
                                { 4,5,6,7,8,9,1,2,3 },
                                { 7,8,9,1,2,3,4,5,6 },

                                { 2,3,4,5,6,7,8,9,1 },
                                { 5,6,7,8,9,1,2,3,4 },
                                { 8,9,1,2,3,4,5,6,7 },

                                { 3,4,5,6,7,8,9,1,2 },
                                { 6,7,8,9,1,2,3,4,5 },
                                { 9,1,2,3,4,5,6,7,8 },
                             };

            // Populate each cell with a value
            for (int row = 0; row < Puzzle.PUZZLE_GRID_SIZE; row++)
                for (int col = 0; col < Puzzle.PUZZLE_GRID_SIZE; col++)
                {
                    _puzzle.GetCell(row, col).Value = values[row, col];
                }
        }


        /// <summary>
        /// Populate Puzzle with data from "World's hardest Sudoku" puzzle
        /// </summary>
        /// <param name="puzzle"></param>
        private void PopulateHardestValues(Puzzle puzzle)
        {
            Puzzle.Cell cell = null;
            byte?[,] values = {
                                { 8,null,null,null,null,null,null,null,null },
                                { null,null,3,6,null,null,null,null,null },
                                { null,7,null,null,9,null,2,null,null },

                                { null,5,null,null,null,7,null,null,null },
                                { null,null,null,null,4,5,7,null,null },
                                { null,null,null,1,null,null,null,3,null },

                                { null,null,1,null,null,null,null,6,8 },
                                { null,null,8,5,null,null,null,1,null },
                                { null,9,null,null,null,null,4,null,null },
                             };

            // Populate each cell with a value
            for (int row = 0; row < Puzzle.PUZZLE_GRID_SIZE; row++)
                for (int col = 0; col < Puzzle.PUZZLE_GRID_SIZE; col++)
                {
                    if (values[row, col].HasValue)
                    {
                        cell = _puzzle.GetCell(row, col);
                        cell.Value = values[row, col];
                        cell.IsLocked = true;
                    }
                }
        }

        /// <summary>
        /// Display the sudoku puzzle 
        /// </summary>
        /// <param name="puzzle"></param>
        private void DisplayPuzzle(Puzzle puzzle)
        {
            int cnt = 1;
            IEnumerable<Puzzle.Cell> cells = _puzzle.GetCells();

            foreach (var cell in cells)
            {
                Console.Write($" {cell.ToString()} ");

                if (cnt % Puzzle.QUADRIENT_GRID_SIZE == 0 &&
                    cnt % Puzzle.PUZZLE_GRID_SIZE != 0)
                    Console.Write(" | ");


                // Determine if we need a blank line
                if (cnt % Puzzle.PUZZLE_GRID_SIZE == 0)
                {
                    Console.WriteLine();
                    if (cnt % (Puzzle.QUADRIENT_GRID_SIZE * Puzzle.PUZZLE_GRID_SIZE) == 0)
                    {
                        for (int i = 0; i < Puzzle.PUZZLE_GRID_SIZE; i++)
                            Console.Write("---");

                        Console.Write("---");
                        Console.WriteLine();
                    }
                }

                cnt++;
            }
        }

        private void LockCellValues(Puzzle puzzle)
        {
            for (int i = 0; i < Puzzle.PUZZLE_GRID_SIZE; i++)
                puzzle.GetCell(i, i).IsLocked = true;
        }

        private void LockAllCellValues(Puzzle puzzle)
        {
            IEnumerable<Puzzle.Cell> cells = _puzzle.GetCells();
            foreach (Puzzle.Cell cell in cells)
                cell.IsLocked = true;
        }


        [TestInitialize]
        public void Initialize()
        {
            _puzzle = new Puzzle();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _puzzle = null;
        }

        [TestMethod]
        public void TestIsComplete_False()
        {
            Assert.IsFalse(_puzzle.IsComplete());
        }

        [TestMethod]
        public void TestIsComplete_True()
        {
            PopulateRandomValues(_puzzle);
            DisplayPuzzle(_puzzle);
            Assert.IsTrue(_puzzle.IsComplete());
        }

        [TestMethod]
        public void TestIsPuzzleSolved_False()
        {
            PopulateRandomValues(_puzzle);
            DisplayPuzzle(_puzzle);
            Assert.IsFalse(_puzzle.IsSolved());
        }

        [TestMethod]
        public void TestIsPuzzleSolved_True()
        {
            PopulateSolvedValues(_puzzle);
            DisplayPuzzle(_puzzle);
            Assert.IsTrue(_puzzle.IsSolved());
        }

        [TestMethod]
        public void TestClear()
        {
            PopulateSolvedValues(_puzzle);
            LockCellValues(_puzzle);
            DisplayPuzzle(_puzzle);

            _puzzle.Clear();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(_puzzle);

            Assert.IsFalse(_puzzle.IsComplete());
        }

        [TestMethod]
        public void TestClearAll()
        {
            PopulateSolvedValues(_puzzle);
            LockCellValues(_puzzle);
            DisplayPuzzle(_puzzle);

            _puzzle.ClearAll();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(_puzzle);

            Assert.IsFalse(_puzzle.IsComplete());
        }

        /// <summary>
        /// Test to solve an empty puzzle
        /// </summary>
        [TestMethod]
        public void TestSolve_Empty()
        {
            _puzzle.Solve();
            DisplayPuzzle(_puzzle);
            Assert.IsTrue(_puzzle.IsSolved());
        }

        /// <summary>
        /// Test to solve a puzzle with some values locked
        /// </summary>
        [TestMethod]
        public void TestSolve_Locked_1()
        {
            PopulateSolvedValues(_puzzle);
            LockCellValues(_puzzle);
            DisplayPuzzle(_puzzle);

            _puzzle.Solve();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(_puzzle);
            Assert.IsTrue(_puzzle.IsSolved());
        }


        /// <summary>
        /// Test to solve a puzzle with all values locked, except for the last two
        /// </summary>
        [TestMethod]
        public void TestSolve_Locked_2()
        {
            PopulateSolvedValues(_puzzle);
            LockAllCellValues(_puzzle);
            DisplayPuzzle(_puzzle);

            // Unlock the last two values
            _puzzle.GetCell(8, 7).IsLocked = false;
            _puzzle.GetCell(8, 8).IsLocked = false;

            _puzzle.Solve();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(_puzzle);
            Assert.IsTrue(_puzzle.IsSolved());
        }


        /// <summary>
        /// Test to solve a puzzle with all values locked, except for the first two
        /// </summary>
        [TestMethod]
        public void TestSolve_Locked_3()
        {
            PopulateSolvedValues(_puzzle);
            LockAllCellValues(_puzzle);
            DisplayPuzzle(_puzzle);

            // Unlock the last two values
            _puzzle.GetCell(0, 0).IsLocked = false;
            _puzzle.GetCell(0, 1).IsLocked = false;

            _puzzle.Solve();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(_puzzle);
            Assert.IsTrue(_puzzle.IsSolved());
        }


        /// <summary>
        /// Test to solve a puzzle with all values locked
        /// </summary>
        [TestMethod]
        public void TestSolve_AllLocked()
        {
            PopulateSolvedValues(_puzzle);
            LockAllCellValues(_puzzle);
            DisplayPuzzle(_puzzle);

            _puzzle.Solve();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(_puzzle);
            Assert.IsTrue(_puzzle.IsSolved());
        }



        /// <summary>
        /// Test to solve a puzzle with all values locked
        /// </summary>
        [TestMethod]
        public void TestSolve_Hardest()
        {
            PopulateHardestValues(_puzzle);
            DisplayPuzzle(_puzzle);

            _puzzle.Solve();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(_puzzle);
            Assert.IsTrue(_puzzle.IsSolved());
        }
    }
}
