using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Controls;
using WPF_Ellipse;

namespace WPF_Hexagones
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<Ellipse> Ellipses { get; set; }
		private Ellipse CurrentEllipse { get; set; }

        private Point PrevPoint { get; set; }
		private uint CountEdges { get; set; }

		private Color currentColor;
		public Color CurrentColor
		{
			get
			{
				return currentColor;
			}
			set
			{
				currentColor = value;
				OnPropertyChanged("CurrentColor");
			}
		}

		//Painting
		public ICommand DrawClick_Command { get; private set; }
		public ICommand ApplyColor_Command { get; set; }

		//File Menu
		public ICommand ClearWindow_Command { get; private set; }
		public ICommand OpenFile_Command { get; private set; }
		public ICommand SaveFile_Command { get; private set; }
		public ICommand CloseWindow_Command { get; private set; }

		//Selecting and draging hexogones
		public ICommand SelectHexagone_Command { get; private set; }
		public ICommand Drag_Command { get; private set; }
		private bool AllowDragging { get; set; }
		private Point MousePosition { get; set; }
		private Polygon SelectedHexagone { get; set; }

		public MainViewModel()
		{
            Ellipses = new ObservableCollection<Ellipse>();      

			CountEdges = 0;
			CurrentColor = Colors.Red;
			CurrentEllipse = new Ellipse();


            PrevPoint = new Point(-1, -1);

			ClearWindow_Command = new RelayCommand(ClearWindow);
			OpenFile_Command = new RelayCommand(OpenFile);
			SaveFile_Command = new RelayCommand(SaveFile);
			CloseWindow_Command = new RelayCommand(CloseWindow);
			DrawClick_Command = new RelayCommand(DrawClick);
			ApplyColor_Command = new RelayCommand(ApplyColor);

			SelectHexagone_Command = new RelayCommand(SelectHexagone);
			Drag_Command = new RelayCommand(Drag);
		}

		private void DrawClick(object obj)
		{
			Point mousePoint = Mouse.GetPosition((IInputElement)obj);

            Point prev = PrevPoint;

            PrevPoint = mousePoint;

            ++CountEdges;

			if(CountEdges==2)
			{
				ColorsWindow colorWin = new ColorsWindow(this);

                double width = Math.Abs(prev.X - PrevPoint.X);
                double height = Math.Abs(prev.Y - PrevPoint.Y);

                double left = Math.Min(prev.X, PrevPoint.X);
                double top = Math.Min(prev.Y, PrevPoint.Y);

                CurrentEllipse.Width = width;
                CurrentEllipse.Height = height;
                CurrentEllipse.Stroke = new SolidColorBrush(Colors.Black);

                CurrentEllipse.Margin = new Thickness(left, top, 0, 0);

                if (colorWin.ShowDialog()==true)
				{
					CurrentEllipse.Fill = new SolidColorBrush(CurrentColor);
				}

				CurrentEllipse.Name = String.Format("Ellipse_{0}", Ellipses.Count + 1);
				Ellipses.Add(CurrentEllipse);

				CurrentEllipse = new Ellipse();
				OnPropertyChanged("Ellipses");
				CountEdges = 0;
			}

		}

		private void ApplyColor(object obj)
		{
			ColorsWindow colorsWindow = (ColorsWindow)obj;
			colorsWindow.DialogResult = true;
			colorsWindow.Close();
		}

		//File Menu
		private void ClearWindow(object obj)
		{
			Ellipses.Clear();
			OnPropertyChanged("Ellipses");
		}

		private void OpenFile(object obj)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.DefaultExt = ".xml";
			openFileDialog.Filter = "XML documents (.xml)|*.xml";
			if (openFileDialog.ShowDialog() == true)
			{
				string fileName = openFileDialog.FileName;
				List<EllipseDTO> ellipses = new List<EllipseDTO>();

				XmlSerializer serializer = new XmlSerializer(typeof(List<EllipseDTO>));

				using (XmlReader reader = XmlReader.Create(fileName))
				{
					ellipses = (List<EllipseDTO>)serializer.Deserialize(reader);
				}

				Ellipses.Clear();

				for(int i=0; i<ellipses.Count; ++i)
				{
					Ellipses.Add(new Ellipse() {
                        Name = String.Format("Ellipse_{0}", i + 1),
                        Stroke = Brushes.Black,
                        Fill = new SolidColorBrush(ellipses[i].Color),
                        Margin = new Thickness(ellipses[i].CenterX, ellipses[i].CenterY, 0, 0),
                        Width = ellipses[i].Width,
                        Height = ellipses[i].Height
                    });
				}

				OnPropertyChanged("Ellipses");
			}
		}

		private void SaveFile(object obj)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.DefaultExt = ".xml";
			saveFileDialog.FileName = "New_shapes.xml";
			saveFileDialog.Filter = "XML documents (.xml)|*.xml";
			if (saveFileDialog.ShowDialog() == true)
			{
				string fileName = saveFileDialog.FileName;
				List<EllipseDTO> ellipses = new List<EllipseDTO>();

				foreach(var elem in Ellipses)
				{
                    ellipses.Add(new EllipseDTO()
                    {
                        CenterX = elem.Margin.Left,
                        CenterY = elem.Margin.Top,
                        Color = (elem.Fill as SolidColorBrush).Color,
                        Height = elem.Height,
                        Width = elem.Width
                    });
				}

				using (Stream outputFile = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(List<EllipseDTO>));
					serializer.Serialize(outputFile, ellipses);
				}
			}
		}

		private void CloseWindow(object obj)
		{
			(obj as MainWindow).Close();
		}

		//Selecting and draging hexogones
		private void SelectHexagone(object obj)
		{
			Polygon curHexagone = (obj as Polygon);
			curHexagone.MouseDown += new MouseButtonEventHandler(Hexagone_MouseDown);
			OnPropertyChanged("Hexagones");
		}

		private void Drag(object obj)
		{
			Canvas plane = (obj as Canvas);
			plane.MouseMove += new MouseEventHandler(Canvas_MouseMove);
			plane.MouseUp += new MouseButtonEventHandler(Canvas_MouseUp);
		}

		//Events
		void Hexagone_MouseDown(object sender, MouseButtonEventArgs e)
		{
			AllowDragging = true;
			SelectedHexagone = sender as Polygon;
			MousePosition = e.GetPosition(SelectedHexagone);
		}

		void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			AllowDragging = false;
		}

		void Canvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (AllowDragging)
			{
				Canvas.SetLeft(SelectedHexagone, e.GetPosition(sender as IInputElement).X - MousePosition.X);
				Canvas.SetTop(SelectedHexagone, e.GetPosition(sender as IInputElement).Y - MousePosition.Y);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
