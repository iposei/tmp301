using System;
using System.Collections;

namespace NationMap
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
		}
	}

	class CellData
	{
		public CellData ()
		{

		}

		~CellData ()
		{
		}

		public int Type{ get; set; }

		public int PosX{ get; set; }

		public int PoxY{ get; set; }
	}

	class NationMap
	{
		public const int VER_CELL_NUM = 256;
		public const int HOR_CELL_NUM = 512;
		public const int CELL_WIDTH = 172;
		public const int CELL_HEIGHT = 86;

		public struct Point
		{
			public int X;
			public int Y;
		}

		public BigMap ()
		{
		}

		~BigMap ()
		{
		}

		/// <summary>
		/// Fetch all cells in the screen
		/// </summary>
		/// <returns>null or an ArrayList instance with cells</returns>
		/// <param name="anchorX">the anchor point coordinates(X) on the big map</param>
		/// <param name="anchorY">the anchor point coordinates(Y) on the big map</param>
		/// <param name="screenW">the original screen width</param>
		/// <param name="screenH">the original screen height</param>
		/// <param name="scale">scale，range = (0.0, 1.0]</param>
		public ArrayList GetCellsInScreen (int anchorX, int anchorY, int oriScreenW, int oriScreenH, float scale)
		{
			if (oriScreenW < 0 || oriScreenH < 0)
				return null;

			if (scale <= 0 || scale > 1.0) {
				return null;
			}

			ArrayList _allCells = new ArrayList ();

			float _realCellWidth = CELL_WIDTH * scale;//the scaled width
			float _realCellHeight = CELL_HEIGHT * scale;//the scaled height

			float _realHalfCellWidth = _realCellWidth / 2;
			float _realHalfCellHeight = _realCellHeight / 2;

			//int _skipTopHalfCell = Math.Floor (anchorY / _realHalfCellHeight);
			//int _skipLeftHalfCell = Math.Floor (anchorX / _realHalfCellWidth);

			int _minY = (int)Math.Floor (anchorY / _realHalfCellHeight);//3
			int _maxY = (int)Math.Ceiling ((anchorY + oriScreenH) / _realHalfCellHeight);//13

			int _minX = (int)Math.Floor (anchorX / _realHalfCellWidth);//3
			int _maxX = (int)Math.Ceiling ((anchorX + oriScreenW) / _realHalfCellWidth);//8

			for (int x = _minX; x < _maxX; x++) {
				for (int y = _minY; y < _maxY; y++) {

				}
			}


			return _allCells;

		}

		public int isLeft(Point P0, Point P1,Point P2)
		{
			int abc= ((P1.X - P0.X) * (P2.Y - P0.Y) - (P2.X - P0.X) * (P1.Y - P0.Y));
			return abc;

		}

		private bool PointInFences(Point pnt1, Point[] fencePnts)
		{
			int wn = 0,j=0; //wn 计数器 j第二个点指针
			for (int i = 0; i < fencePnts.Length; i++)
			{//开始循环
				if (i == fencePnts.Length - 1)
					j = 0;//如果 循环到最后一点 第二个指针指向第一点
				else
					j = j + 1; //如果不是 ，则找下一点

				if (fencePnts[i].Y <= pnt1.Y) // 如果多边形的点 小于等于 选定点的 Y 坐标
				{
					if (fencePnts[j].Y > pnt1.Y) // 如果多边形的下一点 大于于 选定点的 Y 坐标
					{
						if (isLeft(fencePnts[i], fencePnts[j], pnt1) > 0)
						{
							wn++;
						}
					}
				}
				else
				{
					if (fencePnts[j].Y <= pnt1.Y)
					{
						if (isLeft(fencePnts[i], fencePnts[j], pnt1) < 0)
						{
							wn--;
						}
					}
				}
			}
			if (wn == 0)
				return false;
			else
				return true;
		}
	}
}
