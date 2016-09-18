using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subwayRoute
{
    public class Station
    {
        public static int MAX = 1000;
        public String Name;//站名
        public ArrayList sides = new ArrayList();//定义与该站点相连的站之间是哪条线的编号
        public ArrayList Sides = new ArrayList();//相连的四个站点，如果没有就是null
        public int lineNumber;//距离（为寻找最短路径准备）
        public int transferNumber;//换乘数（为寻找最少换乘准备）
        public Station(String name)
        {
            //Name是站点的名称
            Name = name;
            this.lineNumber = MAX;
            this.transferNumber = MAX;
        }
        public void nextStation(int routeNumber, int stationNumber,String[][] Map)
        {
            //routeNumber是该站点所在线路的编号
            //stationNumber是该站点在线路中的位置，方便存储相邻的站点
            //注意环线：2号线(编号1)，10号线(编号9)，表现在他们的线路名称中含有（环线）
            String[] str = Map[routeNumber][0].Split(new char[] { '（' });
            //Console.Write(str.Length);
            sides.Add(routeNumber);
            if (stationNumber != 1 && stationNumber!=Map[routeNumber].Length-1)
            {
                //若该站点不是起始站点也不是结束站点
                sides.Add(routeNumber);
                Sides.Add(Map[routeNumber][stationNumber - 1]);
                Sides.Add(Map[routeNumber][stationNumber + 1]);
            }
            else if(str.Length!=1&&stationNumber==1)
            {
                //如果该站点是2号线或十号线的起始站点
                sides.Add(routeNumber);
                int l = Map[routeNumber].Length;
                Sides.Add(Map[routeNumber][l-1]);
                Sides.Add(Map[routeNumber][stationNumber + 1]);
            }
            else if (str.Length!=1&& stationNumber == Map[routeNumber].Length-1)
            {
                //如果该站点是2号线或十号线的结束站点
                sides.Add(routeNumber);
                // int l = Map[routeNumber].Length;
                Sides.Add(Map[routeNumber][stationNumber - 1]);
                Sides.Add(Map[routeNumber][1]);
            }
            else if(str.Length==1 && stationNumber == 1)
            {
                //其余情况，即非环线的起始站点
                Sides.Add(Map[routeNumber][stationNumber + 1]);
            }
            else if (str.Length==1 && stationNumber == Map[routeNumber].Length - 1)
            {
                //其余情况，即非环线的结束站点
                Sides.Add(Map[routeNumber][stationNumber - 1]);
            }
            else
            {
                System.Console.WriteLine("出错啦~");
            }
        }
    }
    public class subwayMap
    {
        // public Station[] map = new Station[];
        public ArrayList subwaymap = new ArrayList();
        public String[][] map = new String[18][];
        public subwayMap(String[][] Map)
        {
            map = Map;
            //初始化所有的地铁站，包括站与站之间的连接关系
            int i, j;
            for (i = 0; i < Map.Length; i++)
            {
                for (j = 1; j < Map[i].Length; j++)
                {
                    //查找该站点是否已经存入地图数组
                    Station s = findSta(Map[i][j],subwaymap);
                    //int index = this.subwaymap.IndexOf(Map[i][j]);
                    //if (index == -1)
                    if(s==null)
                    {
                        //若该站点没有存进数组，则构造一个新的站点，补充相邻站点信息后存入数组
                        Station sta = new Station(Map[i][j]);
                        //System.Console.WriteLine(Map[i][j]);
                        sta.nextStation(i, j, Map);
                        subwaymap.Add(sta);
                        //System.Console.WriteLine(subwaymap.Count);
                    }
                    else
                    {
                        //若该站点已经在数组里了，那么对这个站点的相邻站点信息进行补充
                        //((Station)subwaymap[index]).nextStation(i, j, Map);
                        int l = 0;
                        for(l = 0; l < subwaymap.Count; l++)
                        {
                            if (((Station)(subwaymap[l])).Equals(s))
                                break;
                        }
                        ((Station)subwaymap[l]).nextStation(i, j, Map);
                    }
                }
            }
            //System.Console.WriteLine(subwaymap.Count);
        }
        public Station findSta(String staName,ArrayList subwaymap)
        {
            int i = 0;
            for (i = 0; i < subwaymap.Count; i++)
            {
                if ((((Station)subwaymap[i]).Name).Equals(staName))
                    break;
            }
            if(i<subwaymap.Count)
                return (Station)subwaymap[i];
            else
            {
                return null;
            }
        }
        public void findNext(String Name)
        {
            Station sta = findSta(Name,subwaymap);
            for (int l = 0; l < sta.Sides.Count; l++)
            {
                Console.WriteLine(sta.Sides[l]);
            }
        }
        public void check()
        {
            Console.WriteLine(subwaymap.Count);
            for(int i = 0; i < subwaymap.Count; i++)
            {
                if (((Station)(subwaymap[i])).Sides.Count ==1 )
                    Console.WriteLine(((Station)(subwaymap[i])).Name);
            }
            
        }
        public void shortestRoute(String startPoint,String endPoint)
        {
            //查找最短路径，采用广搜
            //广搜：从起点开始，把起点放入队列，然后每次取队头，然后把队头站点周围的站点赋值，放入队列中
            int start=0, end=0;
            Queue queue = new Queue();
            Station sta = null;
            Station temp = null;
            //找到起点和终点在数组中的位置
            for (start = 0; start < subwaymap.Count; start++)
            {
                if (((Station)subwaymap[start]).Name.Equals(startPoint))
                    break;
            }
            for (end = 0; end < subwaymap.Count; end++)
            {
                if (((Station)subwaymap[end]).Name.Equals(endPoint))
                    break;
            }
            //先把起点和起点的相邻站点放进队列
            ((Station)(subwaymap[start])).lineNumber = 0;
            queue.Enqueue(subwaymap[start]);
            for(int i = 0; i < ((Station)subwaymap[start]).Sides.Count; i++)
            {
                temp = findSta(((String)(((Station)(subwaymap[start])).Sides[i])),subwaymap);
                temp.lineNumber = ((Station)subwaymap[start]).lineNumber + 1;
                queue.Enqueue(temp);
            }
            //搜索终点：当队列为空或者找到终点则停下
            while (queue.Count!=0)
            {
                sta = (Station)queue.Dequeue();//取出队头元素
                if (sta.Equals(subwaymap[end]))//若从队头取出的元素刚好是终点，则搜索停止
                    break;
                for(int i = 0; i < sta.Sides.Count; i++)//把队头元素周围没有存过的站点存进队列
                {
                    temp = (Station)(findSta(((String)(sta.Sides[i])),subwaymap));
                    if (temp.lineNumber == Station.MAX)
                    {
                        temp.lineNumber = sta.lineNumber + 1;
                        queue.Enqueue(temp);
                    }
                }
            }
            //根据搜索到的终点反向找路径
            ArrayList route = new ArrayList();//存放路径上的站点
            ArrayList line = new ArrayList();//存放乘坐的路线
            while(sta.lineNumber!=0)
            {
                for(int j = 0; j < sta.Sides.Count; j++)
                {
                    temp = (Station)(findSta(((String)(sta.Sides[j])),subwaymap));
                    if (temp.lineNumber == sta.lineNumber - 1)
                    {
                        //在当前站点的周围结点中找到一个距离比当前距离小1的站点,加入数组中
                        route.Add(temp);
                        line.Add(sta.sides[j]);
                        sta = temp;
                        break;
                    }
                }
            }
            //输出路径
            Console.WriteLine(route.Count + 1);
            int lineCode=(int)line[line.Count-1];
            for (int i = route.Count-1; i >= 0; i--)
            {
                Console.Write(((Station)route[i]).Name);
                if (lineCode == (int)line[i])
                {
                    Console.WriteLine("");
                }
                else
                {
                    Console.Write("换乘");
                    Console.WriteLine(((map[(int)line[i]][0]).Split('（'))[0]);
                }
                lineCode = (int)line[i];
            }
            Console.WriteLine(endPoint);
        }
    }
}
