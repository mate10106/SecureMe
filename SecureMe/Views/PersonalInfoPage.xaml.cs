using SecureMe.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecureMe.Views
{
    /// <summary>
    /// Interaction logic for PersonalInfoPage.xaml
    /// </summary>
    public partial class PersonalInfoPage : Page
    {
        public PersonalInfoPage()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            PersonalInformation personalInfo = new PersonalInformation
            {
                Name = string.IsNullOrWhiteSpace(txtFullName.Text) ? null : txtFullName.Text,
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text,
                Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtAddress.Text,  
                Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text,
                Company = string.IsNullOrWhiteSpace(txtCompany.Text) ? null : txtCompany.Text,
                Website = string.IsNullOrWhiteSpace(txtWebsite.Text) ? null : txtWebsite.Text
            };

            PersonalInformationManager.SavePersonalInformation(personalInfo);

            MessageBox.Show("Personal information saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
