using QuanLyKhachSan.KhachHang;
using QuanLyKhachSan.Phong;
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

namespace QuanLyKhachSan.DichVu
{
    public partial class FormDatDV : Form
    {
        public FormDatDV()
        {
            InitializeComponent();
        }
        public void loadlaidataData()
        {
            loadData();
        }
        private void FormDatDV_Load(object sender, EventArgs e)
        {
            loadData();
            SetNextID();
        }
        #region
        string connectionString = "Data Source=DESKTOP-JKB5K32\\SQLEXPRESS;Initial Catalog=KhachSan;Integrated Security=True;Encrypt=False";
        string query = "select IDThueDV as 'ID',tb_KhachHang.IDKH as 'Mã Khách Hàng',TenDV as 'Tên Dịch Vụ',Gia as 'Giá',LoaiDV as 'Loại Dịch Vụ',SoLuong as 'Số Lượng'" +
                       " from tb_SuDungDV inner join tb_DichVu on tb_SuDungDV.IDDV = tb_DichVu.IDDV" +
                                              " join tb_KhachHang on tb_SuDungDV.IDKH = tb_KhachHang.IDKH";
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adt;
        #endregion

        void loadData()
        {

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.Columns["Giá"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return;
            }
            LoadComboBoxMaKH();
            LoadComboBoxLoaiDV();
        }
        void LoadComboBoxMaKH()
        {
            string query = "SELECT IDKH FROM tb_KhachHang";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    cmbIDKH.DataSource = dt;
                    cmbIDKH.DisplayMember = "IDKH";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        void LoadComboBoxLoaiDV()
        {
            string query = "SELECT DISTINCT tb_DichVu.LoaiDV  FROM tb_DichVu";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    cmbLoaiDV.DataSource = dt;
                    cmbLoaiDV.DisplayMember = "LoaiDV";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        void LoadComboBoxTenDVByLoaiDV(string loaiDV)
        {
            string query = "SELECT TenDV, Gia FROM tb_DichVu WHERE LoaiDV = @LoaiDV";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@LoaiDV", loaiDV);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    cmbTenDV.DataSource = dt;
                    cmbTenDV.DisplayMember = "TenDV";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }    
        void SetNextID()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ISNULL(MAX(IDThueDV), 0) + 1 FROM tb_SuDungDV";
                SqlCommand cmd = new SqlCommand(query, con);

                try
                {
                    con.Open();
                    int nextID = (int)cmd.ExecuteScalar();
                    txbID.Text = nextID.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        void Them()
        {
            // Kiểm tra xem số lượng có được nhập vào không
            if (string.IsNullOrWhiteSpace(txbSoLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập số lượng.");
                return;
            }
            // Kiểm tra xem số lượng có phải là dạng số hay không
            if (!int.TryParse(txbSoLuong.Text, out int soluong))
            {
                MessageBox.Show("Số lượng phải là dạng số nguyên.");
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string queryInsert = "INSERT INTO tb_SuDungDV (IDThueDV, IDDV, IDKH, SoLuong) " +
                                     "VALUES (@IDThueDV, (SELECT IDDV FROM tb_DichVu WHERE TenDV = @TenDV), @IDKH, @SoLuong)";

                SqlCommand cmdInsert = new SqlCommand(queryInsert, con);

                // Lấy các giá trị từ các control trên form
                string idThuedv = txbID.Text; // Lấy IDThueDV từ TextBox txbID
                string tenDV = cmbTenDV.Text;  // Lấy Tên Dịch Vụ từ ComboBox cmbTenDV
                string idKH = cmbIDKH.Text;   // Lấy Mã Khách Hàng từ ComboBox cmbIDKH
               

                // Thêm các tham số vào SqlCommand
                cmdInsert.Parameters.AddWithValue("@IDThueDV", idThuedv);
                cmdInsert.Parameters.AddWithValue("@TenDV", tenDV);
                cmdInsert.Parameters.AddWithValue("@IDKH", idKH);
                cmdInsert.Parameters.AddWithValue("@SoLuong", soluong);

                SqlTransaction transaction = null; // Khai báo biến transaction ở đây

                try
                {
                    con.Open();
                    transaction = con.BeginTransaction();

                    // Thực hiện thêm vào bảng tb_SuDungDV
                    cmdInsert.Transaction = transaction;
                    int rowsInserted = cmdInsert.ExecuteNonQuery();

                    // Commit transaction nếu không có lỗi xảy ra
                    transaction.Commit();
                    MessageBox.Show("Thêm dịch vụ thành công!");
                    loadData();

                    // Cập nhật lại ID cho lần thêm tiếp theo
                    SetNextID();
                    NotifyDataChanged();
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi xảy ra
                    MessageBox.Show("Lỗi: " + ex.Message);
                    // Nếu transaction được khởi tạo, rollback để đảm bảo tính nhất quán dữ liệu
                    transaction?.Rollback();
                }
            }
        }

        private void cmbLoaiDV_SelectedIndexChanged(object sender, EventArgs e)
        {
            string loaiDV = cmbLoaiDV.Text;
            LoadComboBoxTenDVByLoaiDV(loaiDV);
        }

        private void cmbTenDV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTenDV.SelectedItem != null)
            {
                // Lấy dòng dữ liệu được chọn từ ComboBox cmbTenDV
                DataRowView selectedRow = cmbTenDV.SelectedItem as DataRowView;
                if (selectedRow != null)
                {
                    // Lấy giá trị của cột "Gia"
                    string giaDVStr = selectedRow["Gia"].ToString();
                    if (decimal.TryParse(giaDVStr, out decimal giaDV))
                    {
                        // Định dạng giá trị với dấu chấm phân cách hàng nghìn
                        txbGia.Text = giaDV.ToString("N0");
                    }
                    else
                    {
                        // Xử lý trường hợp giá trị không thể chuyển đổi thành số
                        MessageBox.Show("Giá trị không hợp lệ.");
                        txbGia.Text = "0";
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Them();
        }

        private void NotifyDataChanged()
        {   

            FormThanhToan formThanhToan = Application.OpenForms["FormThanhToan"] as FormThanhToan;
            formThanhToan?.LoadlaiData();
        }
    }
}

