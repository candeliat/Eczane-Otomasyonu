using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;


namespace Eczane_otomasyonu
{

    public partial class SiparisIslemleri : Form
    {
        private Index mainForm;

        public SiparisIslemleri(Index form)
        {
            InitializeComponent();
            this.mainForm = form;
            

        }

        
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public void mailGonder()
        {
            try
            {
                // Form girişlerini kontrol edin
                if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!IsValidEmail(textBox1.Text))
                {
                    MessageBox.Show("Geçersiz e-posta adresi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MailMessage mesajım = new MailMessage();
                SmtpClient istemci = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential("abdullahcandeliat1@gmail.com", "wnou wxzf lrda iqjl"),
                    EnableSsl = true
                };

                mesajım.To.Add(textBox1.Text);
                mesajım.From = new MailAddress("abdullahcandeliat1@gmail.com");
                mesajım.Subject = textBox2.Text;
                mesajım.Body = textBox3.Text;

                istemci.Send(mesajım);

                MessageBox.Show("E-posta başarıyla gönderildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"E-posta gönderiminde hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GeriPicBox_Click(object sender, EventArgs e)
        {
            if (mainForm.Panel2.Controls.Count > 0)
                mainForm.Panel2.Controls[0].Dispose();

            PSecimEkranı form = new PSecimEkranı(mainForm);
            mainForm.OpenFormInPanel(form);
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            mailGonder();
        }
    }
}
