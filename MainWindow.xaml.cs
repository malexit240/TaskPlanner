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
            string result=""+'\n';
            for (int i = 0; i < deep; i++)
            {
                result += '\t';
            }
            result += root.Task+" "+root.Id.ToString();
            for (int i = 0; i < root.child.Count; i++)
            {
                result += addText(root.child[i], deep + 1);
            }
            result += '\n';
            return result;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            TreeNode root = ViewModel.root;
            string tree = addText(root, 0);

            Tree.Text = tree;
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

    }

   

    

}
