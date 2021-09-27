using System;
using MySql.Data.MySqlClient;
using System.Windows;

namespace AGMARaffle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void drawButton_Click(object sender, RoutedEventArgs e)
        {
            // initialize and open database
            using var rafDraw_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString);
            rafDraw_conn.Open();

            // fetch random record from database
            String rafDraw_sql =
                "SELECT id, accountno, accountname FROM registeredattendees " +
                "WHERE accountname NOT IN " +
                "(SELECT accountname FROM registeredattendees WHERE flag_selected = 1) " +
                "ORDER BY RAND() LIMIT 1";

            using var rafDraw_cmd = new MySqlCommand(rafDraw_sql, rafDraw_conn);
            using MySqlDataReader rafDraw_rdr = rafDraw_cmd.ExecuteReader();
            rafDraw_rdr.Read();

            if (rafDraw_rdr.HasRows)
            {

                // set variable
                String id = rafDraw_rdr.GetString(0);
                String actno = rafDraw_rdr.GetString(1);
                String actname = rafDraw_rdr.GetString(2);

                // show results
                txt_accountno.Text = actno;
                txt_accountname.Text = actname;
                //listResults.Items.Add(new { idno = id, acctno = actno, acctname = actname });
                listResults.Items.Add(new { acctno = actno, acctname = actname });

                // initialize and open database
                using var rafUpd_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString);
                rafUpd_conn.Open();

                // update record, set flag to true
                String dtnow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                String rafUpd_sql =
                    "UPDATE registeredattendees " +
                    "SET flag_selected = 1, " +
                    "datetimeselected = '" + dtnow + "' " +
                    "WHERE id = " + id;

                using var rafUpd_cmd = new MySqlCommand();
                rafUpd_cmd.Connection = rafUpd_conn;
                rafUpd_cmd.CommandText = rafUpd_sql;
                rafUpd_cmd.ExecuteNonQuery();

            } else
            {
                txt_accountno.Text = "";
                txt_accountname.Text = "No more qualified attendees to draw.";
            }
        }

        private void listWinnersButton_Click(object sender, RoutedEventArgs e)
        {
            // open another Window showing list of winners
            var winnerListWindow = new WinnersList();
            winnerListWindow.Show();
        }

    }
}
