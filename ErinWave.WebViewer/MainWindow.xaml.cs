using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using wv2 = Microsoft.Web.WebView2.Wpf;

namespace ErinWave.WebViewer
{
    public partial class MainWindow : FluentWindow
    {
        private class BookmarkItem
        {
            public string Name { get; set; } = string.Empty;
            public string? Url { get; set; }
            public List<BookmarkItem> Children { get; set; } = new List<BookmarkItem>();
        }

        private class OpenTab
        {
            public BookmarkItem Bookmark { get; set; }
            public wv2.WebView2 WebView { get; set; }
            public Wpf.Ui.Controls.Button TabButton { get; set; }

            public OpenTab(BookmarkItem bookmark, wv2.WebView2 webView, Wpf.Ui.Controls.Button tabButton)
            {
                Bookmark = bookmark;
                WebView = webView;
                TabButton = tabButton;
            }
        }

        private static readonly HttpClient httpClient = new HttpClient();
        private readonly string faviconCacheDir;
        private readonly List<BookmarkItem> _bookmarkBarItems = new List<BookmarkItem>();
        private readonly List<OpenTab> _openTabs = new List<OpenTab>();
        private OpenTab? _activeOpenTab = null;
        private ContextMenu? _bookmarkContextMenu = null;

        public MainWindow()
        {
            InitializeComponent();
            faviconCacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ErinWave.WebViewer", "FaviconCache");
            Directory.CreateDirectory(faviconCacheDir);
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // WebView2 instances will be initialized dynamically
            LoadBookmarks();
        }

        #region Bookmark Loading and Menu

        private void LoadBookmarks()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string bookmarksPath = Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "Bookmarks");
            if (!File.Exists(bookmarksPath)) return;

