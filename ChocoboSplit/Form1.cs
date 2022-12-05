//画面キャプチャ
//https://dobon.net/vb/dotnet/graphics/screencapture.html

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChocoboSplit
{
    public partial class Form1 : Form
    {
        //インスタンス
        private KeyboardHook kh = new KeyboardHook();

        private async void CaptureScreenTask()
        {
            //ストップウォッチ
            Stopwatch sw = new Stopwatch();

            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;

            //切り抜き後のサイズ
            Rectangle rect = new Rectangle(w / 8, 0, h * 4 / 3, h);

            //スクショの座標
            Point p = new Point(0, 0);

            //Bitmapの作成
            Bitmap bmp = new Bitmap(w, h);
            Bitmap cut = new Bitmap(rect.Width, rect.Height);

            await Task.Run(() =>
            {
                while (true)
                {
                    //計測開始
                    sw.Start();

                    //Graphicsの作成
                    Graphics g = Graphics.FromImage(bmp);

                    //画面全体をスクショ
                    g.CopyFromScreen(p, p, bmp.Size);

                    //解放
                    g.Dispose();

                    //トリミング
                    cut = bmp.Clone(rect, bmp.PixelFormat);

                    //スクショボタンが押されてた場合
                    if (kh.screenShotFlag)
                    {
                        DateTime dt = DateTime.Now;
                        string fn = dt.ToString("yyyyMMdd_HHmmss");
                        cut.Save("img/"+fn+".png");
                        kh.flagReset();
                    }

                    //解放
                    cut.Dispose();

                    //ガベコレ
                    //GC.Collect();

                    //計測終了
                    sw.Stop();

                    int t = sw.Elapsed.Milliseconds;

                    //タイマーリセット
                    sw.Reset();

                }
            });
        }

        public Form1()
        {
            InitializeComponent();

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //画像ディレクトリ作成
            string path = "img";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //キャプチャ開始
            CaptureScreenTask();

            //キーフック
            kh.Hook();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //フック解除
            kh.HookEnd();
        }
    }
}
