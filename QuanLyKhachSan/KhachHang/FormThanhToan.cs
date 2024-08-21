using QuanLyKhachSan.DichVu;
using QuanLyKhachSan.Phong;
using QuanLyKhachSan.ThongKe;
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

namespace QuanLyKhachSan.KhachHang
{
    public partial class FormThanhToan : Form
    {
        private string employeeID;


        public FormThanhToan()
        {
            InitializeComponent();
            this.Name = "FormThanhToan";
            employeeID = Session.Idnv;
            txbIDNV.Text = employeeID;
         
        }

        public void LoadlaiData()
        {
            loadData();
        }
        private void FormThanhToan_Load(object sender, EventArgs e)
        {
            loadData();
            DisplayInvoiceID();
            txbIDNV.Text = employeeID;
        }
        #region
        string connectionString = "Data Source=DESKTOP-JKB5K32\\SQLEXPRESS;Initial Catalog=KhachSan;Integrated Security=True;Encrypt=False";
        string query = @"SELECT 
    dp.IDKH as 'Mã Khách Hàng', 
    ISNULL(kh.HoTen, '') as 'Tên Khách Hàng',
    ISNULL(p.TenP, '') as 'Phòng Đã Đặt', 
    ISNULL(dp.NgayDat, '') as 'Ngày Đặt', 
    ISNULL(lp.Gia, 0) as 'Giá Phòng', 
    ISNULL(STRING_AGG(ISNULL(ldv.TenDV, '') + ' (' + CAST(ISNULL(dv.SoLuong, 0) AS VARCHAR) + ')', ', '), '') AS 'Dịch Vụ Đã Dùng' 
FROM tb_DatPhong dp 
LEFT JOIN tb_SuDungDV dv ON dp.IDKH = dv.IDKH 
LEFT JOIN tb_Phong p ON dp.IDPhong = p.IDPhong 
LEFT JOIN tb_LoaiPhong lp ON lp.IDLoaiP = p.IDLoaiP 
LEFT JOIN tb_DichVu ldv ON ldv.IDDV = dv.IDDV 
LEFT JOIN tb_KhachHang kh ON dp.IDKH = kh.IDKH 
GROUP BY dp.IDKH, p.TenP, dp.NgayDat, lp.Gia, kh.HoTen

UNION

SELECT 
    dv.IDKH as 'Mã Khách Hàng', 
    ISNULL(kh.HoTen, '') as 'Tên Khách Hàng',
    '' as 'Phòng Đã Đặt', 
     ISNULL(CONVERT(varchar, GETDATE(), 23), '') as 'Ngày Đặt', 
    0 as 'Giá Phòng', 
    STRING_AGG(ISNULL(ldv.TenDV, '') + ' (' + CAST(ISNULL(dv.SoLuong, 0) AS VARCHAR) + ')', ', ') AS 'Dịch Vụ Đã Dùng' 
FROM tb_SuDungDV dv
LEFT JOIN tb_DichVu ldv ON ldv.IDDV = dv.IDDV 
LEFT JOIN tb_KhachHang kh ON dv.IDKH = kh.IDKH 
GROUP BY dv.IDKH, kh.HoTen
HAVING dv.IDKH NOT IN (SELECT IDKH FROM tb_DatPhong)";
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
                    dataGridView1.Columns["Giá Phòng"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            binding(dataGridView1);
     

        }
        void binding(DataGridView dtgv)
        {
            Binding bdIDKH = new Binding("Text", dtgv.DataSource, "Mã Khách Hàng", true, DataSourceUpdateMode.OnPropertyChanged);
            txbIDKH.DataBindings.Clear();
            txbIDKH.DataBindings.Add(bdIDKH);

            Binding bdTenP = new Binding("Text", dtgv.DataSource, "Phòng Đã Đặt", true, DataSourceUpdateMode.OnPropertyChanged);
            txbIDP.DataBindings.Clear();
            txbIDP.DataBindings.Add(bdTenP);

            Binding bdDV = new Binding("Text", dtgv.DataSource, "Dịch Vụ Đã Dùng", true, DataSourceUpdateMode.OnPropertyChanged);
            txbDV.DataBindings.Clear();
            txbDV.DataBindings.Add(bdDV);

            Binding bdNgayDat = new Binding("Value", dtgv.DataSource, "Ngày Đặt", true, DataSourceUpdateMode.OnPropertyChanged);
            dtpNgayDat.DataBindings.Clear();
            dtpNgayDat.DataBindings.Add(bdNgayDat);



        }
        private string GenerateInvoiceID()
        {
            string monthYear = DateTime.Now.ToString("MM-yy");

            string query = "SELECT ISNULL(MAX(RIGHT(IDHoaDon, 3)), 0) + 1 AS MaxID FROM tb_HoaDon WHERE LEFT(IDHoaDon, 5) = @MonthYear";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@MonthYear", monthYear);
                int maxID = (int)cmd.ExecuteScalar();

                string formattedID = maxID.ToString("D3");

                string invoiceID = $"{monthYear}-{formattedID}";

                return invoiceID;
            }

        }
        private void DisplayInvoiceID()
        {
            // Gọi hàm sinh ID hóa đơn
            string invoiceID = GenerateInvoiceID();

            // Gán giá trị ID hóa đơn vào TextBox tương ứng
            txbIDHD.Text = invoiceID;
        }

