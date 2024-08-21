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

namespace QuanLyKhachSan.KhachHang
{
    public partial class FormThongTin : Form
    {
        public FormThongTin()
        {
            InitializeComponent();
            this.Name = "FormThongTin";
        }
  
        private void FormThongTin_Load(object sender, EventArgs e)
        {
            loadData();
        }

        #region
        string connectionString = "Data Source=DESKTOP-JKB5K32\\SQLEXPRESS;Initial Catalog=KhachSan;Integrated Security=True;Encrypt=False";
        string query = "select IDKH as 'ID', HoTen as 'Họ Tên', NgaySinh as 'Ngày Sinh', GioiTinh as'Giới Tính',SDT,CCCD from tb_KhachHang";
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
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return;
                }
            binding(dataGridView1);
        }
        void binding(DataGridView dtgv)
        {
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = dtgv.DataSource;
            Binding bdID = new Binding("Text", dtgv.DataSource, "ID", true, DataSourceUpdateMode.OnPropertyChanged);
            txbID.DataBindings.Clear();
            txbID.DataBindings.Add(bdID);

            Binding bdTenKH = new Binding("Text", dtgv.DataSource, "Họ Tên", true, DataSourceUpdateMode.OnPropertyChanged);
            txbTenKH.DataBindings.Clear();
            txbTenKH.DataBindings.Add(bdTenKH);

            Binding bdNgaySinh = new Binding("Value", dtgv.DataSource, "Ngày Sinh", true, DataSourceUpdateMode.OnPropertyChanged);
            dtpNgaySinh.DataBindings.Clear();
            dtpNgaySinh.DataBindings.Add(bdNgaySinh);

            Binding bdGioiTinh = new Binding("Text", dtgv.DataSource, "Giới Tính", true, DataSourceUpdateMode.OnPropertyChanged);
            txbGioiTinh.DataBindings.Clear();
            txbGioiTinh.DataBindings.Add(bdGioiTinh);

            Binding bdSDT = new Binding("Text", dtgv.DataSource, "SDT", true, DataSourceUpdateMode.OnPropertyChanged);
            txbSDT.DataBindings.Clear();
            txbSDT.DataBindings.Add(bdSDT);

            Binding bdCCCD = new Binding("Text", dtgv.DataSource, "CCCD", true, DataSourceUpdateMode.OnPropertyChanged);
            txbCCCD.DataBindings.Clear();
            txbCCCD.DataBindings.Add(bdCCCD);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            Sua();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            Them();
        }
        void Them()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                var maxId = db.tb_KhachHang.Select(m => m.IDKH).Max();
                string id = (maxId == null) ? "1" : (int.Parse(maxId) + 1).ToString();
                string tenkh = txbTenKH.Text;
                DateTime ngaysinh = dtpNgaySinh.Value;
                string GioiTinh = txbGioiTinh.Text;
                string SDT = txbSDT.Text;
                string CCCD = txbCCCD.Text;

                tb_KhachHang p = new tb_KhachHang()
                {
                    IDKH = id,
                    HoTen = tenkh,
                    NgaySinh = ngaysinh,
                    GioiTinh = GioiTinh,
                    SDT = SDT,
                    CCCD = CCCD,


                };
                db.tb_KhachHang.Add(p);
                db.SaveChanges();
                MessageBox.Show("Thêm Khách Hàng Thành Công");
                loadData();
            }
        }
        void Xoa()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                string id = txbID.Text;

                // Tìm khách hàng
                var khachHang = db.tb_KhachHang.Find(id);
                if (khachHang == null)
                {
                    MessageBox.Show("Khách hàng không tồn tại.");
                    return;
                }

                // Xóa các bản ghi liên quan trong bảng tb_DatPhong
                var datPhongLienQuan = db.tb_DatPhong.Where(dp => dp.IDKH == id).ToList();
                if (datPhongLienQuan.Count > 0)
                {
                    db.tb_DatPhong.RemoveRange(datPhongLienQuan);
                }

                // Xóa khách hàng
                db.tb_KhachHang.Remove(khachHang);

                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Xóa Khách Hàng Thành Công");           
                    loadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        void Sua()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                string id = txbID.Text;
                string hoten = txbTenKH.Text;
                string gioitinh = txbGioiTinh.Text;
                string SDT = txbSDT.Text;
                string CCCD = txbCCCD.Text;
                DateTime ngaysinh = dtpNgaySinh.Value;
                tb_KhachHang KH = db.tb_KhachHang.Find(id); 
                KH.NgaySinh = ngaysinh;
                KH.GioiTinh = gioitinh;
                KH.SDT = SDT;
                KH.CCCD = CCCD;
                KH.HoTen = hoten;
                
                db.SaveChanges();
                MessageBox.Show("Sửa Thông Tin Khách Hàng Thành Công");
                loadData();
            }
        }
        void ResetID()
        {
            using (KhachSanEntities db = new KhachSanEntities())
            {
                var allKH = db.tb_KhachHang.OrderBy(dv => dv.IDKH).ToList();
                int newId = 1;

                foreach (var khachhang in allKH)
                {
                    khachhang.IDKH = newId.ToString();
                    newId++;
                }

                db.SaveChanges();
            }
        }
    }
}
