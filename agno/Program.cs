using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;

namespace agno {
    internal class Program {
        static void Main(string[] args) {

            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--headless");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--allow-running-insecure-content");
            options.AddExcludedArgument("enable-logging");
            options.AddArgument("--silent");

            IWebDriver driver = new ChromeDriver(service, options);

            try {

                // login page
                Console.WriteLine("Login ekranına gidiliyor..");
                driver.Navigate().GoToUrl("https://obs.dpu.edu.tr/oibs/ogrenci/login.aspx");
                Thread.Sleep(1000);

                Console.WriteLine("Giriş bilgileri dolduruluyor..");
                IWebElement emailBox = driver.FindElement(By.Name("txtParamT01"));
                emailBox.SendKeys("");

                IWebElement passwordBox = driver.FindElement(By.Name("txtParamT02"));
                passwordBox.SendKeys("");

                ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile("sc.png");

                string filePath = "sc.png";

                Process resimGoruntuleyici = Process.Start("explorer.exe", filePath);

                Console.WriteLine("Güvenlik Kodunu Giriniz:");
                string securityCode = Console.ReadLine();

                IWebElement securityBox = driver.FindElement(By.Name("txtSecCode"));
                securityBox.SendKeys(securityCode);

                Console.WriteLine("Giriş yapılıyor..");
                IWebElement login = driver.FindElement(By.Id("btnLogin"));
                login.Click();
                Thread.Sleep(1000);

                // main console page
                IWebElement iframe = driver.FindElement(By.Id("IFRAME1"));
                driver.SwitchTo().Frame(iframe);

                string iframeSource = driver.PageSource;

                IWebElement agno = driver.FindElement(By.Id("lblAGNO"));
                Console.WriteLine(agno.Text);

                driver.SwitchTo().DefaultContent();

                Console.WriteLine("Not ekranına gidiliyor..\n");
                IWebElement op = driver.FindElement(By.XPath("/html/body/form/div[6]/nav/ul[1]/li/div/a/i"));
                op.Click();
                Thread.Sleep(1000);

                IWebElement dersVeDonemIslemleri = driver.FindElement(By.XPath("/html/body/form/div[6]/aside/div[2]/nav/span/ul/li[3]"));
                dersVeDonemIslemleri.Click();
                Thread.Sleep(1000);

                IWebElement noteListElement = driver.FindElement(By.XPath("/html/body/form/div[6]/aside/div[2]/nav/span/ul/li[3]/ul/li[4]/a"));
                noteListElement.Click();
                Thread.Sleep(1000);

                // note list page
                var noteListPageSource = driver.PageSource;

                iframe = driver.FindElement(By.Id("IFRAME1"));
                driver.SwitchTo().Frame(iframe);

                IList<IWebElement> lessons = driver.FindElements(By.XPath("/html/body/form/div[5]/div[1]/table/tbody/tr[3]/td/div/table/tbody/tr"));
                var modelList = new List<Model>();
                Model model;

                foreach (IWebElement lesson in lessons) {
                    var details = lesson.FindElements(By.TagName("td"));

                    if (details.Count != 0) {
                        model = new Model();

                        model.lessonName = details[2].Text;
                        model.notes = details[4].Text;
                        model.average = details[5].Text;
                        model.letter = details[6].Text;
                        model.status = details[7].Text;

                        Console.WriteLine("Ders: " + model.lessonName);
                        Console.WriteLine("Notlar: " + model.notes);
                        Console.WriteLine("Ortalama: " + model.average);
                        Console.WriteLine("Harf Notu: " + model.letter);
                        Console.WriteLine("Ders Geçme Durumu: " + model.status);
                        Console.WriteLine();

                        modelList.Add(model);
                    }
                }

                driver.Quit();
                Console.ReadLine();

            } catch (Exception e) {
                Console.WriteLine("Bir hata oluştu: " + e.Message);
                Thread.Sleep(1000);
                Console.WriteLine("Program sonlandırılıyor..");
                driver.Quit();
            }
        }
    }
}