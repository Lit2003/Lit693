using QuanLyKhachSan.KhachHang;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace QuanLyKhachSan
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        
        }
        string connectionString = "Data Source=DESKTOP-JKB5K32\\SQLEXPRESS;Initial Catalog=KhachSan;Integrated Security=True;Encrypt=False";
        private void btnDangNhap_Click(object sender, EventArgs e)
        {

            string username = txbuser.Text;
            string password = txbpass.Text;

            string employeeID = GetEmployeeID(username, password);

            if (!string.IsNullOrEmpty(employeeID))
            {
                Session.Idnv = employeeID;     
                FormMain f = new FormMain();
                f.Show();
                this.Hide();
                f.FormClosed += F_FormClosed;
            }
            else
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu không đúng.");
            }
        }

        private void F_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show();
            txbpass.Clear();
            txbuser.Clear();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
        private string GetEmployeeID(string username, string password)
        {
            string employeeID = "";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT IDNV FROM tb_User WHERE UID=@username AND PassW=@password";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    employeeID = cmd.ExecuteScalar()?.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return employeeID;
        }
    }
}
