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
    public partial class ContactSelectorWindow : Window
    {
        public event EventHandler<ContactEvent> RaiseCustomEvent;
        Contact SelectedContact;
        public ContactSelectorWindow()
        {
            InitializeComponent();
            Search();
        }
        //Overloaded constructor to allow pre-selected contact
        public ContactSelectorWindow(Contact currentContact)
        {
            InitializeComponent();
            UpdateContact(currentContact);
            Search();
        }

        //Contact Datagrid search method
        public void Search()
        {
            //Get Input Values
            string name = contactName_txt.Text.Trim();
            string phone = contactPhone_txt.Text.Trim();
            string email = contactEmail_txt.Text.Trim();

            using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
            {
                var contactQuery = (from contacts in HOTLOADDBEntity.Contacts
                                    where
                                    (name == null || contacts.contact_name.Contains(name)) &&
                                    (phone == null || contacts.contact_phone.Contains(phone)) &&
                                    (email == null || contacts.contact_email.Contains(email))
                                    select new
                                    {
                                        id = contacts.id,
                                        contact_name = contacts.contact_name,
                                        contact_phone = contacts.contact_phone,
                                        contact_email = contacts.contact_email,
                                    }).ToList();

                List<Contact> matchedContacts =
                                    contactQuery.Select(x => new Contact
                                    {
                                        id = x.id,
                                        contact_name = x.contact_name,
                                        contact_phone = x.contact_phone,
                                        contact_email = x.contact_email,
                                    }).ToList();

                ContactSearchDataGrid.ItemsSource = matchedContacts;
            }
        }

        //Method to return contact object from DB
        private List<Contact> SearchContacts(Contact searchedContact)
        {
            List<Contact> returnedList;
            using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
            {
                //Search database to see if record exists
                var contactQuery = (from contacts in HOTLOADDBEntity.Contacts
                                    where
                                    (searchedContact.contact_name == contacts.contact_name) &&
                                    (searchedContact.contact_phone == contacts.contact_phone) &&
                                    (searchedContact.contact_email == contacts.contact_email)
                                    select new
                                    {
                                        id = contacts.id,
                                        contact_name = contacts.contact_name,
                                        contact_phone = contacts.contact_phone,
                                        contact_email = contacts.contact_email,
                                    }
                                    ).ToList();

                returnedList =
                                    contactQuery.Select(x => new Contact
                                    {
                                        id = x.id,
                                        contact_name = x.contact_name,
                                        contact_phone = x.contact_phone,
                                        contact_email = x.contact_email,
                                    }).ToList();
            }
            return returnedList;
        }

        //MEthod to update current contact & associated fields
        private void UpdateContact(Contact chosenContact)
        {
            try
            {
                contactName_txt.Text = chosenContact.contact_name;
                contactPhone_txt.Text = chosenContact.contact_phone;
                contactEmail_txt.Text = chosenContact.contact_email;
                SelectedContact = chosenContact;
            }
            catch (Exception ex)
            {
                //Ignore
            }
        }

        //Datagrid double click select method
        public void SelectContact()
        {
            if (ContactSearchDataGrid.SelectedIndex != -1)
            {
                Contact selectedContact = (Contact)ContactSearchDataGrid.SelectedItem;

                //Set Input Fields to selection
                UpdateContact(selectedContact);
            }
        }

        //Button trigger methods
        private void ContactSearchDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ContactSearchDataGrid.SelectedIndex != -1)
            {
                Contact selectedContact = (Contact)ContactSearchDataGrid.SelectedItem;

                //Set Input Fields to selection
                UpdateContact(selectedContact);
            }
        }

        private void contactName_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void contactPhone_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void contactEmail_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        private void confirm_btn_Click(object sender, RoutedEventArgs e)
        {
            Contact newContact = new Contact();
            newContact.contact_name = contactName_txt.Text.Trim();
            newContact.contact_phone = contactPhone_txt.Text.Trim();
            newContact.contact_email = contactEmail_txt.Text.Trim();

            List<Contact> contactMatch = SearchContacts(newContact);

            //Make sure entry does not already exist

            if (contactMatch.Count > 0)
            {
                newContact = contactMatch.ElementAt(0);
            }
            else//If it does not add it
            {
                using (HOTLOADDBEntities HOTLOADDBEntity = new HOTLOADDBEntities())
                {
                    HOTLOADDBEntity.Contacts.Add(newContact);
                    HOTLOADDBEntity.SaveChanges();
                }
                List<Contact> addedContact = SearchContacts(newContact);
                newContact = addedContact.ElementAt(0);
            }

            UpdateContact(newContact);
            //Pass data to parent
            RaiseCustomEvent(this, new ContactEvent(SelectedContact));
            this.Close();
        }

        private void clear_btn_Click(object sender, RoutedEventArgs e)
        {
            contactName_txt.Text = "";
            contactPhone_txt.Text = "";
            contactEmail_txt.Text = "";
            Search();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    //Event to pass data to parent
    public class ContactEvent : EventArgs
    {
        private Contact returnedContact;
        public ContactEvent(Contact contact)
        {
            returnedContact = contact;
        }

        public Contact ReturnContact
        {
            get { return returnedContact; }
        }
    }
}
