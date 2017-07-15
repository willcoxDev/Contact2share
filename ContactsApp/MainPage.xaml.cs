using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.Storage.Streams;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ContactsApp
{
    public sealed partial class MainPage : Page
    {

        //string XMLPath = Path.Combine(Package.Current.InstalledLocation.Path, "Contacts.xml");
        private ObservableCollection<string> lstd = new ObservableCollection<string>();
        public ObservableCollection<string> myList { get { return lstd; } }
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Grid_Loading(FrameworkElement sender, object args)
        {
            loadContacts();
        }
        public async void loadContacts()
        {

            StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("Contacts.xml");
            XmlReader xmlReader = XmlReader.Create(file.Path);
            while (xmlReader.Read())
            {
                if (xmlReader.Name.Equals("ID") && (xmlReader.NodeType == XmlNodeType.Element))
                {
                    lstd.Add(xmlReader.ReadElementContentAsString());
                }
            }
            DataContext = this;
            xmlReader.Dispose();
        }

        private async void lstBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("Contacts.xml");
            try
            {
                XDocument xml = XDocument.Load(file.Path);
                if (lstBox.SelectedIndex != -1)
                {
                    var nodes = (from n in xml.Descendants("Contact").
                Where(r => r.Element("ID").Value == lstBox.SelectedItem.ToString())
                                 select new
                                 {
                                     ID = (string)n.Element("ID").Value,
                                     FirstName = (string)n.Element("FirstName").Value,
                                     LastName = (string)n.Element("LastName").Value,
                                     Mobile = (string)n.Element("Mobile").Value,
                                     Email = (string)n.Element("Email").Value
                                 });
                    foreach (var n in nodes)
                    {
                        txtID.Text = n.ID;
                        txtFirstName.Text = n.FirstName;
                        txtLastName.Text = n.LastName;
                        txtMobile.Text = n.Mobile;
                        txtEmail.Text = n.Email;
                    };
                }
                else
                {
                    txtID.Text = "";
                    txtFirstName.Text = "";
                    txtLastName.Text = "";
                    txtMobile.Text = "";
                    txtEmail.Text = "";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private async void updateXMLFile(XDocument xdoc)
        {
            try
            {
                //StorageFile file = await installedLocation.CreateFileAsync("Contacts.xml", CreationCollisionOption.ReplaceExisting);
                StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("Contacts.xml"); //This line was the replacement for the one above.
                await FileIO.WriteTextAsync(file, xdoc.ToString());
            }
            catch (Exception ex)
            {
                String s = ex.ToString();
            }
        }

        private async void btnUpdateContact_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("Contacts.xml");
            XDocument xdoc = XDocument.Load(file.Path);
            if (lstBox.SelectedIndex != -1)
            {
                XElement upd = (from elements in xdoc.Descendants("Contact")
                                where elements.Element("ID").Value == lstBox.SelectedItem.ToString()
                                select elements).Single();
                upd.Element("ID").Value = txtID.Text;
                upd.Element("FirstName").Value = txtFirstName.Text;
                upd.Element("LastName").Value = txtLastName.Text;
                upd.Element("Mobile").Value = txtMobile.Text;
                upd.Element("Email").Value = txtEmail.Text;
                updateXMLFile(xdoc);
            }
        }


        private async void btnDeleteContact_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("Contacts.xml");
            XDocument xdoc = XDocument.Load(file.Path);
            if (lstBox.SelectedIndex != -1)
            {
                xdoc.Element("Contacts")
                    .Elements("Contact")
                    .Where(x => (string)x.Element("ID") == lstBox.SelectedItem.ToString()).Remove();
                lstBox.SelectedIndex = -1; 
                updateXMLFile(xdoc);

                myList.Clear();
                
                loadContacts();
            }
        }
    }
}






