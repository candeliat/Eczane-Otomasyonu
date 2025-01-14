using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eczane_otomasyonu
{
    public partial class Index : Form
    {

        public Index()
        {
            InitializeComponent();

            
        }

        public Panel Panel2
        {
            get { return panel2; } // `panel2` isimli paneli dışarıya açar.
        }


        

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        public void OpenFormInPanel(Form childForm)
        {
            // Paneldeki mevcut formu kapat
            if (panel2.Controls.Count > 0)
                panel2.Controls[0].Dispose();

            // Yeni formu ayarla
            childForm.TopLevel = false; // Formu üst düzey form olmaktan çıkar
            childForm.FormBorderStyle = FormBorderStyle.None; // Kenarlıkları kaldır
            //childForm.Dock = DockStyle.Fill; // Panele tam oturt

            int x = (panel2.Width - childForm.Width) / 2;
            int y = (panel2.Height - childForm.Height) / 2;
            childForm.Location = new Point(x, y);

            // Formu panele ekle ve göster
            panel2.Controls.Add(childForm);
            panel2.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {

        }

        private void panel2_SizeChanged(object sender, EventArgs e)
        {
            if(panel2.Controls.Count > 0)
    {
                Form childForm = (Form)panel2.Controls[0];
                int x = (panel2.Width - childForm.Width) / 2;
                int y = (panel2.Height - childForm.Height) / 2;
                childForm.Location = new Point(x, y);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       

        private void Form1_Load(object sender, EventArgs e)
        {
            Giris form2 = new Giris(this);
            OpenFormInPanel(form2);
        }
    }
}
