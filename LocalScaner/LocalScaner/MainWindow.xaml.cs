using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace LocalScaner
{
   
    public class ARP
    {
        public string IP { get; set; }
        public string MAC { get; set; }
        public string Type { get; set; }
        public string Vendor { get; set; }
        public string Device { get; set; }
        public string Num_port { get; set; }
        public string MAC_Host { get; set; }

    }
    
    public class ARP_ans
    {
        public string IP { get; set; }
        public string MAC { get; set; }
        public string Vendor { get; set; }
        public string Device { get; set; }
        public string Num_port { get; set; }
        public string MAC_Host { get; set; }
        public string Date { get; set; }
        public string Mark { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summ
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            
        }
        public class InputBox
        {

            Window Box = new Window();//window for the inputbox
            FontFamily font = new FontFamily("Tahoma");//font for the whole inputbox
            int FontSize = 30;//fontsize for the input
            StackPanel sp1 = new StackPanel();// items container
            string title = "InputDialog";//title as heading
            string boxcontent;//title
            string defaulttext = "Введите текст...";//default textbox content
            string errormessage = "Некорректные данные";//error messagebox content
            string errortitle = "Ошибка";//error messagebox heading title
            string okbuttontext = "OK";//Ok button content
            Brush BoxBackgroundColor = Brushes.Gray;// Window Background
            Brush InputBackgroundColor = Brushes.Ivory;// Textbox Background
            bool clicked = false;
            TextBox input = new TextBox();
            Button ok = new Button();
            bool inputreset = false;

            public InputBox(string content)
            {
                try
                {
                    boxcontent = content;
                }
                catch { boxcontent = "Ошибка!"; }
                windowdef();
            }

            public InputBox(string content, string Htitle, string DefaultText)
            {
                try
                {
                    boxcontent = content;
                }
                catch { boxcontent = "Ошибка!"; }
                try
                {
                    title = Htitle;
                }
                catch
                {
                    title = "Ошибка!";
                }
                try
                {
                    defaulttext = DefaultText;
                }
                catch
                {
                    DefaultText = "Ошибка!";
                }
                windowdef();
            }

            public InputBox(string content, string Htitle, string Font, int Fontsize)
            {
                try
                {
                    boxcontent = content;
                }
                catch { boxcontent = "Ошибка!"; }
                try
                {
                    font = new FontFamily(Font);
                }
                catch { font = new FontFamily("Tahoma"); }
                try
                {
                    title = Htitle;
                }
                catch
                {
                    title = "Ошибка!";
                }
                if (Fontsize >= 1)
                    FontSize = Fontsize;
                windowdef();
            }

            private void windowdef()// window building - check only for window size
            {
                Box.Height = 200;// Box Height
                Box.Width = 300;// Box Width
                Box.Background = BoxBackgroundColor;
                Box.Title = title;
                Box.Content = sp1;
                Box.Closing += Box_Closing;
                TextBlock content = new TextBlock();
                content.TextWrapping = TextWrapping.Wrap;
                content.Background = null;
                content.HorizontalAlignment = HorizontalAlignment.Center;
                content.Text = boxcontent;
                content.FontFamily = font;
                content.FontSize = FontSize;
                sp1.Children.Add(content);

                input.Background = InputBackgroundColor;
                input.FontFamily = font;
                input.FontSize = FontSize;
                input.HorizontalAlignment = HorizontalAlignment.Center;
                input.Text = defaulttext;
                input.MinWidth = 200;
                input.MouseEnter += input_MouseDown;
                sp1.Children.Add(input);
                ok.Width = 70;
                ok.Height = 30;
                ok.Click += ok_Click;
                ok.Content = okbuttontext;
                ok.HorizontalAlignment = HorizontalAlignment.Center;
                sp1.Children.Add(ok);

            }

            void Box_Closing(object sender, System.ComponentModel.CancelEventArgs e)
            {
                if (!clicked)
                    e.Cancel = true;
            }

            private void input_MouseDown(object sender, MouseEventArgs e)
            {
                if ((sender as TextBox).Text == defaulttext && inputreset == false)
                {
                    (sender as TextBox).Text = null;
                    inputreset = true;
                }
            }

            void ok_Click(object sender, RoutedEventArgs e)
            {
                clicked = true;
                if (input.Text == defaulttext || input.Text == "")
                    MessageBox.Show(errormessage, errortitle);
                else
                {
                    Box.Close();
                }
                clicked = false;
            }

            public string ShowDialog()
            {
                Box.ShowDialog();
                return input.Text;
            }
        }
        MySqlConnection conn;
        List<ARP> arpTable = new List<ARP> { };
        List<ARP_ans> global_ans = new List<ARP_ans> { };

        public string MySqlRequest(string sql)
        {
            //ConnectToMysql();
            MySqlCommand sqlCom = new MySqlCommand(sql, conn);
            sqlCom.ExecuteNonQuery();
           MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            string ans=null;// контейнер для формирования данных из запроса
            var myData = dt.Select();
            for (int i = 0; i < myData.Length; i++)
            {
                for (int j = 0; j < myData[i].ItemArray.Length; j++)
                {
                    ans =   Convert.ToString(myData[i][j]);//формирования контейнера
                }
            }return ans;//возврат контейнера
        }
        public void MySqlRequest_void(string sql, List<ARP_ans> ans)
        {
            //ConnectToMysql();
            MySqlCommand sqlCom = new MySqlCommand(sql, conn);
            sqlCom.ExecuteNonQuery();
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            // контейнер для формирования данных из запроса
            var myData = dt.Select();
            for (int i = 0; i < myData.Length; i++)
            {
               
                    ans.Add(new ARP_ans {IP= Convert.ToString(myData[i][0]), MAC = Convert.ToString(myData[i][1]), Vendor = Convert.ToString(myData[i][2]), Device = Convert.ToString(myData[i][3]), Num_port = Convert.ToString(myData[i][4]),MAC_Host= Convert.ToString(myData[i][5]),Date= Convert.ToString(myData[i][6]), Mark = Convert.ToString(myData[i][7]) });

                


            }
        }

        public void ConnectToMysql()
        {
            string serverName = "localhost"; // Адрес сервера 
            string userName = "root"; // Имя пользователя
            string dbName = "scan_db"; //Имя базы данных
            string port = "3306"; // Порт для подключения
            string password = ""; // Пароль для подключения
            string connStr = "server=" + serverName +
                ";user=" + userName +
                ";database=" + dbName +
                ";port=" + port +
                ";password=" + password + ";";
            conn = new MySqlConnection(connStr);
            conn.Open(); 
        }
        public bool CheckVendor(string vendor)
        {
            bool code = false;
            try
            {
                ConnectToMysql();
            }
            catch (Exception) { code = false; }
            string ans;
            string sql = "SELECT `ID` FROM `vendors` WHERE `vendors`.`Vendor`='"+vendor+"'";
            try
            {
                ans = MySqlRequest(sql);
            }
            catch (Exception) { ans = null; }
            if (ans == null)
            {
                code = false;
            }
            else code = true;
            return code;
        }
        public async Task<List<ARP>> ButtonAction(List <ARP> x)
        {
            //List <ARP> arpTable = new List<ARP> { };
            var arpStream = ExecuteCommandLine("arp", "-a");

            //Удаляем первые три строки, т.к они содержат
            //пустую строку
            //имя интерфейса
            //заголовки столбцов
            for (int i = 0; i < 3; i++)
            {
                arpStream.ReadLine();
            }

            //Циклически проходим по входному потоку
            //Пока функция EndOfStream не вернет значение true
            //указывающая, что текущая позиция потока
            //находится в конце потока
            while (!arpStream.EndOfStream)
            {
                //Получаем одну строку из текущего потока
                var line = arpStream.ReadLine().Trim();

                //Так как между столбцами есть несколько пробелов
                //их необходимо сократить до одного
                while (line.Contains("  "))
                {
                    line = line.Replace("  ", " ");
                }

                //Чтобы распределить полученные данные по столбцам таблицы их
                // необходимо разделить с помощью метода Split
                // который возвращает массив, элементы которого содержат
                //подстроки данного экземпляра, разделенные одним или более
                //знаками указанных в его значении.
                var parts = line.Split(' ');

                //Если значение первого столбца пустое, значит
                //данную строку необходимо пропустить
                
                if (parts[0].Trim() != string.Empty)
                {
                    if (parts[2].Trim() == "динамический")
                    {
                        Thread.Sleep(1000);
                        string n_mac = parts[1].Trim().Replace('-', ':');//замена символовок в строке мак адреса
                        string l_device=null;
                        string l_vendor =  await ConnectAsync(n_mac);// обращение к апи для получения производителя
                        string sql = "SELECT `Devices` FROM `vendors` WHERE `vendors`.`Vendor`='"+l_vendor+"'";
                        if (CheckVendor(l_vendor))
                        {
                            l_device = MySqlRequest(sql);
                        }
                        else { l_device = "Нет информации"; }
                        x.Add(new ARP { IP = parts[0].Trim(), MAC =n_mac , Type = parts[2].Trim(),Vendor= l_vendor, Device=l_device });//заполнение полей класса АРП

                    }
                }

            }return x;//возвращаем массив класса АРП
            

        }
       
        
        public static async Task<string> ConnectAsync(string mac)// работа АПИ для определения вендора
        {
            WebRequest request = WebRequest.Create("https://api.macvendors.com/" + mac);
            request.Method = "GET";
            WebResponse response = await request.GetResponseAsync();
            string answer = string.Empty;
            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    answer = await reader.ReadToEndAsync();
                    response.Close();
                    return answer;
                }
            }
            
            
        }


        public static StreamReader ExecuteCommandLine(String file, String arguments = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            //Задаем значение, указывающее необходимость запускать
            //процесс в новом окне.
            startInfo.CreateNoWindow = true;

            //Устанавливаем скрытый стиль окна. Окно может быть видимым или скрытым.
            //Система отображает скрытое окно, не прорисовывая его.
            //Если окно скрыто, оно эффективно отключено.
            //Скрытое окно может обрабатывать сообщения от системы или
            //от других окон, но не может обрабатывать ввод от пользователя
            //или отображать вывод. Часто, приложение может держать новое окно
            //скрытым, пока приложение определит внешний вид окна, а затем
            //сделать стиль окна Normal.
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            //Задаем значение, указывающее, что не нужно использовать
            //оболочку операционной системы для запуска процесса.
            startInfo.UseShellExecute = false;

            //Задаем значение, указывающее необходимость записывать выходные
            //данные приложения в поток System.Diagnostics.Process.StandardOutput.
            startInfo.RedirectStandardOutput = true;

            //Задаем приложение для запуска.
            startInfo.FileName = file;

            //Задаем набор аргументов командной строки, используемых при
            //запуске приложения.
            startInfo.Arguments = arguments;

            //Задаем предпочтительную кодировку для стандартного вывода.
            startInfo.StandardOutputEncoding = Encoding.GetEncoding(866);

            //Запускам ресурс процесса, с указанными выше параметрами и связываем
            //ресурс с новым компонентом System.Diagnostics.Process.
            Process process = Process.Start(startInfo);

            //Возвращаем System.IO.StreamReader, который может использоваться
            //для чтения потока стандартного вывода приложения.
            return process.StandardOutput;
        }
        public void btn_db_action()
        {
            try
            {
                ConnectToMysql();
                string l_ip = null;
                string l_mac = null;
                string l_vendor = null;
                string l_device = null;
                string l_hn_port = null;
                string l_host = null;
                string l_mark = null;
                int ind;
                l_mark = new InputBox("Введите короткий комментарий").ShowDialog();

                for (int i = 0; i < arpTable.Count; i++)
                {

                    var local = arpTable.ElementAt(i);
                    l_ip = local.IP;
                    l_mac = local.MAC;
                    l_vendor = local.Vendor;
                    l_device = local.Device;
                    l_hn_port = "test";
                    l_host = "test";
                    if (!CheckVendor(l_vendor))
                    {
                        string l_sql = "INSERT IGNORE INTO `vendors`(`Vendor`, `Devices`) VALUES('" + l_vendor + "','Не указано')";
                        string l_ans = MySqlRequest(l_sql);

                    }
                    string sql = "SELECT `ID` FROM `vendors` WHERE `vendors`.`Vendor`='" + l_vendor + "'";
                    ind = Convert.ToInt32(MySqlRequest(sql));
                    string main_sql = "INSERT IGNORE INTO `scan_tab`(`IP`, `MAC`, `Vendor`, `Device`,`H_n_port`,`Host`, `Date`, `Mark`) VALUES ('" + l_ip + "','" + l_mac + "'," + ind + ",'" + l_device + "','" + l_hn_port + "','" + l_host + "',CURRENT_DATE,'" + l_mark + "')";
                    string ans = MySqlRequest(main_sql);
                }
                MessageBox.Show("Данные внесены");
            }
            catch (Exception) { MessageBox.Show("Подключение к серверу отсутсвует!\nНевозможно внести данные в БД!");}
            
        }
        private  async void btn_scan_Click(object sender, RoutedEventArgs e)
        {

            
            
            
            try
            {
                arpTable = await ButtonAction(arpTable);
            }
            catch (Exception) { MessageBox.Show("Ошибка!"); }
            if (arpTable.Count != 0)
            {
                ArpPanel.ItemsSource = arpTable;
            }else MessageBox.Show("Найти устройства не удалось!\nВозможно у вас отсутсвует подключение к сети или в Вашей сети нет активных устройств.");
        }

        private void Btn_clear_Click(object sender, RoutedEventArgs e)
        {
            ArpPanel.ItemsSource = "";
            arpTable.Clear();
        }

        private void Btn_db_Click(object sender, RoutedEventArgs e)
        {
            if (arpTable.Count == 0)
            {
                MessageBox.Show("Нет данных для записи!\nВыполните сканирование!");
            }
            else
            {

                btn_db_action();

            }
        }
        
        private void Btn_all_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectToMysql();
                string sql = "SELECT `scan_tab`.`IP`,`scan_tab`.`MAC`,`vendors`.`Vendor`,`scan_tab`.`Device`,`scan_tab`.`H_n_port`,`scan_tab`.`Host`,`scan_tab`.`Date`,`scan_tab`.`Mark` FROM `scan_tab` JOIN `vendors` ON `scan_tab`.`Vendor`=`vendors`.`ID` ";
                
                MySqlRequest_void(sql, global_ans);
                scan_dg.ItemsSource = global_ans;
            }
            catch (Exception) { MessageBox.Show("Подключение к серверу отсутствует, проверьте наличие соединения с сетью Интернет"); }

            
        }

        private void Btn_export_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            
            if (scan_dg.Columns.Count != 0)
            {
                app.Visible = true;
                worksheet = workbook.Sheets["Лист1"];
                worksheet = workbook.ActiveSheet;
                for (int i = 1; i < scan_dg.Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = scan_dg.Columns[i - 1].Header;
                }

                var t = scan_dg.Items;



                for (int i = 1; i <t.Count-1; i++) {
                    var dg = global_ans[i];
                    worksheet.Cells[i+1, 1] = dg.IP;
                    worksheet.Cells[i+1, 2] = dg.MAC;
                    worksheet.Cells[i+1, 3] = dg.Vendor;
                    worksheet.Cells[i+1, 4] = dg.Device;
                    worksheet.Cells[i+1, 5] = dg.Num_port;
                    worksheet.Cells[i+1, 6] = dg.MAC_Host;
                    worksheet.Cells[i+1, 7] = dg.Date;
                    worksheet.Cells[i+1, 8] = dg.Mark;

                }
            }
            else MessageBox.Show("Нет данных для экспорта. Произведите выборку!");
                
            
        }
    }
}
