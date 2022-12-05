using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChocoboSplit
{
    internal class KeyboardHook
    {
        //スクショフラグ
        public bool screenShotFlag = false;

        //スクショフラグのセッター
        public void flagReset()
        {
            screenShotFlag = false;
        }

        delegate int delegateHookCallback(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, delegateHookCallback lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        IntPtr hookPtr = IntPtr.Zero;

        public void Hook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                // フックを行う
                // 第1引数   フックするイベントの種類
                //   13はキーボードフックを表す
                // 第2引数 フック時のメソッドのアドレス
                //   フックメソッドを登録する
                // 第3引数   インスタンスハンドル
                //   現在実行中のハンドルを渡す
                // 第4引数   スレッドID
                //   0を指定すると、すべてのスレッドでフックされる
                hookPtr = SetWindowsHookEx(
                    13,
                    HookCallback,
                    GetModuleHandle(curModule.ModuleName),
                    0
                );
            }
        }

        int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // フックしたキー
            //Console.WriteLine((Keys)(short)Marshal.ReadInt32(lParam));
            //MessageBox.Show(Marshal.ReadInt32(lParam).ToString());

            //スクショが押されたとき
            if (Marshal.ReadInt32(lParam) == 44 && (int)wParam == 256)
            {
                screenShotFlag = true;
            }

            // 1を戻すとフックしたキーが捨てられます
            return 0;
        }

        public void HookEnd()
        {
            UnhookWindowsHookEx(hookPtr);
            hookPtr = IntPtr.Zero;
        }
    }
}
