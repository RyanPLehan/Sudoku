using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Sudoku;


namespace PuzzleTest
{
    [TestClass]
    public class PuzzleTest
    {
        /// <summary>
        /// Populate each Puzzle cell with a random value
        /// </summary>
        /// <param name="puzzle"></param>
        private void PopulateRandomValues(Puzzle puzzle)
        {
            IEnumerable<Puzzle.Cell> cells = puzzle.GetCells();
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
                    puzzle.GetCell(row, col).Value = values[row, col];
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
        private void DisplayPuzzle(Puzzle puzzle)
        {
            int cnt = 1;
            IEnumerable<Puzzle.Cell> cells = puzzle.GetCells();

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
            IEnumerable<Puzzle.Cell> cells = puzzle.GetCells();
            foreach (Puzzle.Cell cell in cells)
                cell.IsLocked = true;
        }


        [TestMethod]
        public void TestIsComplete_False()
        {
            Puzzle puzzle = new Puzzle();
            Assert.IsFalse(Validator.IsComplete(puzzle));
        }

        [TestMethod]
        public void TestIsComplete_True()
        {
            Puzzle puzzle = new Puzzle();
            PopulateRandomValues(puzzle);
            DisplayPuzzle(puzzle);
            Assert.IsTrue(Validator.IsComplete(puzzle));
        }

        [TestMethod]
        public void TestIsPuzzleSolved_False()
        {
            Puzzle puzzle = new Puzzle();
            PopulateRandomValues(puzzle);
            DisplayPuzzle(puzzle);
            Assert.IsFalse(Validator.IsSolved(puzzle));
        }

        [TestMethod]
        public void TestIsPuzzleSolved_True()
        {
            Puzzle puzzle = new Puzzle();
            PopulateSolvedValues(puzzle);
            DisplayPuzzle(puzzle);
            Assert.IsTrue(Validator.IsSolved(puzzle));
        }

        [TestMethod]
        public void TestClear()
        {
            Puzzle puzzle = new Puzzle();
            PopulateSolvedValues(puzzle);
            LockCellValues(puzzle);
            DisplayPuzzle(puzzle);

            puzzle.Clear();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(puzzle);

            Assert.IsFalse(Validator.IsComplete(puzzle));
        }

        [TestMethod]
        public void TestClearAll()
        {
            Puzzle puzzle = new Puzzle();
            PopulateSolvedValues(puzzle);
            LockCellValues(puzzle);
            DisplayPuzzle(puzzle);

            puzzle.ClearAll();
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(puzzle);

            Assert.IsFalse(Validator.IsComplete(puzzle));
        }

        /// <summary>
        /// Test to solve an empty puzzle
        /// </summary>
        [TestMethod]
        public void TestSolve_Empty()
        {
            Puzzle puzzle = new Puzzle();
            Puzzle solved = Solver.Solve(puzzle);
            DisplayPuzzle(solved);
            Assert.IsTrue(Validator.IsSolved(solved));
        }

        /// <summary>
        /// Test to solve a puzzle with some values locked
        /// </summary>
        [TestMethod]
        public void TestSolve_Locked_1()
        {
            Puzzle puzzle = new Puzzle();
            
            PopulateSolvedValues(puzzle);
            LockCellValues(puzzle);
            DisplayPuzzle(puzzle);

            Puzzle solved = Solver.Solve(puzzle);
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(solved);
            Assert.IsTrue(Validator.IsSolved(solved));
        }


        /// <summary>
        /// Test to solve a puzzle with all values locked, except for the last two
        /// </summary>
        [TestMethod]
        public void TestSolve_Locked_2()
        {
            Puzzle puzzle = new Puzzle();
            PopulateSolvedValues(puzzle);
            LockAllCellValues(puzzle);
            DisplayPuzzle(puzzle);

            // Unlock the last two values
            puzzle.GetCell(8, 7).IsLocked = false;
            puzzle.GetCell(8, 8).IsLocked = false;

            Puzzle solved = Solver.Solve(puzzle);
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(solved);
            Assert.IsTrue(Validator.IsSolved(solved));
        }


        /// <summary>
        /// Test to solve a puzzle with all values locked, except for the first two
        /// </summary>
        [TestMethod]
        public void TestSolve_Locked_3()
        {
            Puzzle puzzle = new Puzzle();
            PopulateSolvedValues(puzzle);
            LockAllCellValues(puzzle);
            DisplayPuzzle(puzzle);

            // Unlock the last two values
            puzzle.GetCell(0, 0).IsLocked = false;
            puzzle.GetCell(0, 1).IsLocked = false;

            Puzzle solved = Solver.Solve(puzzle);
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(solved);
            Assert.IsTrue(Validator.IsSolved(solved));
        }


        /// <summary>
        /// Test to solve a puzzle with all values locked
        /// </summary>
        [TestMethod]
        public void TestSolve_AllLocked()
        {
            Puzzle puzzle = new Puzzle();
            PopulateSolvedValues(puzzle);
            LockAllCellValues(puzzle);
            DisplayPuzzle(puzzle);

            Puzzle solved = Solver.Solve(puzzle);
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(solved);
            Assert.IsTrue(Validator.IsSolved(solved));
        }



        /// <summary>
        /// Test to solve a puzzle with all values locked
        /// </summary>
        [TestMethod]
        public void TestSolve_Hardest()
        {
            Puzzle puzzle = new Puzzle();
            PopulateHardestValues(puzzle);
            DisplayPuzzle(puzzle);

            Puzzle solved = Solver.Solve(puzzle);
            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(solved);
            Assert.IsTrue(Validator.IsSolved(solved));
        }



        /// <summary>
        /// Test to generate a puzzle with 5 random values
        /// </summary>
        [TestMethod]
        public void Test_Generate_5()
        {
            const int NUM = 5;

            Generator gen = new Generator(NUM);
            gen.Generate();
            DisplayPuzzle(gen.GeneratedPuzzle);

            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(gen.SolvedPuzzle);
            Assert.IsTrue(Validator.IsSolved(gen.SolvedPuzzle));
            Assert.AreEqual(NUM, gen.NumberOfRandomValues);
        }


        /// <summary>
        /// Test to generate a puzzle with 10 random values
        /// </summary>
        [TestMethod]
        public void Test_Generate_10()
        {
            const int NUM = 10;

            Generator gen = new Generator(NUM);
            gen.Generate();
            DisplayPuzzle(gen.GeneratedPuzzle);

            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(gen.SolvedPuzzle);
            Assert.IsTrue(Validator.IsSolved(gen.SolvedPuzzle));
            Assert.AreEqual(NUM, gen.NumberOfRandomValues);
        }



        /// <summary>
        /// Test to generate a puzzle with 15 random values
        /// </summary>
        [TestMethod]
        public void Test_Generate_15()
        {
            const int NUM = 15;

            Generator gen = new Generator(NUM);
            gen.Generate();
            DisplayPuzzle(gen.GeneratedPuzzle);

            Console.WriteLine();
            Console.WriteLine();
            DisplayPuzzle(gen.SolvedPuzzle);
            Assert.IsTrue(Validator.IsSolved(gen.SolvedPuzzle));
            Assert.AreEqual(NUM, gen.NumberOfRandomValues);
        }
    }
}
