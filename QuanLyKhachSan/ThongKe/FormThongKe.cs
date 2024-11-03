using Microsoft.Reporting.WinForms;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QuanLyKhachSan.ThongKe
{
    public partial class FormThongKe : Form
    {
        public FormThongKe()
        {
            InitializeComponent();
            this.Name = "FormThu";
        }
    public void loadlaidataData()
        {
            loaddata();
            this.reportViewer1.RefreshReport();
        }


    string connectionString = "Data Source=DESKTOP-JKB5K32\\SQLEXPRESS;Initial Catalog=KhachSan;Integrated Security=True;Encrypt=False";
        private void FormThongKe_Load(object sender, EventArgs e)
        {
            loaddata();
            LoadComboBoxThang();
            LoadComboBoxNam();
            this.reportViewer1.RefreshReport();
        }
        private void loaddata()
        {

        
            string query = @"
                            SELECT 
    hd.IDHoaDon,
    hd.IDKH,
    kh.HoTen,
	hd.PhongDaDat,
	hd.DVDaDung,
    hd.TongTien,
    hd.NgayThanhToan,
    hd.Note,
    hd.IDNV
FROM tb_HoaDon hd
inner JOIN tb_KhachHang kh ON hd.IDKH = kh.IDKH
GROUP BY 
    hd.IDHoaDon, 
    hd.IDKH, 
    kh.HoTen, 
    hd.TongTien, 
    hd.NgayThanhToan,
    hd.Note,
    hd.IDNV,
	hd.PhongDaDat,
	hd.DVDaDung;";

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dataTable);
                connection.Close();
            }
            // Gắn kết dữ liệu với ReportViewer
            ReportDataSource rds = new ReportDataSource("DataSet1", dataTable);
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(rds);
            this.reportViewer1.LocalReport.ReportPath = "D:\\QuanLyKS\\QuanLyKhachSan\\QuanLyKhachSan\\ThongKe\\rptThongKeDoanhThu.rdlc";

            this.reportViewer1.RefreshReport();
        }
        private void LoadComboBoxThang()
        {
            cmbThang.Items.Clear();
            cmbThang.Items.Add(""); // Thêm lựa chọn xem tat ca
            cmbThang.SelectedIndex = 0; // Chọn "Chọn tất cả" làm mặc định


            string query = @"
                SELECT DISTINCT LEFT(IDHoaDon, 2) AS TwoCharID
                FROM tb_HoaDon;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string twoCharID = reader["TwoCharID"].ToString();
                    cmbThang.Items.Add(twoCharID);
                }
                reader.Close();
                connection.Close();
            }
        }
        private void LoadComboBoxNam()
        {
            cmbNam.Items.Clear();
            cmbNam.Items.Add(""); // Thêm lựa chọn xem tat ca
            cmbNam.SelectedIndex = 0; // Chọn "Chọn tất cả" làm mặc định
            string query = @"
                SELECT DISTINCT SUBSTRING(IDHoaDon, 4, 2) AS TwoCharID
                FROM tb_HoaDon;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string twoCharID = reader["TwoCharID"].ToString();
                    cmbNam.Items.Add(twoCharID);
                }
                reader.Close();
                connection.Close();
            }
        }
        private void SearchByMaHoaDon()
        {
            string selectedTwoCharID12 = cmbThang.SelectedItem?.ToString();
            string selectedTwoCharID45 = cmbNam.SelectedItem?.ToString();

           
            string query = @"
        SELECT 
            hd.IDHoaDon,
            hd.IDKH,
            kh.HoTen,
            hd.PhongDaDat,
            hd.DVDaDung,
            hd.TongTien,
            hd.NgayThanhToan,
            hd.Note,
            hd.IDNV
        FROM tb_HoaDon hd
        INNER JOIN tb_KhachHang kh ON hd.IDKH = kh.IDKH
        WHERE (LEFT(hd.IDHoaDon, 2) LIKE @TwoCharID12 OR @TwoCharID12 IS NULL)
              AND (SUBSTRING(hd.IDHoaDon, 4, 2) LIKE @TwoCharID45 OR @TwoCharID45 IS NULL)
        GROUP BY 
            hd.IDHoaDon, 
            hd.IDKH, 
            kh.HoTen, 
            hd.TongTien, 
            hd.NgayThanhToan,
            hd.Note,
            hd.IDNV,
            hd.PhongDaDat,
            hd.DVDaDung;";

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.SelectCommand.Parameters.AddWithValue("@TwoCharID12", string.IsNullOrEmpty(selectedTwoCharID12) ? DBNull.Value : (object)selectedTwoCharID12);
                adapter.SelectCommand.Parameters.AddWithValue("@TwoCharID45", string.IsNullOrEmpty(selectedTwoCharID45) ? DBNull.Value : (object)selectedTwoCharID45);
                adapter.Fill(dataTable);
                connection.Close();
            }

            ReportDataSource rds = new ReportDataSource("DataSet1", dataTable);
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(rds);
            this.reportViewer1.LocalReport.ReportPath = "D:\\Đồ_án\\QuanLyKhachSan\\QuanLyKhachSan\\ThongKe\\rptThongKeDoanhThu.rdlc";

            this.reportViewer1.RefreshReport();
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {
            this.reportViewer1.RefreshReport();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SearchByMaHoaDon();

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
