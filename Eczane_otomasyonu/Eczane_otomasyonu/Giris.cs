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
    public partial class Giris : Form
    {
        private Index mainForm;
        public static string Giris_Tipi;


        public string giris_tipi;


        public Giris(Index form)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.mainForm = form;
           
        }

        public void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        

        private void checkboxShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !checkboxShowPass.Checked;
        }

        

        private void CalisanRadioBttn_CheckedChanged(object sender, EventArgs e)
        {

        }

       


        

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            string rol = comboBox1.SelectedItem?.ToString();
            string kullaniciAdi = txtUsername.Text;
            string sifre = txtPassword.Text;

            if (rol == "Patron" && sifre == "123456789" && kullaniciAdi == "abdullahcandeliat")
            {

                PSecimEkranı form = new PSecimEkranı(mainForm);
                this.Hide();
                mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
                Giris_Tipi = "Patron";

            }
            else if (rol == "Çalışan" && sifre == "2" && kullaniciAdi == "2")
            {

                PSecimEkranı form = new PSecimEkranı(mainForm);
                this.Hide();
                mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
                Giris_Tipi = "Çalışan";

            }

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
        }
    }
}
