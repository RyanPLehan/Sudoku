using System;


namespace Sudoku
{
    /// <summary>
    /// General utilities 
    /// </summary>
    /// <history>
    /// Date        Author                  Description
    /// 2019.09.26  Lehan, Ryan             Re-factored into separation of concerns from monolithic class
    /// </history>
    internal static class Utils
    {
        /// <summary>
        /// This will transpose the 2 dimensional array to a single dimensional array
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="rowLength"></param>
        /// <param name="colIndex"></param>
        /// <param name="colLength"></param>
        /// <returns></returns>
        public static T[] Transpose<T>(T[,] source, int rowIndex, int rowLength, int colIndex, int colLength)
        {
            // Calculate maximum array size
            T[] ret = new T[rowLength * colLength];

            int rowMax = rowIndex + rowLength;
            int colMax = colIndex + colLength;

            // Iterate through source and reference to (not clone) existing array item
            int idx = 0;
            for (int row = rowIndex; row < rowMax; row++)
                for (int col = colIndex; col < colMax; col++)
                    ret[idx++] = source[row, col];

            return ret;
        }
    }
}
