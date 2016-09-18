using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subwayRoute
{
    public class Transfer
    {
        //用来计算换乘数的类
        //主要属性有：一个站点，该站点的前一个站点该站点到起点的距离，换乘次数，以及每次换乘的线路的编号。
        public Station station;
        public Transfer foreStation;
        public int distance;
        public int transferNumber;
        public ArrayList lineNumber;
        public Transfer(Station sta,int dis,int transNum)
        {
            station = sta;
            distance = dis;
            transferNumber = transNum;
        }
    }

    public class transferMap
    {
        //换乘地图
        //ArrayList transfermap = new ArrayList();
        public static ArrayList transfermap = new ArrayList();//最终存储所有遍历过的站点的信息
        public ArrayList tempTrans = new ArrayList();//存储每一波要添加的站点
        public static String[][] Map = new String[18][];//地图的二维字符串数组
        public subwayMap subwaymap;
        public transferMap(String[][] map,subwayMap m)
        {
            Map = map;
            subwaymap = m;
        }
        public int findDistance(int line,int sta1,int sta2)
        {
            //根据map[][]，求编号为line的线上，下标为sta1和sta2的两个站之间的距离
            int distance=Station.MAX;
            String[] str = Map[line][0].Split('（');
            if (str.Length == 1)//如果编号为line的线路不是环线
            {
                distance = sta1 > sta2 ? (sta1 - sta2) : (sta2 - sta1);
            }
            else if (str.Length > 1)//如果编号为line的线路是环线
            {
                int temp_length1, temp_length2;
                temp_length1 = sta1 > sta2 ? (sta1 - sta2) : (sta2 - sta1);
                temp_length2 = Map[line].Length - temp_length1-1;
                distance = temp_length1 > temp_length2 ? temp_length2 : temp_length1;
            }
            else
                Console.WriteLine("求距离出错了！");
            return distance;
        }
        //找到t所在的line号线路上所有的站点加入数组tempTrans
        public void insertLine(int line,int transnumber,Transfer t)
        {
            //找到t所在的line号线路上所有的站点加入数组
            //########(一条线路)
            int  j, number;
            Transfer fore = t;
            for (number = 0; number < Map[line].Length; number++)
            {//找到t在Map[line]中对应的下标，方便把这条线路上其他站点加入数组时，计算t到站点的距离（直接下标相减即可）
                if (t.station.Name.Equals(Map[line][number]))
                    break;
            }
            for (j = 1; j < Map[line].Length; j++)
            {
                Station temp_s = subwaymap.findSta(Map[line][j],subwaymap.subwaymap);//找到每个Map[][]的站名所对应的station
                Transfer temp_t;
                temp_t = new Transfer(temp_s, t.distance + findDistance(line, j, number), transnumber);//构造出找到的这个station的Transfer
                temp_t.foreStation = t;//根据t找到的其他节点的前置站点都是t
                tempTrans.Add(temp_t);//把找到的Transfer存进数组中
            }
        }
        public void insertSta(Transfer sta,String[][] map,int transnumber)
        {
            //找到sta所在的每一条线路，然后对每一条线路调用insertLine
            int i, j;
            for (i = 0; i < map.Length; i++)//找sta所在的线路
            {
                for (j = 0; j < map[i].Length; j++)
                {
                    if (sta.station.Name.Equals(map[i][j]))
                    {
                        //找到某条线路时，调用insertLine方法把这条线路的所有站点加入数组
                        insertLine(i, transnumber, sta);
                    }
                }
            }
        }
        public int insertWave(int transnumber,String endPoint)
        {
            //一波insert包括多个点，调用多次insertSta，一个点包括多条线，调用多次insertLine
            //每一波insert的时候，与前一波的相同的就不再放了，但是同一波中可能存在多个名字相同的站。
            //首先把数组transfermap中的元素复制一份出来到一个新的数组里
            ArrayList Temp = new ArrayList();
            for(int i = 0; i < transfermap.Count; i++)
                Temp.Add(transfermap[i]);
            //然后对Temp里的每一个元素调用insertSta方法，找到的站点都存在tempTrans里面
            for(int i = 0; i < Temp.Count; i++)
            {
                insertSta((Transfer)Temp[i],Map,transnumber);
            }
            //把tempTrans里的元素与Temp里的元素对比，站名不一样的就放进transmap里面
            int flag1=0;//flag1是为了在每一次循环里判断元素是否在之前出现过的标志
            int flag = 0;//flag是为了检验这一波中是否扩散到了终点的标志，若这一波里出出现了终点，则flag置为1
            for (int i = 0; i < tempTrans.Count; i++)
            {
                flag1 = 0;//每一次循环之前置flag1为0
                for(int j = 0; j < Temp.Count; j++)
                {
                    //Console.WriteLine(((Transfer)(Temp[j])).station.Name);//可以输出朱辛庄
                    //Console.WriteLine(tempTrans.Count);
                    //Console.WriteLine("i=" + i);
                    //if (((Transfer)(tempTrans[1])).station == null)
                    //    Console.WriteLine("null");
                    //Console.WriteLine(((Transfer)(tempTrans[i])).station.Name);
                    
                    if (((Transfer)(tempTrans[i])).station.Name.Equals(((Transfer)(Temp[j])).station.Name))
                        //如果发现tempTrans里的元素的站名和Temp里某元素的站名相同就置flag1为1
                        flag1 = 1;
                }
                if (flag1 == 0)
                {
                    //如果flag1为0，说明tempTrans中的这个元素中没有与之前元素站名相同的元素，则把该元素放入transfermap中
                    transfermap.Add((Transfer)(tempTrans[i]));
                }
                if (((Transfer)(tempTrans[i])).station.Name.Equals(endPoint))
                    flag = 1;
            }
            return flag;

        }
        public ArrayList writeRoute(String start,String end)
        {
            //返回值是这两个站点所在的线路编号
            int line=Station.MAX,i,m,n;
            ArrayList Route = new ArrayList();
            for(i = 0; i < Map.Length; i++)
            {
                for(m = 0; m < Map[i].Length; m++)
                {
                    if (Map[i][m].Equals(start))
                        break;
                }
                for(n = 0; n < Map[i].Length; n++)
                {
                    if (Map[i][n].Equals(end))
                        break;
                }
                if (m < n && m!=Map[i].Length && n!=Map[i].Length && ((Map[i][0].Split('（')).Length == 1))
                {
                    line = i;
                    for(int j = m+1; j <= n; j++)
                    {
                        //Console.WriteLine(Map[i][j]);
                        Route.Add(Map[i][j]);
                    }
                    break;
                }
                else if (m > n && m != Map[i].Length && n != Map[i].Length && ((Map[i][0].Split('（')).Length == 1))
                {
                    line = i;
                    for (int j = m-1; j >= n; j--)
                    {
                        //Console.WriteLine(Map[i][j]);
                        Route.Add(Map[i][j]);
                    }
                    break;
                }
                else if(m!= n && m != Map[i].Length && n != Map[i].Length && ((Map[i][0].Split('（')).Length != 1))
                {
                    //如果是环线
                    line = i;
                    int d1, d2;
                    d1 = m > n ? (m - n) : (n - m);
                    d2 = (Map[i].Length-1 - d1) > d1 ? d1 : Map[i].Length - d1;
                    if (d2 >= d1)
                    {
                        //此时直接输出就行
                        if (m > n)
                        {
                            for (int j = m - 1; j >= n; j--)
                                Route.Add(Map[i][j]);
                        }
                        else
                        {
                            for (int j = m + 1; j <= n; j++)
                                Route.Add(Map[i][j]);
                        }
                    }
                    else
                    {
                        //输出环线的另一边
                        if (m > n)
                        {
                            for (int j = m+1; j < Map[i].Length; j++)
                                Route.Add(Map[i][j]);
                            for (int j = 1; j <= n; j++)
                                Route.Add(Map[i][j]);
                        }
                        else
                        {
                            for (int j = m -1; j >0; j--) 
                                Route.Add(Map[i][j]);
                            for (int j = Map[i].Length-1; j >= n; j--)
                                Route.Add(Map[i][j]);
                        }
                    }
                }
            }
            return Route;
        }
        public int findLine(String start,String end)
        {
            int line = Station.MAX;
            int i,m, n;
            for (i = 0; i < Map.Length; i++)
            {
                for (m = 0; m < Map[i].Length; m++)
                {
                    if (Map[i][m].Equals(start))
                        break;
                }
                for (n = 0; n < Map[i].Length; n++)
                {
                    if (Map[i][n].Equals(end))
                        break;
                }
                if (m != n && m != Map[i].Length && n != Map[i].Length)
                {
                    line = i;
                    break;
                }
            }
            return line;
        }
        public void lessTransfer(String startPoint,String endPoint)
        {
            //找最少换乘数中的最小路线
            //先把起点加入数组中，然后调用多次insertWave，直到把终点已经加入数组中
            int flag = 0;
            int transnumber = 0;
            Transfer start = new Transfer(subwaymap.findSta(startPoint,subwaymap.subwaymap),1,0);
            transfermap.Add(start);
            //反复调用insertWave，把元素插入数组，直至找到终点
            for(transnumber=0; flag==0; transnumber++)
            {
                flag=insertWave(transnumber,endPoint);
            }
            //for (int i = 1; i < transfermap.Count; i++)
            //{
            //    Console.WriteLine(((Transfer)(transfermap[i])).station.Name + "   距离" + ((Transfer)(transfermap[i])).distance + "    换乘次数" + ((Transfer)(transfermap[i])).transferNumber+"  前置站点"+ ((Transfer)(transfermap[i])).foreStation.station.Name.ToString());
            //}
            //for (int i = 0; i < transfermap.Count; i++)
            //{
            //    Console.WriteLine(((Transfer)(transfermap[i])).station.Name + "   距离" + ((Transfer)(transfermap[i])).distance + "    换乘次数" + ((Transfer)(transfermap[i])).transferNumber);
            //}
            //元素插入结束后，准备输出
            //先在transfermap里面找到距离最小的那个终点
            //#############3debug
            //int k = 0;
            //for (int l = 0; l < transfermap.Count - 1; l++)
            //{
            //    if (((Transfer)(transfermap[l])).foreStation == null)
            //    {
            //        Console.WriteLine(((Transfer)(transfermap[l])).station.Name);
            //    }
            //}
            //Console.WriteLine("transfermap.Count=" + transfermap.Count);
            //Console.WriteLine("k=" + k);
            //#################debug
            Transfer temp=null;
            int dis=Station.MAX;
            for(int i = 0; i < transfermap.Count; i++)
            {
                if (((Transfer)(transfermap[i])).station.Name.Equals(endPoint) && ((Transfer)(transfermap[i])).distance < dis)
                {
                    temp = (Transfer)transfermap[i];
                    dis = ((Transfer)(transfermap[i])).distance;
                }
            }
            ArrayList transferRoute = new ArrayList();//新建一个数组用来存储最少换乘线路
            if (temp != null)
                transferRoute.Add(temp);
            while (true)
            {
                //数组transferRoute最后一个元素的前置站点
                //########debug
                //if (((Transfer)(transferRoute[transferRoute.Count - 1])).foreStation == null)
                //    Console.WriteLine("NUllll");
                //Console.WriteLine((Transfer)(transferRoute[transferRoute.Count - 1]));
                //Console.WriteLine(((Transfer)(transferRoute[transferRoute.Count - 1])).station.Name);
                //#debug
                Station s=((Transfer)(transferRoute[transferRoute.Count-1])).foreStation.station;
                //Console.WriteLine("((Transfer)(transferRoute[transferRoute.Count-1])).Name=" + ((Transfer)(transferRoute[transferRoute.Count - 1])).station.Name);
                //Console.WriteLine("前置站点为" + s.Name);
                
                //在transfermap中找前置站点中距离最小的那个
                int distance = Station.MAX;
                int No = Station.MAX;
                for(int i = 0; i < transfermap.Count; i++)
                {
                    if (((Transfer)(transfermap[i])).station.Equals(s) && ((Transfer)(transfermap[i])).distance<distance)
                    {
                        distance = ((Transfer)(transfermap[i])).distance;
                        No = i;
                    }
                }
                //找到距起点距离最小的前置站点后，把该站点放入transferRoute数组中
                transferRoute.Add((Transfer)(transfermap[No]));
                if (s.Name.Equals(startPoint))
                    break;
            }
            //Console.WriteLine("transferRoute.Count=" + transferRoute.Count);
            //transferRoute相当于只存了换乘站点，现在先输出起点，然后写一个函数，输出两站之间的所有站
            //Console.WriteLine(startPoint);
            //Console.WriteLine("transferRoute.Count=" + transferRoute.Count);
            //Console.WriteLine(((Transfer)(transferRoute[transferRoute.Count - 1])).station.Name);
            Console.WriteLine(dis);
            Console.WriteLine(startPoint);
            ArrayList route = new ArrayList();
            //找到最开始乘坐的线路
            int line= findLine(((Transfer)(transferRoute[transferRoute.Count-1])).station.Name, ((Transfer)(transferRoute[transferRoute.Count-2])).station.Name);
            route = writeRoute(((Transfer)(transferRoute[transferRoute.Count - 1])).station.Name, ((Transfer)(transferRoute[transferRoute.Count - 2])).station.Name);
            for(int i = 0; i < route.Count-1; i++)
            {
                Console.WriteLine(route[i]);
            }
            Console.Write(route[route.Count - 1]);
            int temp_line;
            for(int j = transferRoute.Count - 3; j >= 0; j--)
            {
                temp_line=findLine(((Transfer)(transferRoute[j+1])).station.Name, ((Transfer)(transferRoute[j])).station.Name);
                if (temp_line != line)
                {
                    line = temp_line;
                    Console.Write("换乘");
                    Console.WriteLine((Map[line][0].Split('（'))[0]);
                }
                route = writeRoute(((Transfer)(transferRoute[j + 1])).station.Name, ((Transfer)(transferRoute[j])).station.Name);
                for (int i = 0; i < route.Count-1; i++)
                {
                    Console.WriteLine(route[i]);
                }
                Console.Write(route[route.Count - 1]);
            }
        }
    }
}
