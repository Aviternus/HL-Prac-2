using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public partial class CarrierSelectorWindow : Window
    {
        public event EventHandler<CarrierEvent> RaiseCarrierEvent;
        Carrier SelectedCarrier;
        Contact billingContact;
        Address billingAddress;
        public CarrierSelectorWindow()
        {
            InitializeComponent();
            Search();
        }
        //Overloaded constructor to allow window to start with a carrier selection
        public CarrierSelectorWindow(Carrier startingCarrier)
        {
            InitializeComponent();
            UpdateCarrier(startingCarrier);
            Search();
        }

        //Carrier Search method
        public void Search()
        {
            //Get the values of the inputs
            string nameInput = carrierName_txt.Text.Trim();
            string mc_numInput = carrierMc_txt.Text.Trim();
            string dot_numInput = carrierDot_txt.Text.Trim();

            //Search the DB for matching carriers
            using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
            {
                var carrierQuery = (
                    from carriers in HOTLOADDBEntity.Carriers
                    where
                    (nameInput == null || carriers.carrier_name.Contains(nameInput)) &&
                    (mc_numInput == null || carriers.mc_num.Contains(mc_numInput)) &&
                    (dot_numInput == null || carriers.dot_num.Contains(dot_numInput))
                    select new
                    {
                        id = carriers.id,
                        carrier_name = carriers.carrier_name,
                        mc_num = carriers.mc_num,
                        dot_num = carriers.dot_num,
                        billing_contact_id = carriers.billing_contact_id,
                        billing_address_id = carriers.billing_address_id,
                    }).ToList();

                List<Carrier> matchedCarriers =
                    carrierQuery.Select(x => new Carrier
                    {
                        id = x.id,
                        carrier_name = x.carrier_name,
                        mc_num = x.mc_num,
                        dot_num = x.dot_num,
                        billing_contact_id = x.billing_contact_id,
                        billing_address_id = x.billing_address_id,
                    }).ToList();

                CarrierSearchGrid.ItemsSource = matchedCarriers;
            }
        }

        //Method to get carrier object from current selection
        private List<Carrier> SearchCarrier(Carrier searchedCarrier)
        {
            List<Carrier> returnedList;
            using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
            {
                //Search database to see if record exists
                var carrierQuery = (from carriers in HOTLOADDBEntity.Carriers
                                    where
                                    (searchedCarrier.carrier_name == carriers.carrier_name) &&
                                    (searchedCarrier.mc_num == carriers.mc_num) &&
                                    (searchedCarrier.dot_num == carriers.dot_num)
                                    select new
                                    {
                                        id = carriers.id,
                                        carrier_name = carriers.carrier_name,
                                        mc_num = carriers.mc_num,
                                        dot_num = carriers.dot_num,
                                        billing_contact_id = carriers.billing_contact_id,
                                        billing_address_id = carriers.billing_address_id,
                                    }
                                    ).ToList();

                returnedList =
                                    carrierQuery.Select(x => new Carrier
                                    {
                                        id = x.id,
                                        carrier_name = x.carrier_name,
                                        mc_num = x.mc_num,
                                        dot_num = x.dot_num,
                                        billing_contact_id = x.billing_contact_id,
                                        billing_address_id = x.billing_address_id,
                                    }).ToList();
            }
            return returnedList;
        }

        //Billing contact update method
        private void UpdateBillingContactFields(Contact newBillingContact)
        {
            billingContact = newBillingContact;
            contactName_lbl.Content = billingContact.contact_name;
            contactPhone_lbl.Content = billingContact.contact_phone;
            contactEmail_lbl.Content = billingContact.contact_email;
        }

        //Billing Address update method
        private void UpdateBillingAddressFields(Address newBillingAddress)
        {
            billingAddress = newBillingAddress;
            addressStreet_lbl.Content = billingAddress.street;
            addressCity_lbl.Content = billingAddress.city;
            addressState_lbl.Content = billingAddress.state;
            addressZip_lbl.Content = billingAddress.zip;
        }

        //Method to set current Carrier and associated fields
        private void UpdateCarrier(Carrier selectedCarrier)
        {
            carrierName_txt.Text = selectedCarrier.carrier_name;
            carrierMc_txt.Text = selectedCarrier.mc_num;
            carrierDot_txt.Text = selectedCarrier.dot_num;

            using (HOTLOADDBEntities HOTLOADEntity = new HOTLOADDBEntities())
            {
                billingContact = HOTLOADEntity.Contacts.Find(selectedCarrier.billing_contact_id);
                UpdateBillingContactFields(billingContact);

                billingAddress = HOTLOADEntity.Addresses.Find(selectedCarrier.billing_address_id);
                UpdateBillingAddressFields(billingAddress);
            }

            SelectedCarrier = selectedCarrier;
        }

        //Method to select carrier from datagrid on doubleclick
        private void SelectCarrier()
        {
            if (CarrierSearchGrid.SelectedIndex != -1)
            {
                Carrier selectedCarrier = (Carrier)CarrierSearchGrid.SelectedItem;

                //Set Input Fields to selection
                UpdateCarrier(selectedCarrier);
            }
        }
        //Datagrid select trigger
        private void CarrierSearchGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectCarrier();
        }

        //TextBox text changed triggers
        private void carrierName_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void carrierMc_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void carrierDot_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        //Event to get Contact from contact selector
        void contactSelector_RaiseCustomEvent(object sender, ContactEvent e)
        {
            UpdateBillingContactFields(e.ReturnContact);
            if (SelectedCarrier != null) //Handles exception if contact is selected but no carrier is selected
            {
                SelectedCarrier.billing_contact_id = billingContact.id;
                using (HOTLOADDBEntities HOTLOADEntity = new HOTLOADDBEntities())
                {
                    HOTLOADEntity.Entry(SelectedCarrier).State = EntityState.Modified;
                    HOTLOADEntity.SaveChanges();
                }
            }
        }

        //Event to get Address from Address selector
        void addressSearch_RaiseCustomEvent(object sender, AddressEvent e)
        {
            UpdateBillingAddressFields(e.ReturnAddress);
            if (SelectedCarrier != null)//Handles exception if address is selected but no carrier is selected
            {
                SelectedCarrier.billing_address_id = billingAddress.id;
                using (HOTLOADDBEntities HOTLOADEntity = new HOTLOADDBEntities())
                {
                    HOTLOADEntity.Entry(SelectedCarrier).State = EntityState.Modified;
                    HOTLOADEntity.SaveChanges();
                }
            }
        }

        //Contact & Address select buttons
        private void contactSelect_btn_Click(object sender, RoutedEventArgs e)
        {
            ContactSelectorWindow contactSelector = new ContactSelectorWindow(billingContact);
            contactSelector.RaiseContactEvent += new EventHandler<ContactEvent>(contactSelector_RaiseCustomEvent);
            contactSelector.Show();
        }

        private void addressSelect_btn_Click(object sender, RoutedEventArgs e)
        {
            
            AddressSelectorWindow addressSearch = new AddressSelectorWindow(billingAddress);            
            addressSearch.RaiseAddressEvent += new EventHandler<AddressEvent>(addressSearch_RaiseCustomEvent);
            addressSearch.Show();
        }

        //Window button triggers
        private void confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            Carrier newCarrier = new Carrier();
            newCarrier.carrier_name = carrierName_txt.Text.Trim();
            newCarrier.mc_num = carrierMc_txt.Text.Trim();
            newCarrier.dot_num = carrierDot_txt.Text.Trim();
            if (billingContact != null)
            {
                newCarrier.billing_contact_id = billingContact.id;
            }
            if (billingAddress != null)
            {
                newCarrier.billing_address_id = billingAddress.id;
            }

            List<Carrier> carrierMatch = SearchCarrier(newCarrier);

            //Make sure entry does not already exist
            if (carrierMatch.Count > 0)
            {
                newCarrier = carrierMatch.ElementAt(0);
            }
            else//If it does not add it
            {
                using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
                {
                    HOTLOADDBEntity.Carriers.Add(newCarrier);
                    HOTLOADDBEntity.SaveChanges();
                }
                List<Carrier> addedCarrier = SearchCarrier(newCarrier);
                newCarrier = addedCarrier.ElementAt(0);
            }

            UpdateCarrier(newCarrier);
            //Pass data to parent
            RaiseCarrierEvent(this, new CarrierEvent(SelectedCarrier));
            this.Close();
        }

        private void clear_btn_Click(object sender, RoutedEventArgs e)
        {
            carrierName_txt.Text = "";
            carrierMc_txt.Text = "";
            carrierDot_txt.Text = "";

            contactName_lbl.Content = "Name:";
            contactPhone_lbl.Content = "Phone:";
            contactEmail_lbl.Content = "Email:";

            addressStreet_lbl.Content = "Street:";
            addressCity_lbl.Content = "City:";
            addressState_lbl.Content = "State:";
            addressZip_lbl.Content = "ZIP:";

            Search();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        
    }

    //Event to pass data to parent
    public class CarrierEvent : EventArgs
    {
        private Carrier returnedCarrier;
        public CarrierEvent(Carrier carrier)
        {
            returnedCarrier = carrier;
        }

        public Carrier ReturnCarrier
        {
            get { return returnedCarrier; }
        }
    }
}
