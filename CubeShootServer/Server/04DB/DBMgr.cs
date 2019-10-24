
/****************************************************
	文件：DBMgr.cs
	作者：Solis
	邮箱: zhaotianshinai@gmail.com
	日期：2019/08/02 21:14   	
	功能：数据库操作
*****************************************************/
using System;
using MySql.Data.MySqlClient;
using PENet;
using PEProtocol;

public  class DBMgr
{
    private static DBMgr instance;
    public static DBMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DBMgr();
            }
            return instance;
        }
    }

    public const string CONNECTIONSTRING = "datasource=localhost;port=3306;user=root;pwd=;database=cubeshoot;Charset = utf8";
    private MySqlConnection conn;

    public void Init()
    {
        PETool.LogMsg("DBMgr Init Down");
        ConnDB();
    }
   

    /// <summary>
    /// 连接数据库
    /// </summary>
    /// <returns></returns>
    private  MySqlConnection ConnDB()
    {
        conn = new MySqlConnection(CONNECTIONSTRING);
        try
        {
            conn.Open();
            PECommon.Log("数据库已经连接上");
            return conn;
        }
        catch (Exception e)
        {
            PECommon.Log("连接数据库发生异常，可能是连接字符串不对" + e);
            return null;
        }

    }

    /// <summary>
    /// 断开连接数据库
    /// </summary>
    private void CloseConnDB(MySqlConnection conn)
    {
        if (conn != null)
        {
            conn.Close();
        }
        else
        {
            PECommon.Log("MySqlConnection-conn---不能关闭空对象");
        }
    }

    /// <summary>
    /// 查找数据库
    /// </summary>
    /// <returns></returns>
    public PlayerData QueryPlayerData(string acct, string pass)
    {
        bool isNewAccount = true;
        PlayerData pd = null;
        MySqlDataReader reader = null;
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from account where acct=@acct and pass=@pass", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pass", pass);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isNewAccount = false;
                pd = new PlayerData
                {
                    id = reader.GetInt32("id"),
                    name = reader.GetString("name"),
                    victoryNum = reader.GetInt32("victorynum"),
                };
            }
        }
        catch (Exception e)
        {
            PECommon.Log("查询PlayerData 错误，Error" + e);
        }
        finally
        {
            if (reader!=null)
            {
                reader.Close();
            }
            //是新的账号（创建一个默认的新账号。）
            if (isNewAccount)
            {
                pd = new PlayerData
                {
                    id = -1,
                    name = "",
                    victoryNum = 0,
                };
                
                //这一步很重要，因为是-1 所有重新让数据库插入然后赋值
                pd.id =  InserPlayerDataToDB(acct, pass, pd);
            }
            //如果服务器的数据库是关闭的。
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                pd = null;
            }
        }
        return pd;
    }


    /// <summary>
    /// 插入玩家资料到数据库
    /// </summary>
    public int InserPlayerDataToDB(string acct,string pass,PlayerData playerData)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd = new MySqlCommand("insert into account set acct =@acct,pass =@pass,name =@name,victorynum=@victorynum", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pass", pass);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("victorynum", playerData.victoryNum);

            //执行语句（不是查询）
            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;
        }
        catch (Exception e)
        {
            PECommon.Log("InserPlayerDataToDB插入用户失败" + e);
        }
        return id;
    }


    /// <summary>
    /// 查询数据库是否有名字相同的
    /// </summary>
    public bool QueryNameRepeat(string name)
    {
        bool isExist = false;
        MySqlDataReader reader = null;
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from account where name=@name", conn);
            cmd.Parameters.AddWithValue("name", name);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isExist = true;
            }
        }
        catch (Exception e)
        {
            PECommon.Log("查询要修改的用户名错误，Error" + e);
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
        }
        return isExist;
    }


    /// <summary>
    /// 根据用户ID来升级数据库中指定ID的用户
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="playerData"></param>
    /// <returns></returns>
    public bool UpdatePlayerData(int playerID, PlayerData playerData)
    {
        try
        {
            MySqlCommand cmd = new MySqlCommand("update account set name =@name,victorynum =@victorynum where id = @id", conn);
            cmd.Parameters.AddWithValue("id", playerID);
            cmd.Parameters.AddWithValue("name", playerData.name);
            cmd.Parameters.AddWithValue("victorynum", playerData.victoryNum);
         
            //执行语句（不是查询）
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            PECommon.Log("UpdatePlayerData升级用户信息失败" + e);
            return false;
        }
        return true;
    }

}

