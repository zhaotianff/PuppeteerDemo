using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Browser browser;

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //默认会从https://storage.googleapis.com下载
            //如果没有科学上网，无法访问
            //await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            //可以通过下面的方式配置为从华为云镜像下载
            BrowserFetcherOptions browserFetcherOptions = new BrowserFetcherOptions();
            browserFetcherOptions.Host = "https://repo.huaweicloud.com";
            await new BrowserFetcher(browserFetcherOptions).DownloadAsync(BrowserFetcher.DefaultRevision);

            browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            //如果await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);执行不成功
            //可以手动访问https://commondatastorage.googleapis.com/chromium-browser-snapshots/index.html?prefix=Win_x64/，下载Chromium浏览器，并解压到指定位置
            //再通过以下代码初始化
            /*
             * LaunchOptions options = new LaunchOptions();
            options.Headless = true;
            options.DefaultViewport = null;
            //忽略证书错误
            options.IgnoreHTTPSErrors = true;

            //chromePath就是下载的Chromium浏览器解压的位置          
             options.ExecutablePath = chromePath;

            browser = await Puppeteer.LaunchAsync(options);
            */

            



            var page = await browser.NewPageAsync();
            await page.GoToAsync(this.textBox1.Text);
            var html = await page.GetContentAsync();
            this.richTextBox1.Text = html;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //获取最后的标签页
            var page = (await browser.PagesAsync()).Last();
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"screenshot.jpg");

            //设置截图选项
            ScreenshotOptions screenshotOptions = new ScreenshotOptions();
            //screenshotOptions.Clip = new PuppeteerSharp.Media.Clip() { Height = 0, Width = 0, X = 0, Y = 0 };//设置截剪区域
            screenshotOptions.FullPage = true; //是否截取整个页面
            screenshotOptions.OmitBackground = false;//是否使用透明背景，而不是默认白色背景
            screenshotOptions.Quality = 100; //截图质量 0-100（png不可用）
            screenshotOptions.Type = ScreenshotType.Jpeg; //截图格式 
            await page.ScreenshotAsync(path,screenshotOptions);
            MessageBox.Show($"截图已经保存至{path}");
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            //获取最后的标签页
            var page = (await browser.PagesAsync()).Last();
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "screenshot.pdf");

            //设置PDF选项
            PdfOptions pdfOptions = new PdfOptions();
            pdfOptions.DisplayHeaderFooter = false; //是否显示页眉页脚
            pdfOptions.FooterTemplate = "";   //页脚文本
            pdfOptions.Format = new PuppeteerSharp.Media.PaperFormat(8.27m,11.69m);  //pdf纸张格式 英寸为单位 
            pdfOptions.HeaderTemplate = "";   //页眉文本
            pdfOptions.Landscape = false;     //纸张方向 false-垂直 true-水平 
            pdfOptions.MarginOptions = new PuppeteerSharp.Media.MarginOptions() { Bottom = "0px", Left = "0px", Right = "0px", Top = "0px" }; //纸张边距，需要设置带单位的值，默认值是None
            pdfOptions.Scale = 1m;            //PDF缩放，从0-1
            await page.PdfAsync(path, pdfOptions);
            MessageBox.Show($"PDF已经保存至{path}");
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //dispose browser
            if (browser != null)
            {
                await browser.CloseAsync();
                browser.Dispose();
            }
        }
    }
}
