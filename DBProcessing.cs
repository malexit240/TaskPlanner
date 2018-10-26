using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Data.Sql;

namespace TaskPlanner
{
    class DBProcessing
    {
        TreeNode root;
        public DBProcessing()
        {
            //createDB();
        }

        public void connectTree(TreeNode root)
        {
            this.root = root;
        }

        private void createDB()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=Database.db; Version=3;");
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand cmd = conn.CreateCommand();
                string sql_command =
                    "CREATE TABLE TASKS(" +
                    "id integer primary key autoincrement," +
                    "task varchar(50),parent_id integer," +
                    "foreign key(parent_id) references TASKS(id));" +
                    "CREATE TABLE TASKSINFO(" +
                    "id integer," +
                    "notation varchar(500)," +
                    "complete integer," +
                    "foreign key(id) references TASKS(id)" +
                    ");" +
                    "INSERT INTO TASKS(task,parent_id) VALUES ('root',1)";
                cmd.CommandText = sql_command;
                cmd.ExecuteNonQuery();
                conn.Dispose();
            }

        }

        public void addTask(string taskname, int parent_id)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=Database.db; Version=3;");
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand cmd = conn.CreateCommand();
                string sql_command = $"INSERT INTO TASKS(task,parent_id) VALUES('{taskname}','{parent_id}')";
                cmd.CommandText = sql_command;
                cmd.ExecuteNonQuery();
                conn.Dispose();
            }
        }

        public void fillTree(TreeNode root)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=Database.db; Version=3;");
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand cmd = conn.CreateCommand();
                string sql_command = "SELECT id, task, parent_id FROM TASKS order by parent_id;";
                cmd.CommandText = sql_command;

                SQLiteDataReader r = cmd.ExecuteReader();

                TreeNode local = null;
                int id,parent_id;
                string task;
                while (r.Read())
                {
                    id = Convert.ToInt32(r["id"]);
                    parent_id = Convert.ToInt32(r["parent_id"]);
                    task = r["task"].ToString();
                    if (id == 1) continue;
                    local = root.getNodeById(parent_id);
                    local.addChild(new TreeNode(local, id,task));

                }
                r.Close();
                conn.Dispose();
            }
        }

        internal void moveNode(int id, int dst)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=Database.db; Version=3;");
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand cmd = conn.CreateCommand();
                string sql_command = $"UPDATE TASKS SET parent_id={dst} WHERE id = {id}";
                cmd.CommandText = sql_command;
                cmd.ExecuteNonQuery();
                conn.Dispose();
            }
        }

        public void deleteNode(List<TreeNode> tn)
        {
                for (int i = tn.Count-1; i >= 0; i--)
                    deleteOneNode(tn[i].Id);
        }

        private void deleteOneNode(int id)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=Database.db; Version=3;");
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand cmd = conn.CreateCommand();
                string sql_command = $"DELETE FROM TASKS WHERE id = {id}";

                cmd.CommandText = sql_command;
                cmd.ExecuteNonQuery();
                conn.Dispose();
            }
        }

        public int getIdByNameAndParent(string taskname,int parent_id)
        {

            SQLiteConnection conn = new SQLiteConnection("Data Source=Database.db; Version=3;");
            conn.Open();

            if (conn.State == System.Data.ConnectionState.Open)
            {
                SQLiteCommand cmd = conn.CreateCommand();
                string sql_command = $"SELECT id FROM TASKS WHERE task = '{taskname}' AND parent_id = {parent_id};";
                cmd.CommandText = sql_command;

                SQLiteDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    return Convert.ToInt32(r["id"]);
                }
                r.Close();
                conn.Dispose();
            }

            return 0;
        }
    }

    public class TreeNode
    {
        public TreeNode parent;
        public List<TreeNode> child;
        public int Id { get; }
        public string Task { get; }

        public TreeNode(TreeNode parent, int id, string task)
        {
            this.parent = parent;
            this.Id = id;
            this.Task = task;
            this.child = new List<TreeNode>();
        }

        public void addChild(TreeNode child)
        {
            this.child.Add(child);
        }

        public List<TreeNode> getTaskList()
        {
            List<TreeNode> result=new List<TreeNode>();

            result.Add(this);
            for (int i = 0; i < child.Count; i++)
                result.AddRange(child[i].getTaskList());

            return result;
        }

        public TreeNode getNodeById(int id, bool isLocal = false)
        {
            List<TreeNode> tn = getTaskList();
            for (int i = 0; i < tn.Count; i++)
            {
                if (isLocal && tn[i].parent.Id != this.Id) continue;
                if (tn[i].Id == id) return tn[i];
            }

            return null;
        }

        public void deleteNode(int id)
        {
            for (int i = 0; i < child.Count; i++)
            {
                
                if(id==child[i].Id)
                {
                    List<TreeNode> tn = child[i].getTaskList();
                    for (int j = 0; j < tn.Count; j++)
                    {
                        tn[i] = null;
                    }
                    child.Remove(child[i]);
                    break;
                }
            }
        }

        public void foregetChild(TreeNode tn)
        {
            child.Remove(tn);
        }
    }
}