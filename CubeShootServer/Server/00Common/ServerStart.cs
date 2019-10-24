/****************************************************
	文件：ServerStart.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/07/30 16:39   	
	功能：服务器入口
*****************************************************/

namespace CubeShootServer
{
    class ServerStart   
    {
        static void Main(string[] args)
        {
            ServerRoot.Instance.Init();

            while (true)
            {
                ServerRoot.Instance.Update();
                //别占这么高的CPU。。。不需要这么高的帧率
                //Thread.Sleep(10);
            }
        }
    }
}
