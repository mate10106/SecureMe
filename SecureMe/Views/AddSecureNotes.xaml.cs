﻿using SecureMe.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SecureMe.Views
{
    /// <summary>
    /// Interaction logic for AddSecureNotes.xaml
    /// </summary>
    public partial class AddSecureNotes : Window
    {
        public SecureNotes NewNote { get; private set; }
        public AddSecureNotes()
        {
            InitializeComponent();
        }

        private void SaveSecureNote_Click(object sender, RoutedEventArgs e)
        {

            NewNote = new SecureNotes
            {
                Title = txtTitle.Text,
                SecuredNotes = txtSecureNote.Text,
                CreatedNotes = DateTime.Now
            };

            SecureNotesManager.AddSecureNote(NewNote);
            DialogResult = true;
        }
    }
}
