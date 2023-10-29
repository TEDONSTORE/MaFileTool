using maFileTool.Core;
using maFileTool.Model;
using maFileTool.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace maFileTool
{
    public class Program
    {
        public static string steam = Environment.CurrentDirectory + "\\Steam.xlsx";
        public static string steamtxt = Environment.CurrentDirectory + "\\Steam.txt";
        public static List<Account> accounts = new List<Account>();
        public static bool quit = false;
        public static bool exit = false;

        static bool secs = false;
        static bool mins = false;

        static bool enterPressed = false;

        static void Main(string[] args)
        {
            string tedonstore = "Powered by tedonstore.com";
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (tedonstore.Length / 2)) + "}", tedonstore));

            #region Checks

            if (!System.IO.File.Exists(String.Format("{0}\\Settings.json", Environment.CurrentDirectory)))
            {
                new Utils().SaveSettings();
                Console.WriteLine("Please specify the settings in Settings.json");
                Console.ReadLine();
                return;
            }

            if (String.IsNullOrEmpty(Worker.settings.MailServer) || String.IsNullOrWhiteSpace(Worker.settings.MailServer))
            {
                Console.WriteLine("Please specify the MailServer in Settings.json");
                Console.ReadLine();
                return;
            }

            if (String.IsNullOrEmpty(Worker.settings.MailPort) || String.IsNullOrWhiteSpace(Worker.settings.MailPort))
            {
                Console.WriteLine("Please specify the MailPort in Settings.json");
                Console.ReadLine();
                return;
            }

            if (String.IsNullOrEmpty(Worker.settings.MailProtocol) || String.IsNullOrWhiteSpace(Worker.settings.MailProtocol))
            {
                Console.WriteLine("Please specify the MailProtocol in Settings.json");
                Console.ReadLine();
                return;
            }

            if (!System.IO.File.Exists(steam))
            {
                Console.WriteLine("Сan't find Steam.xlsx");
                Console.ReadLine();
                return;
            }

            if (!System.IO.File.Exists(steamtxt))
            {
                Console.WriteLine("Сan't find Steam.txt");
                Console.ReadLine();
                return;
            }

            #endregion

            Scanning();
            if (exit) return;
            while (true)
            {
                DoWork();

                if (quit) { Console.WriteLine("Exit due to an error. Goodbye."); break; }
                else
                {
                    TimerRendering("mins");
                    accounts.Clear();
                    Scanning();
                    if (accounts.Count() <= 0)
                    {
                        Console.WriteLine("All tasks have been completed successfully. Goodbye.");
                        break;
                    }
                }
            }
            Console.ReadLine();
        }

        static void DoWork() 
        {
            foreach (var account in accounts)
            {
                secs = false;
                mins = false;

                new Worker(account.Login, account.Password, account.Email, account.EmailPassword).DoWork();
                if (quit) break;

                if (account != accounts.Last())
                    TimerRendering("secs");
            }
        }

        static void TimerRendering(string mode)
        {
            if (mode == "mins")
            {
                mins = false;

                int waiting = 15;

                while (waiting > 0)
                {
                    if (mins)
                    {
                        Console.SetCursorPosition(String.Format("Sleep {0} minutes before rescanning.", (waiting + 1)).Length, Console.CursorTop - 1);
                        do { Console.Write("\b \b"); } while (Console.CursorLeft > 0);
                        Console.WriteLine("Sleep {0} minutes before rescanning.", waiting);
                    }
                    else
                    {
                        mins = true;
                        Console.WriteLine("Sleep {0} minutes before rescanning.", waiting);
                    }

                    Thread.Sleep(1 * 60 * 1000);
                    waiting--;
                }

                Console.SetCursorPosition(String.Format("Sleep {0} minutes before rescanning.", waiting).Length, Console.CursorTop - 1);
                do { Console.Write("\b \b"); } while (Console.CursorLeft > 0);
            }
            else 
            {
                int waiting = Int32.Parse(Worker.settings.BindingTimeout) * 60;

                while (waiting > 0)
                {
                    if (secs)
                    {
                        Console.SetCursorPosition(String.Format("Sleep {0} seconds before linking new account.", (waiting + 1)).Length, Console.CursorTop - 1);
                        do { Console.Write("\b \b"); } while (Console.CursorLeft > 0);
                        Console.WriteLine("Sleep {0} seconds before linking new account.", waiting);
                    }
                    else
                    {
                        secs = true;
                        Console.WriteLine("Sleep {0} seconds before linking new account.", waiting);
                    }

                    Thread.Sleep(1 * 1000);
                    waiting--;
                }

                Console.SetCursorPosition(String.Format("Sleep {0} seconds before linking new account.", waiting).Length, Console.CursorTop - 1);
                do { Console.Write("\b \b"); } while (Console.CursorLeft > 0);
            }
        }

        static void Scanning() 
        {
            string mode = Worker.settings.Mode.ToLower();

            switch (mode)
            {
                case "excel":
                    accounts = new Excel().ReadFromExcel(steam);
                    accounts.RemoveAll(t => String.IsNullOrEmpty(t.Login) || t.Login == "Логин" || t.Login == "Login");

                    int count = accounts.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Account account = accounts[i];
                        string date = account.Phone;
                        try
                        {
                            DateTime accDate = DateTime.ParseExact(date, "dd.MM.yy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                            if (accDate > DateTime.Now)
                            {
                                accounts.Remove(account);
                                i--; count--;
                            }
                        }
                        catch (FormatException)
                        {
                            accounts.Remove(account);
                            i--; count--;
                        }
                        catch (ArgumentNullException)
                        {
                            //Пустые оставляем
                        }
                    }
                    if (!enterPressed)
                    {
                        Console.WriteLine("Loaded - {0} accounts. Press enter to start.", accounts.Count());
                        Console.ReadLine();
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        enterPressed = true;
                    }
                    else Console.WriteLine("Rescanning... Loaded - {0} accounts.", accounts.Count());
                    break;
                case "txt":
                    string[] acs = System.IO.File.ReadAllLines(steamtxt);
                    acs = acs.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    acs = acs.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    
                    //Не самое элегантное решение
                    List<string> accs = new List<string>();

                    foreach (var ac in acs) 
                    {
                        try
                        {
                            string date = ac.Split(':')[4];
                            if (date.Contains("+")) { continue; }
                            date = $"{ac.Split(':')[4]}:{ac.Split(':')[5]}";
                            DateTime accDate = DateTime.ParseExact(date, "dd.MM.yy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                            if (accDate < DateTime.Now) accs.Add(ac);
                        }
                        catch (FormatException)
                        {
                            //В теории никогда не возникнет
                        }
                        catch (IndexOutOfRangeException)
                        {
                            //Оставляем
                            accs.Add(ac);
                        }
                    }

                    acs = (string[])accs.ToArray();

                    int id = 0;
                    foreach (var a in acs)
                    {
                        if (!a.Contains(':')) continue;
                        id++;
                        Account account = new Account();
                        account.Id = id.ToString();
                        account.Login = a.Split(':')[0];
                        account.Password = a.Split(':')[1];
                        account.Email = a.Split(':')[2];
                        account.EmailPassword = a.Split(':')[3];
                        accounts.Add(account);
                    }
                    if (!enterPressed)
                    {
                        Console.WriteLine("Loaded - {0} accounts. Press enter to start.", accounts.Count());
                        Console.ReadLine(); enterPressed = true;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                    }
                    else Console.WriteLine("Rescanning... Loaded - {0} accounts.", accounts.Count());
                    break;
                default:
                    Console.WriteLine("Please specify the Mode in Settings.json");
                    Console.ReadLine();
                    exit = true;
                    return;
            }
        }
    }
}
