using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using NumberedTextBox;

namespace XMLCompare
{
    public partial class XMLCompare : Form
    {
        HashSet<String> allPathLeft = new HashSet<string>();
        HashSet<String> allPathRight = new HashSet<string>();

        public XMLCompare()
        {
            InitializeComponent();
        }

        private void xmlLoad(String xml, HashSet<String> hashset)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml);

            XmlNodeList rootNodes = xmldoc.DocumentElement.ChildNodes;
            enumNodes(rootNodes, hashset);
        }

        private void enumNodes(XmlNodeList rootNodes,HashSet<String> hashset)
        {
            if (rootNodes==null || rootNodes.Count < 1) return;

            for (int i = 0; i < rootNodes.Count; i++)
            {
                XmlNode node  = rootNodes[i];
                if(node.NodeType==XmlNodeType.Element)
                {
                    hashset.Add(GetPathToNode(node));
                    enumNodes(node.ChildNodes, hashset);
                }
            }
        }

        static string GetPathToNode(XmlNode node)
        {
            if (node.ParentNode == null)
            {
                //到达根节点
                return string.Empty;
            }

            return String.Format("{0}/{1}", GetPathToNode(node.ParentNode), node.Name);
        }

        private void initAndLoad(NumberedTextBoxUC xml, NumberedTextBoxUC res, HashSet<String> hashSet)
        {
            try
            {
                res.Text = "";
                hashSet.Clear();
                xmlLoad(xml.Text, hashSet);
                res.Text = "Path数量："+hashSet.Count.ToString();

                foreach (String s in hashSet)
                {
                    res.Text += "\n" + s;
                }
            }
            catch (Exception ex)
            {
                res.Text = ex.Message;
            }
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            initAndLoad(xmlLeft, resLeft, allPathLeft);
            initAndLoad(xmlRight, resRight, allPathRight);

            Compare(allPathLeft, allPathRight, resLeft,resRight);
        }

        private void Compare(HashSet<String> left, HashSet<String> right, NumberedTextBoxUC resLeft,NumberedTextBoxUC resRight)
        {
            HashSet<String> leftLost = new HashSet<string>();
            HashSet<String> rightLost = new HashSet<string>();

            foreach (String s in left)
            {
                if (!right.Contains(s))
                {
                    rightLost.Add(s);
                }
            }

            foreach (String s in right)
            {
                if (!left.Contains(s))
                {
                    leftLost.Add(s);
                }
            }


            resLeft.Text += "\n\n比右侧缺少节点：";
            foreach (String s in leftLost)
            {
                resLeft.Text += "\n" + s;
            }

            resRight.Text += "\n\n比左侧缺少节点：";
            foreach (String s in rightLost)
            {
                resRight.Text += "\n" + s;
            }
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }
    }
}
