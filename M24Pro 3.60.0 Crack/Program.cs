using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Net;

namespace M24Pro_Crack_by_AloneDev
{
    class Program
    {

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, int size, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        public static void SuspendProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        public static bool byteAra(ref byte[] kaynak, ref byte[] aranan, ref int sira)
        {
            bool buldunMu = true;
            for (int i = 0; i < kaynak.Length - aranan.Length; i++)
            {
                buldunMu = true;
                for (int j = 0; j < aranan.Length; j++)
                {
                    if (aranan[j] != kaynak[i + j])
                    {
                        buldunMu = false;
                        break;
                    }
                }
                if (buldunMu)
                {
                    sira = i;
                    return true;
                }
            }
            return false;
        }
        public static bool bulDegistir(IntPtr pHandle, ref byte[] kaynak, ref byte[] aranacakBytes, ref byte[] degisimBytes, uint baslangic, uint bitis)
        {
            bool bulunduMu = false;
            int bytesOkundu = 0, bytesYazildi = 0, sira = 0;
            while (baslangic <= bitis)
            {
                ReadProcessMemory(pHandle, (IntPtr)baslangic, kaynak, 256, out bytesOkundu);
                if (byteAra(ref kaynak, ref aranacakBytes, ref sira))
                {
                    WriteProcessMemory(pHandle, (IntPtr)baslangic + sira, degisimBytes, degisimBytes.Length, out bytesYazildi);
                    bulunduMu = true;
                    break;
                }
                baslangic += 256;
            }
            if (bulunduMu)
                return true;
            else
                return false;
        }

        public static void yeniSatir(int deger, int ikinciDeger)
        {
            if (deger == ikinciDeger)
            {
                Console.WriteLine(Environment.NewLine);
            }
            if (deger == 0)
            {
                Console.WriteLine(Environment.NewLine);
            }
        }

        static void Main(string[] args)
        {
            CrackBegin:
            Console.Title = "M24PRO 3.60.0 Crack";
            Console.Write(Environment.NewLine + "PID Giriniz : ");
            int processId = Convert.ToInt32(Console.ReadLine());
            if (Process.GetProcesses().Any(p => p.Id == processId))
            {
                Process islem = Process.GetProcessById(processId);
                string processName = islem.ProcessName.ToString();
                Process[] kontrol = Process.GetProcessesByName(processName);
                if (kontrol.Length > 0)
                {
                    IntPtr pHandle = OpenProcess(0xFFFF, true, islem.Id);
                    byte[] aranacakBytes = { 0x80, 0x7D, 0xA9, 0x00, 0x0F, 0x84, 0x80, 0x00 };
                    byte[] degisimBytes = { 0x80, 0x7D, 0xA9, 0x00, 0x90, 0x90, 0x90, 0x90 };
                    byte[] kaynak = new byte[0x100];
                    uint baslangic = (uint)islem.MainModule.BaseAddress;
                    uint bitis = 0xFF00000;

                    if (bulDegistir(pHandle, ref kaynak, ref aranacakBytes, ref degisimBytes, baslangic, bitis))
                    {
                        if (bulDegistir(pHandle, ref kaynak, ref aranacakBytes, ref degisimBytes, baslangic, bitis))
                        {
                            Console.WriteLine("Crack işlemi başarıyla tamamlandı");
                            Console.WriteLine("Yazmış olduğunuz PID'a ait uygulama 50saniye içerisinde donduralacak");

                            int sayac = 50;
                            bool durum = true;
                            bool deger = sayac <= 0;
                            while (durum)
                            {
                                Thread.Sleep(1000);
                                if (!deger)
                                {
                                    Console.Write(sayac + " - ");
                                }
                                if (sayac <= 0)
                                {
                                    SuspendProcess(processId);
                                    Console.WriteLine(Environment.NewLine + processId + " PID'ının sahip olduğu uygulama durduruldu !");
                                    sayac = 50;
                                    durum = false;

                                }
                                yeniSatir(sayac, 41);
                                yeniSatir(sayac, 31);
                                yeniSatir(sayac, 21);
                                yeniSatir(sayac, 11);
                                yeniSatir(sayac, 0);
                                sayac--;
                            }
                            Console.WriteLine("Tekrar crack yapmak istermisiniz (E/H)");
                            string answer = Console.ReadLine().ToUpper();
                            if (answer == "E")
                            {
                                Console.Clear();
                                goto CrackBegin;
                            }
                            else
                            {
                                Console.WriteLine("Çıkış yapmak için bir tuşa basınız.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Crack işlemi tamamlanamadı , Yönetici olarak çalıştırmayı deneyiniz");
                            Console.WriteLine("Tekrar crack yapmak istermisiniz (E/H)");
                            string hs = Console.ReadLine();
                            string answer = Console.ReadLine().ToUpper();
                            if (answer == "E")
                            {
                                Console.Clear();
                                goto CrackBegin;
                            }
                            else
                            {
                                Console.WriteLine("Çıkış yapmak için bir tuşa basınız.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Crack işlemi tamamlanamadı , Yönetici olarak çalıştırmayı deneyin");
                        Console.WriteLine("Tekrar crack yapmak istermisiniz (E/H)");
                        string answer = Console.ReadLine().ToUpper();
                        if (answer == "E")
                        {
                            Console.Clear();
                            goto CrackBegin;
                        }
                        else
                        {
                            Console.WriteLine("Çıkış yapmak için bir tuşa basınız.");
                        }
                    }
                    CloseHandle(pHandle);
                }
            }
            else
            {
                Console.WriteLine("Belirttiğiniz PID'a ait uygulama bulunamadı");
                Console.WriteLine("Tekrar denemek istermisiniz (E/H)");
                string hs = Console.ReadLine();
                string answer = Console.ReadLine().ToUpper();
                if (answer == "E")
                {
                    Console.Clear();
                    goto CrackBegin;
                }
                else
                {
                    Console.WriteLine("Çıkış yapmak için bir tuşa basınız.");
                }
            }
            Console.ReadKey();
        }
    }
}
