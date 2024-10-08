﻿using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace AGMARaffle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.Loaded += new RoutedEventHandler(windowRaffle_Loaded);
            InitializeComponent();
        }


        void windowRaffle_Loaded(object sender, RoutedEventArgs e)
        {
            // initialize and open database
            //using var prizeList_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString)

            using (MySqlConnection prizeList_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString))
            {
                try
                {
                    prizeList_conn.Open();

                    // fetch list of prizes
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
                catch (MySqlException ex)
                {
                    if (ex.Message.Contains("Unable to connect"))
                    {
                        MessageBox.Show("Cannot connect to the database.");
                    } 
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                finally
                {
                    prizeList_conn.Close();
                }
            }

        }

        private void prizeDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // selected value from combobox
            String selectedItem = prizeDropdown.SelectedItem.ToString();

            // initialize and open database
            using (MySqlConnection prizeNo_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString))
            {
                try
                {
                    prizeNo_conn.Open();

                    // fetch record from database based on selected prize item
                    String prizeNo_sql =
                        "SELECT total, issued FROM prizes WHERE prize_name = @prizename";

                    using var prizeNo_cmd = new MySqlCommand(prizeNo_sql, prizeNo_conn);
                    prizeNo_cmd.Parameters.AddWithValue("@prizename", selectedItem);
                    using MySqlDataReader prizeNo_rdr = prizeNo_cmd.ExecuteReader();
                    prizeNo_rdr.Read();

                    // deduct issued from total
                    int remaining = prizeNo_rdr.GetInt32(0) - prizeNo_rdr.GetInt32(1);

                    // Update label with remaining prize
                    txt_prizeNo.Text = remaining.ToString();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    prizeNo_conn.Close();
                }
            }
                

                    
        }


        private async void drawButton_Click(object sender, RoutedEventArgs e)
        {
            txt_stubno.Content = "";

            // check if there's a value from ComboBox
            if (prizeDropdown.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a raffle prize.");
            }
            else
            {
                // set selectedPrize variable
                String selectedPrize = prizeDropdown.SelectedItem.ToString();

                // counter
                int otherdist_count = 0;
                int prefdist = 0;

                // default label content
                txt_winners.Content = "searching....";
                txt_prize.Content = "";
                txt_stubno.Content = "";

                var wheelWindow = new wheel();
                wheelWindow.Show();

                // wait three seconds
                await Task.Delay(5000);

                wheelWindow.Close();

                using (MySqlConnection rafDistrict_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    try
                    {
                        rafDistrict_conn.Open();

                        String rafDistrict_sql =
                            "SELECT preferreddistrict, otherdist_count FROM prizes " +
                            "WHERE prize_name = @selectedPrize";

                        using var rafDistrict_cmd = new MySqlCommand(rafDistrict_sql, rafDistrict_conn);
                        rafDistrict_cmd.Parameters.AddWithValue("@selectedPrize", selectedPrize);

                        using MySqlDataReader rafDistrict_rdr = rafDistrict_cmd.ExecuteReader();
                        rafDistrict_rdr.Read();

                        if (rafDistrict_rdr.HasRows)
                        {
                            // set variable
                            prefdist = rafDistrict_rdr.GetInt32(0);
                            otherdist_count = rafDistrict_rdr.GetInt32(1);

                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        rafDistrict_conn.Close();
                    }

                }

                // initialize and open database
                using (MySqlConnection rafDraw_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    try
                    {
                        rafDraw_conn.Open();
                        String rafDraw_sql = "";

                        // fetch random record based on preferred district
                        if (otherdist_count > 0)
                        {
                            rafDraw_sql =
                            "SELECT id, stubno, mconame, district FROM registeredmcos " +
                            "WHERE flag_winner IS NULL " +
                            "ORDER BY RAND() LIMIT 1 ";
                        } else
                        {
                            rafDraw_sql =
                            "SELECT id, stubno, mconame, district FROM registeredmcos " +
                            "WHERE district = @district " +
                            "AND flag_winner IS NULL " +
                            "ORDER BY RAND() LIMIT 1 ";
                        }

                        // fetch random record from database
                        using var rafDraw_cmd = new MySqlCommand(rafDraw_sql, rafDraw_conn);

                        if (otherdist_count == 0)
                        {
                            rafDraw_cmd.Parameters.AddWithValue("@district", prefdist);
                        }

                        using MySqlDataReader rafDraw_rdr = rafDraw_cmd.ExecuteReader();
                        rafDraw_rdr.Read();

                        if (rafDraw_rdr.HasRows)
                        {

                            // set variable
                            String id = rafDraw_rdr.GetString(0);
                            String stubno = rafDraw_rdr.GetString(1);
                            String mconame = rafDraw_rdr.GetString(2);
                            String district = rafDraw_rdr.GetString(3);

                            // show results
                            txt_winners.Content = mconame;
                            txt_stubno.Content = "Stub No. " + stubno;
                            txt_prize.Content = selectedPrize;
                            txt_id.Content = id;
                            txt_otherdist_count.Content = otherdist_count;
                            listResults.Items.Add(new { stubno = stubno, mconame = mconame, prize = selectedPrize });



                            // update registeredmcos with prize and draw time
                            using (MySqlConnection rafUpd_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString))
                            {
                                try
                                {
                                    rafUpd_conn.Open();

                                    // update record, set flag to true
                                    String dtnow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    String rafUpd_sql =
                                        "UPDATE registeredmcos " +
                                        "SET flag_winner = 1, " +
                                        "drawtime = '" + dtnow + "', " +
                                        "prize = @selectedPrize " +
                                        "WHERE id = " + id;

                                    using var rafUpd_cmd = new MySqlCommand();
                                    rafUpd_cmd.Connection = rafUpd_conn;
                                    rafUpd_cmd.CommandText = rafUpd_sql;
                                    rafUpd_cmd.Parameters.AddWithValue("@selectedPrize", selectedPrize);
                                    rafUpd_cmd.ExecuteNonQuery();
                                }
                                catch (MySqlException ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    rafUpd_conn.Close();
                                }
                                    
                            }

                        }
                        else
                        {
                            txt_stubno.Content = "";
                            txt_winners.Content = "No more qualified attendees to draw";
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        rafDraw_conn.Close();
                    }
                }
                    
            }

            
        }

        private void listWinnersButton_Click(object sender, RoutedEventArgs e)
        {
            // open another Window showing list of winners
            var winnerListWindow = new WinnersList();
            winnerListWindow.Show();
        }

        private void claimButton_click(object sender, RoutedEventArgs e)
        {
            // variable for id label
            String id = txt_id.Content.ToString();

            if (id == null || id == "")
            {
                MessageBox.Show("Please draw first.");

            } else
            {
                // update prize table
                using (MySqlConnection prizeUpd_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString))
                {
                    String selectedPrize = prizeDropdown.SelectedItem.ToString();
                    String otherdist_count_txt = txt_otherdist_count.Content.ToString();
                    int otherdist_count = int.Parse(otherdist_count_txt);


                    // update claimed flag
                    using (MySqlConnection rafUpd_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString))
                    {
                        try
                        {
                            rafUpd_conn.Open();

                            // update record, set flag to true
                            String dtnow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            String rafUpd_sql =
                                "UPDATE registeredmcos " +
                                "SET claimed = 1 " +
                                "WHERE id = " + id;

                            using var rafUpd_cmd = new MySqlCommand();
                            rafUpd_cmd.Connection = rafUpd_conn;
                            rafUpd_cmd.CommandText = rafUpd_sql;
                            rafUpd_cmd.Parameters.AddWithValue("@selectedPrize", selectedPrize);
                            rafUpd_cmd.ExecuteNonQuery();
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            rafUpd_conn.Close();
                        }

                    }


                    // update prize issued
                    try
                    {
                        prizeUpd_conn.Open();
                        String prizeUpd_sql = "";

                        if (otherdist_count > 0)
                        {
                            prizeUpd_sql =
                            "UPDATE prizes " +
                            "SET issued = issued + 1, " +
                            "otherdist_count = @otherdist_count " +
                            "WHERE prize_name = @prizeName ";
                        }
                        else
                        {
                            prizeUpd_sql =
                            "UPDATE prizes " +
                            "SET issued = issued + 1 " +
                            "WHERE prize_name = @prizeName ";
                        }

                        using var prizeUpd_cmd = new MySqlCommand();
                        prizeUpd_cmd.Connection = prizeUpd_conn;
                        prizeUpd_cmd.CommandText = prizeUpd_sql;
                        prizeUpd_cmd.Parameters.AddWithValue("@prizeName", selectedPrize);
                        if (otherdist_count > 0)
                        {
                            prizeUpd_cmd.Parameters.AddWithValue("@otherdist_count", otherdist_count - 1);
                        }
                        prizeUpd_cmd.ExecuteNonQuery();

                        Int32 prizeTotal = Convert.ToInt32(txt_prizeNo.Text);
                        prizeTotal--;
                        txt_prizeNo.Text = prizeTotal.ToString();
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        prizeUpd_conn.Close();
                    }
                }
            }
        }

    }
}
