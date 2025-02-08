using Newtonsoft.Json;
using SecureMe.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SecureMe.Views
{
    /// <summary>
    /// Interaction logic for SecureNotesPage.xaml
    /// </summary>
    public partial class SecureNotesPage : Page
    {
        private List<SecureNotes> _allNotes = new List<SecureNotes>();
        public SecureNotesPage()
        {
            InitializeComponent();
            LoadSecureNotes();
            NotesList.SelectionChanged += NotesList_SelectionChanged;
            SearchBox.TextChanged += SearchBox_TextChanged;
        }

        private void LoadSecureNotes()
        {
            try
            {
                _allNotes = SecureNotesManager.LoadSecureNotes();
                NotesList.ItemsSource = _allNotes;

                NotesList.Visibility = _allNotes.Any() ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading notes: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NotesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotesList.SelectedItem is SecureNotes selectedNote)
            {
                NoteContent.Text = selectedNote.SecuredNotes;
                NoteContent.IsReadOnly = false;
                NoteContent.Background = Brushes.Transparent;
            }
        }

        private void OpenAddSecureNotes_Click(object sender, RoutedEventArgs e)
        {
            var addNoteDialog = new AddSecureNotes();
            if (addNoteDialog.ShowDialog() == true)
            {
                LoadSecureNotes();
                NotesList.SelectedItem = addNoteDialog.NewNote;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchQuery = SearchBox.Text.Trim().ToLower();

            NotesList.ItemsSource = string.IsNullOrWhiteSpace(searchQuery)
                ? _allNotes
                : _allNotes.Where(n =>
                    (n.Title?.ToLower().Contains(searchQuery) ?? false) ||
                    (n.SecuredNotes?.ToLower().Contains(searchQuery) ?? false)
                  ).ToList();
        }
    }
}
