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

namespace QuanLyKhachSan.Phong
{
    public partial class FormLoaiPhong : Form
    {
        public FormLoaiPhong()
        {
            InitializeComponent();
        }
        #region
        string connectionString = "Data Source=DESKTOP-JKB5K32\\SQLEXPRESS;Initial Catalog=KhachSan;Integrated Security=True;Encrypt=False";
        string query = "select IDLoaiP as ID,TenLoaiP as 'Tên Loại Phòng',TenLoaiP as 'Loại Phòng',Gia as 'Giá',SoGiuong as 'Số Giường',SoNguoi as 'Số Người' " +
            "from tb_LoaiPhong ";
        
        #endregion
        private void FormLoaiPhong_Load(object sender, EventArgs e)
        {
            loadDataLoaiP();
        }
        void loadDataLoaiP()
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
                }

            }
            bindingLP(dataGridView1);       
        }
        void bindingLP(DataGridView dtgv)
        {
            Binding bdID = new Binding("Text", dtgv.DataSource, "ID", true, DataSourceUpdateMode.OnPropertyChanged);
            txbID.DataBindings.Clear();
            txbID.DataBindings.Add(bdID);

            Binding bdTenLP = new Binding("Text", dtgv.DataSource, "Tên Loại Phòng", true, DataSourceUpdateMode.OnPropertyChanged);
            txbTenP.DataBindings.Clear();
            txbTenP.DataBindings.Add(bdTenLP);

            Binding bdGia = new Binding("Text", dtgv.DataSource, "Giá", true, DataSourceUpdateMode.OnPropertyChanged);
            bdGia.Format += (s, e) => { e.Value = string.Format("{0:N0}", e.Value); };
            bdGia.Parse += (s, e) => { e.Value = float.Parse(e.Value.ToString(), System.Globalization.NumberStyles.AllowThousands); };
            txbGia.DataBindings.Clear();
            txbGia.DataBindings.Add(bdGia);

            Binding bdSoNguoi = new Binding("Text", dtgv.DataSource, "Số Người", true, DataSourceUpdateMode.OnPropertyChanged);
            txbSoNguoi.DataBindings.Clear();
            txbSoNguoi.DataBindings.Add(bdSoNguoi);

            Binding bdSoG = new Binding("Text", dtgv.DataSource, "Số Giường", true, DataSourceUpdateMode.OnPropertyChanged);
            txbSoGiuong.DataBindings.Clear();
            txbSoGiuong.DataBindings.Add(bdSoG);

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Them();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            xoa();         
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            Sua();
        }
        void Them()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                int id = db.tb_LoaiPhong.Select(m => m.IDLoaiP).Max()+1;
          
                string ten = txbTenP.Text;
                if (db.tb_LoaiPhong.Select(t => t.TenLoaiP).Contains(ten))
                {
                    MessageBox.Show("Tên Loại Phòng Đã Tồn Tại");
                    return;
                }
                float Gia = float.Parse(txbGia.Text); 
                int SoGiuong = int.Parse(txbSoGiuong.Text);
                int SoNguoi = int.Parse(txbSoNguoi.Text);

                tb_LoaiPhong lp = new tb_LoaiPhong()
                {
                    IDLoaiP = id,
                    TenLoaiP = ten,
                    Gia = Gia,
                    SoGiuong = SoGiuong,
                    SoNguoi = SoNguoi,

                };
                db.tb_LoaiPhong.Add(lp);
                db.SaveChanges();
                MessageBox.Show("Thêm Loại Phòng Thành Công");
                NotifyDataChanged();
                loadDataLoaiP();
            }
        }
        void xoa()
        {

            string tenloaiphong = txbTenP.Text; 
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Kiểm tra phòng đã đặt
                int bookedRoomsCount = (int)new SqlCommand(@"
            SELECT COUNT(*) 
            FROM tb_Phong 
            WHERE IDLoaiP = (SELECT IDLoaiP FROM tb_LoaiPhong WHERE TenLoaiP = @TenLoaiP) 
            AND IDTrangThai = '2'", con).ExecuteScalar();

                if (bookedRoomsCount > 0)
                {
                    MessageBox.Show("Không thể cập nhật trạng thái vì có phòng đang được đặt.");
                    return;
                }

                // Cập nhật trạng thái phòng
                var cmdUpdatePhong = new SqlCommand(@"
            UPDATE tb_Phong 
            SET IDTrangThai = '3', IDLoaiP = '1' 
            WHERE IDLoaiP = (SELECT IDLoaiP FROM tb_LoaiPhong WHERE TenLoaiP = @TenLoaiP)", con);
                cmdUpdatePhong.Parameters.AddWithValue("@TenLoaiP", tenloaiphong);

                int rowsAffected = cmdUpdatePhong.ExecuteNonQuery();
                if (rowsAffected > 0)
                    MessageBox.Show("Cập nhật trạng thái phòng thành công.");
                else
                    MessageBox.Show($"Không có phòng nào có loại '{tenloaiphong}' để cập nhật.");
            }

            // Xóa loại phòng
            using (KhachSanEntities db = new KhachSanEntities())
            {
                int Id = int.Parse(txbID.Text);
                db.tb_LoaiPhong.Remove(db.tb_LoaiPhong.Find(Id));
                db.SaveChanges();
                MessageBox.Show("Xóa Loại Phòng Thành Công");
                loadDataLoaiP();
            }

            NotifyDataChanged();
            loadDataLoaiP();
        }
        void Sua()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                int id =int.Parse(txbID.Text);
                tb_LoaiPhong loaiphong = db.tb_LoaiPhong.Find(id);
                string tenlp = txbTenP.Text;
                float Gia = float.Parse(txbGia.Text);
                int SoGiuong = int.Parse(txbSoGiuong.Text);
                int SoNguoi = int.Parse(txbSoNguoi.Text);
                loaiphong.TenLoaiP = tenlp;
                loaiphong.Gia = Gia;
                loaiphong.SoNguoi = SoNguoi;
                loaiphong.SoGiuong = SoGiuong;
                db.SaveChanges();
                MessageBox.Show("Sửa Phòng Thành Công");
                NotifyDataChanged();
                loadDataLoaiP();
                
            }
        }
        private void NotifyDataChanged()
        {
            FormDSPhong formPhong = Application.OpenForms["FormDSPhong"] as FormDSPhong;
            formPhong?.LoadlaiDataphong();
            FormDatPhong formdatPhong = Application.OpenForms["FormDatPhong"] as FormDatPhong;
            formdatPhong?.loadlaidataDatPhong();
        }

    }
}
