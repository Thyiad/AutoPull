using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;


namespace AutoPull
{
    class Program
    {
        static void Main(string[] args)
        {
            string workPath = Environment.CurrentDirectory;
            var repositories = Directory.GetDirectories(workPath);

            repositories = repositories.ToList().FindAll(repo=>Directory.Exists(repo+"\\.git")).ToArray();

            if (repositories.Length == 0)
            {
                Console.WriteLine("没有查找到 git 仓库，按下任意键退出......");
                Console.ReadKey();
                Environment.Exit(0);
            }

            Console.WriteLine($"已查找到{repositories.Length}个git仓库......");
            Console.WriteLine();

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口

            List<string> output = new List<string>();
            for (int i = 0; i < repositories.Length; i++)
            {
                string repositoryName = Regex.Match(repositories[i], @"[^\\]+$").Value;
                p.Start();//启动程序
                Console.WriteLine($"正在对第{i + 1}个仓库({repositoryName})执行 pull ......");
                p.StandardInput.WriteLine($"cd {repositories[i]}");
                p.StandardInput.WriteLine("git pull");
                p.StandardInput.WriteLine("exit");
                var curOutput = p.StandardOutput.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                curOutput.RemoveRange(0, 3);
                curOutput.RemoveAt(curOutput.Count - 1);
                curOutput.Insert(0, $"------------------------------{repositoryName}------------------------------");
                output.AddRange(curOutput);
            }
            
            p.WaitForExit();
            p.Close();

            Console.WriteLine();
            Console.WriteLine($"{repositories.Length}个仓库已全部更新，按 Y 键查看详细输出，其他键退出......");

            var key = Console.ReadLine();
            if(key.ToUpperInvariant() == "Y")
            {
                Console.WriteLine(string.Join(Environment.NewLine, output));
                Console.WriteLine("按下任意键退出......");
                Console.ReadKey();
            }
        }
    }
}
