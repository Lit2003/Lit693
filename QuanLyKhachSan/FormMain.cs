using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyKhachSan.Phong;
using QuanLyKhachSan.DichVu;
using QuanLyKhachSan.KhachHang;
using QuanLyKhachSan.ThongKe;

namespace QuanLyKhachSan
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        //mdi
        #region
        private void danhSáchPhòngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmpFrm = Application.OpenForms["FormDSPhong"];
            if (tmpFrm == null)
            {
                // Nếu form chưa được mở, khởi tạo và hiển thị form mới
                FormDSPhong frmP = new FormDSPhong();
                frmP.MdiParent = this;
                frmP.Show();
            }
            else
            {
                // Nếu form đã được mở, đưa form lên phía trước
                tmpFrm.Activate();
            }
        }

        private void loạiPhòngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmpFrm = Application.OpenForms["FormLoaiPhong"];
            if (tmpFrm == null)
            {
                FormLoaiPhong frmP = new FormLoaiPhong();
                frmP.MdiParent = this;
                frmP.Show();
            }
            else
            {
                tmpFrm.Activate();
            }
        }

        private void đặtPhòngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmpFrm = Application.OpenForms["FormDatPhong"];
            if (tmpFrm == null)
            {
                FormDatPhong frmP = new FormDatPhong();
                frmP.MdiParent = this;
                frmP.Show();
            }
            else
            {
                tmpFrm.Activate();
            }
        }

        private void danhSáchDịchVụToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmpFrm = Application.OpenForms["FormDanhSachDichVu"];
            if (tmpFrm == null)
            {
                FormDanhSachDichVu frmP = new FormDanhSachDichVu();
                frmP.MdiParent = this;
                frmP.Show();
            }
            else
            {
                tmpFrm.Activate();
            }
        }

        private void đặtDịchVụToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmpFrm = Application.OpenForms["FormDatDV"];
            if (tmpFrm == null)
            {
                FormDatDV frmP = new FormDatDV();
                frmP.MdiParent = this;
                frmP.Show();
            }
            else
            {
                tmpFrm.Activate();
            }
        }

        private void thôngTinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmpFrm = Application.OpenForms["FormThongTin"];
            if (tmpFrm == null)
            {
                FormThongTin frmP = new FormThongTin();
                frmP.MdiParent = this;
                frmP.Show();
            }
            else
            {
                tmpFrm.Activate();
            }
        }

        private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmpFrm = Application.OpenForms["FormThanhToan"];
            if (tmpFrm == null)
            {
                FormThanhToan frmP = new FormThanhToan();
                frmP.MdiParent = this;
                frmP.Show();
            }
            else
            {
                tmpFrm.Activate();
            }
        }
        private void thốngKêToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tmpFrm = Application.OpenForms["FormThu"];
            if (tmpFrm == null)
            {
                FormThongKe frmP = new FormThongKe();
                frmP.MdiParent = this;
                frmP.Show();
            }
            else
            {
                tmpFrm.Activate();
            }
        }


        #endregion

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }
    }
}
