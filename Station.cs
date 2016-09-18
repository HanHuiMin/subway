using System;

public class Station
{
    public static int MAX = 100;
    String Name;//站名
    ArrayList sides=new ArrayList();//定义与该站点相连的站之间是哪条线的编号
    ArrayList Sides = new ArrayList();//相连的四个站点，如果没有就是null
    int lineNumber;//距离（为寻找最短路径准备）
    int transferNumber;//换乘数（为寻找最少换乘准备）
	public Station(String Namer)
	{
        //Name是站点的名称
        this.Name = Name;
        this.lineNumber = 0;
        this.transferNumber = 0;
        for(int i = 0; i < 4; i++)
        {
            sides[i] = MAX;
            Sides[i] = null;
        }
	}
    public void nextStation(int routeNumber, int stationNumber)
    {
        //routeNumber是该站点所在线路的编号
        //stationNumber是该站点在线路中的位置，方便存储相邻的站点
        //注意环线：2号线，10号线

    }
}

public class subwayMap
{
    // public Station[] map = new Station[];
    public ArrayList subwayMap = new ArrayList();
    public subwayMap(String [][] Map)
    {
        //初始化所有的地铁站，包括站与站之间的连接关系
        int i, j;
        for (i = 0; i < Map.Length; i++)
        {
            for (j = 1; j < Map[i].Length; j++)
            {
                int index = this.subwayMap.IndexOf(Map[i][j]);
                if (index == -1)
                {
                    //若该站点没有存进数组，则构造一个新的站点，存入数组
                    Station sta = new Station(Map[i][j]);
                }
                else
                {

                }
            }
        }
    }
}
