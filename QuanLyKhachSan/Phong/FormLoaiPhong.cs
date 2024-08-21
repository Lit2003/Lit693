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
            CapNhatTrangThaiPhong();         
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
        void CapNhatTrangThaiPhong()
        {
            string tenloaiphong = txbTenP.Text; // Loại phòng VIP

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string queryUpdatePhong = "UPDATE tb_Phong SET IDTrangThai = '3',IDLoaiP = '1' WHERE IDLoaiP = (SELECT IDLoaiP FROM tb_LoaiPhong WHERE TenLoaiP = @TenLoaiP)";
                SqlCommand cmdUpdatePhong = new SqlCommand(queryUpdatePhong, con);
                cmdUpdatePhong.Parameters.AddWithValue("@TenLoaiP", tenloaiphong);

                try
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();
                    cmdUpdatePhong.Transaction = transaction;

                    // Thực hiện cập nhật trạng thái phòng
                    int rowsAffected = cmdUpdatePhong.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                       // MessageBox.Show($"Đã cập nhật trạng thái các phòng có loại phòng '{tenloaiphong}' thành 'Bảo trì'.");
                        // Có thể thêm thao tác load lại danh sách phòng sau khi cập nhật
                    }
                    else
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Không có phòng nào có loại phòng '{tenloaiphong}' để cập nhật.");
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
