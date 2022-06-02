using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace AGMARaffle
{
    /// <summary>
    /// Interaction logic for WinnersList.xaml
    /// </summary>
    public partial class WinnersList : Window
    {
        public WinnersList()
        {
            this.Loaded += new RoutedEventHandler(winnersList_Loaded);
            InitializeComponent();
        }

        void winnersList_Loaded(object sender, RoutedEventArgs e)
        {
            // initialize and open database
            using (MySqlConnection raffWinners_conn = new MySqlConnection(Properties.Settings.Default.ConnectionString))
            {
                try
                {
                    raffWinners_conn.Open();

                    // fetch list of winners
                    String raffWinners_sql =
                        "SELECT stubno AS 'Stub No', mconame AS 'MCO Name', " +
                        "prize AS 'Prize Won', drawtime AS 'Draw Time' " +
                        "FROM registeredmcos " +
                        "WHERE flag_winner = 1 " +
                        "ORDER BY drawtime ASC";

                    using var raffWinners_cmd = new MySqlCommand(raffWinners_sql, raffWinners_conn);
                    MySqlDataAdapter raffAdapter = new MySqlDataAdapter(raffWinners_cmd);
                    DataSet raffDataset = new DataSet();
                    raffAdapter.Fill(raffDataset, "LoadDataBinding");
                    listWinners.DataContext = raffDataset;

                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    raffWinners_conn.Close();
                }
            }

        }
    }
}