        private void updateSoNgay()
        {
            // Kiểm tra xem có hàng nào được chọn trong DataGridView không
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
            {
                MessageBox.Show("Vui lòng chọn một hàng từ bảng để cập nhật số ngày.");
                return;
            }

            try
            {
                // Lấy mã phòng từ DataGridView
                string tenPhong = dataGridView1.CurrentRow.Cells["Phòng Đã Đặt"].Value.ToString();
                if (string.IsNullOrEmpty(tenPhong))
                {
                    txbSoNgayO.Text = ""; // Xóa nội dung TextBox Số Ngày
                    return;
                }   


                // Lấy ngày đặt từ DataGridView
                DateTime ngayDat = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["Ngày Đặt"].Value);

                // Lấy ngày hiện tại
                DateTime ngayHienTai = DateTime.Now.Date; // Lấy ngày hiện tại bỏ đi phần giờ, phút, giây
                                                          
                // Tính số ngày đã đặt cho phòng đã chọn, đảm bảo ít nhất là 1 ngày
                int soNgay = (int)Math.Max(1, (ngayHienTai - ngayDat).TotalDays);

                // Hiển thị số ngày đã đặt trong TextBox 'Số Ngày'
                txbSoNgayO.Text = soNgay.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật số ngày: " + ex.Message);
            }
           
        }
        private float TinhTongTienPhong(string idKH)
        {
            float tongTienPhong = 0;

            // Kiểm tra xem có hàng nào được chọn trong DataGridView không
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index < 0)
            {
                MessageBox.Show("Vui lòng chọn một hàng từ bảng để tính tổng tiền phòng.");
                return tongTienPhong;
            }

            try
            {
                // Lấy số ngày ở TextBox 'Số Ngày'
                int soNgayDat = 0;
                if (!string.IsNullOrEmpty(txbSoNgayO.Text))
                {
                    soNgayDat = Convert.ToInt32(txbSoNgayO.Text);
                }

                // Lấy tên phòng từ DataGridView
                string tenPhong = dataGridView1.CurrentRow.Cells["Phòng Đã Đặt"].Value.ToString();
                if (string.IsNullOrEmpty(tenPhong))
                {
                    txbSoNgayO.Text = ""; // Xóa nội dung TextBox Số Ngày
                    txbTongTienP.Text = ""; // Xóa nội dung TextBox Tổng Tiền Phòng
                    return tongTienPhong;
                }

                // Kết nối đến CSDL và thực hiện truy vấn
                string query = @"
        SELECT SUM(lp.Gia * @SoNgayDat) AS TongTienPhong
        FROM tb_Phong p
        JOIN tb_LoaiPhong lp ON lp.IDLoaiP = p.IDLoaiP
        WHERE p.TenP = @TenPhong;";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SoNgayDat", soNgayDat);
                    cmd.Parameters.AddWithValue("@TenPhong", tenPhong);

                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        tongTienPhong = Convert.ToSingle(result);
                    }
                }

                // Hiển thị tổng tiền phòng lên TextBox tương ứng
                txbTongTienP.Text = tongTienPhong.ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tính tổng tiền phòng: " + ex.Message);
            }

            return tongTienPhong;
        }
        private float TinhTongTienDichVu(string idKH)
        {
            float tongTienDichVu = 0;

            // Kết nối đến CSDL và thực hiện truy vấn
            string query = @"
        SELECT SUM(dv.Gia * sddv.SoLuong) AS TongTienDichVu
        FROM tb_SuDungDV sddv
        JOIN tb_DichVu dv ON sddv.IDDV = dv.IDDV
        WHERE sddv.IDKH = @IDKH
        GROUP BY sddv.IDKH;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IDKH", idKH);

                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    tongTienDichVu = Convert.ToSingle(result);
                }
            }

            // Hiển thị tổng tiền dịch vụ lên TextBox tương ứng
            txbTongTienDV.Text = tongTienDichVu.ToString("N0");
            return tongTienDichVu;
        }
        private void TinhTongTienTongCong(string idKH)
        {
            float tongTienPhong = TinhTongTienPhong(idKH);
            float tongTienDichVu = TinhTongTienDichVu(idKH);

            // Tổng tiền tổng cộng
            float tongTienTongCong = tongTienPhong + tongTienDichVu;

            // Hiển thị tổng tiền tổng cộng lên TextBox
            txbTongTien.Text = tongTienTongCong.ToString("N0");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            updateSoNgay();
            string idKH = dataGridView1.CurrentRow.Cells["Mã Khách Hàng"].Value.ToString();

            // Tính và hiển thị tổng tiền dịch vụ
            TinhTongTienDichVu(idKH);

            // Tính và hiển thị tổng tiền phòng
            TinhTongTienPhong(idKH);

            // Tính và hiển thị tổng tiền tổng cộng
            TinhTongTienTongCong(idKH);


        }

        private void btnTimK_Click(object sender, EventArgs e)
        {
            string searchText = txbTimKiem.Text.Trim();

            if (!string.IsNullOrEmpty(searchText))
            {
                // Cập nhật lại truy vấn SQL để tìm theo tên khách hàng   
                string searchQuery = "SELECT dp.IDKH as 'Mã Khách Hàng', kh.HoTen as 'Tên Khách Hàng',p.TenP as 'Phòng Đã Đặt', dp.NgayDat as 'Ngày Đặt', lp.Gia as 'Giá Phòng', \r\nSTRING_AGG(ldv.TenDV + ' (' + CAST(dv.SoLuong AS VARCHAR) + ')', ', ') AS 'Dịch Vụ Đã Dùng' \r\nFROM tb_DatPhong dp \r\nJOIN tb_SuDungDV dv ON dp.IDKH = dv.IDKH \r\nJOIN tb_Phong p ON dp.IDPhong = p.IDPhong \r\nJOIN tb_LoaiPhong lp ON lp.IDLoaiP = p.IDLoaiP \r\nJOIN tb_DichVu ldv ON ldv.IDDV = dv.IDDV \r\nJOIN tb_KhachHang kh ON dp.IDKH = kh.IDKH\r\nWHERE kh.HoTen LIKE @SearchTerm\r\nGROUP BY dp.IDKH, p.TenP, dp.NgayDat, lp.Gia,HoTen;";

                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(searchQuery, con);
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@SearchTerm", searchText + "%");
                        DataTable dt = new DataTable();
                        dataAdapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                // Nếu ô tìm kiếm trống, tải lại dữ liệu ban đầu
                loadData();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbTongTien.Text))
            {
                MessageBox.Show("Vui lòng nhập tổng tiền trước khi thanh toán.");
                return;
            }

            CapNhatTrangThaiPhong(txbIDP.Text);
            SaveInvoice();
            NotifyDataChanged();
        }
        private void SaveInvoice()
        {
            string invoiceID = txbIDHD.Text;
            string customerID = txbIDKH.Text;
            string employeeID = txbIDNV.Text;
            DateTime invoiceDate = DateTime.Now;
            float totalAmount = float.Parse(txbTongTien.Text);
            string note = txbNote.Text;
            string DVdadung = txbDV.Text;
            string TenP = txbIDP.Text;

            if (note.Length > 500)
            {
                MessageBox.Show("Ghi chú không được quá 500 ký tự.");
                return;
            }

            string insertInvoiceQuery = @"INSERT INTO tb_HoaDon (IDHoaDon, IDKH, IDNV, NgayThanhToan, TongTien, Note,PhongDaDat,DVDaDung)
                                  VALUES (@InvoiceID, @CustomerID, @EmployeeID, @InvoiceDate, @TotalAmount, @Note,@TenP,@DVdadung)";

            string deleteDatPhongQuery = "DELETE FROM tb_DatPhong WHERE IDKH = @CustomerID AND IDPhong = (SELECT IDPhong FROM tb_Phong where TenP = @TenP)";
            string deleteSuDungDVQuery = "DELETE FROM tb_SuDungDV WHERE IDKH = @CustomerID ";    

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // Thêm thông tin hóa đơn vào bảng tb_HoaDon
                    SqlCommand cmdInsertInvoice = new SqlCommand(insertInvoiceQuery, con, transaction);
                    cmdInsertInvoice.Parameters.AddWithValue("@InvoiceID", invoiceID);
                    cmdInsertInvoice.Parameters.AddWithValue("@CustomerID", customerID);
                    cmdInsertInvoice.Parameters.AddWithValue("@EmployeeID", employeeID);
                    cmdInsertInvoice.Parameters.AddWithValue("@InvoiceDate", invoiceDate);
                    cmdInsertInvoice.Parameters.AddWithValue("@TotalAmount", totalAmount);
                    cmdInsertInvoice.Parameters.AddWithValue("@Note", note);
                    cmdInsertInvoice.Parameters.AddWithValue("@TenP", TenP);
                    cmdInsertInvoice.Parameters.AddWithValue("@DVdadung",DVdadung);
                    cmdInsertInvoice.ExecuteNonQuery();

                    // Xóa dữ liệu từ bảng tb_DatPhong liên quan đến hóa đơn hiện tại
                    SqlCommand deleteDatPhongCmd = new SqlCommand(deleteDatPhongQuery, con, transaction);
                    deleteDatPhongCmd.Parameters.AddWithValue("@CustomerID", customerID);
                    deleteDatPhongCmd.Parameters.AddWithValue("@InvoiceID", invoiceID);
                    deleteDatPhongCmd.Parameters.AddWithValue("@TenP", TenP);
                    deleteDatPhongCmd.ExecuteNonQuery();

                    // Xóa dữ liệu từ bảng tb_SuDungDV liên quan đến hóa đơn hiện tại
                    SqlCommand deleteSuDungDVCmd = new SqlCommand(deleteSuDungDVQuery, con, transaction);
                    deleteSuDungDVCmd.Parameters.AddWithValue("@CustomerID", customerID);
                    deleteSuDungDVCmd.Parameters.AddWithValue("@InvoiceID", invoiceID);
                    deleteSuDungDVCmd.ExecuteNonQuery();

                    // Commit transaction khi tất cả các thao tác thành công
                    transaction.Commit();
                    MessageBox.Show("Thanh Toán Thành Công");
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi xảy ra
                    transaction.Rollback();
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
                }
            }

            // Sau khi lưu thành công, cập nhật lại ID hóa đơn tiếp theo và tải lại dữ liệu
            DisplayInvoiceID();
            loadData();
            NotifyDataChanged();
        }
        private void CapNhatTrangThaiPhong(string tenPhong)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Câu truy vấn SQL để lấy IDPhong từ tên phòng
                string queryGetIDPhong = @"
            SELECT IDPhong
            FROM tb_Phong
            WHERE TenP = @TenPhong";

                SqlCommand cmdGetIDPhong = new SqlCommand(queryGetIDPhong, con);
                cmdGetIDPhong.Parameters.AddWithValue("@TenPhong", tenPhong);

                try
                {
                    con.Open();
                    // Lấy IDPhong từ tên phòng
                    string IDPhong = cmdGetIDPhong.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(IDPhong))
                    {
                        // Câu truy vấn SQL để cập nhật trạng thái phòng
                        string queryUpdatePhong = @"
                    UPDATE tb_Phong
                    SET IDTrangThai = '1'
                    FROM tb_Phong
                    INNER JOIN tb_DatPhong ON tb_Phong.IDPhong = tb_DatPhong.IDPhong
                    WHERE tb_DatPhong.IDPhong = @IDPhong";

                        SqlCommand cmdUpdatePhong = new SqlCommand(queryUpdatePhong, con);
                        cmdUpdatePhong.Parameters.AddWithValue("@IDPhong", IDPhong);

                        SqlTransaction transaction = con.BeginTransaction();
                        cmdUpdatePhong.Transaction = transaction;

                        // Thực hiện cập nhật trạng thái phòng
                        int rowsAffected = cmdUpdatePhong.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            transaction.Commit();
                            // MessageBox.Show("Đã cập nhật trạng thái phòng thành 'Trống'.");
                        }
                        else
                        {
                            transaction.Rollback();
                            // MessageBox.Show("Không có phòng nào để cập nhật trạng thái.");
                        }
                    }
                    else
                    {
                        // MessageBox.Show("Không tìm thấy phòng với tên đã cho.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật trạng thái phòng: {ex.Message}");
                }
                finally
                {
                    con.Close();
                }
            }

            loadData();
            NotifyDataChanged();
        }

        private void NotifyDataChanged()
        {
            FormDSPhong formPhong = Application.OpenForms["FormDSPhong"] as FormDSPhong;
            formPhong?.LoadlaiDataphong();
            FormDatPhong formDatPhong = Application.OpenForms["FormDatPhong"] as FormDatPhong;
            formDatPhong?.loadlaidataDatPhong();
            FormDatDV FormDV = Application.OpenForms["FormDatDV"] as FormDatDV;
            FormDV?.loadlaidataData();
            FormThongKe FormTT = Application.OpenForms["FormThu"] as FormThongKe;
            FormTT?.loadlaidataData();

        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Kiểm tra xem tổng tiền có hợp lệ không
            if (string.IsNullOrWhiteSpace(txbTongTien.Text))
            {
                MessageBox.Show("Vui lòng nhập tổng tiền trước khi in hóa đơn.");
                return;
            }

            // Khai báo biến số thực
            float totalAmount, TienPhong = 0, TienDV = 0;

            // Kiểm tra và phân tích dữ liệu tổng tiền
            if (!float.TryParse(txbTongTien.Text, out totalAmount))
            {
                MessageBox.Show("Tổng tiền không hợp lệ. Vui lòng kiểm tra lại.");
                return;
            }

            // Kiểm tra và phân tích dữ liệu tiền phòng nếu có giá trị
            if (!string.IsNullOrWhiteSpace(txbTongTienP.Text) && !float.TryParse(txbTongTienP.Text, out TienPhong))
            {
                MessageBox.Show("Tổng tiền phòng không hợp lệ. Vui lòng kiểm tra lại.");
                return;
            }

            // Kiểm tra và phân tích dữ liệu tiền dịch vụ nếu có giá trị
            if (!string.IsNullOrWhiteSpace(txbTongTienDV.Text) && !float.TryParse(txbTongTienDV.Text, out TienDV))
            {
                MessageBox.Show("Tổng tiền dịch vụ không hợp lệ. Vui lòng kiểm tra lại.");
                return;
            }

            // Lấy thông tin hóa đơn cần xem trước
            string invoiceID = txbIDHD.Text;
            string customerID = txbIDKH.Text;
            string employeeID = txbIDNV.Text;
            string note = txbNote.Text;
            string DVdadung = txbDV.Text;
            string TenP = txbIDP.Text;
            DateTime NgayDat = dtpNgayDat.Value;
            string songayo = txbSoNgayO.Text;

            // Tạo một thể hiện của FormHoaDon
            FormHoaDon formXemTruocHoaDon = new FormHoaDon();

            // Truyền dữ liệu hóa đơn sang FormHoaDon
            formXemTruocHoaDon.DisplayInvoice(invoiceID, customerID, NgayDat, songayo, employeeID, totalAmount, note, DVdadung, TenP, TienPhong, TienDV);

            // Hiển thị FormHoaDon lên
            formXemTruocHoaDon.Show();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
