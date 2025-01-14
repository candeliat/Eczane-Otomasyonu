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

namespace Eczane_otomasyonu
{
    public partial class Cari : Form
    {
        private Index mainForm;
        SqlConnection baglanti = new SqlConnection("Data Source=CAN\\SQLEXPRESS;Initial Catalog=DBEczane;Integrated Security=True");

        sqlbaglantisi bag = new sqlbaglantisi(); // sql bağlantı classımızdan nesne oluşturduk ve bağlantı için kullanacağız
        SqlCommand kmt = new SqlCommand(); //sql ekleme silme güncelleme listeleme işlemleri için sqlcommand nesnesi oluşturduk
        DataSet dtst = new DataSet();//datagridviewlere sql serverdaki tabloları aktarmak için kullanıyoruz.

        public Cari(Index form)
        {
            InitializeComponent();
            this.mainForm = form;

        }

        private void GeriPicBox_Click(object sender, EventArgs e)
        {
            if (mainForm.Panel2.Controls.Count > 0)
                mainForm.Panel2.Controls[0].Dispose();

            PSecimEkranı form = new PSecimEkranı(mainForm);
            mainForm.OpenFormInPanel(form);
        }
        public void satilanIlacSayisi()
        {
            //satılan ilaçla beraber hasta kaydı girildi için hasta sayısı satılan ilaç sayısını verecek
            kmt.Connection = bag.baglan(); //sql komutuna bağlantı oluşturduk
            kmt.CommandText = "SELECT COUNT(*)from HastaBilgileri";
            //hasta kayıt sayısını döndürecek fonksiyonu yazdık
            SqlDataReader oku;
            oku = kmt.ExecuteReader();
            if (oku.Read())
            {//hasta sayısını label6ya adeti aktardık
                label6.Text = oku[0].ToString() + " Adet";
            }

            oku.Dispose();


        }
        public void toplamPersonelSayisi()
        {

            kmt.Connection = bag.baglan();//sql komutuna bağlantı oluşturduk
            kmt.CommandText = "SELECT COUNT(DISTINCT TC_Kimlik_No)from Personel";
            //personel sayisini tc ile tekrarlanmicak şekilde kayıt sayısını döndürecek fonksiyonu yazdık
            SqlDataReader oku;
            oku = kmt.ExecuteReader();
            if (oku.Read())
            {//buldumuz veriyi llabel10'a aktardık
                label10.Text = oku[0].ToString() + " Personel";
            }

            oku.Dispose();


        }

        public void hastaSayisiToplam()
        {

            kmt.Connection = bag.baglan();//sql komutuna bağlantı oluşturduk
            kmt.CommandText = "SELECT COUNT(DISTINCT TC_Kimlik_No)from HastaBilgileri";
            //hasta sayisini tc ile tekrarlanmicak şekilde kayıt sayısını döndürecek fonksiyonu yazdık
            SqlDataReader oku;
            oku = kmt.ExecuteReader();
            if (oku.Read())
            {//buldumuz veriyi llabel9'a aktardık
                label9.Text = oku[0].ToString() + " Hasta";
            }

            oku.Dispose();


        }

        public void toplamvurulanAsi()
        {

            kmt.Connection = bag.baglan();//sql komutuna bağlantı oluşturduk
            kmt.CommandText = "SELECT COUNT(*)from AsiBilgileri";
            //asitablosundaki kayıt sayısını döndürecek fonksiyonu yazdık
            SqlDataReader oku;
            oku = kmt.ExecuteReader();
            if (oku.Read())
            {//buldumuz veriyi label8de yazdırdık
                label8.Text = oku[0].ToString() + " Adet";
            }

            oku.Dispose();


        }
        public void kazanilanUcret()
        {

            kmt.Connection = bag.baglan();//sql komutuna bağlantı oluşturduk
            kmt.CommandText = "SELECT sum(IlacBilgileri.Fiyati) FROM IlacBilgileri";
            SqlDataReader oku;//hasta tablosundaki alınan ilaçların fiyatını ilaç tablosundan fiyatlarını çekerek toplattık
            oku = kmt.ExecuteReader();
            if (oku.Read())
            {
                //toplanan veriyi label7 ye aktardık
                label7.Text = oku[0].ToString() + " TL";
            }

            oku.Dispose();


        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Cari_Load(object sender, EventArgs e)
        {
            //oluşturdumuz fonksiyonları form yüklenirken çağırdık
            satilanIlacSayisi();
            kazanilanUcret();
            toplamvurulanAsi();
            hastaSayisiToplam();
            toplamPersonelSayisi();
        }
    }
}
