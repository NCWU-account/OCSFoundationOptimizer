using OCSFoundationOptimizer.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace OCSFoundationOptimizer
{
    public partial class MainWindow : Window
    {
        // =====================================================
        // 图片拖动
        // =====================================================

        private bool _isDragging = false;

        private Point _lastMousePosition;


        // =====================================================
        // 当前图片缩放比例
        // =====================================================

        private double _currentScale = 1.0;


        // =====================================================
        // 图片原始尺寸
        // =====================================================

        private double _imageWidth;

        private double _imageHeight;


        // =====================================================
        // 构造函数
        // =====================================================

        public MainWindow()
        {
            InitializeComponent();

            DataContext =
                new MainViewModel();

            Loaded += MainWindow_Loaded;
        }


        // =====================================================
        // 窗口加载
        // =====================================================

        private void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(
                new Action(
                    FitImageToViewer));
        }


        // =====================================================
        // 图片自适应
        // =====================================================

        private void FitImageToViewer()
        {
            if (SampleImage.Source == null)
            {
                return;
            }


            _imageWidth =
                SampleImage.Source.Width;

            _imageHeight =
                SampleImage.Source.Height;


            if (_imageWidth <= 0 ||
                _imageHeight <= 0)
            {
                return;
            }


            double viewerWidth =
                ImageViewer.ActualWidth;

            double viewerHeight =
                ImageViewer.ActualHeight;


            if (viewerWidth <= 0 ||
                viewerHeight <= 0)
            {
                return;
            }


            // =================================================
            // 留出边距
            // =================================================

            double availableWidth =
                viewerWidth - 20;

            double availableHeight =
                viewerHeight - 20;


            // =================================================
            // 根据宽高计算缩放比例
            // =================================================

            double scaleX =
                availableWidth / _imageWidth;

            double scaleY =
                availableHeight / _imageHeight;


            // =================================================
            // 保持比例
            // =================================================

            _currentScale =
                Math.Min(
                    scaleX,
                    scaleY);


            if (_currentScale <= 0 ||
                double.IsNaN(_currentScale) ||
                double.IsInfinity(_currentScale))
            {
                _currentScale = 1.0;
            }


            // =================================================
            // 设置缩放
            // =================================================

            ImageScale.ScaleX =
                _currentScale;

            ImageScale.ScaleY =
                _currentScale;


            // =================================================
            // 计算居中位置
            // =================================================

            double scaledWidth =
                _imageWidth *
                _currentScale;

            double scaledHeight =
                _imageHeight *
                _currentScale;


            double offsetX =
                (viewerWidth -
                 scaledWidth) / 2.0;


            double offsetY =
                (viewerHeight -
                 scaledHeight) / 2.0;


            // =================================================
            // 设置图片位置
            // =================================================

            ImageTranslate.X =
                Math.Max(0, offsetX);

            ImageTranslate.Y =
                Math.Max(0, offsetY);


            // =================================================
            // 更新布局
            // =================================================

            ImageScrollViewer.ScrollToHome();
        }


        // =====================================================
        // 鼠标滚轮
        // =====================================================

        private void ImageViewer_MouseWheel(
            object sender,
            MouseWheelEventArgs e)
        {
            if (SampleImage.Source == null)
            {
                return;
            }


            // =================================================
            // 当前鼠标位置
            // =================================================

            Point mousePosition =
                e.GetPosition(ImageViewer);


            // =================================================
            // 计算缩放倍率
            // =================================================

            double zoomFactor =
                e.Delta > 0
                    ? 1.15
                    : 1.0 / 1.15;


            double oldScale =
                _currentScale;


            double newScale =
                oldScale *
                zoomFactor;


            // =================================================
            // 缩放范围
            // =================================================

            const double minScale = 0.1;

            const double maxScale = 5.0;


            newScale =
                Math.Max(
                    minScale,
                    Math.Min(
                        maxScale,
                        newScale));


            if (Math.Abs(
                    newScale -
                    oldScale) < 0.00001)
            {
                e.Handled = true;
                return;
            }


            // =================================================
            // 当前图片左上角
            // =================================================

            double oldX =
                ImageTranslate.X;

            double oldY =
                ImageTranslate.Y;


            // =================================================
            // 计算鼠标相对于图片的位置
            //
            // 缩放以后保持鼠标指向的位置不变
            // =================================================

            double imagePointX =
                (mousePosition.X -
                 oldX) /
                oldScale;


            double imagePointY =
                (mousePosition.Y -
                 oldY) /
                oldScale;


            // =================================================
            // 设置新的缩放
            // =================================================

            _currentScale =
                newScale;


            ImageScale.ScaleX =
                newScale;

            ImageScale.ScaleY =
                newScale;


            // =================================================
            // 重新计算图片位置
            // =================================================

            double newX =
                mousePosition.X -
                imagePointX *
                newScale;


            double newY =
                mousePosition.Y -
                imagePointY *
                newScale;


            ImageTranslate.X =
                newX;

            ImageTranslate.Y =
                newY;


            e.Handled = true;
        }


        // =====================================================
        // 鼠标左键按下
        // =====================================================

        private void ImageViewer_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (e.LeftButton !=
                MouseButtonState.Pressed)
            {
                return;
            }


            _isDragging = true;


            _lastMousePosition =
                e.GetPosition(ImageViewer);


            ImageViewer
                .CaptureMouse();


            Mouse.OverrideCursor =
                Cursors.Hand;


            e.Handled = true;
        }


        // =====================================================
        // 鼠标移动
        // =====================================================

        private void ImageViewer_MouseMove(
            object sender,
            MouseEventArgs e)
        {
            if (!_isDragging)
            {
                return;
            }


            if (e.LeftButton !=
                MouseButtonState.Pressed)
            {
                StopDragging();
                return;
            }


            Point currentPosition =
                e.GetPosition(ImageViewer);


            Vector movement =
                currentPosition -
                _lastMousePosition;


            // =================================================
            // 图片跟随鼠标移动
            // =================================================

            ImageTranslate.X +=
                movement.X;

            ImageTranslate.Y +=
                movement.Y;


            _lastMousePosition =
                currentPosition;


            e.Handled = true;
        }


        // =====================================================
        // 鼠标左键释放
        // =====================================================

        private void ImageViewer_MouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            StopDragging();

            e.Handled = true;
        }


        // =====================================================
        // 停止拖动
        // =====================================================

        private void StopDragging()
        {
            if (!_isDragging)
            {
                return;
            }


            _isDragging = false;


            if (ImageViewer.IsMouseCaptured)
            {
                ImageViewer.ReleaseMouseCapture();
            }


            Mouse.OverrideCursor = null;
        }


        // =====================================================
        // 双击恢复自适应
        // =====================================================

        private void ImageViewer_MouseDoubleClick(
            object sender,
            MouseButtonEventArgs e)
        {
            StopDragging();

            FitImageToViewer();

            e.Handled = true;
        }


        // =====================================================
        // 窗口尺寸变化
        // =====================================================

        protected override void OnRenderSizeChanged(
            SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);


            if (!_isDragging)
            {
                Dispatcher.BeginInvoke(
                    new Action(
                        FitImageToViewer));
            }
        }


        // =====================================================
        // 窗口关闭
        // =====================================================

        protected override void OnClosed(
            EventArgs e)
        {
            StopDragging();

            base.OnClosed(e);
        }
    }
}