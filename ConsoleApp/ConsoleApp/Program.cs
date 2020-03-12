using ConsoleApp.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        //定义数据库连接[SQL Server]
        static IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
        static void Main(string[] args)
        {
            #region 基本CRUD
            //GetAll();
            //Insert();
            //Search1();
            //Search2();
            //Update();
            //Delete();
            #endregion

            #region 更多用法
            //PageList(5, 8, "");
            //QueryMutiTable();
            //Do_Proc_Get_Table();
            Do_Proc_Get_Result();
            #endregion
        }

        #region 获取所有数据
        /// <summary>
        /// 获取所有数据
        /// </summary>
        private static void GetAll()
        {
            string query = "select * from UserInfo";
            IEnumerable<UserInfo> list = db.Query<UserInfo>(query);
            foreach (var item in list)
            {
                Console.WriteLine(item.StuName + " " + item.StuAddress + "\n");
            }
            Console.ReadKey();
        }
        #endregion

        #region 插入数据
        /// <summary>
        /// Insert()
        /// </summary>
        private static void Insert()
        {
            UserInfo info = new UserInfo()
            {
                StuName = "han",
                StuSex = 1,
                StuAddress = "BAT"
            };
            string query = @"Insert into UserInfo values (@StuName, @StuSex, @StuAddress); Select Cast (Scope_Identity() as int)";
            int id = db.Query<int>(query, info).Single();
            Console.WriteLine("新增加的ID值是:" + id.ToString() + "\n");
            Console.ReadKey();
        }
        #endregion

        #region 查询数据1
        /// <summary>
        /// 查询数据1
        /// </summary>
        private static void Search1()
        {
            string query = @"select * from UserInfo";

            UserInfo info = db.Query<UserInfo>(query).Where(x=>x.StuName.Contains("张")).SingleOrDefault<UserInfo>();
            Console.WriteLine("查询到的用户姓名是:" + info.StuName.ToString() + "\n");
            Console.ReadKey();
        }
        #endregion

        #region 查询数据2
        /// <summary>
        /// 查询数据2
        /// </summary>
        private static void Search2()
        {
            string query = @"select * from UserInfo where StuName like '%张%'";

            UserInfo info = db.Query<UserInfo>(query).SingleOrDefault();
            Console.WriteLine("查询到的用户姓名是:" + info.StuName.ToString() + "\n");
            Console.ReadKey();
        }
        #endregion

        #region 更新数据
        /// <summary>
        /// 更新数据
        /// </summary>
        private static void Update()
        {
            string query = @"Update UserInfo set StuName = @name where StuNo = @id";
            int result = db.Execute(query,new { name = "刘德华",id = 1005});
            if(result >0)
                Console.WriteLine("更新成功\n");
            else
                Console.WriteLine("更新失败\n");
            Console.ReadKey();
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        private static void Delete()
        {
            string query = @"Delete UserInfo where StuNo = @id";
            int result = db.Execute(query, new { id = 1005 });
            if (result > 0)
                Console.WriteLine("更新成功\n");
            else
                Console.WriteLine("更新失败\n");
            Console.ReadKey();
        }
        #endregion

        //--------------------高级用法---------------------//

        #region 分页获取数据
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageCount">当前取第几页的数据</param>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="where">过滤条件</param>
        private static void PageList(int pageCount,int pageSize,string where)
        {
            string query = @"select * from UserInfo where 1=1 ";

            IEnumerable<UserInfo> list = db.Query<UserInfo>(query).Skip((pageCount-1)*pageSize).Take(pageSize).OrderByDescending(x=>x.StuNo);
            foreach (var item in list)
            {
                Console.WriteLine(item.StuName + " " + item.StuAddress + "\n");
            }
            Console.ReadKey();
            Console.ReadKey();
        }
        #endregion

        #region 多表联查
        private static void QueryMutiTable()
        {
            string query = @"select * from UserInfo where StuName = '张三';select * from test where name = '张三'";
            using (var multipleresult = db.QueryMultiple(query))
            {
                UserInfo userinfo = multipleresult.Read<UserInfo>().SingleOrDefault();
                List<Class> class1 = multipleresult.Read<Class>().ToList();
                if (userinfo != null && class1 != null)
                {
                    Console.WriteLine(userinfo.StuName + "的各科成绩是\n");
                    foreach (var item in class1)
                    {
                        Console.WriteLine(item.subject + " " + item.source + "\n");
                    }
                }
            }
            
            Console.ReadKey();
        }
        #endregion

        #region  调用存储过程,获取返回表
        /// <summary>
        /// 调用存储过程,获取返回表
        /// </summary>
        private static void Do_Proc_Get_Table()
        {
            using (var multipleresult = db.QueryMultiple("sp_Get_Student_Info", new { name ="张三" }, commandType: CommandType.StoredProcedure))
            {
                UserInfo userinfo = multipleresult.Read<UserInfo>().SingleOrDefault();
                List<Class> class1 = multipleresult.Read<Class>().ToList();
                if (userinfo != null && class1 != null)
                {
                    Console.WriteLine(userinfo.StuName + "的各科成绩是\n");
                    foreach (var item in class1)
                    {
                        Console.WriteLine(item.subject + " " + item.source + "\n");
                    }
                }
            }
            Console.ReadKey();
        }
        #endregion 

        #region  调用存储过程,获取返回值
        private static void Do_Proc_Get_Result()
        {
            #region 初始化数据
            Class class1 = new Class();
            class1.name = "张三";
            class1.source = 80;
            class1.subject = "历史";
            #endregion 

            int result = 0;

            //定义参数
            var parameter = new DynamicParameters();
            parameter.Add("@Id", class1.id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            parameter.Add("@name", class1.name);
            parameter.Add("@source", class1.source);
            parameter.Add("@subject", class1.subject);
            //调用execute执行
            db.Execute("sp_Add_Student_Source", parameter, commandType: CommandType.StoredProcedure);

            result = parameter.Get<int>("@Id");
            if (result > 0)
                Console.WriteLine("插入成功，新ID值是：" + result.ToString());
            else
                Console.WriteLine("插入失败");

            Console.ReadKey();
        }
        #endregion 


    }
}
