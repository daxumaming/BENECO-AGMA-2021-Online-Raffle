﻿using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;

namespace AGMARaffle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public delegate void Invoker();

        public MainWindow()
        {
            this.Loaded += new RoutedEventHandler(windowRaffle_Loaded);
            InitializeComponent();
        }


        void windowRaffle_Loaded(object sender, RoutedEventArgs e)
        {
            // initialize and open database
            using var prizeList_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString);
            prizeList_conn.Open();

            // fetch random record from database
            String prizeList_sql =
                "SELECT prize_name FROM prizes ORDER BY prizeid ASC";

            using var prizeList_cmd = new MySqlCommand(prizeList_sql, prizeList_conn);
            using MySqlDataReader prizeList_rdr = prizeList_cmd.ExecuteReader();

            if (prizeList_rdr.HasRows)
            {
                //ComboBox prizeDropdown = new ComboBox();

                while (prizeList_rdr.Read())
                {
                    prizeDropdown.Items.Add(prizeList_rdr.GetString(0));
                }

            }
            else
            {
                MessageBox.Show("Error retrieving list of prizes.");
            }

        }

        private void prizeDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // selected value from combobox
            String selectedItem = prizeDropdown.SelectedItem.ToString();

            // initialize and open database
            using var prizeNo_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString);
            prizeNo_conn.Open();

            // fetch random record from database
            String prizeNo_sql =
                "SELECT total, issued FROM prizes WHERE prize_name = @prizename";

            using var prizeNo_cmd = new MySqlCommand(prizeNo_sql, prizeNo_conn);
            prizeNo_cmd.Parameters.AddWithValue("@prizename", selectedItem);
            using MySqlDataReader prizeNo_rdr = prizeNo_cmd.ExecuteReader();
            prizeNo_rdr.Read();
            int remaining = prizeNo_rdr.GetInt32(0) - prizeNo_rdr.GetInt32(1);

            txt_prizeNo.Text = remaining.ToString();
        }


        private void drawButton_Click(object sender, RoutedEventArgs e)
        {
            // check if there's a value from ComboBox
            if (prizeDropdown.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a raffle prize.");
            }
            else
            {
                // initialize and open database
                using var rafDraw_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString);
                rafDraw_conn.Open();

                // fetch random record from database
                String rafDraw_sql =
                    "SELECT id, accountno, firstname, lastname, emailaddress FROM registeredattendees " +
                    "WHERE accountname NOT IN " +
                    "(SELECT accountname FROM registeredattendees WHERE flag_winner = 1) " +
                    "AND accountno != '' " +
                    "ORDER BY RAND() LIMIT 1 ";

                using var rafDraw_cmd = new MySqlCommand(rafDraw_sql, rafDraw_conn);
                using MySqlDataReader rafDraw_rdr = rafDraw_cmd.ExecuteReader();
                rafDraw_rdr.Read();

                if (rafDraw_rdr.HasRows)
                {

                    // set variable
                    String id = rafDraw_rdr.GetString(0);
                    String actno = rafDraw_rdr.GetString(1);
                    String actname = rafDraw_rdr.GetString(2) + " " + rafDraw_rdr.GetString(3);
                    String emailaddress = rafDraw_rdr.GetString(4);

                    // show results
                    txt_accountno.Text = actno;
                    txt_accountname.Text = actname;
                    listResults.Items.Add(new { acctno = actno, acctname = actname, emailadd = emailaddress });

                    /*
                    // initialize and open database
                    using var rafUpd_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString);
                    rafUpd_conn.Open();

                    // update record, set flag to true
                    String dtnow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    String rafUpd_sql =
                        "UPDATE registeredattendees " +
                        "SET flag_winner = 1, " +
                        "drawtime = '" + dtnow + "' " +
                        "WHERE id = " + id;

                    using var rafUpd_cmd = new MySqlCommand();
                    rafUpd_cmd.Connection = rafUpd_conn;
                    rafUpd_cmd.CommandText = rafUpd_sql;
                    rafUpd_cmd.ExecuteNonQuery();
                    */

                }
                else
                {
                    txt_accountno.Text = "";
                    txt_accountname.Text = "No more qualified attendees to draw.";
                }
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
