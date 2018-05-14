using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

namespace WpfAppAsync
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void buttonWebAsync_Click(object sender, RoutedEventArgs e)
        {
            // Call and await separately.
            //Task<int> getLengthTask = AccessTheWebAsync();
            //// You can do independent work here.
            textBlockResult.Text = "";
            int contentLength = await AccessTheWebAsync();

            textBlockResult.Text +=$"\r\nLength of {contentLength} bytes were downloaded at Time:{ System.DateTime.Now}.\r\n";

        }

        private void buttonDoOtherThings_Click(object sender, RoutedEventArgs e)
        {
            DoIndependentWork(" after launch await ");
        }

        async Task<int> AccessTheWebAsync()
        {
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync("http://msdn.microsoft.com");

            DoIndependentWork(" before launch await ");

            string urlContents = await getStringTask;
            return urlContents.Length;
        }

        void DoIndependentWork(string action)
        {
            textBlockResult.Text += $"\r\n\r\n{System.DateTime.Now}: Other work is done.{action}\r\n";
        }

        private void btnEndlessWork_Click(object sender, RoutedEventArgs e)
        {
            textBlockResult.Text += $"I will begin endless working at {System.DateTime.Now}...";
            
            while (true)
            {
                for(int i=0; i < 10000; i++) { }
                textBlockResult.Text += " * ";
            }

        }

        private async void buttonMultiAwait_Click(object sender, RoutedEventArgs e)
        {
            textBlockResult.Text = "";
            await CreateMultipleTasksAsync();
            Button btn = sender as Button;
            textBlockResult.Text += $"\r\n\r\n MultiAwait returned to {btn.GetType()} {btn.Content}";
        }

        private async Task CreateMultipleTasksAsync()
        {
            // Declare an HttpClient object, and increase the buffer size. The 
            // default buffer size is 65,536.
            HttpClient client =
                new HttpClient() { MaxResponseContentBufferSize = 1000000 };

            // Create and start the tasks. As each task finishes, DisplayResults  
            // displays its length.
            Task<int> download1 =
                ProcessURLAsync("http://msdn.microsoft.com", client);
            Task<int> download2 =
                ProcessURLAsync("http://msdn.microsoft.com/en-us/library/hh156528(VS.110).aspx", client);
            Task<int> download3 =
                ProcessURLAsync("http://msdn.microsoft.com/en-us/library/67w7t67f.aspx", client);

            // Await each task. 
            int length1 = await download1;
            int length2 = await download2;
            int length3 = await download3;

            int total = length1 + length2 + length3;

            // Display the total count for the downloaded websites.
            textBlockResult.Text +=
                string.Format("\r\n\r\nTotal bytes returned:  {0}\r\n", total);
        }


        async Task<int> ProcessURLAsync(string url, HttpClient client)
        {
            var byteArray = await client.GetByteArrayAsync(url);
            DisplayResults(url, byteArray);
            return byteArray.Length;
        }


        private void DisplayResults(string url, byte[] content)
        {
            // Display the length of each website. The string format  
            // is designed to be used with a monospaced font, such as 
            // Lucida Console or Global Monospace. 
            var bytes = content.Length;
            // Strip off the "http://".
            var displayURL = url.Replace("http://", " :");
            textBlockResult.Text +=string.Format("\n{2}:{0,-60} {1,8}", displayURL, bytes, System.DateTime.Now);
        }

    }
}
