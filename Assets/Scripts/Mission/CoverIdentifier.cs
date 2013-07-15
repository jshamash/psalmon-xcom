using System;
using UnityEngine;
using System.Collections;

[ProcessingPriority(3)]
public class CoverIdentifier : BuildGraph.IProcessGrid
{

    public class Cover
    {
        [Flags]
        public enum CoverDirection
        {
            North = 1,
            NorthEast = 2,
            East = 4,
            SouthEast = 8,
            South = 16,
            SouthWest = 32,
            West = 64,
            NorthWest = 128
        }

        private static readonly CoverDirection[] Directions =
            {
                CoverDirection.North, CoverDirection.NorthEast, CoverDirection.East, CoverDirection.SouthEast, CoverDirection.South, CoverDirection.SouthWest,
                CoverDirection.West, CoverDirection.NorthWest
            };

        public static readonly Vector3[] CoverVectors =
        {
            Vector3.forward,
            (Vector3.forward + Vector3.right).normalized,
            Vector3.right,
            (Vector3.right - Vector3.forward).normalized,
            -Vector3.forward,
            -(Vector3.forward + Vector3.right).normalized,
            -Vector3.right,
            (-Vector3.right + Vector3.forward).normalized
        };

        public byte[,] lowCover;
        public byte[,] highCover;

        
        public int GetCoverScore(BuildGraph.GridPosition position)
        {
            var result = CountBits(lowCover[position.x, position.y]) + 
                CountBits(highCover[position.x, position.y]) * 2;

            return 24 - result;
        }

        public int GetCoverScore(BuildGraph.GridPosition position, Vector3 fromDirection)
        {
            var result = 0;
            var dir = Cover.GetDirection(fromDirection);
            if (((CoverDirection)lowCover[position.x, position.y] & dir) != 0)
                result++;
            if (((CoverDirection)highCover[position.x, position.y] & dir) != 0)
                result+=2;
            return 3 - result;
        }

        private static int CountBits(byte c)
        {
            var result = 0;
            for (var i = 0; i < 8; i++)
            {
                if ((c & (1 << i)) != 0)
                {
                    result++;
                }
            }
            return result;
        }

        public static CoverDirection GetDirection(Vector3 direction)
        {
            var f = Vector3.Angle(Vector3.forward, direction);
            f = (direction.x < 0f ? 180f + f : f) - 22.5f ;
            if (f < 0) f += 360;
            var angle = Mathf.FloorToInt(f/45);
            return Directions[angle];
        }
    }

    

    public void ProcessGrid(BuildGraph builder)
    {
        var cover = builder.cells.Get<Cover>();
        cover.lowCover = new byte[builder.width,builder.height];
        cover.highCover = new byte[builder.width, builder.height];
        foreach (var cell in builder.allWalkableCells)
        {
            //Cast rays for each direction
            for (var i = 0; i < 8; i++)
            {
                
                var startPosition = builder.GetWorldPosition(cell) + Vector3.up * 0.4f;

                if (Physics.Raycast(startPosition, Cover.CoverVectors[i], 4, builder.culling))
                {
                    cover.lowCover[cell.x, cell.y] = (byte)(cover.lowCover[cell.x, cell.y] | (byte)(1 << i));
                }
                startPosition += Vector3.up;
                if (Physics.Raycast(startPosition, Cover.CoverVectors[i], 4, builder.culling))
                {
                    cover.highCover[cell.x, cell.y] = (byte)(cover.highCover[cell.x, cell.y] | (byte)(1 << i));
                }
            }
        }
    }
}
