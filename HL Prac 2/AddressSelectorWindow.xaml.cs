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

namespace HL_Prac_2
{
    /// <summary>
    /// Interaction logic for AddressSelectorWindow.xaml
    /// </summary>
    public partial class AddressSelectorWindow : Window
    {
        public event EventHandler<AddressEvent> RaiseAddressEvent;
        public Address selectedAddress;
        public AddressSelectorWindow()
        {
            InitializeComponent();
            Search();
        }
        //Overloaded constructor to allow outside setting of Address
        public AddressSelectorWindow(Address initialAddress)
        {
            InitializeComponent();
            UpdateAddress(initialAddress);
            Search();
        }

        //Datagrid Search method
        private void Search()
        {
            //Get data from input fields
            string street = addressStreet_txt.Text.Trim();
            string city = addressCity_txt.Text.Trim();
            //Handle null values from combobox
            string state;
            if ((addressState_cmbo.SelectedValue == null) || string.IsNullOrEmpty(addressState_cmbo.SelectedValue.ToString()))
            {
                state = null;
            }
            else
            {
                state = addressState_cmbo.Text.Trim();
            }
            string zip = addressZip_txt.Text.Trim();

            using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
            {
                var addressQuery = (from addresses in HOTLOADDBEntity.Addresses
                                    where
                                    (street == null || addresses.street.Contains(street)) &&
                                    (city == null || addresses.city.Contains(city)) &&
                                    (state == null || addresses.state.Contains(state)) &&
                                    (zip == null || addresses.zip.Contains(zip))
                                    select new
                                    {
                                        id = addresses.id,
                                        street = addresses.street,
                                        city = addresses.city,
                                        state = addresses.state,
                                        zip = addresses.zip,
                                    }).ToList();
                List<Address> matchedAddresses =
                                    addressQuery.Select(x => new Address
                                    {
                                        id = x.id,
                                        street = x.street,
                                        city = x.city,
                                        state = x.state,
                                        zip = x.zip,
                                    }).ToList();

                AddressSearchDataGrid.ItemsSource = matchedAddresses;
            }
        }

        //Method to return selected Address as an object
        private List<Address> SearchAddress(Address searchedAdress)
        {
            List<Address> returnedList;
            using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
            {
                //Search database to see if record exists
                var addressQuery = (from addresses in HOTLOADDBEntity.Addresses
                                    where
                                    (searchedAdress.street == addresses.street) &&
                                    (searchedAdress.city == addresses.city) &&
                                    (searchedAdress.state == addresses.state) &&
                                    (searchedAdress.zip == addresses.zip)
                                    select new
                                    {
                                        id = addresses.id,
                                        street = addresses.street,
                                        city = addresses.city,
                                        state = addresses.state,
                                        zip = addresses.zip,
                                    }
                                    ).ToList();

                returnedList =
                                    addressQuery.Select(x => new Address
                                    {
                                        id = x.id,
                                        street = x.street,
                                        city = x.city,
                                        state = x.state,
                                        zip = x.zip,
                                    }).ToList();
            }
            return returnedList;
        }

        //Update address fields and current selected address
        private void UpdateAddress(Address chosenAddress)
        {
            if (chosenAddress != null)
            {
                selectedAddress = chosenAddress;
                addressStreet_txt.Text = chosenAddress.street;
                addressCity_txt.Text = chosenAddress.city;
                addressState_cmbo.Text = chosenAddress.state;
                addressZip_txt.Text = chosenAddress.zip;
            }
        }

        //Button trigger methods
        private void AddressSearchDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AddressSearchDataGrid.SelectedIndex != -1)
            {
                Address selectedAddress = (Address)AddressSearchDataGrid.SelectedItem;

                //Set Input Fields to selection
                UpdateAddress(selectedAddress);
            }
        }

        private void addressStreet_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void addressCity_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void addressState_cmbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Search();
        }

        private void addressZip_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            Address newAddress = new Address();
            newAddress.street = addressStreet_txt.Text.Trim();
            newAddress.city = addressCity_txt.Text.Trim();
            newAddress.state = addressState_cmbo.Text.Trim();
            newAddress.zip = addressZip_txt.Text.Trim();

            List<Address> addressMatch = SearchAddress(newAddress);  //TODO REFACTOR THIS USING SEARCHADDRESS METHODS, DO SAME WITH OTHER SUBWINDOWS

            //Make sure entry does not already exist
            if (addressMatch.Count > 0)
            {
                newAddress = addressMatch.ElementAt(0);
            }
            else//If it does not add it
            {
                using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
                {
                    HOTLOADDBEntity.Addresses.Add(newAddress);
                    HOTLOADDBEntity.SaveChanges();
                }
                List<Address> addedAddress = SearchAddress(newAddress);
                newAddress = addedAddress.ElementAt(0);
            }

            UpdateAddress(newAddress);
            //Pass data to parent
            RaiseAddressEvent(this, new AddressEvent(selectedAddress));
            this.Close();
        }

        private void clear_btn_Click(object sender, RoutedEventArgs e)
        {
            addressStreet_txt.Text = "";
            addressCity_txt.Text = "";
            addressState_cmbo.Text = "";
            addressZip_txt.Text = "";
            Search();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    //Event to pass data to parent
    public class AddressEvent : EventArgs
    {
        private Address returnedAddress;
        public AddressEvent(Address address)
        {
            returnedAddress = address;
        }

        public Address ReturnAddress
        {
            get { return returnedAddress; }
        }
    }
}
