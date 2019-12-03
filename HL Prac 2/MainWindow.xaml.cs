using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Windows;
namespace HL_Prac_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Entity Model
        HOTLOADDBEntities2 HotLoadModel2 = new HOTLOADDBEntities2();

        public MainWindow()
        {
            InitializeComponent();
            PopulateGrid();
            Clear();
        }

        public void Clear()
        {
            bol_txt.Text = "0";
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

            //Populate Load Board
            PopulateGrid();

            //disable copy & delete buttons
            delete_btn.IsEnabled = false;
            copy_btn.IsEnabled = false;
        }
        //Clear textfields button
        private void clear_btn_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        //Fill datagrid from DB
        public void PopulateGrid()
        {
            LoadBoard.ItemsSource = HotLoadModel2.Loads.ToList();
        }

        //Update or Create Button
        private void update_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Load model
                Load loadModel = new Load();
                //Get the load data from textboxes
                loadModel.bol_num = Convert.ToInt32(bol_txt.Text.Trim());
                loadModel.pro_num = pro_txt.Text.Trim();
                loadModel.quote_num = quote_txt.Text.Trim();
                loadModel.ref_num = ref_txt.Text.Trim();
                loadModel.weight = Convert.ToDouble(weight_txt.Text.Trim());
                loadModel.pieces = Convert.ToInt32(pieces_txt.Text.Trim());
                loadModel.commodity = commodity_txt.Text.Trim();
                loadModel.mileage = Convert.ToDouble(mileage_txt.Text.Trim());
                loadModel.carrier_rate = Convert.ToDecimal(carrierRate_txt.Text.Trim());
                loadModel.customer_rate = Convert.ToDecimal(customerRate_txt.Text.Trim());
                loadModel.pick_appointment = Convert.ToDateTime(pickDate_picker.SelectedDate);
                loadModel.drop_appointment = Convert.ToDateTime(dropDate_picker.SelectedDate);
                loadModel.driver_id = Convert.ToInt32(driver_txt.Text.Trim());
                loadModel.dispatch_id = Convert.ToInt32(dispatch_txt.Text.Trim());
                loadModel.customer_id = Convert.ToInt32(customer_txt.Text.Trim());
                loadModel.broker_id = Convert.ToInt32(broker_txt.Text.Trim());

                //Save the load to the database
                using(HOTLOADDBEntities2 HOTLOADEntity = new HOTLOADDBEntities2())
                {
                    if(loadModel.bol_num == 0)//Insert
                    {
                        HOTLOADEntity.Loads.Add(loadModel);
                        Clear();
                        MessageBox.Show("Saved Succesfully");
                    }
                    else//Update
                    {
                        HOTLOADEntity.Entry(loadModel).State = EntityState.Modified;
                        Clear();
                    }
                    //Save the changes
                    HOTLOADEntity.SaveChanges();
                }
                PopulateGrid();
            }
            //Entity Model Exception handler
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

        private void copy_btn_Click(object sender, RoutedEventArgs e)
        {

        }
        //Fill textboxes with selected values
        private void LoadBoard_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LoadBoard.SelectedIndex != -1)
            {
                //Load model
                Load loadModel = (Load)LoadBoard.SelectedItem;
                using (HOTLOADDBEntities2 HOTLOADEntity = new HOTLOADDBEntities2())
                {
                    loadModel = HOTLOADEntity.Loads.Where(x => x.bol_num == loadModel.bol_num).FirstOrDefault();
                    bol_txt.Text = loadModel.bol_num.ToString();
                    pro_txt.Text = loadModel.pro_num.ToString();
                    quote_txt.Text = loadModel.quote_num.ToString();
                    ref_txt.Text = loadModel.ref_num.ToString();
                    weight_txt.Text = loadModel.weight.ToString();
                    pieces_txt.Text = loadModel.pieces.ToString();
                    commodity_txt.Text = loadModel.commodity.ToString();
                    mileage_txt.Text = loadModel.mileage.ToString();
                    carrierRate_txt.Text = loadModel.carrier_rate.ToString();
                    customerRate_txt.Text = loadModel.customer_rate.ToString();
                    pickDate_picker.Text = loadModel.pick_appointment.ToString();
                    dropDate_picker.Text = loadModel.drop_appointment.ToString();
                    driver_txt.Text = loadModel.driver_id.ToString();
                    dispatch_txt.Text = loadModel.dispatch_id.ToString();
                    customer_txt.Text = loadModel.customer_id.ToString();
                    broker_txt.Text = loadModel.broker_id.ToString();
                }
                //Enable copy & delete buttons
                delete_btn.IsEnabled = true;
                copy_btn.IsEnabled = true;
            }
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
