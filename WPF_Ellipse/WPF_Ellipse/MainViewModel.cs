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

namespace WPF_Ellipse
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Canvas DrawCanvas { get; set; }
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

        public ICommand DrawClick_Command { get; private set; }
        public ICommand ApplyColor_Command { get; set; }

        public ICommand ClearWindow_Command { get; private set; }
        public ICommand OpenFile_Command { get; private set; }
        public ICommand SaveFile_Command { get; private set; }
        public ICommand CloseWindow_Command { get; private set; }

        public ICommand SelectEllipse_Command { get; private set; }
        public ICommand Drag_Command { get; private set; }
        private bool AllowDragging { get; set; }
        private Point MousePosition { get; set; }

        private Ellipse SelectedEllipse { get; set; }

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

            SelectEllipse_Command = new RelayCommand(SelectEllipse);
            Drag_Command = new RelayCommand(Drag);

            MousePosition = default(Point);
        }

        private void DrawClick(object obj)
        {
            if(MousePosition != default(Point))
            {
                return;
            }

            Point mousePoint = Mouse.GetPosition((IInputElement)obj);

            Point prev = PrevPoint;

            PrevPoint = mousePoint;

            ++CountEdges;

            if (CountEdges == 2)
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

                if (colorWin.ShowDialog() == true)
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

                for (int i = 0; i < ellipses.Count; ++i)
                {
                    Ellipses.Add(new Ellipse()
                    {
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

                foreach (var elem in Ellipses)
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


        private void SelectEllipse(object obj)
        {
            if (MousePosition != default(Point))
            {
                MousePosition = default(Point);
            }
            else
            {
                Ellipse curEllipse = (obj as Ellipse);

                MousePosition = new Point(-5, -5);

                curEllipse.MouseDown += new MouseButtonEventHandler(Ellipse_MouseDown);
                curEllipse.MouseUp += new MouseButtonEventHandler(Canvas_MouseUp);
                curEllipse.MouseMove += new MouseEventHandler(Canvas_MouseMove);
            }
        }

        private void Drag(object obj)
        {
            Canvas plane = (obj as Canvas);
        }

        void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AllowDragging = true;
            SelectedEllipse = sender as Ellipse;
            MousePosition = e.GetPosition(SelectedEllipse);

            SelectedEllipse.CaptureMouse();
        }

        void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AllowDragging = false;
            SelectedEllipse.ReleaseMouseCapture();
        }

        void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (AllowDragging)
            {
                Point punto = e.GetPosition(sender as IInputElement);
                int mouseX = (int)punto.X;
                int mouseY = (int)punto.Y;

                double _left = SelectedEllipse.Margin.Left + e.GetPosition(sender as IInputElement).X - MousePosition.X;
                double _top = SelectedEllipse.Margin.Top + e.GetPosition(sender as IInputElement).Y - MousePosition.Y;

                SelectedEllipse.Margin = new Thickness(_left, _top, 0, 0);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
