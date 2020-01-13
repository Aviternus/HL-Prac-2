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
    public partial class CustomerSelectorWindow : Window
    {
        public event EventHandler<CustomerEvent> RaiseCustomerEvent;
        Customer SelectedCustomer;
        Contact infoContact;
        Contact billingContact;
        Address billingAddress;
        public CustomerSelectorWindow()
        {
            InitializeComponent();
            Search();
        }
        //Overloaded constructor to allow window to start with a customer selection
        public CustomerSelectorWindow(Customer startingCustomer)
        {
            InitializeComponent();
            UpdateCustomer(startingCustomer);
            Search();
        }

        //Customer Search method
        public void Search()
        {
            //Get the values of the inputs
            string nameInput = customerName_txt.Text.Trim();

            //Search the DB for matching customers
            using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
            {
                var customerQuery = (
                    from customers in HOTLOADDBEntity.Customers
                    where
                    (nameInput == null || customers.customer_name.Contains(nameInput))
                    select new
                    {
                        id = customers.id,
                        customer_name = customers.customer_name,
                        info_contact_id = customers.info_contact_id,
                        billing_contact_id = customers.billing_contact_id,
                        billing_address_id = customers.billing_address_id,
                        terms = customers.terms,
                    }).ToList();

                List<Customer> matchedCustomers =
                    customerQuery.Select(x => new Customer
                    {
                        id = x.id,
                        customer_name = x.customer_name,
                        info_contact_id = x.info_contact_id,
                        billing_contact_id = x.billing_contact_id,
                        billing_address_id = x.billing_address_id,
                        terms = x.terms,
                    }).ToList();

                CustomerSearchGrid.ItemsSource = matchedCustomers;
            }
        }

        //Method to get customer object from current selection
        private List<Customer> SearchCustomer(Customer searchedCustomer)
        {
            List<Customer> returnedList;

            using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
            {
                //Search database to see if record exists
                var customerQuery = (from customers in HOTLOADDBEntity.Customers
                                    where
                                    (searchedCustomer.customer_name == customers.customer_name)
                                    select new
                                    {
                                        id = customers.id,
                                        customer_name = customers.customer_name,
                                        info_contact_id = customers.info_contact_id,
                                        billing_contact_id = customers.billing_contact_id,
                                        billing_address_id = customers.billing_address_id,
                                        terms = customers.terms,
                                    }
                                    ).ToList();

                returnedList =
                                    customerQuery.Select(x => new Customer
                                    {
                                        id = x.id,
                                        customer_name = x.customer_name,
                                        info_contact_id = x.info_contact_id,
                                        billing_contact_id = x.billing_contact_id,
                                        billing_address_id = x.billing_address_id,
                                        terms = x.terms,
                                    }).ToList();
            }
            return returnedList;
        }

        //Billing contact update method
        private void UpdateInfoContactFields(Contact newInfoContact)
        {
            infoContact = newInfoContact;
            infoContactName_lbl.Content = infoContact.contact_name;
            infoContactPhone_lbl.Content = infoContact.contact_phone;
            infoContactEmail_lbl.Content = infoContact.contact_email;
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

        //Method to set current Customer and associated fields
        private void UpdateCustomer(Customer selectedCustomer)
        {
            customerName_txt.Text = selectedCustomer.customer_name;

            using (HOTLOADDBEntities HOTLOADEntity = new HOTLOADDBEntities())
            {
                infoContact = HOTLOADEntity.Contacts.Find(selectedCustomer.info_contact_id);
                UpdateInfoContactFields(infoContact);

                billingContact = HOTLOADEntity.Contacts.Find(selectedCustomer.billing_contact_id);
                UpdateBillingContactFields(billingContact);

                billingAddress = HOTLOADEntity.Addresses.Find(selectedCustomer.billing_address_id);
                UpdateBillingAddressFields(billingAddress);
            }

            SelectedCustomer = selectedCustomer;
        }

        //Method to select customer from datagrid on doubleclick
        private void SelectCustomer()
        {
            if (CustomerSearchGrid.SelectedIndex != -1)
            {
                Customer selectedCustomer = (Customer)CustomerSearchGrid.SelectedItem;

                //Set Input Fields to selection
                UpdateCustomer(selectedCustomer);
            }
        }

        private void CustomerSearchGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectCustomer();
        }

        private void customerName_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        //Event to get Contact from contact selector for info contact
        void infoContactSelector_RaiseCustomEvent(object sender, ContactEvent e)
        {
            UpdateInfoContactFields(e.ReturnContact);
            if (SelectedCustomer != null) //Handles exception if contact is selected but no customer is selected
            {
                SelectedCustomer.info_contact_id = infoContact.id;
                using (HOTLOADDBEntities HOTLOADEntity = new HOTLOADDBEntities())
                {
                    HOTLOADEntity.Entry(SelectedCustomer).State = EntityState.Modified;
                    HOTLOADEntity.SaveChanges();
                }
            }
        }

        //Event to get Contact from contact selector for billing contact
        void billingContactSelector_RaiseCustomEvent(object sender, ContactEvent e)
        {
            UpdateBillingContactFields(e.ReturnContact);
            if (SelectedCustomer != null) //Handles exception if contact is selected but no customer is selected
            {
                SelectedCustomer.info_contact_id = billingContact.id;
                using (HOTLOADDBEntities HOTLOADEntity = new HOTLOADDBEntities())
                {
                    HOTLOADEntity.Entry(SelectedCustomer).State = EntityState.Modified;
                    HOTLOADEntity.SaveChanges();
                }
            }
        }

        //Event to get Address from Address selector
        void addressSearch_RaiseCustomEvent(object sender, AddressEvent e)
        {
            UpdateBillingAddressFields(e.ReturnAddress);
            if (SelectedCustomer != null)//Handles exception if address is selected but no carrier is selected
            {
                SelectedCustomer.billing_address_id = billingAddress.id;
                using (HOTLOADDBEntities HOTLOADEntity = new HOTLOADDBEntities())
                {
                    HOTLOADEntity.Entry(SelectedCustomer).State = EntityState.Modified;
                    HOTLOADEntity.SaveChanges();
                }
            }
        }

        private void infoContactSelect_btn_Click(object sender, RoutedEventArgs e)
        {
            ContactSelectorWindow infoContactSelector = new ContactSelectorWindow(infoContact);
            infoContactSelector.RaiseContactEvent += new EventHandler<ContactEvent>(infoContactSelector_RaiseCustomEvent);
            infoContactSelector.Show();
        }

        private void contactSelect_btn_Click(object sender, RoutedEventArgs e)
        {
            ContactSelectorWindow contactSelector = new ContactSelectorWindow(billingContact);
            contactSelector.RaiseContactEvent += new EventHandler<ContactEvent>(billingContactSelector_RaiseCustomEvent);
            contactSelector.Show();
        }

        private void addressSelect_btn_Click(object sender, RoutedEventArgs e)
        {
            AddressSelectorWindow addressSearch = new AddressSelectorWindow(billingAddress);
            addressSearch.RaiseAddressEvent += new EventHandler<AddressEvent>(addressSearch_RaiseCustomEvent);
            addressSearch.Show();
        }

        private void confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            Customer newCustomer = new Customer();
            newCustomer.customer_name = customerName_txt.Text.Trim();
            if (infoContact != null)
            {
                newCustomer.info_contact_id = infoContact.id;
            }
            if (billingContact != null)
            {
                newCustomer.billing_contact_id = billingContact.id;
            }
            if (billingAddress != null)
            {
                newCustomer.billing_address_id = billingAddress.id;
            }

            List<Customer> customerMatch = SearchCustomer(newCustomer);

            //Make sure entry does not already exist
            if (customerMatch.Count > 0)
            {
                newCustomer = customerMatch.ElementAt(0);
            }
            else//If it does not add it
            {
                using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
                {
                    HOTLOADDBEntity.Customers.Add(newCustomer);
                    HOTLOADDBEntity.SaveChanges();
                }
                List<Customer> addedCustomer = SearchCustomer(newCustomer);
                newCustomer = addedCustomer.ElementAt(0);
            }

            UpdateCustomer(newCustomer);
            //Pass data to parent
            RaiseCustomerEvent(this, new CustomerEvent(newCustomer));
            this.Close();
        }

        private void clear_btn_Click(object sender, RoutedEventArgs e)
        {
            customerName_txt.Text = "";

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
    public class CustomerEvent : EventArgs
    {
        private Customer returnedCustomer;
        public CustomerEvent(Customer customer)
        {
            returnedCustomer = customer;
        }

        public Customer ReturnCustomer
        {
            get { return returnedCustomer; }
        }
    }
}
