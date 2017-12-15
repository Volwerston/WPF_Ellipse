using System;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPF_Hexagones
{
	[Serializable]
	public class Hexagone
	{
		private PointCollection points;
		public PointCollection Points
		{
			get
			{
				return points;
			}
			set
			{
				if(value.Count!=5)
				{
					throw new ArgumentException("Number of point must be 5");
				}
				points = value;
			}
		}
		public Color HexagoneColor { get; set; }
		public Hexagone() { }
		public Hexagone(Polygon figure)
		{
			Points = figure.Points;
			HexagoneColor = (figure.Fill as SolidColorBrush).Color;
		}
	}
}
