using System;
using System.Collections;
using System.Collections.Generic;

namespace NationMap
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			/*
			for (int i = 0; i<100; i++) {
				Random rnd = new Random (unchecked((int)DateTime.Now.Ticks));
				int loc = rnd.Next (1, 100);
				Console.WriteLine (loc);
			}
			return;
			*/
			NationMap map = new NationMap ();

			ArrayList alCells = map.GetCellsData (0, 0, 640, 1136, 1.0f);
			if (alCells == null || alCells.Count == 0)
				Console.WriteLine ("Do not fetch any cell data.");
			else {
				Console.WriteLine ("Count: " + alCells.Count);
				Console.WriteLine ("---------------------------");

				foreach (CellData cell in alCells)
					Console.WriteLine (cell.ToString ());

				Console.WriteLine ("---------------------------");
				Console.WriteLine ("Count: " + alCells.Count);
			}
		}
	}

	class CellData
	{
		private CellData ()
		{

		}

		public CellData (int x, int y, CellType type)
		{
			this.PosX = x;
			this.PoxY = y;
			this.Type = type;
		}

		public CellData (int x, int y)
		{
			CellType type = CellType.DISABLED;
			int[] percent = new int[] { 5, 40, 25, 20, 10 };
			if (percent.Length != (int)CellType.NUM)
				throw new Exception ("You need setting enough percent parameter.");
			int total = 0;
			foreach (int p in percent)
				total += p;
			if (total != 100)
				throw new Exception ("All the Numbers together is not equal to 100.");

			Random rnd = new Random (unchecked((int)DateTime.Now.Ticks));
			int loc = rnd.Next (1, 100);
			for (int i = 0; i < percent.Length; i++) {
				loc -= percent [i];
				if (loc < 0) {
					type = (CellType)i;
					break;
				}
			}
			if (loc > 0)
				type = CellType.OCCUPIED;

			this.PosX = x;
			this.PoxY = y;
			this.Type = type;
		}

		~CellData ()
		{
		}

		public enum CellType
		{
			DISABLED = 0,
			FREE = 1,
			FORESTRY = 2,
			MINE = 3,
			OCCUPIED = 4,
			NUM = 5,
		}

		public CellType Type{ get; set; }

		public int PosX{ get; set; }

		public int PoxY{ get; set; }

		public override string ToString ()
		{
			string str = Convert.ToString (this.PosX); 
			str = str.PadLeft (4, ' '); 
			str += "," + this.PoxY;

			str = str.PadRight (12, ' '); 


			switch (this.Type) {
			case CellType.DISABLED:
				str += "DISABLED";
				break;
			case CellType.FREE:
				str += "FREE";
				break;
			case CellType.FORESTRY:
				str += "FORESTRY";
				break;
			case CellType.MINE:
				str += "MINE";
				break;
			case CellType.OCCUPIED:
				str += "OCCUPIED";
				break;
			default:
				str += "UNKNOWN";
				break;
			}

			return str;
		}
	}

	class NationMap
	{
		public const int HOR_CELL_NUM = 256;
		public const int VER_CELL_NUM = 512;
		public const int CELL_WIDTH = 172;
		public const int CELL_HEIGHT = 86;
		public readonly static Dictionary<string, CellData> s_CellDataHolder = new Dictionary<string, CellData> ();

		public NationMap ()
		{
			//insert all disabled cell
			int x = 0;
			for (int _y = 0; _y <= VER_CELL_NUM + 1; _y++) {
				s_CellDataHolder.Add (x + "," + _y, new CellData (x, _y, CellData.CellType.DISABLED));
			}

			int y = 0;
			for (int _x = 0; _x <= VER_CELL_NUM + 1; _x++) {
				if (!s_CellDataHolder.ContainsKey (_x + "," + y))
					s_CellDataHolder.Add (_x + "," + y, new CellData (_x, y, CellData.CellType.DISABLED));
			}
		}

		~NationMap ()
		{
		}

		/// <summary>
		/// Fetch cells
		/// </summary>
		/// <returns>null or an ArrayList instance with cells</returns>
		/// <param name="anchorX">the anchor point coordinates(X) on the big map</param>
		/// <param name="anchorY">the anchor point coordinates(Y) on the big map</param>
		/// <param name="screenW">the original screen width</param>
		/// <param name="screenH">the original screen height</param>
		/// <param name="scale">scale，range = (0.0, 1.0]</param>
		public ArrayList GetCellsData (int anchorX, int anchorY, int oriScreenW, int oriScreenH, float scale)
		{
			if (oriScreenW < 0 || oriScreenH < 0)
				return null;

			if (scale <= 0 || scale > 1.0) {
				return null;
			}

			if (anchorX < 0)
				anchorX = 0;

			if (anchorX + oriScreenW > HOR_CELL_NUM * CELL_WIDTH)
				anchorX = HOR_CELL_NUM * CELL_WIDTH - oriScreenW;

			if (anchorY < 0)
				anchorY = 0;

			if (anchorY + oriScreenH > VER_CELL_NUM * CELL_HEIGHT)
				anchorY = VER_CELL_NUM * CELL_HEIGHT - oriScreenH;

			ArrayList _allCells = new ArrayList ();

			float _realCellWidth = CELL_WIDTH * scale;//the scaled width
			float _realCellHeight = CELL_HEIGHT * scale;//the scaled height

			float _realHalfCellWidth = _realCellWidth / 2;
			float _realHalfCellHeight = _realCellHeight / 2;

			//int _skipTopHalfCell = Math.Floor (anchorY / _realHalfCellHeight);
			//int _skipLeftHalfCell = Math.Floor (anchorX / _realHalfCellWidth);

			int _minY = (int)Math.Floor (anchorY / _realHalfCellHeight);//3
			int _maxY = (int)Math.Ceiling ((anchorY + oriScreenH) / _realHalfCellHeight);//13

			//expand range
			_minY -= (int)Math.Ceiling (oriScreenH / _realCellHeight);
			_maxY += (int)Math.Ceiling (oriScreenH / _realCellHeight);

			if (_minY < 0)
				_minY = 0;

			if (_maxY >= VER_CELL_NUM)
				_maxY = VER_CELL_NUM - 1;

			int _minX = (int)Math.Floor (anchorX / _realHalfCellWidth);//3
			int _maxX = (int)Math.Ceiling ((anchorX + oriScreenW) / _realHalfCellWidth);//8

			//expand range
			_minX -= (int)Math.Ceiling (oriScreenW / _realCellWidth);
			_maxX += (int)Math.Ceiling (oriScreenW / _realCellWidth);

			if (_minX < 0)
				_minX = 0;

			if (_maxX >= HOR_CELL_NUM)
				_maxX = HOR_CELL_NUM - 1;

			for (int x = _minX; x < _maxX; x++) {
				for (int y = _minY; y < _maxY; y++) {
					string key = x + "," + y;
					if (!s_CellDataHolder.ContainsKey (key)) {
						CellData cell = FetchCell (x, y);
						if (cell != null) {
							s_CellDataHolder.Add (key, cell);
							_allCells.Add (cell);
						}
					} else {
						_allCells.Add (s_CellDataHolder [key]);
					}
				}
			}

			return _allCells;
		}

		private CellData FetchCell (int x, int y)
		{
			if (x < 0 || y < 0)
				return null;

			if (y % 2 != 0 && x > HOR_CELL_NUM)
				return null;

			if (x % 2 != 0 && y > VER_CELL_NUM)
				return null;

			CellData cell = null;

			try {
				cell = new CellData (x, y);
			} catch (Exception e) {
				throw e;
			}

			return cell;
		}
	}
}
