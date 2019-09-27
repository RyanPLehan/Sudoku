using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Sudoku;


namespace PuzzleTest
{
    [TestClass]
    public class PuzzleMonolithicTest
    {
        /// <summary>
        /// Populate each PuzzleMonolithic cell with a random value
        /// </summary>
        /// <param name="puzzle"></param>
        private void PopulateRandomValues(PuzzleMonolithic puzzle)
        {
            IEnumerable<PuzzleMonolithic.Cell> cells = puzzle.GetCells();
            Random rand = new Random();

            foreach (var cell in cells)
            {
                cell.Value = Convert.ToByte((rand.Next() % PuzzleMonolithic.MAX_VALUE) + 1);
            }
        }

        /// <summary>
        /// Populate each cell with a value such that the puzzle is considered solved
        /// </summary>
        /// <param name="puzzle"></param>
        private void PopulateSolvedValues(PuzzleMonolithic puzzle)
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
            for (int row = 0; row < PuzzleMonolithic.PUZZLE_GRID_SIZE; row++)
                for (int col = 0; col < PuzzleMonolithic.PUZZLE_GRID_SIZE; col++)
                {
                    puzzle.GetCell(row, col).Value = values[row, col];
                }
        }


        /// <summary>
        /// Populate PuzzleMonolithic with data from "World's hardest Sudoku" puzzle
        /// </summary>
        /// <param name="puzzle"></param>
        private void PopulateHardestValues(PuzzleMonolithic puzzle)
        {
            PuzzleMonolithic.Cell cell = null;
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
            for (int row = 0; row < PuzzleMonolithic.PUZZLE_GRID_SIZE; row++)
                for (int col = 0; col < PuzzleMonolithic.PUZZLE_GRID_SIZE; col++)
                {
                    if (values[row, col].HasValue)
                    {
                        cell = puzzle.GetCell(row, col);
                        cell.Value = values[row, col];
                        cell.IsLocked = true;
                    }
                }
        }

        /// <summary>
        /// Display the sudoku puzzle 
        /// </summary>
        /// <param name="puzzle"></param>
        private void DisplayPuzzle(PuzzleMonolithic puzzle)
        {
            int cnt = 1;
            IEnumerable<PuzzleMonolithic.Cell> cells = puzzle.GetCells();

            foreach (var cell in cells)
            {
                Console.Write($" {cell.ToString()} ");

                if (cnt % PuzzleMonolithic.QUADRIENT_GRID_SIZE == 0 &&
                    cnt % PuzzleMonolithic.PUZZLE_GRID_SIZE != 0)
                    Console.Write(" | ");


                // Determine if we need a blank line
                if (cnt % PuzzleMonolithic.PUZZLE_GRID_SIZE == 0)
                {
                    Console.WriteLine();
                    if (cnt % (PuzzleMonolithic.QUADRIENT_GRID_SIZE * PuzzleMonolithic.PUZZLE_GRID_SIZE) == 0)
                    {
                        for (int i = 0; i < PuzzleMonolithic.PUZZLE_GRID_SIZE; i++)
                            Console.Write("---");

                        Console.Write("---");
                        Console.WriteLine();
                    }
                }

                cnt++;
            }
        }

        private void LockCellValues(PuzzleMonolithic puzzle)
        {
            for (int i = 0; i < PuzzleMonolithic.PUZZLE_GRID_SIZE; i++)
                puzzle.GetCell(i, i).IsLocked = true;
        }

        private void LockAllCellValues(PuzzleMonolithic puzzle)
        {
            IEnumerable<PuzzleMonolithic.Cell> cells = puzzle.GetCells();
            foreach (PuzzleMonolithic.Cell cell in cells)
                cell.IsLocked = true;
        }


        [TestMethod]
        public void TestIsComplete_False()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            Assert.IsFalse(puzzle.IsComplete());
        }

        [TestMethod]
        public void TestIsComplete_True()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            PopulateRandomValues(puzzle);
            DisplayPuzzle(puzzle);
            Assert.IsTrue(puzzle.IsComplete());
        }

        [TestMethod]
        public void TestIsPuzzleSolved_False()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            PopulateRandomValues(puzzle);
            DisplayPuzzle(puzzle);
            Assert.IsFalse(puzzle.IsSolved());
        }

        [TestMethod]
        public void TestIsPuzzleSolved_True()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            PopulateSolvedValues(puzzle);
            DisplayPuzzle(puzzle);
            Assert.IsTrue(puzzle.IsSolved());
        }

        [TestMethod]
        public void TestClear()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            PopulateSolvedValues(puzzle);
            LockCellValues(puzzle);
            DisplayPuzzle(puzzle);

            puzzle.Clear();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(puzzle);

            Assert.IsFalse(puzzle.IsComplete());
        }

        [TestMethod]
        public void TestClearAll()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            PopulateSolvedValues(puzzle);
            LockCellValues(puzzle);
            DisplayPuzzle(puzzle);

            puzzle.ClearAll();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(puzzle);

            Assert.IsFalse(puzzle.IsComplete());
        }

        /// <summary>
        /// Test to solve an empty puzzle
        /// </summary>
        [TestMethod]
        public void TestSolve_Empty()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            puzzle.Solve();
            DisplayPuzzle(puzzle);
            Assert.IsTrue(puzzle.IsSolved());
        }

        /// <summary>
        /// Test to solve a puzzle with some values locked
        /// </summary>
        [TestMethod]
        public void TestSolve_Locked_1()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            PopulateSolvedValues(puzzle);
            LockCellValues(puzzle);
            DisplayPuzzle(puzzle);

            puzzle.Solve();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(puzzle);
            Assert.IsTrue(puzzle.IsSolved());
        }


        /// <summary>
        /// Test to solve a puzzle with all values locked, except for the last two
        /// </summary>
        [TestMethod]
        public void TestSolve_Locked_2()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            PopulateSolvedValues(puzzle);
            LockAllCellValues(puzzle);
            DisplayPuzzle(puzzle);

            // Unlock the last two values
            puzzle.GetCell(8, 7).IsLocked = false;
            puzzle.GetCell(8, 8).IsLocked = false;

            puzzle.Solve();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(puzzle);
            Assert.IsTrue(puzzle.IsSolved());
        }


        /// <summary>
        /// Test to solve a puzzle with all values locked, except for the first two
        /// </summary>
        [TestMethod]
        public void TestSolve_Locked_3()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            PopulateSolvedValues(puzzle);
            LockAllCellValues(puzzle);
            DisplayPuzzle(puzzle);

            // Unlock the last two values
            puzzle.GetCell(0, 0).IsLocked = false;
            puzzle.GetCell(0, 1).IsLocked = false;

            puzzle.Solve();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(puzzle);
            Assert.IsTrue(puzzle.IsSolved());
        }


        /// <summary>
        /// Test to solve a puzzle with all values locked
        /// </summary>
        [TestMethod]
        public void TestSolve_AllLocked()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            PopulateSolvedValues(puzzle);
            LockAllCellValues(puzzle);
            DisplayPuzzle(puzzle);

            puzzle.Solve();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(puzzle);
            Assert.IsTrue(puzzle.IsSolved());
        }



        /// <summary>
        /// Test to solve a puzzle with all values locked
        /// </summary>
        [TestMethod]
        public void TestSolve_Hardest()
        {
            PuzzleMonolithic puzzle = new PuzzleMonolithic();
            PopulateHardestValues(puzzle);
            DisplayPuzzle(puzzle);

            puzzle.Solve();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(puzzle);
            Assert.IsTrue(puzzle.IsSolved());
        }
    }
}
