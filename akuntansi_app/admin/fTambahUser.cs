using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using _connectMySQL;
using _tools;

namespace akuntansi_app.admin
{
    public partial class FTambahUser : Form
    {
        #region KOMPONEN WAJIB
        readonly CConnection _connect = new CConnection();
        readonly CTools _tools = new CTools();
        private MySqlConnection _connection;
        private string _sqlQuery;
        private readonly string _configurationManager = Properties.Settings.Default.Setting;
        #endregion

        public FTambahUser()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Control theControl = null; // the control which will be returned

            string errMsg = "";
            _connection = _connect.Connect(_configurationManager, ref errMsg, "123");

            int maxid = _connect.GetMaxId(_connection, "tbl_user", "id") + 1;

            _sqlQuery = "insert into db_toko.tMenu values (" + maxid + ", '" + txtNama.Text + "', '" + txtNama.Text +
                        "', )";

            if (!string.IsNullOrEmpty(errMsg))
            {
                MessageBox.Show(errMsg);
                return;
            }

            if (_tools.CheckEmptyTextBox(this, ref theControl))
            {
                MessageBox.Show(@"Silahkan Lengkapi Data");
                theControl.Focus();
                return;
            }
            try
            {
                _connect.Insertion(_sqlQuery
                    , _connection);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(@"Error performing insert to database : " + ex.Message);
                _connection.Close();
                return;
            }
            _connection.Close();
            ShowUser();
            _tools.ClearControlText(this);
        }

        private void FTambahUser_Load(object sender, EventArgs e)
        {
            ShowUser();
        }

        private void ShowUser()
        {
            List<ListViewItem> groupItems = new List<ListViewItem>();
            string errMsg = "";

            _connection = _connect.Connect(_configurationManager, ref errMsg, "123");

            MySqlDataReader reader = _connect.Reading("SELECT a.user, a.nama , a.pass " +
                "tbl_user a", _connection);

            if (!string.IsNullOrEmpty(errMsg))
            {
                MessageBox.Show(errMsg);
                return;
            }

            int noUrut = 1;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ListViewItem item = new ListViewItem(Convert.ToString(noUrut));
                    item.SubItems.Add(Convert.ToString(reader[0]));
                    item.SubItems.Add(Convert.ToString(reader[1]));
                    item.SubItems.Add(Convert.ToString(reader[2]));
                    groupItems.Add(item);
                    noUrut++;
                }
                reader.Close();
            }
            lvDaftarUser.BeginUpdate();
            lvDaftarUser.Items.Clear();
            lvDaftarUser.Items.AddRange(groupItems.ToArray());
            lvDaftarUser.EndUpdate();
            groupItems.Clear();

            _tools.AutoResizeListView(lvDaftarUser, true);
            _connection.Close();
        }
    }
}
