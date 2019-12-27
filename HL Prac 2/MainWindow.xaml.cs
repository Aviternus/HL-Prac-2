using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Linq.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
//C# DateTime format = 11/12/2019 12:00:00 AM MM/DD/YYYY HH:MM:SS AM
namespace HL_Prac_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Entity Model
        HOTLOADDBEntities HOTLOADEntity;

        public MainWindow()
        {
            InitializeComponent();
            Search();
        }

        //Program Initialization
        public void Initialize()
        {
            
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

            //Change Update/New button text
            update_btn.Content = "New Load";

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
            HOTLOADEntity = new HOTLOADDBEntities();
            LoadBoard.ItemsSource = HOTLOADEntity.Loads.ToList();
        }

        //DateBuilding method
        public DateTime DateBuilder(DateTime datePicker, string timeText)
        {
            //Starting value
            DateTime newDate = datePicker;
            //Set default time value
            if (timeText=="")
            {
                timeText = "00:00";
            }

            //Zero out previous time values
            newDate = newDate.AddHours(-newDate.Hour);
            newDate = newDate.AddMinutes(-newDate.Minute);

            //Add in time values
            string[] splitTime = timeText.Split(':');
            newDate = newDate.AddHours(Convert.ToInt32(splitTime[0]));
            newDate = newDate.AddMinutes(Convert.ToInt32(splitTime[1]));

            return newDate;
        }

        //Time Span Builder
        public TimeSpan TimeSpanBuilder(string timeString)
        {
            var hours = Int32.Parse(timeString.Split(':')[0]);
            var minutes = Int32.Parse(timeString.Split(':')[1]);

            var timeSpan = new TimeSpan(hours, minutes, 0);
            return timeSpan;
        }

        public string TimeStringBuilder(TimeSpan span)
        {
            string format = @"hh\:mm";
            string time;

            time = span.ToString(format);
            
            return time;
        }

        public int ParseStatus(string statusString)
        {
            int comboBoxIndex = 0;
            switch (statusString)
            {
                case "Unnassigned":
                    comboBoxIndex = 0;
                    break;
                case "Assigned":
                    comboBoxIndex = 1;
                    break;
                case "Rate Confirmation":
                    comboBoxIndex = 2;
                    break;
                case "Dispatched":
                    comboBoxIndex = 3;
                    break;
                case "At Shipper":
                    comboBoxIndex = 4;
                    break;
                case "In Transit":
                    comboBoxIndex = 5;
                    break;
                case "At Consignee":
                    comboBoxIndex = 6;
                    break;
                case "Delivered":
                    comboBoxIndex = 7;
                    break;
                case "Paid":
                    comboBoxIndex = 8;
                    break;
                case "Invoiced":
                    comboBoxIndex = 9;
                    break;
                case "Collected":
                    comboBoxIndex = 10;
                    break;
                default:
                    comboBoxIndex = 0;
                    break;
            }
            return comboBoxIndex;
        }

        //Update or Create Button
        private void update_btn_Click(object sender, RoutedEventArgs e)
        {
            //Value Setting process
            try
            {
                //Load model
                Load loadModel = new Load();
                //Get the load data from input fields
                loadModel.bol_num = Convert.ToInt32(bol_txt.Text.Trim());
                loadModel.load_status = loadStatus_cmbo.Text;
                loadModel.pro_num = pro_txt.Text.Trim();
                loadModel.quote_num = quote_txt.Text.Trim();
                loadModel.ref_num = ref_txt.Text.Trim();
                try
                {
                    loadModel.weight = Convert.ToDouble(weight_txt.Text.Trim());
                }
                catch (System.FormatException ex){ MessageBox.Show(ex.Message, "Invalid Pieces Entry", MessageBoxButton.OK, MessageBoxImage.Error);}
                try
                { 
                    loadModel.pieces = Convert.ToInt32(pieces_txt.Text.Trim()); 
                }
                catch(System.FormatException ex){MessageBox.Show(ex.Message, "Invalid Pieces Entry", MessageBoxButton.OK, MessageBoxImage.Error);}
                
                loadModel.commodity = commodity_txt.Text.Trim();
                loadModel.mileage = Convert.ToDouble(mileage_txt.Text.Trim());
                loadModel.carrier_rate = Convert.ToDecimal(carrierRate_txt.Text.Trim());
                loadModel.customer_rate = Convert.ToDecimal(customerRate_txt.Text.Trim());

                //Pick Date & Time setter
                loadModel.pick_date = pickDate_picker.SelectedDate.Value;
                loadModel.pick_time = TimeSpanBuilder(pickAptTime_txt.Text);

                //Drop Date & Time setter
                loadModel.drop_date = dropDate_picker.SelectedDate.Value;
                loadModel.drop_time = TimeSpanBuilder(dropAptTime_txt.Text);

                loadModel.driver_id = Convert.ToInt32(driver_txt.Text.Trim());
                loadModel.dispatch_id = Convert.ToInt32(dispatch_txt.Text.Trim());
                loadModel.customer_id = Convert.ToInt32(customer_txt.Text.Trim());
                loadModel.broker_id = Convert.ToInt32(broker_txt.Text.Trim());

                //Last updated
                loadModel.last_updated_time = DateTime.Now;

                //Save the load to the database
                using(HOTLOADDBEntities HOTLOADEntity = new HOTLOADDBEntities())
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
                    Search();
                }
                
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

        //Copy Load button
        private void copy_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        //Fill textboxes with selected values
        private void LoadBoard_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LoadBoard.SelectedIndex != -1)
            {
                //Load model
                ViewModel SelectedItem = (ViewModel)LoadBoard.SelectedItem;
                Load loadModel = HOTLOADEntity.Loads.Find(SelectedItem.bol_num);
                using (HOTLOADDBEntities HOTLOADEntity = new HOTLOADDBEntities())
                {
                    loadModel = HOTLOADEntity.Loads.Where(x => x.bol_num == loadModel.bol_num).FirstOrDefault();
                    bol_txt.Text = loadModel.bol_num.ToString();
                    loadStatus_cmbo.SelectedIndex = ParseStatus(loadModel.load_status);
                    pro_txt.Text = loadModel.pro_num.ToString();
                    quote_txt.Text = loadModel.quote_num.ToString();
                    ref_txt.Text = loadModel.ref_num.ToString();
                    weight_txt.Text = loadModel.weight.ToString();
                    pieces_txt.Text = loadModel.pieces.ToString();
                    commodity_txt.Text = loadModel.commodity.ToString();
                    mileage_txt.Text = loadModel.mileage.ToString();
                    carrierRate_txt.Text = loadModel.carrier_rate.ToString();
                    customerRate_txt.Text = loadModel.customer_rate.ToString();

                    //Dates & Times
                    pickDate_picker.Text = loadModel.pick_date.ToString();
                    pickAptTime_txt.Text = TimeStringBuilder(loadModel.pick_time.Value);

                    dropDate_picker.Text = loadModel.drop_date.ToString();
                    dropAptTime_txt.Text = TimeStringBuilder(loadModel.drop_time.Value);

                    driver_txt.Text = loadModel.driver_id.ToString();
                    dispatch_txt.Text = loadModel.dispatch_id.ToString();
                    customer_txt.Text = loadModel.customer_id.ToString();
                    broker_txt.Text = loadModel.broker_id.ToString();

                    lastUpdated_lbl.Content = "Last Updated: " + loadModel.last_updated_time;
                }
                //Change Update/New button text
                update_btn.Content = "Update Load";

                //Enable copy & delete buttons
                delete_btn.IsEnabled = true;
                copy_btn.IsEnabled = true;
            }
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            //Get the selected load from Datagrid
            HOTLOADDBEntities HOTLOADEntity = new HOTLOADDBEntities();
            ViewModel SelectedItem = (ViewModel)LoadBoard.SelectedItem;
            Load loadModel = HOTLOADEntity.Loads.Find(SelectedItem.bol_num);
            if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm Deletion", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                using (HOTLOADEntity) 
                {
                    var entry = HOTLOADEntity.Entry(loadModel);
                    if(entry.State == EntityState.Detached)
                    {
                        HOTLOADEntity.Loads.Attach(loadModel);
                    }
                    //Remove the selected load from database
                    HOTLOADEntity.Loads.Remove(loadModel);
                    HOTLOADEntity.SaveChanges();
                    PopulateGrid();
                    Clear();
                    MessageBox.Show("Deleted Load #" + loadModel.bol_num + " succesfully.", "Load Deleted", MessageBoxButton.OK);
                }
        }

        //Search function
        private void Search()
        {
            HOTLOADEntity = new HOTLOADDBEntities();

            //Timespan handling
            TimeSpan pickTimeStart = TimeSpan.Zero;
            TimeSpan pickTimeEnd = TimeSpan.Zero;
            TimeSpan dropTimeStart = TimeSpan.Zero;
            TimeSpan dropTimeEnd = TimeSpan.Zero;
            try { pickTimeStart = TimeSpanBuilder(pickTimeStartSearch_txt.Text); }catch (System.Exception){//Ignore
            }
            try{ pickTimeEnd = TimeSpanBuilder(pickTimeEndSearch_txt.Text); }catch (System.Exception){//Ignore
            }
            try { dropTimeStart = TimeSpanBuilder(dropTimeStartSearch_txt.Text); }catch (System.Exception){//Ignore
            }
            try { dropTimeEnd = TimeSpanBuilder(dropTimeEndSearch_txt.Text); }catch (System.Exception){//Ignore
            }

            var matchedLoads = (
                            from loads in HOTLOADEntity.Loads
                            //Drivers join
                            join drivers in HOTLOADEntity.Contacts
                            on loads.driver_id equals drivers.id
                            //Dispatchers join
                            join dispatchers in HOTLOADEntity.Contacts
                            on loads.dispatch_id equals dispatchers.id

                            where 
                            loads.bol_num.ToString().Contains(bolSearch_txt.Text) &&
                            loads.pro_num.ToString().Contains(proSearch_txt.Text) &&
                            loads.quote_num.ToString().Contains(quoteSearch_txt.Text) &&
                            loads.ref_num.ToString().Contains(refSearch_txt.Text) &&

                            //Pick Date search terms
                            ((pickDateStart_dtpckr.SelectedDate == null || loads.pick_date >= pickDateStart_dtpckr.SelectedDate) &&

                            (pickDateEnd_dtpckr.SelectedDate == null || loads.pick_date <= pickDateEnd_dtpckr.SelectedDate)) &&

                            //Pick Time search terms
                            ((pickTimeStartSearch_txt.Text == null || pickTimeStart == TimeSpan.Zero || loads.pick_time.Value >= pickTimeStart) &&
                            (pickTimeEndSearch_txt.Text == null || pickTimeEnd == TimeSpan.Zero || loads.pick_time.Value <= pickTimeEnd)) &&

                            //Drop Date Search terms
                            ((dropDateStart_dtpckr.SelectedDate == null || loads.drop_date >= dropDateStart_dtpckr.SelectedDate) &&

                            (dropDateEnd_dtpckr.SelectedDate == null || loads.drop_date <= dropDateEnd_dtpckr.SelectedDate)) &&

                            //Drop Time search terms
                            ((dropTimeStartSearch_txt.Text == null || dropTimeStart == TimeSpan.Zero || loads.drop_time.Value >= dropTimeStart) &&
                            (dropTimeEndSearch_txt.Text == null || dropTimeEnd == TimeSpan.Zero || loads.drop_time.Value <= dropTimeEnd))

                            select new ViewModel
                            {
                                //Load properties
                                bol_num = loads.bol_num,
                                load_status = loads.load_status,
                                pro_num = loads.pro_num,
                                quote_num = loads.quote_num,
                                ref_num = loads.ref_num,
                                weight = loads.weight,
                                pieces = loads.pieces,
                                commodity = loads.commodity,
                                mileage = loads.mileage,
                                carrier_rate = loads.carrier_rate,
                                customer_rate = loads.customer_rate,
                                pick_date = loads.pick_date,
                                pick_time = loads.pick_time,
                                drop_date = loads.drop_date,
                                drop_time = loads.drop_time,
                                last_updated_time = loads.last_updated_time,
                                driver_id = loads.driver_id,
                                dispatch_id = loads.dispatch_id,
                                customer_id = loads.customer_id,
                                broker_id = loads.broker_id,
                                account_id = loads.account_id,

                                //Driver properties
                                driverContact_name = drivers.contact_name,
                                driverContact_phone = drivers.contact_phone,
                                driverContact_email = drivers.contact_email,

                                //Dispatch properties
                                dispatchContact_name = dispatchers.contact_name,
                                dispatchContact_phone = dispatchers.contact_phone,
                                dispatchContact_email = dispatchers.contact_email,
                            });

            LoadBoard.ItemsSource = matchedLoads.ToList();
        }

        private void Search(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Search();
        }

        private void Search(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Search();
        }
        //Column Display Control
        private void ColumnController(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            List<System.Windows.Controls.MenuItem> MenuItems = new List<System.Windows.Controls.MenuItem> {
                bolController_mnu,
                statusController_mnu,
                proController_mnu,
                quoteController_mnu,
                refController_mnu,
                weightController_mnu,
                piecesController_mnu,
                commodityController_mnu,
                mileageController_mnu,
                carrierRateController_mnu,
                customerRateController_mnu,
                pDateController_mnu,
                pTimeController_mnu,
                dDateController_mnu,
                dTimeController_mnu,
                lastUpdateController_mnu,
                driverNameController_mnu,
                dispatchNameController_mnu,
                customerController_mnu,
                brokerController_mnu
            };

            List<System.Windows.Controls.DataGridTextColumn> columns = new List<System.Windows.Controls.DataGridTextColumn> {
                bolColumn,
                statusColumn,
                proColumn,
                quoteColumn,
                refColumn,
                weightColumn,
                piecesColumn,
                commodityColumn,
                mileageColumn,
                carrierColumn,
                customerRateColumn,
                pickDateColumn,
                pickTimeColumn,
                dropDateColumn,
                dropTimeColumn,
                lastUpdatedColumn,
                driverNameColumn,
                DispatchNameColumn,
                CustomerColumn,
                BrokerColumn
            };

            foreach (System.Windows.Controls.MenuItem MenuItem in MenuItems)
            {
                int index = MenuItems.IndexOf(MenuItem);

                if (MenuItem.IsChecked)
                {
                    columns[index].Visibility = Visibility.Visible;
                }
                else if (!MenuItem.IsChecked)
                {
                    columns[index].Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
