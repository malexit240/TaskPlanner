using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskPlanner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            
            InitializeComponent();
        }

        private void addTree_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.addNode(this.taskname.Text, Convert.ToInt32(this.parent_id.Text));
            UpdateTree();
        }

        private void UpdateTree()
        {
            TreeNode root = ViewModel.root;
            string tree = addText(root, 0);

            Tree.Text = tree;
        }

        private string addText(TreeNode root,int deep)
        {
            string result="\n";
            for (int i = 0; i < deep; i++)
            {
                result += '\t';
            }
            result += root.Task+" "+root.Id.ToString();
            for (int i = 0; i < root.child.Count; i++)
            {
                result += addText(root.child[i], deep + 1);
            }

            return result;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            TreeNode root = ViewModel.root;
            string tree = addText(root, 0);

            Tree.Text = tree;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.deleteNode(Convert.ToInt32( ID.Text));
        }

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.moveNode(Convert.ToInt32(ID.Text),Convert.ToInt32(nextID.Text));
        }
    }

    public static class ViewModel
    {
        public static TreeNode root;
        private static DBProcessing db;
        static ViewModel()
        {
            root = new TreeNode(null, 1, "root");
            db = new DBProcessing();
            db.connectTree(root);
            db.fillTree(root);

        }

        public static void addNode(string taskname,int parent_id)
        {
            db.addTask(taskname, parent_id);
            int id = db.getIdByNameAndParent(taskname, parent_id);
            TreeNode parent = root.getNodeById(parent_id);
            parent.addChild(new TreeNode(parent, id, taskname));
        }

        public static void deleteNode(int id)
        {
            TreeNode self = root.getNodeById(id);
            List<TreeNode> tn = self.getTaskList();
            db.deleteNode(tn);
            self.parent.deleteNode(id);

        }

        internal static void moveNode(int id,int dst)
        {
            db.moveNode(id, dst);

            TreeNode tn = root.getNodeById(id);
            TreeNode parent_dst = root.getNodeById(dst);
            parent_dst.addChild(tn);
            tn.parent.foregetChild(tn);
            tn.parent = parent_dst;

        }
    }

   

    

}
