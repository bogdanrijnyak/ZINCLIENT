using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace ZINCLI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const int Port = 53501;
        private const string SavePath = @"C:\LABA4\Response-1.xml";

        public async Task StartListeningAsync()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            while (true)
            {
                using (TcpClient client = await listener.AcceptTcpClientAsync())
                using (NetworkStream networkStream = client.GetStream())
                using (FileStream fileStream = File.Create(SavePath))
                {
                    await networkStream.CopyToAsync(fileStream);
                }

                // Зчитуємо текст з XML
                XDocument doc = XDocument.Load(SavePath);
                string text = doc.Root.Value;

                // Відображаємо MessageBox з текстом з XML
                MessageBox.Show(text);
            }
        }

        public void StartListening()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            while (true)
            {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream networkStream = client.GetStream())
                using (FileStream fileStream = File.Create(SavePath))
                {
                    networkStream.CopyTo(fileStream);
                }

                // Зчитуємо текст з XML
                XDocument doc = XDocument.Load(SavePath);
                string text = doc.Root.Value;

                // Відображаємо MessageBox з текстом з XML
                MessageBox.Show(text);
            }
        }
        public void SendFileToServer(string fileName, string serverIP, int port)
        {
            try
            {
                using (TcpClient client = new TcpClient(serverIP, port))
                {
                    using (NetworkStream networkStream = client.GetStream())
                    {
                        using (FileStream fileStream = File.OpenRead(fileName))
                        {
                            fileStream.CopyTo(networkStream);
                        }
                    }
                }

                MessageBox.Show("Файл успішно відправлено на сервер.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при відправці файлу: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Розділити слова в textBox1 через кому
            string[] words = textBox1.Text.Split(',');

            // Створити новий XML документ
            XDocument doc = new XDocument(
                new XElement("words",
                    new XAttribute("count", words.Length),
                    from word in words
                    select new XElement("word", word.Trim())
                )
            );

            // Зберегти XML документ в C:\LABA4 під назвою Request-1.xml
            string path = @"C:\LABA4\Request-1.xml";
            doc.Save(path);

            MessageBox.Show($"XML документ збережено в {path}");
            SendFileToServer(@"C:\LABA4\Request-1.xml", "14.88.14.87", 53500);
            StartListening();

        }
    }
}
