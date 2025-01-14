using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Eczane_otomasyonu
{
    public partial class webVerileri : Form
    {
        private Index mainForm;

        public webVerileri(Index form)
        {
            InitializeComponent();
           
            this.mainForm = form;
            Haber();
            kur();
        }

        
        public void Haber()
        {
            Uri url = new Uri("https://www.trthaber.com/haber/saglik/");

            WebClient client = new WebClient();
            string html = client.DownloadString(url);
            HtmlAgilityPack.HtmlDocument dokuman = new HtmlAgilityPack.HtmlDocument();
            dokuman.LoadHtml(html);
            HtmlNodeCollection basliklar = dokuman.DocumentNode.SelectNodes("//a");

            foreach(var baslik in basliklar)
            {
                string link = baslik.Attributes["href"].Value;
                listBox1.Items.Add(duzeltMetin(baslik.InnerText));
                
            }
        }
        string duzeltMetin(string text)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(text);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public void kur()
        {
            string bugun = "http://www.tcmb.gov.tr/kurlar/today.xml";
            var xmldoc = new XmlDocument();
            xmldoc.Load(bugun);
            // Tarihi al
            DateTime tarih = Convert.ToDateTime(xmldoc.SelectSingleNode("//Tarih_Date").Attributes["Tarih"].Value);

            // USD kurunu al
            string USD = xmldoc.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteSelling").InnerXml;

            // Listeye ekle
            listBox2.Items.Add(string.Format("Tarih: {0} | USD: {1}", tarih.ToShortDateString(), USD));

            // USD kurunu al
            string EURO = xmldoc.SelectSingleNode("Tarih_Date/Currency[@Kod='EUR']/BanknoteSelling").InnerXml;

            // Listeye ekle
            listBox2.Items.Add(string.Format("Tarih: {0} | EURO: {1}", tarih.ToShortDateString(), USD));

            // USD kurunu al
            string PAUND = xmldoc.SelectSingleNode("Tarih_Date/Currency[@Kod='GBP']/BanknoteSelling").InnerXml;

            // Listeye ekle
            listBox2.Items.Add(string.Format("Tarih: {0} | PAUND: {1}", tarih.ToShortDateString(), USD));
        }


        private void GeriPicBox_Click(object sender, EventArgs e)
        {
            if (mainForm.Panel2.Controls.Count > 0)
                mainForm.Panel2.Controls[0].Dispose();

            PSecimEkranı form = new PSecimEkranı(mainForm);
            mainForm.OpenFormInPanel(form);
        }
    }
}
