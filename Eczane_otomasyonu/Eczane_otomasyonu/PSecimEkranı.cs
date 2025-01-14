using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

using Eczane_otomasyonu;

namespace Eczane_otomasyonu
{
    public partial class PSecimEkranı : Form
    {
        public PSecimEkranı(Index form)
        {
            InitializeComponent();
            this.mainForm = form;
            HavaDurumu();
            Zaman();
        }

        private Index mainForm;

        public void Zaman()
        {
            DateTime tarihSaat = DateTime.Now; // Yerel tarih ve saat
            label4.Text = tarihSaat.ToString(); // Format: 13.01.2025 14:23:45
        }

        public void HavaDurumu()
        {
            string api = "d85e027adf65beb742e7165a99bcd45e";
            string connection = "https://api.openweathermap.org/data/2.5/weather?q=Elazığ&mode=xml&lang=tr&units=metric&APPID=" + api;
            try { 
            XDocument weather = XDocument.Load(connection);
            var temp = weather.Descendants("temperature").ElementAt(0).Attribute("value").Value;
            var weatherState = weather.Descendants("weather").ElementAt(0).Attribute("value").Value;

            label2.Text = temp.ToString() + " C°";
            label3.Text = weatherState.ToUpper() ;
            }
            catch
            {
                label2.Text = "İnternete Bağlı Değilsiniz!";
                label3.Text = "İnternete Bağlı Değilsiniz!";
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


            if (mainForm.Panel2.Controls.Count > 0)
                mainForm.Panel2.Controls[0].Dispose();

            Giris form = new Giris(mainForm);
            mainForm.OpenFormInPanel(form);
        }

        

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            SiparisIslemleri form = new SiparisIslemleri(mainForm);
            this.Hide();
            mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Giris.Giris_Tipi == "Patron")
            {
                PersonelIslemleri form = new PersonelIslemleri(mainForm);
                this.Hide();
                mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
            }
            else
            {
                MessageBox.Show("Erişim Engeli");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            
                HastaKayitIslemleri form = new HastaKayitIslemleri(mainForm);
                this.Hide();
            mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
          

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Giris.Giris_Tipi == "Patron")
            {
                Cari form = new Cari(mainForm);
                this.Hide();
                mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
            }
            else
            {
                MessageBox.Show("Erişim Engeli");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HastaTakip form = new HastaTakip(mainForm);
            this.Hide();
            mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
        }

        private void button5_Click(object sender, EventArgs e)
        {
            İlacBilgileri form = new İlacBilgileri(mainForm);
            this.Hide();
            mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AsiTakip form = new AsiTakip(mainForm);
            this.Hide();
            mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
        }

        private void button6_Click(object sender, EventArgs e)
        {
            webVerileri form = new webVerileri(mainForm);
            this.Hide();
            mainForm.OpenFormInPanel(form); // Formu panel2 içinde aç
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }
    }
}
