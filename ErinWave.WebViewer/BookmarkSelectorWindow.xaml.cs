using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using Wpf.Ui.Controls;

namespace ErinWave.WebViewer
{
    public class BookmarkItem : INotifyPropertyChanged
    {
        private bool? _isSelected = false;

        public string Name { get; set; } = string.Empty;
        public string? Url { get; set; }
        public ObservableCollection<BookmarkItem> Children { get; set; } = new ObservableCollection<BookmarkItem>();

        public bool? IsSelected
        {
            get => _isSelected;
            set => SetIsSelected(value, true, true);
        }

        void SetIsSelected(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isSelected) return;
            _isSelected = value;
            if (updateChildren && _isSelected.HasValue)
            {
                foreach (var child in Children)
                {
                    child.SetIsSelected(_isSelected, true, false);
                }
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public partial class BookmarkSelectorWindow : FluentWindow
    {
        public List<BookmarkItem> SelectedBookmarks { get; private set; } = new List<BookmarkItem>();

        public BookmarkSelectorWindow()
        {
            InitializeComponent();
            LoadAllBookmarks();
        }

        private void LoadAllBookmarks()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string bookmarksPath = Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "Bookmarks");

            if (!File.Exists(bookmarksPath))
            {
                System.Windows.MessageBox.Show("Chrome bookmarks file not found.");
                return;
            }

            var bookmarkItems = new ObservableCollection<BookmarkItem>();
            string bookmarksJson = File.ReadAllText(bookmarksPath);

            using (JsonDocument doc = JsonDocument.Parse(bookmarksJson))
            {
                if (doc.RootElement.TryGetProperty("roots", out JsonElement roots) &&
                    roots.TryGetProperty("bookmark_bar", out JsonElement bookmarkBar))
                {
                    var node = ParseBookmarkNode(bookmarkBar);
                    if(node != null) bookmarkItems.Add(node);
                }
            }
            BookmarkTreeView.ItemsSource = bookmarkItems;
        }

        private BookmarkItem? ParseBookmarkNode(JsonElement node)
        {
            if (!node.TryGetProperty("type", out var typeElement)) return null;

            string type = typeElement.GetString() ?? "";
            string name = node.TryGetProperty("name", out var nameElement) ? nameElement.GetString() ?? "" : "";

            var item = new BookmarkItem { Name = name };

            if (type.Equals("url", StringComparison.OrdinalIgnoreCase))
            {
                item.Url = node.TryGetProperty("url", out var urlElement) ? urlElement.GetString() : null;
                return item;
            }
            else if (type.Equals("folder", StringComparison.OrdinalIgnoreCase))
            {
                if (node.TryGetProperty("children", out var childrenElement))
                {
                    foreach (var child in childrenElement.EnumerateArray())
                    {
                        var childItem = ParseBookmarkNode(child);
                        if(childItem != null) item.Children.Add(childItem);
                    }
                }
                return item;
            }
            return null;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedBookmarks = GetSelected(BookmarkTreeView.ItemsSource as IEnumerable<BookmarkItem>);
            DialogResult = true;
        }

        private List<BookmarkItem> GetSelected(IEnumerable<BookmarkItem>? items)
        {
            var selected = new List<BookmarkItem>();
            if (items == null) return selected;

            foreach (var item in items)
            {
                if (item.IsSelected == true && !string.IsNullOrEmpty(item.Url))
                {
                    selected.Add(item);
                }
                selected.AddRange(GetSelected(item.Children));
            }
            return selected;
        }
    }
}
