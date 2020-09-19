using BTD_Backend.IO;
using BTD_Backend.Natives;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BTD_Backend
{
    public class TreeView_Handling
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jet"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFile(Zip jet, string path) => IsFile(jet.Archive, path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zipFile"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFile(ZipFile zipFile, string path) => zipFile.EntryFileNames.Contains(path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jet"></param>
        /// <param name="source"></param>
        public static void PopulateTreeView(Zip jet, TreeView source, SearchOption searchOption, bool useTreeviewIcons = false)
        {
            var folders = jet.GetEntries(Zip.EntryType.Directories, searchOption);
            var files = jet.GetEntries(Zip.EntryType.Files, searchOption);

            var treeHandling = new TreeView_Handling();

            if (useTreeviewIcons)
                treeHandling.AddToTreeView(source, folders, Properties.Resources.folder_icon_1);
            else
                treeHandling.AddToTreeView(source, folders);
            treeHandling.AddToTreeView(source, files);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jet"></param>
        /// <param name="source"></param>
        /// <param name="treeItemPath"></param>
        public static void PopulateTreeView(Zip jet, TreeViewItem source, SearchOption searchOption, string treeItemPath, bool useTreeviewIcons = false)
        {
            var folders = jet.GetEntries(Zip.EntryType.Directories, searchOption, treeItemPath);
            var files = jet.GetEntries(Zip.EntryType.Files, searchOption, treeItemPath);

            var treeHandling = new TreeView_Handling();

            if (useTreeviewIcons)
                treeHandling.AddItemToTreeView(source, folders, Properties.Resources.folder_icon_1);
            else
                treeHandling.AddItemToTreeView(source, folders);

            treeHandling.AddItemToTreeView(source, files);
        }


        public StackPanel CustomTreeViewItem(string itemText, Bitmap img)
        {
            var newImg = new System.Windows.Controls.Image();
            newImg.Source = Utility.Bitmap2BitmapSource(img);
            newImg.Width = 16;
            newImg.Height = 16;

            // Create TextBlock
            TextBlock lbl = new TextBlock();
            lbl.Text = "  " + itemText;

            // Add to stack
            StackPanel stkPanel = new StackPanel();
            stkPanel.Orientation = Orientation.Horizontal;

            stkPanel.Children.Add(newImg);
            stkPanel.Children.Add(lbl);


            return stkPanel;
        }
        
        public void AddToTreeView(TreeView tree, List<string> entries, Bitmap img = null)
        {
            var sortedEntries = SortEntries(tree, entries);

            foreach (var item in sortedEntries)
            {
                if (tree.Items.Cast<TreeViewItem>().Any(t => t.Tag.ToString() == item))
                    continue;

                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Expanded += TreeItem_Expanded;
                treeItem.Tag = item;

                if (img != null)
                    treeItem.Header = CustomTreeViewItem(item, img);
                else
                    treeItem.Header = item;
                
                tree.Items.Add(treeItem);
            }
        }

        public void AddItemToTreeView(TreeViewItem tree, List<string> entries, Bitmap img = null)
        {
            var sortedEntries = SortEntries(tree, entries);

            foreach (var item in sortedEntries)
            {
                if (tree.Items.Cast<TreeViewItem>().Any(t => t.Tag.ToString() == item))
                    continue;

                TreeViewItem treeItem = new TreeViewItem();
                treeItem.Expanded += TreeItem_Expanded;
                treeItem.Tag = item;

                if (img != null)
                    treeItem.Header = CustomTreeViewItem(item, img);
                else
                    treeItem.Header = item;

                tree.Items.Add(treeItem);
            }
        }

        private void TreeItem_Expanded(object sender, System.Windows.RoutedEventArgs e)
        {
            TreeViewItem source = (TreeViewItem)e.Source;
            TreeView_HandlingEventArgs args = new TreeView_HandlingEventArgs();
            args.Source = source;
            OnTreeItemExpanded(args);
        }

        private List<string> SortEntries(ItemsControl tree, List<string> entries)
        {
            List<string> sortedEntries = new List<string>();
            foreach (var entry in entries)
            {
                string item = entry;
                item = item.TrimEnd('/');
                string[] itemSplit = item.Split('/');

                //Does the source already have this item? If so skip
                if (tree.Items.Cast<TreeViewItem>().Any(t => t.Header.ToString() == item))
                    continue;

                sortedEntries.Add(itemSplit[itemSplit.Length - 1]);
            }
            return sortedEntries;
        }

        public static string GetHeaderPath(TreeViewItem current)
        {
            string path = "";

            while (current.Parent is TreeViewItem || current.Parent is TreeView)
            {
                if (current.Parent is TreeViewItem)
                {
                    path = current.Tag + "/" + path;
                    current = (TreeViewItem)current.Parent;
                }
                else
                {
                    path = current.Tag + "/" + path;
                    break;
                }
            }
            if (String.IsNullOrEmpty(path))
                return "";

            return path.Substring(0, path.Length - 1);
        }


        #region Events
        public static event EventHandler<TreeView_HandlingEventArgs> TreeItemExpanded;

        public class TreeView_HandlingEventArgs : EventArgs
        {
            public TreeViewItem Source { get; set; }
        }

        public void OnTreeItemExpanded(TreeView_HandlingEventArgs e)
        {
            EventHandler<TreeView_HandlingEventArgs> handler = TreeItemExpanded;
            if (handler != null)
                handler(this, e);
        }
        #endregion
    }
}
