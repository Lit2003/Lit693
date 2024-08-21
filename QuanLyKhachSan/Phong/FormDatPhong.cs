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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QuanLyKhachSan.Phong
{
    public partial class FormDatPhong : Form
    {
        public FormDatPhong()
        {
            InitializeComponent();
            this.Name = "FormDatPhong";

        }
        public void loadlaidataDatPhong()
        {
            loadData();
        }
        private void FormDatPhong_Load(object sender, EventArgs e)
        {
            loadData();
            SetNextID();
        }

        #region
        string connectionString = "Data Source=DESKTOP-JKB5K32\\SQLEXPRESS;Initial Catalog=KhachSan;Integrated Security=True;Encrypt=False";
        string query = "select IDThueP as 'ID',TenP as 'Tên Phòng',Gia as 'Giá',tb_DatPhong.SoNguoi as 'Số Người',tb_KhachHang.IDKH as 'Mã Khách Hàng',tb_LoaiPhong.TenLoaiP as 'Loại Phòng',NgayDat as 'Ngày Đặt'" +
            "from tb_DatPhong inner join tb_Phong on tb_DatPhong.IDPhong = tb_Phong.IDPhong" +
            " join tb_LoaiPhong on tb_Phong.IDLoaiP = tb_LoaiPhong.IDLoaiP" +
            " join tb_KhachHang on tb_DatPhong.IDKH = tb_KhachHang.IDKH";
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adt;
        #endregion
        void loadData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {

                try
                {
                    con.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.Columns["Giá"].DefaultCellStyle.Format = "N0";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return;
                }
               // LoadComboBoxTenP();
                LoadComboBoxMaKH();
                LoadComboBoxLoaiPhong();
       
            }

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
        void LoadComboBoxLoaiPhong()
        {
            string query = "SELECT DISTINCT TenLoaiP FROM tb_LoaiPhong WHERE TenLoaiP != N'Bảo Trì'";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    cmbLoaiP.DataSource = dt;
                    cmbLoaiP.DisplayMember = "TenLoaiP";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        void LoadComboBoxTenPByLoaiPhong(string loaiPhong)
        {
            string query = "SELECT TenP FROM tb_Phong INNER JOIN tb_LoaiPhong ON tb_Phong.IDLoaiP = tb_LoaiPhong.IDLoaiP WHERE TenLoaiP = @TenLoaiP AND IDTrangThai = '1'";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@TenLoaiP", loaiPhong);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
               
                        cmbTenP.DataSource = dt;
                        cmbTenP.DisplayMember = "TenP";
                    

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void cmbLoaiP_SelectedIndexChanged(object sender, EventArgs e)
        {
            string loaiPhong = cmbLoaiP.Text;
            LoadComboBoxTenPByLoaiPhong(loaiPhong);

            string tenloaiPhong = cmbLoaiP.Text;
            int soNguoiToiDa = 0;
            float gia =0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT tb_LoaiPhong.SoNguoi, Gia FROM tb_LoaiPhong WHERE TenLoaiP = @TenLoaiP ";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@TenLoaiP", tenloaiPhong);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        soNguoiToiDa = Convert.ToInt32(reader["SoNguoi"]);
                        gia = Convert.ToSingle(reader["Gia"]);
                 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }

            // Tạo danh sách các lựa chọn cho ComboBox "Số Người" từ 1 đến số người tối đa
            List<int> soNguoiList = Enumerable.Range(1, soNguoiToiDa).ToList();

            // Thiết lập DataSource cho ComboBox "Số Người" là danh sách lựa chọn vừa tạo
            cmbSoNguoi.DataSource = soNguoiList;

            // Cập nhật giá phòng
            txbGia.Text = gia.ToString("N0");

        }
        void SetNextID()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ISNULL(MAX(IDThueP), 0) + 1 FROM tb_DatPhong";
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
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string queryCheckRoom = "SELECT COUNT(*) FROM tb_Phong WHERE TenP = @TenP AND IDTrangThai = '1'";
                string queryInsert = "INSERT INTO tb_DatPhong (IDThueP, IDPhong, IDKH, SoNguoi, NgayDat) " +
                                     "VALUES (@IDThueP, (SELECT IDPhong FROM tb_Phong WHERE TenP = @TenP), @IDKH, @SoNguoi, @NgayDat)";
                string queryUpdate = "UPDATE tb_Phong SET IDTrangThai = '2' WHERE TenP = @TenP"; // 2 là mã trạng thái "Đã thuê"

                SqlCommand cmdCheckRoom = new SqlCommand(queryCheckRoom, con);
                SqlCommand cmdInsert = new SqlCommand(queryInsert, con);
                SqlCommand cmdUpdate = new SqlCommand(queryUpdate, con);

                // Lấy các giá trị từ các control trên form
                string idThueP = txbID.Text; // Lấy IDThueP từ TextBox txbID
                string tenP = cmbTenP.Text;  // Lấy Tên Phòng từ ComboBox cmbTenP
                string idKH = cmbIDKH.Text;   // Lấy Mã Khách Hàng từ ComboBox cmbIDKH
                int soNguoi = Convert.ToInt32(cmbSoNguoi.Text);  // Lấy Số Người từ ComboBox cmbSoNguoi
                DateTime ngayDat = dtpNgayDat.Value; // Lấy Ngày Đặt từ DateTimePicker dtpNgayDat

                // Thêm các tham số vào SqlCommand
                cmdCheckRoom.Parameters.AddWithValue("@TenP", tenP);
                cmdInsert.Parameters.AddWithValue("@IDThueP", idThueP);
                cmdInsert.Parameters.AddWithValue("@TenP", tenP);
                cmdInsert.Parameters.AddWithValue("@IDKH", idKH);
                cmdInsert.Parameters.AddWithValue("@SoNguoi", soNguoi);
                cmdInsert.Parameters.AddWithValue("@NgayDat", ngayDat);

                cmdUpdate.Parameters.AddWithValue("@TenP", tenP);

                try
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    // Kiểm tra xem phòng có còn trống không
                    cmdCheckRoom.Transaction = transaction;
                    int roomCount = (int)cmdCheckRoom.ExecuteScalar();

                    if (roomCount > 0)
                    {
                        // Thực hiện thêm vào bảng tb_DatPhong
                        cmdInsert.Transaction = transaction;
                        int rowsInserted = cmdInsert.ExecuteNonQuery();

                        // Thực hiện cập nhật trạng thái phòng trong bảng tb_Phong
                        if (rowsInserted > 0)
                        {
                            cmdUpdate.Transaction = transaction;
                            int rowsUpdated = cmdUpdate.ExecuteNonQuery();
                            if (rowsUpdated > 0)
                            {
                                transaction.Commit();
                                MessageBox.Show("Đặt phòng thành công.");
                                loadData(); // Load lại dữ liệu sau khi thêm thành công
                                SetNextID(); // Cập nhật lại ID cho lần thêm tiếp theo
                                NotifyDataChanged();
                            }
                            else
                            {
                                transaction.Rollback();
                                MessageBox.Show("Đặt phòng không thành công.");
                            }
                        }
                        else
                        {
                            transaction.Rollback();
                            MessageBox.Show("Đặt phòng không thành công.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tất cả các phòng đã được đặt. Không thể thực hiện đặt phòng.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            Them();
        }
        private void NotifyDataChanged()
        {
            FormDSPhong formPhong = Application.OpenForms["FormDSPhong"] as FormDSPhong;
            formPhong?.LoadlaiDataphong();

            FormThanhToan formThanhToan = Application.OpenForms["FormThanhToan"] as FormThanhToan;
            formThanhToan?.LoadlaiData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}