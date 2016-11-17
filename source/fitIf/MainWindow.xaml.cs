using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using fitSharp.Fit.Model;
using fitSharp.Machine.Model;

namespace fitIf {
    public partial class MainWindow: MainView {

        public MainWindow() {
            InitializeComponent();
            controller = new MainController(this);
            controller.Initialize();
        }

        public void ShowResult(string path) {
            Browser.Dispatcher.BeginInvoke(new Action(() => {
                if (string.IsNullOrEmpty(path)) {
                    Browser.NavigateToString("<html />");
                }
                else {
                    Browser.Navigate(new Uri("file:///" + path));
                }
            }));
        }

        public void ShowTests(Tree<TestFile> tests) {
            TestTree.Items.Clear();
            AddTests(tests, TestTree);
        }

        static void AddTests(Tree<TestFile> tests, ItemsControl item) {
            var header = new WrapPanel();
            if (!string.IsNullOrEmpty(tests.Value.TestStatus)) {
                header.Children.Add(new Ellipse {
                    Height = 10,
                    Width = 10,
                    Fill = MakeBrush(tests.Value.TestStatus),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 2, 0)
                });
            }
            header.Children.Add(new TextBlock {Text = tests.Value.DisplayName});
            var newItem = new TreeViewItem {Header = header, IsExpanded = true, Tag = tests.Value};
            item.Items.Add(newItem);
            foreach (var test in tests.Branches) {
                AddTests(test, newItem);
            }
        }

        static SolidColorBrush MakeBrush(string status) {
            return status == TestStatus.Right
                ? new SolidColorBrush(Color.FromRgb(170, 255, 170))
                : status == TestStatus.Wrong
                    ? new SolidColorBrush(Color.FromRgb(255, 170, 170))
                    : status == TestStatus.Exception
                        ? new SolidColorBrush(Color.FromRgb(255, 255, 170))
                        : new SolidColorBrush(Color.FromRgb(204, 204, 204));
        }

        void TestTree_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            var item = TestTree.SelectedItem as TreeViewItem;
            if (item != null) {
                controller.SelectTest((TestFile)item.Tag);
            }
        }

        readonly MainController controller;
    }
}