            string bookmarksJson = File.ReadAllText(bookmarksPath);
            using (JsonDocument doc = JsonDocument.Parse(bookmarksJson))
            {
                if (doc.RootElement.TryGetProperty("roots", out JsonElement roots) &&
                    roots.TryGetProperty("bookmark_bar", out JsonElement bookmarkBar) &&
                    bookmarkBar.TryGetProperty("children", out JsonElement children))
                {
                    foreach (var child in children.EnumerateArray())
                    {
                        var item = ParseBookmarkNode(child);
                        if (item != null) _bookmarkBarItems.Add(item);
                    }
                }
            }
        }

        private BookmarkItem? ParseBookmarkNode(JsonElement node)
        {
            if (!node.TryGetProperty("type", out var typeElement)) return null;
            string type = typeElement.GetString() ?? "";
            string name = node.TryGetProperty("name", out var nameElement) ? nameElement.GetString() ?? "" : "";
            if (string.IsNullOrEmpty(name)) return null;

            var item = new BookmarkItem { Name = name };
            if (type.Equals("url", StringComparison.OrdinalIgnoreCase))
            {
                item.Url = node.TryGetProperty("url", out var urlElement) ? urlElement.GetString() : null;
                if (string.IsNullOrEmpty(item.Url)) return null;
                return item;
            }
            else if (type.Equals("folder", StringComparison.OrdinalIgnoreCase))
            {
                if (node.TryGetProperty("children", out var childrenElement))
                {
                    foreach (var child in childrenElement.EnumerateArray())
                    {
                        var childItem = ParseBookmarkNode(child);
                        if (childItem != null) item.Children.Add(childItem);
                    }
                }
                return item;
            }
            return null;
        }

        private void BookmarksButton_Click(object sender, RoutedEventArgs e)
        {
            if (_bookmarkContextMenu == null)
            {
                _bookmarkContextMenu = new System.Windows.Controls.ContextMenu();
                PopulateMenu(_bookmarkContextMenu.Items, _bookmarkBarItems);
            }
            var button = (Wpf.Ui.Controls.Button)sender;
            _bookmarkContextMenu.PlacementTarget = button;
            _bookmarkContextMenu.IsOpen = true;
        }

        private void PopulateMenu(ItemCollection menuItems, List<BookmarkItem> bookmarkNodes)
        {
            foreach (var node in bookmarkNodes)
            {
                var menuItem = new System.Windows.Controls.MenuItem { Header = node.Name };
                if (node.Children.Count > 0)
                {
                    PopulateMenu(menuItem.Items, node.Children);
                }
                else
                {
                    menuItem.Tag = node;
                    menuItem.Click += BookmarkMenuItem_Click;
                    _ = LoadFaviconForMenuItemAsync(menuItem, node);
                }
                menuItems.Add(menuItem);
            }
        }

        private void BookmarkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem { Tag: BookmarkItem bookmark })
            {
                OpenOrSwitchToTab(bookmark);
            }
        }

        #endregion

        #region Tab Management

        private async void OpenOrSwitchToTab(BookmarkItem bookmark)
        {
            OpenTab? targetTab = _openTabs.FirstOrDefault(t => t.Bookmark.Url == bookmark.Url);

            if (targetTab == null)
            {
                // Create new WebView2
                var newWebView = new wv2.WebView2();
                newWebView.Visibility = Visibility.Collapsed; // Start hidden
                WebViewContainer.Children.Add(newWebView);

                // Initialize WebView2
                await newWebView.EnsureCoreWebView2Async();
                newWebView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
                newWebView.CoreWebView2.Settings.AreDevToolsEnabled = true;
                newWebView.CoreWebView2.Navigate(bookmark.Url);

                // Create tab button
                var tabButton = new Wpf.Ui.Controls.Button
                {
                    Width = 48,
                    Height = 48,
                    Padding = new Thickness(8),
                    Background = Brushes.Transparent,
                    BorderBrush = Brushes.Transparent,
                    CornerRadius = new CornerRadius(12),
                    ToolTip = bookmark.Name,
                    Tag = bookmark,
                    Content = new SymbolIcon(SymbolRegular.Question24) // Placeholder
                };
                tabButton.Click += TabButton_Click;

                targetTab = new OpenTab(bookmark, newWebView, tabButton);
                _openTabs.Add(targetTab);
                TabsPanel.Children.Add(tabButton);
                _ = LoadFaviconForButtonAsync(tabButton, bookmark);
            }

            // Hide all WebViews and show the target one
            foreach (wv2.WebView2 wv in WebViewContainer.Children.OfType<wv2.WebView2>())
            {
                wv.Visibility = Visibility.Collapsed;
            }
            targetTab.WebView.Visibility = Visibility.Visible;

            // Update button styles
            UpdateButtonStyles(targetTab.TabButton);
        }

        private void RefreshTabsPanel()
        {
            // This method is now mostly for re-ordering or re-creating buttons if needed
            // For now, it's implicitly handled by OpenOrSwitchToTab adding/removing buttons
            // and UpdateButtonStyles handling activation.
            // If _openTabs order changes, this would be used to re-render TabsPanel.Children
        }

        private void TabButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button { Tag: BookmarkItem bookmark })
            {
                OpenOrSwitchToTab(bookmark);
            }
        }

        private void ClosePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_activeOpenTab == null) return;

            // Remove WebView from container
            WebViewContainer.Children.Remove(_activeOpenTab.WebView);
            // Remove button from panel
            TabsPanel.Children.Remove(_activeOpenTab.TabButton);
            // Remove from tracking list
            _openTabs.Remove(_activeOpenTab);

            // Activate next tab if any, or go to blank
            var nextTabToActivate = _openTabs.FirstOrDefault();
            if (nextTabToActivate != null)
            {
                OpenOrSwitchToTab(nextTabToActivate.Bookmark);
            }
            else
            {
                _activeOpenTab = null;
            }
        }

        private void UpdateButtonStyles(Wpf.Ui.Controls.Button activeButton)
        {
            foreach (Wpf.Ui.Controls.Button button in TabsPanel.Children.OfType<Wpf.Ui.Controls.Button>())
            {
                button.Background = Brushes.Transparent;
            }
            activeButton.Background = new SolidColorBrush(Color.FromArgb(50, 120, 120, 120));

            // Update active tab tracking
            _activeOpenTab = _openTabs.FirstOrDefault(t => t.TabButton == activeButton);
        }

        #endregion

        #region Favicon

        private async Task LoadFaviconForButtonAsync(Wpf.Ui.Controls.Button button, BookmarkItem bookmark)
        {
            ImageSource? imageSource = await GetFaviconAsync(bookmark);
            if (imageSource != null)
            {
                Dispatcher.Invoke(() =>
                {
                    var image = new System.Windows.Controls.Image { Source = imageSource, Width = 32, Height = 32 };
                    var grid = new System.Windows.Controls.Grid { Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240)) };
                    grid.Children.Add(image);
                    button.Content = grid;
                });
            }
        }

        private async Task LoadFaviconForMenuItemAsync(System.Windows.Controls.MenuItem menuItem, BookmarkItem bookmark)
        {
            ImageSource? imageSource = await GetFaviconAsync(bookmark);
            if (imageSource != null)
            {
                Dispatcher.Invoke(() =>
                {
                    var image = new System.Windows.Controls.Image { Source = imageSource, Width = 16, Height = 16 };
                    var grid = new System.Windows.Controls.Grid { Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240)) };
                    grid.Children.Add(image);
                    menuItem.Icon = grid;
                });
            }
        }

        private async Task<ImageSource?> GetFaviconAsync(BookmarkItem bookmark)
        {
            if (string.IsNullOrEmpty(bookmark.Url)) return null;
            try
            {
                var host = new Uri(bookmark.Url).Host;
                var cachePath = Path.Combine(faviconCacheDir, $"{host}.png");

                if (File.Exists(cachePath))
                {
                    return new BitmapImage(new Uri(cachePath));
                }
                else
                {
                    var faviconUrl = $"https://www.google.com/s2/favicons?sz=32&domain_url={bookmark.Url}";
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(faviconUrl);
                    File.WriteAllBytes(cachePath, imageBytes);

                    var bitmapImage = new BitmapImage();
                    using (var mem = new MemoryStream(imageBytes))
                    {
                        mem.Position = 0;
                        bitmapImage.BeginInit();
                        bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.UriSource = null;
                        bitmapImage.StreamSource = mem;
                        bitmapImage.EndInit();
                    }
                    bitmapImage.Freeze();
                    return bitmapImage;
                }
            }
            catch (Exception) { return null; }
        }

        #endregion
    }
}
