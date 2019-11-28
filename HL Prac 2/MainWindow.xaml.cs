using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
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
namespace HL_Prac_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Entity Model
        HOTLOADDBEntities HotloadModel = new HOTLOADDBEntities();
   
        public MainWindow()
        {
            InitializeComponent();
            PopulateGrid();
        }

        public void PopulateGrid()
        {
            LoadBoard.ItemsSource = HotloadModel.Loads.ToList();
        }

        public void Clear()
        {
            bol_txt.Clear();
            pro_txt.Clear();
            quote_txt.Clear();
            ref_txt.Clear();
            weight_txt.Clear();
            pieces_txt.Clear();
            commodity_txt.Clear();
            mileage_txt.Clear();
            carrierRate_txt.Clear();
            customerRate_txt.Clear();

            pickDate_picker.Text = "";
            pickAptTime_txt.Clear();
            PickIn_text.Clear();
            PickOut_text.Clear();

            dropDate_picker.Text = "";
            dropAptTime_txt.Clear();
            DropIn_txt.Clear();
            DropOut_txt.Clear();

            driver_txt.Clear();
            dispatch_txt.Clear();
            customer_txt.Clear();
            broker_txt.Clear();

            PopulateGrid();
        }

        private void clear_btn_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void new_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var load = new Load
                {
                    pro_num = pro_txt.Text,
                    quote_num = quote_txt.Text,
                    ref_num = ref_txt.Text,
                    weight = Convert.ToDouble(weight_txt.Text),
                    pieces = Convert.ToInt32(pieces_txt.Text),
                    commodity = commodity_txt.Text,
                    mileage = Convert.ToDouble(mileage_txt.Text),
                    carrier_rate = Convert.ToDecimal(carrierRate_txt.Text),
                    customer_rate = Convert.ToDecimal(customerRate_txt.Text),
                    pick_appointment = Convert.ToDateTime(pickDate_picker.SelectedDate),
                    drop_appointment = Convert.ToDateTime(dropDate_picker.SelectedDate),
                    driver_id = Convert.ToInt32(driver_txt.Text),
                    dispatch_id = Convert.ToInt32(dispatch_txt.Text),
                    customer_id = Convert.ToInt32(customer_txt.Text),
                    broker_id = Convert.ToInt32(broker_txt.Text)
                };

                HotloadModel.Loads.Add(load);
                HotloadModel.SaveChanges();
                PopulateGrid();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation(
                              "Class: {0}, Property: {1}, Error: {2}",
                              validationErrors.Entry.Entity.GetType().FullName,
                              validationError.PropertyName,
                              validationError.ErrorMessage);
                    }
                }
            }
        }

        private void update_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void copy_btn_Click(object sender, RoutedEventArgs e)
        {

        }
        //Database search method
        /*
        private void Search()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Establish connection
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(
                        "SELECT * FROM Loads" +
                        "WHERE "
                        , conn);
                    DataTable dtbl = new DataTable();
                    adapter.Fill(dtbl);

                    LoadBoard.ItemsSource = dtbl.DefaultView;
                }
            }
            catch (SqlException sqlError)
            {
                //Display SQL error
                MessageBox.Show(sqlError.Message, "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        */
    }
}
