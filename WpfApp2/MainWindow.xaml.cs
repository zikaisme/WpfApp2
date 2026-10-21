using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2026_WpfApp3
{
    public partial class MainWindow : Window
    {
        // 核心狀態變數
        Color strokeColor = Colors.Black;
        Color fillColor = Colors.Transparent;
        int strokeThickness = 1;
        string shapeType = "Line";
        string actionType = "Draw"; // "Draw" 或 "Eraser"
        Point start = new Point(0, 0);

        Shape currentShape;

        public MainWindow()
        {
            InitializeComponent();
            DisplayStatus();
        }

        // 同步更新狀態列
        private void DisplayStatus()
        {
            if (ShapeLabel != null) ShapeLabel.Content = $"模式/工具 : {shapeType}";
            if (ColorLabel != null) ColorLabel.Content = $"筆刷色彩:{strokeColor} 填滿色彩:{fillColor} 線條粗細:{strokeThickness}";
        }

        // 切換工具 (RadioButton: 包含形狀與橡皮擦)
        private void ShapeButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag != null)
            {
                shapeType = rb.Tag.ToString();

                if (shapeType == "Eraser")
                {
                    actionType = "Eraser";
                    if (MyCanvas != null) MyCanvas.Cursor = Cursors.Hand;
                }
                else
                {
                    actionType = "Draw";
                    if (MyCanvas != null) MyCanvas.Cursor = Cursors.Pen;
                }

                this.Title = $"WPF小畫家v0.1 - 目前工具：{shapeType}";
                DisplayStatus();
            }
        }

        // 清除整個畫布
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            MyCanvas.Children.Clear();
            if (StatusLabel != null) StatusLabel.Content = "畫布已清空";
        }

        // 筆刷顏色變更
        private void StrokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                strokeColor = e.NewValue.Value;
                DisplayStatus();
            }
        }

        // 填滿顏色變更
        private void FillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                fillColor = e.NewValue.Value;
                DisplayStatus();
            }
        }

        // 筆刷粗細變更
        private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = (int)e.NewValue;
            DisplayStatus();
        }

        // 滑鼠進入畫布區域
        private void MyCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            MyCanvas.Cursor = (actionType == "Eraser") ? Cursors.Hand : Cursors.Pen;
        }

        // 滑鼠按下 (繪圖起點建立 / 點擊擦除)
        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(MyCanvas);

            if (actionType == "Draw")
            {
                MyCanvas.Cursor = Cursors.Cross;
                var stroke = new SolidColorBrush(strokeColor);
                var fill = new SolidColorBrush(fillColor);

                switch (shapeType)
                {
                    case "Line":
                        currentShape = new Line { X1 = start.X, Y1 = start.Y, X2 = start.X, Y2 = start.Y, Stroke = stroke, StrokeThickness = strokeThickness };
                        break;
                    case "Rectangle":
                        currentShape = new Rectangle { Stroke = stroke, Fill = fill, StrokeThickness = strokeThickness };
                        Canvas.SetLeft(currentShape, start.X);
                        Canvas.SetTop(currentShape, start.Y);
                        break;
                    case "Ellipse":
                        currentShape = new Ellipse { Stroke = stroke, Fill = fill, StrokeThickness = strokeThickness };
                        Canvas.SetLeft(currentShape, start.X);
                        Canvas.SetTop(currentShape, start.Y);
                        break;
                    case "Freeline":
                    case "Polyline":
                        var polyline = new Polyline { Stroke = stroke, StrokeThickness = strokeThickness };
                        polyline.Points.Add(start);
                        currentShape = polyline;
                        break;
                }

                if (currentShape != null) MyCanvas.Children.Add(currentShape);
            }
            else if (actionType == "Eraser")
            {
                // 點擊擦除：若點擊對象為 Shape 元件則直接移除
                if (e.OriginalSource is Shape clickedShape)
                {
                    MyCanvas.Children.Remove(clickedShape);
                }
            }
        }

        // 滑鼠移動 (動態預覽拖曳 / 座標追蹤 / 拖曳擦除)
        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point dest = e.GetPosition(MyCanvas);
            if (StatusLabel != null) StatusLabel.Content = $"座標 : ({Math.Round(dest.X)}, {Math.Round(dest.Y)})";

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (actionType == "Draw" && currentShape != null)
                {
                    if (currentShape is Line line)
                    {
                        line.X2 = dest.X;
                        line.Y2 = dest.Y;
                    }
                    else if (currentShape is Polyline polyline)
                    {
                        polyline.Points.Add(dest);
                    }
                    else
                    {
                        // 反向拖曳尺寸與原點校正
                        Canvas.SetLeft(currentShape, Math.Min(start.X, dest.X));
                        Canvas.SetTop(currentShape, Math.Min(start.Y, dest.Y));
                        currentShape.Width = Math.Abs(dest.X - start.X);
                        currentShape.Height = Math.Abs(dest.Y - start.Y);
                    }
                }
                else if (actionType == "Eraser")
                {
                    // 滑動擦除：拖曳經過處的圖形物件自動刪除
                    if (e.OriginalSource is Shape hoveredShape)
                    {
                        MyCanvas.Children.Remove(hoveredShape);
                    }
                }
            }
        }

        // 放開滑鼠
        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyCanvas.Cursor = (actionType == "Eraser") ? Cursors.Hand : Cursors.Pen;
            currentShape = null;
        }
    }
}