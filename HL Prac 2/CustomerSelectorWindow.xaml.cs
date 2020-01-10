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
            contactName_lbl.Content = infoContact.contact_name;
            contactPhone_lbl.Content = infoContact.contact_phone;
            contactEmail_lbl.Content = infoContact.contact_email;
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

        private void infoContactSelect_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void contactSelect_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addressSelect_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void confirm_btn_Click(object sender, RoutedEventArgs e)
        {

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
