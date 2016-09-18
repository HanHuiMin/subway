using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace subwayRoute
{
    class Program
    {
        public static void Main(string[] args)
        {
            String[][] Map = new String[18][];
            ArrayList subway = new ArrayList();
            Program a = new Program();
            //String path = Console.ReadLine();
            //Map = read(path);
            Map = read("F:\\subway.txt");
            subwayMap map = new subwayMap(Map);//构建一个地图
            String start = Console.ReadLine();
            String end = Console.ReadLine();
            //map.shortestRoute(start,end);
            transferMap transmap = new transferMap(Map,map);
            transmap.lessTransfer(start,end);
            //注意每次命令处理结束，要把所有的站点里的数据都清除
            //if (args.Length == 1)//根据命令行参数选定处理函数
            //{
            //    a.writeRoute(args[0], Map);
            //}
            //// 一号线
            //else if (args.Length == 3)
            //{
            //    if (args[0].Equals("-b"))
            //    {
            //        map.shortestRoute(args[1], args[2]);
            //    }
            //    else if (args[0].Equals("-c"))
            //    {
            //        transmap.lessTransfer(args[1], args[2]);
            //    }
            //    else
            //    {
            //        Console.WriteLine("命令格式不正确！");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("命令参数不正确！");
            //}
        }
        public static String[][] read(String path)
        {
            int i;
            String line;
            ArrayList subwayline = new ArrayList();
            String[][] Map = new String[18][];
            StreamReader sr = new StreamReader(path, Encoding.Default);
            for (i = 0; (line = sr.ReadLine()) != null; i++)
            {
                subwayline.Add(line);
            }
            for (i = 0; i < 18; i++)
            {
                String str = (String)subwayline[i];
                Map[i] = str.Split(new char[] { '：', '，' });
            }
            return Map;
        }
        public void writeRoute(String routeName, String[][] Map)
        {
            int i = 0;
            for (i = 0; i < Map.Length; i++)
            {
                if (routeName.Equals(Map[i][0]))
                {
                    for (int j = 0; j < Map[i].Length; j++)
                        System.Console.WriteLine(Map[i][j]);
                }
                if (i == 17)
                    System.Console.WriteLine("不存在该线路！");
            }
        }
    }
}

