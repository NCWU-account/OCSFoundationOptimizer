using OCSFoundationOptimizer.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OCSFoundationOptimizer
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();

            DataContext = _viewModel;
        }
        

        // =========================================================
        // 计算
        // =========================================================

        private void CalculateButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            _viewModel.Calculate();
        }


        // =========================================================
        // 生成计算书
        // =========================================================

        private void GenerateButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            // 后面在这里写 Word / PDF 计算书生成
            MessageBox.Show("后续实现计算书生成");
        }


        // =========================================================
        // 图片1缩放
        // =========================================================

        private void Image1_MouseWheel(
            object sender,
            MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ZoomImage(ImageScale1, 1.1);
            }
            else
            {
                ZoomImage(ImageScale1, 0.9);
            }

            e.Handled = true;
        }


        private void ZoomIn1_Click(
            object sender,
            RoutedEventArgs e)
        {
            ZoomImage(ImageScale1, 1.2);
        }


        private void ZoomOut1_Click(
            object sender,
            RoutedEventArgs e)
        {
            ZoomImage(ImageScale1, 0.8);
        }


        private void ResetZoom1_Click(
            object sender,
            RoutedEventArgs e)
        {
            ImageScale1.ScaleX = 1;
            ImageScale1.ScaleY = 1;
        }


        // =========================================================
        // 图片2缩放
        // =========================================================

        private void Image2_MouseWheel(
            object sender,
            MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ZoomImage(ImageScale2, 1.1);
            }
            else
            {
                ZoomImage(ImageScale2, 0.9);
            }

            e.Handled = true;
        }


        private void ZoomIn2_Click(
            object sender,
            RoutedEventArgs e)
        {
            ZoomImage(ImageScale2, 1.2);
        }


        private void ZoomOut2_Click(
            object sender,
            RoutedEventArgs e)
        {
            ZoomImage(ImageScale2, 0.8);
        }


        private void ResetZoom2_Click(
            object sender,
            RoutedEventArgs e)
        {
            ImageScale2.ScaleX = 1;
            ImageScale2.ScaleY = 1;
        }


        // =========================================================
        // 图片缩放
        // =========================================================

        private void ZoomImage(
            ScaleTransform scale,
            double factor)
        {
            double newScale = scale.ScaleX * factor;

            // 最大放大 5 倍
            if (newScale > 5)
                newScale = 5;

            // 最小缩小到 0.2 倍
            if (newScale < 0.2)
                newScale = 0.2;

            scale.ScaleX = newScale;
            scale.ScaleY = newScale;
        }
    }
}